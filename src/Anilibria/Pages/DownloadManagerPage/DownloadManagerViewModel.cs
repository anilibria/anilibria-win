using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.DownloadManagerPage.PresentationClasses;
using Anilibria.Services;
using Anilibria.Services.Implementations.PresentationClasses;

namespace Anilibria.Pages.DownloadManagerPage {

	/// <summary>
	/// Download manager view model.
	/// </summary>
	public class DownloadManagerViewModel : ViewModel, INavigation {

		private string m_FilterByName;

		private DownloadSectionItem m_SelectedSection;

		private bool m_IsMultipleSelect;

		private IDownloadService m_DownloadService;

		private ObservableCollection<DownloadSectionItem> m_Sections = new ObservableCollection<DownloadSectionItem> (
			new List<DownloadSectionItem> {
				new DownloadSectionItem {
					Title = "Все релизы",
					Type = DownloadSectionType.All
				},
				new DownloadSectionItem {
					Title = "Скачанные",
					Type = DownloadSectionType.Downloaded
				},
				new DownloadSectionItem {
					Title = "Скачиваемые",
					Type = DownloadSectionType.Downloading
				},
				new DownloadSectionItem {
					Title = "Не скаченные",
					Type = DownloadSectionType.NotDownloaded
				},
			}
		);

		private IEnumerable<DownloadItemModel> m_Downloads = Enumerable.Empty<DownloadItemModel> ();

		public DownloadManagerViewModel ( IDownloadService downloadService ) {
			m_DownloadService = downloadService ?? throw new ArgumentNullException ( nameof ( downloadService ) );
			CreateCommands ();

			m_SelectedSection = m_Sections.First ();
		}

		/// <summary>
		/// Initialize.
		/// </summary>
		/// <returns></returns>
		public async Task Initialize () => await m_DownloadService.StartDownloadProcess ();

		private void CreateCommands () {
			ShowSidebarCommand = CreateCommand ( OpenSidebar );
		}

		private void OpenSidebar () {
			ShowSidebar?.Invoke ();
		}

		public void NavigateFrom () {
		}

		public void RefreshDownloadItems () {
			DownloadSectionType type = DownloadSectionType.All;
			switch ( m_SelectedSection.Type ) {
				case DownloadSectionType.All:
					type = DownloadSectionType.All;
					break;
				case DownloadSectionType.Downloading:
					type = DownloadSectionType.Downloading;
					break;
				case DownloadSectionType.Downloaded:
					type = DownloadSectionType.Downloaded;
					break;
				case DownloadSectionType.NotDownloaded:
					type = DownloadSectionType.NotDownloaded;
					break;
				default: throw new NotSupportedException ( $"Type {type} not supported" );
			}
			Downloads = m_DownloadService.GetDownloads ( DownloadItemsMode.All );
		}

		public void NavigateTo ( object parameter ) => RefreshDownloadItems ();

		/// <summary>
		/// Filter by name.
		/// </summary>
		public string FilterByName
		{
			get => m_FilterByName;
			set => Set ( ref m_FilterByName , value );
		}

		/// <summary>
		/// Is multiple select.
		/// </summary>
		public bool IsMultipleSelect
		{
			get => m_IsMultipleSelect;
			set => Set ( ref m_IsMultipleSelect , value );
		}

		/// <summary>
		/// Downloads.
		/// </summary>
		public IEnumerable<DownloadItemModel> Downloads
		{
			get => m_Downloads;
			set => Set ( ref m_Downloads , value );
		}

		/// <summary>
		/// Filter by name.
		/// </summary>
		public DownloadSectionItem SelectedSection
		{
			get => m_SelectedSection;
			set
			{
				if ( !Set ( ref m_SelectedSection , value ) ) return;

				RefreshDownloadItems ();
			}
		}

		/// <summary>
		/// Sections;
		/// </summary>
		public ObservableCollection<DownloadSectionItem> Sections
		{
			get => m_Sections;
			set => Set ( ref m_Sections , value );
		}

		/// <summary>
		/// Show sidebar.
		/// </summary>
		public Action ShowSidebar
		{
			get;
			set;
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
