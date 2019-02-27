using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.HomePage.PresentationClasses;
using Anilibria.Services;
using Anilibria.Services.Exceptions;
using Anilibria.Services.PresentationClasses;
using Windows.ApplicationModel;

namespace Anilibria.Pages.HomePage {

	/// <summary>
	/// Home view model.
	/// </summary>
	public class HomeViewModel : ViewModel {

		const string ReleasesPage = "Releases";

		const string PlayerPage = "Player";

		const string AuthorizePage = "Authorize";

		const string YoutubePage = "Youtube";

		private readonly IAnilibriaApiService m_AnilibriaApiService;
		
		private IEnumerable<SplitViewItem> m_MenuItems;

		private ObservableCollection<SplitViewItem> m_Items;

		private string m_Version = "";

		private SplitViewItem m_SelectedItem;

		private UserModel m_UserModel;

		private bool m_IsAuthorized;

		public HomeViewModel ( IAnilibriaApiService anilibriaApiService ) {
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );

			var version = Package.Current.Id.Version;
			Version = $"{version.Major}.{version.Minor}.{version.Build}";

			m_MenuItems = new List<SplitViewItem> {
				new SplitViewItem {
					Name = "Каталог релизов",
					Page = ReleasesPage,
					Icon = "\xF168",
					IsVisible = StubMenuIsVisible
				},
				new SplitViewItem {
					Name = "Видеоплеер",
					Page = PlayerPage,
					Icon = "\xE714",
					IsVisible = StubMenuIsVisible
				},
				new SplitViewItem {
					Name = "Yotube",
					Page = YoutubePage,
					Icon = "\xE786",
					IsVisible = StubMenuIsVisible
				},
				new SplitViewItem {
					Name = "Войти",
					Page = AuthorizePage,
					Icon = "\xE77B",
					IsVisible = AuthorizeOptionIsVisible
				}
				/*,
				new SplitViewItem {
					Name = "Менеджер загрузок",
					Page = "Torrents",
					Icon = "\xE896"
				}*/
			};
			RefreshOptions ();

			CreateCommands ();
		}

		private void CreateCommands () {
			SignoutCommand = CreateCommand ( Signout );
		}

		private async void Signout () {
			await m_AnilibriaApiService.Logout ();
			RefreshOptions ();
			UserModel = null;
			await RefreshFavorites?.Invoke ();
		}

		public async Task Initialize () {
			SelectedItem = Items.First ();

			await ChangeUserSession ();
		}

		private bool AuthorizeOptionIsVisible () {
			return !m_AnilibriaApiService.IsAuthorized ();
		}

		private bool StubMenuIsVisible () => true;

		public void ChangeSelectedItem ( SplitViewItem splitViewItem) {
			m_SelectedItem = splitViewItem;
			RaisePropertyChanged ( () => SelectedItem );
		}

		/// <summary>
		/// Refresh options.
		/// </summary>
		public void RefreshOptions () {
			var selectedItem = SelectedItem;
			Items = new ObservableCollection<SplitViewItem> ( m_MenuItems.Where ( a => a.IsVisible () ) );

			if ( selectedItem != null ) SelectedItem = Items.FirstOrDefault ( a => a.Page == selectedItem.Page );
		}

		/// <summary>
		/// Change user session.
		/// </summary>
		public async Task ChangeUserSession () {
			try {
				if ( m_AnilibriaApiService.IsAuthorized () ) {
					var model = await m_AnilibriaApiService.GetUserData ();
					model.ImageUrl = m_AnilibriaApiService.GetUrl ( model.Avatar );

					await RefreshFavorites?.Invoke ();

					UserModel = model;
				}
			}
			catch ( AuthorizeDeletedException ) {
				RefreshOptions ();
				UserModel = null;
			}
		}

		/// <summary>
		/// User model.
		/// </summary>
		public UserModel UserModel
		{
			get => m_UserModel;
			set
			{
				if ( !Set ( ref m_UserModel , value ) ) return;

				IsAuthorized = m_UserModel != null;
			}
		}

		/// <summary>
		/// Is authorized.
		/// </summary>
		public bool IsAuthorized
		{
			get => m_IsAuthorized;
			set => Set ( ref m_IsAuthorized , value );
		}

		/// <summary>
		/// Items.
		/// </summary>
		public ObservableCollection<SplitViewItem> Items
		{
			get => m_Items;
			set => Set ( ref m_Items , value );
		}

		/// <summary>
		/// Selected item.
		/// </summary>
		public SplitViewItem SelectedItem
		{
			get => m_SelectedItem;
			set
			{
				if ( !Set ( ref m_SelectedItem , value ) ) return;

				if ( m_SelectedItem != null ) ChangePage?.Invoke ( m_SelectedItem.Page , null );
			}
		}

		/// <summary>
		/// Version.
		/// </summary>
		public string Version
		{
			get => m_Version;
			set => Set ( ref m_Version , value );
		}

		/// <summary>
		/// Change page handler.
		/// </summary>
		public Action<string , object> ChangePage
		{
			get;
			set;
		}

		/// <summary>
		/// Refresh favorites.
		/// </summary>
		public Func<Task> RefreshFavorites
		{
			get;
			set;
		}
		
		/// <summary>
		/// Signout command.
		/// </summary>
		public ICommand SignoutCommand
		{
			get;
			set;
		}

	}

}
