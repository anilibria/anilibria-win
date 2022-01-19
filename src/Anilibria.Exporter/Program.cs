using Anilibria.Storage.Entities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SystemJsonSerializer = System.Text.Json.JsonSerializer;

namespace Anilibria.Exporter {
	class Program {
		static async Task Main ( string[] args ) {
			Console.WriteLine ( "Welcome to AniLibria Exporter!" );
			Console.WriteLine ( "Finding Anilibria Win10 Application Data...." );
			var pathToStateFolder = "AppData/Local/Packages/53309GeekIT.AniLibria_vtk6wwa88hb3y/LocalState";
			var path = Path.Combine ( Environment.GetFolderPath ( Environment.SpecialFolder.UserProfile ) , pathToStateFolder , "mainc.db" );
			if ( !File.Exists ( path ) ) {
				Console.WriteLine ( "Sorry but we don't find file mainc.db on your system!" );
				return;
			}
			
			Directory.CreateDirectory ( "export" );

			LiteDatabase database = null;
			try {
				database = new LiteDatabase ( path );
			} catch {
				Console.WriteLine ( "Error while opening database :(((" );
			}

			ExportCinemahall ( database );

			await ExportHistory ( pathToStateFolder );

			ExportSeens ( database );

			Console.WriteLine ( "All processes completed!" );
		}

		private static async Task ExportHistory(string pathToStateFolder ) {
			Console.WriteLine ( "Export history..." );
			var releasesPath = Path.Combine ( Environment.GetFolderPath ( Environment.SpecialFolder.UserProfile ) , pathToStateFolder , "releases.cache" );
			if ( !File.Exists ( releasesPath ) ) {
				Console.WriteLine ( "Sorry but we don't find file releases.cache on your system!" );
				return;
			}
			var releasesCache = SystemJsonSerializer.Deserialize<IEnumerable<ReleaseEntity>> ( await File.ReadAllTextAsync ( releasesPath ) );

			var history = releasesCache
				.Where ( a => a.LastViewTimestamp != 0 || a.LastWatchTimestamp != 0 )
				.Select (
					a => new {
						id = a.Id ,
						timestamp = a.LastViewTimestamp ,
						watchTimestamp = a.LastWatchTimestamp
					}
				)
				.ToList ();
			File.WriteAllText ( "export/history.cache" , SystemJsonSerializer.Serialize ( history ) );
		}

		private static void ExportCinemahall ( LiteDatabase database ) {
			Console.WriteLine ( "Export cinemahall..." );

			var cinemahallCollection = database.GetCollection<CinemaHallReleaseEntity> ();
			var cinemahallItems = cinemahallCollection.FindAll ().ToList ();
			if ( cinemahallItems.Any () && cinemahallItems.First ().Releases.Any () ) {
				var releases = cinemahallItems.First ().Releases.Select(a => Convert.ToInt32(a )).ToArray ();
				File.WriteAllText ( "export/cinemahall.cache" , SystemJsonSerializer.Serialize ( releases ) );
			}

			Console.WriteLine ( "Completed!" );
		}

		private static void ExportSeens ( LiteDatabase database ) {
			Console.WriteLine ( "Export seens..." );
			var seensCollection = database.GetCollection<ReleaseVideoStateEntity> ().FindAll ();
			var seens = seensCollection
				.Where ( a => a.VideoStates != null && a.VideoStates.Any () )
				.Select (
					a => {
						return new {
							id = a.ReleaseId ,
							timestamp = 0 ,
							videoId = a.VideoStates.OrderByDescending ( b => b.Id ).First ().Id ,
							videoPosition = 0
						};
					}
				)
				.ToList ();
			File.WriteAllText ( "export/seen.cache" , SystemJsonSerializer.Serialize ( seens ) );

			Console.WriteLine ( "Completed!" );

			Console.WriteLine ( "Export seen marks..." );

			var seenMarks = seensCollection
				.Where ( a => a.VideoStates != null && a.VideoStates.Any () )
				.SelectMany (
					a => {
						return a.VideoStates
							.Where ( b => b.IsSeen )
							.Select ( b => $"{a.ReleaseId}.{b.Id}" )
							.ToList();
					}
				)
				.ToList ();

			File.WriteAllText ( "export/seenmark.cache" , SystemJsonSerializer.Serialize ( seenMarks ) );

			Console.WriteLine ( "Completed!" );
#if !DEBUG
			Console.ReadKey ();
#endif
		}
	}

}
