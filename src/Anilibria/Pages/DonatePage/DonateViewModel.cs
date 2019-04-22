using System;
using System.Windows.Input;
using Anilibria.MVVM;

namespace Anilibria.Pages.DonatePage {

	/// <summary>
	/// Donate page view model.
	/// </summary>
	public class DonateViewModel : ViewModel, INavigation {

		public DonateViewModel () {
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
