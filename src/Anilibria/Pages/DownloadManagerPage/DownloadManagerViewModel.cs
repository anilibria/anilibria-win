using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Anilibria.MVVM;
using Anilibria.Pages.DownloadManagerPage.PresentationClasses;

namespace Anilibria.Pages.DownloadManagerPage {

	/// <summary>
	/// Download manager view model.
	/// </summary>
	public class DownloadManagerViewModel : ViewModel, INavigation {

		private string m_FilterByName;

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

		private DownloadSectionItem m_SelectedSection;

		public DownloadManagerViewModel () {
			CreateCommands ();

			m_SelectedSection = m_Sections.First ();
		}

		private void CreateCommands () {
			ShowSidebarCommand = CreateCommand ( OpenSidebar );
		}

		private void OpenSidebar () {
			ShowSidebar?.Invoke ();
		}

		public void NavigateFrom () {
		}

		public void NavigateTo ( object parameter ) {
		}

		/// <summary>
		/// Filter by name.
		/// </summary>
		public string FilterByName
		{
			get => m_FilterByName;
			set => Set ( ref m_FilterByName , value );
		}

		/// <summary>
		/// Filter by name.
		/// </summary>
		public DownloadSectionItem SelectedSection
		{
			get => m_SelectedSection;
			set => Set ( ref m_SelectedSection , value );
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
