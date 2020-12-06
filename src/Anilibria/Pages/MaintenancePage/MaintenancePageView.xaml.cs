using Anilibria.Services.Implementations;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.MaintenancePage
{
	/// <summary>
	/// Page contains advances functions for control over application.
	/// </summary>
	public sealed partial class MaintenancePageView : UserControl
	{

		private readonly MaintenancePageViewModel m_ViewModel;

		public MaintenancePageView () {
			InitializeComponent ();
			
			m_ViewModel = new MaintenancePageViewModel ( new AnalyticsService () );
			DataContext = m_ViewModel;
		}

	}
}
