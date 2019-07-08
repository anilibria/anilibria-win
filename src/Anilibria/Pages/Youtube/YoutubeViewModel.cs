using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anilibria.Collections;
using Anilibria.MVVM;
using Anilibria.Pages.Youtube.PresentationClasses;
using Anilibria.Services;
using Windows.System;

namespace Anilibria.Pages.Youtube {

	/// <summary>
	/// List with Youtube videos.
	/// </summary>
	public class YoutubeViewModel : ViewModel, INavigation {

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		private readonly IAnalyticsService m_AnalyticsService;

		private IncrementalLoadingCollection<YoutubeVideoModel> m_Collection;

		private ObservableCollection<YoutubeVideoModel> m_SelectedVideos;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="anilibriaApiService">Anilibria api service.</param>
		public YoutubeViewModel ( IAnilibriaApiService anilibriaApiService , IAnalyticsService analyticsService ) {
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
			m_AnalyticsService = analyticsService ?? throw new ArgumentNullException ( nameof ( analyticsService ) );

			CreateCommands ();
			RefreshVideos ();
			RefreshSelectedVideos ();
		}

		private void CreateCommands () {
			ShowSidebarCommand = CreateCommand ( ToggleSidebar );
		}

		/// <summary>
		/// Refresh videos.
		/// </summary>
		private void RefreshVideos () {
			m_Collection = new IncrementalLoadingCollection<YoutubeVideoModel> {
				PageSize = 20 ,
				GetPageFunction = GetItemsPageAsync
			};
			RaisePropertyChanged ( () => Collection );
		}

		private void RefreshSelectedVideos () {
			SelectedVideos = new ObservableCollection<YoutubeVideoModel> ();
			SelectedVideos.CollectionChanged += SelectedReleasesChanged;
		}

		private async void SelectedReleasesChanged ( object sender , NotifyCollectionChangedEventArgs e ) {
			if ( SelectedVideos.Count == 1 ) {
				var video = SelectedVideos.First ();
				await Launcher.LaunchUriAsync ( video.VideoUrl );
				RefreshSelectedVideos ();
			}
		}

		private async Task<IEnumerable<YoutubeVideoModel>> GetItemsPageAsync ( int page , int pageSize ) {
			//TODO: network error handling
			var videos = await m_AnilibriaApiService.GetYoutubeVideosPage ( page , pageSize );

			return videos
				.Select (
					a => new YoutubeVideoModel {
						Id = a.Id ,
						Comments = a.Comments ,
						Timestamp = a.Timestamp ,
						Views = a.Views ,
						Title = a.Title ,
						Image = m_AnilibriaApiService.GetUrl ( a.Image ) ,
						VideoUrl = new Uri ( "https://www.youtube.com/watch?v=" + a.VId )
					}
				)?.ToList () ?? Enumerable.Empty<YoutubeVideoModel> ();
		}

		private void ToggleSidebar () {
			ShowSidebar?.Invoke ();
		}

		/// <summary>
		/// Collection.
		/// </summary>
		public IncrementalLoadingCollection<YoutubeVideoModel> Collection
		{
			get => m_Collection;
			set => Set ( ref m_Collection , value );
		}

		/// <summary>
		/// Selected release.
		/// </summary>
		public ObservableCollection<YoutubeVideoModel> SelectedVideos
		{
			get => m_SelectedVideos;
			set => Set ( ref m_SelectedVideos , value );
		}

		/// <summary>
		/// Show sidebar.
		/// </summary>
		public Action ShowSidebar
		{
			get;
			set;
		}

		public void NavigateFrom () {
		}

		public void NavigateTo ( object parameter ) {
			m_AnalyticsService.TrackEvent ( "YoutubeVideo" , "NavigatedTo" , "Simple" );
		}

		/// <summary>
		/// Show sidebar command.
		/// </summary>
		public ICommand ShowSidebarCommand
		{
			get;
			set;
		}

	}

}
