using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Anilibria.MVVM;
using Anilibria.Pages.HomePage.PresentationClasses;
using Windows.ApplicationModel;

namespace Anilibria.Pages.HomePage {

	/// <summary>
	/// Home view model.
	/// </summary>
	public class HomeViewModel : ViewModel {

		const string ReleasesPage = "Releases";

		const string PlayerPage = "Player";

		private ObservableCollection<SplitViewItem> m_Items = new ObservableCollection<SplitViewItem> (
			new List<SplitViewItem> {
				new SplitViewItem {
					Name = "Каталог релизов",
					Page = ReleasesPage,
					Icon = "\xF168"
				},
				new SplitViewItem {
					Name = "Видео плеер",
					Page = PlayerPage,
					Icon = "\xE714"
				},
				new SplitViewItem {
					Name = "Менеджер загрузок",
					Page = "Torrents",
					Icon = "\xE896"
				}
			}
		);

		private string m_Version = "";

		private SplitViewItem m_SelectedItem;

		public void Initialize () {
			var version = Package.Current.Id.Version;
			Version = $"{version.Major}.{version.Minor}.{version.Build}";

			SelectedItem = Items.First ();

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

				ChangePage?.Invoke ( m_SelectedItem.Page , null );
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
		public Action<string, object> ChangePage
		{
			get;
			set;
		}

	}

}
