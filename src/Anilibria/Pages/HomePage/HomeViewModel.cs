using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anilibria.MVVM;
using Anilibria.Pages.HomePage.PresentationClasses;
using Windows.ApplicationModel;

namespace Anilibria.Pages.HomePage {

	/// <summary>
	/// Home view model.
	/// </summary>
	public class HomeViewModel : ViewModel {

		private ObservableCollection<SplitViewItem> m_Items = new ObservableCollection<SplitViewItem> (
			new List<SplitViewItem> {
				new SplitViewItem {
					Name = "Каталог релизов",
					Page = "Releases",
					Icon = "\xF168"
				},
				new SplitViewItem {
					Name = "Видео плеер",
					Page = "Player",
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

		public HomeViewModel () {

			var version = Package.Current.Id.Version;
			m_Version = $"{version.Major}.{version.Minor}.{version.Revision}";
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
		/// Version.
		/// </summary>
		public string Version
		{
			get => m_Version;
			set => Set ( ref m_Version , value );
		}

	}

}
