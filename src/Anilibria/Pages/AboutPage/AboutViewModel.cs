using System;
using System.Windows.Input;
using Anilibria.MVVM;
using Windows.ApplicationModel;

namespace Anilibria.Pages.AboutPage {

	/// <summary>
	/// About view model.
	/// </summary>
	public class AboutViewModel : ViewModel, INavigation {

		public AboutViewModel () {

			var version = Package.Current.Id.Version;
			Version = $"{version.Major}.{version.Minor}.{version.Build}";

			CreateCommands ();
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
		/// Version.
		/// </summary>
		public string Version
		{
			get;
			set;
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
