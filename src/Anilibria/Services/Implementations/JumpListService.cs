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

			var jumpList = await JumpList.LoadCurrentAsync ();

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
			
			await jumpList.SaveAsync ();
		}

		/// <summary>
		/// Add history items.
		/// </summary>
		/// <param name="historyItems">History items.</param>
		public async Task ChangeHistoryItems ( IDictionary<long , string> historyItems ) {
			if ( !JumpList.IsSupported () ) return;

			var jumpList = await JumpList.LoadCurrentAsync ();

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

			await jumpList.SaveAsync ();
		}

		/// <summary>
		/// Add history items.
		/// </summary>
		/// <param name="historyItems">History items.</param>
		public async Task ChangeWatchHistoryItems ( IDictionary<long , string> historyItems ) {
			if ( !JumpList.IsSupported () ) return;

			var jumpList = await JumpList.LoadCurrentAsync ();

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

			await jumpList.SaveAsync ();
		}

	}

}
