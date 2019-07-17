using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Jump list service.
	/// </summary>
	public class JumpListService {

		private const string HistoryGroupName = "История";

		private const string WatchHistoryGroupName = "История просмотра";

		private const string PagesGroupName = "Страницы";

		/// <summary>
		/// Refresh group contains main pages of application.
		/// </summary>
		public async Task RefreshPagesGroup () {
			if ( !JumpList.IsSupported () ) return;

			JumpList jumpList = null;
			try {
				jumpList = await JumpList.LoadCurrentAsync ();
			}
			catch {
				return;
			}

			var oldItems = jumpList.Items
				.ToList ()
				.Where ( a => a.GroupName == PagesGroupName )
				.ToList ();
			foreach ( var oldItem in oldItems ) jumpList.Items.Remove ( oldItem );

			var releasesItem = JumpListItem.CreateWithArguments ( $"openreleasepage" , "Каталог релизов" );
			releasesItem.GroupName = PagesGroupName;
			jumpList.Items.Add ( releasesItem );

			var videoPlayerItem = JumpListItem.CreateWithArguments ( $"openvideoplayer" , "Видеоплеер" );
			videoPlayerItem.GroupName = PagesGroupName;
			jumpList.Items.Add ( videoPlayerItem );

			OrderItems ( jumpList );

			try {
				await jumpList.SaveAsync ();
			}
			catch {
				// Sometimes it was cause application failure
			}
		}

		private static void OrderItems ( JumpList jumpList ) {
			var orderedItems = jumpList.Items.OrderByDescending ( a => a.GroupName ).ToList ();
			jumpList.Items.Clear ();
			foreach ( var item in orderedItems ) jumpList.Items.Add ( item );
		}

		/// <summary>
		/// Add history items.
		/// </summary>
		/// <param name="historyItems">History items.</param>
		public async Task ChangeHistoryItems ( IDictionary<long , string> historyItems ) {
			if ( !JumpList.IsSupported () ) return;

			JumpList jumpList = null;
			try {
				jumpList = await JumpList.LoadCurrentAsync ();
			}
			catch {
				return;
			}

			var oldItems = jumpList.Items
				.ToList ()
				.Where ( a => a.GroupName == HistoryGroupName )
				.ToList ();
			foreach ( var oldItem in oldItems ) jumpList.Items.Remove ( oldItem );

			foreach ( var historyItem in historyItems ) {
				var item = JumpListItem.CreateWithArguments ( $"releasecardhistory:{historyItem.Key}" , historyItem.Value );
				item.GroupName = HistoryGroupName;
				jumpList.Items.Add ( item );
			}

			OrderItems ( jumpList );

			try {
				await jumpList.SaveAsync ();
			}
			catch {
				// Sometimes it was cause application failure
			}
		}

		/// <summary>
		/// Add history items.
		/// </summary>
		/// <param name="historyItems">History items.</param>
		public async Task ChangeWatchHistoryItems ( IDictionary<long , string> historyItems ) {
			if ( !JumpList.IsSupported () ) return;

			JumpList jumpList = null;
			try {
				jumpList = await JumpList.LoadCurrentAsync ();
			}
			catch {
				return;
			}

			var oldItems = jumpList.Items
				.ToList ()
				.Where ( a => a.GroupName == WatchHistoryGroupName )
				.ToList ();
			foreach ( var oldItem in oldItems ) jumpList.Items.Remove ( oldItem );

			foreach ( var historyItem in historyItems ) {
				var item = JumpListItem.CreateWithArguments ( $"releasewatchhistory:{historyItem.Key}" , historyItem.Value );
				item.GroupName = WatchHistoryGroupName;
				jumpList.Items.Add ( item );
			}

			OrderItems ( jumpList );

			try {
				await jumpList.SaveAsync ();
			}
			catch {
				// Sometimes it was cause application failure
			}
		}

	}

}
