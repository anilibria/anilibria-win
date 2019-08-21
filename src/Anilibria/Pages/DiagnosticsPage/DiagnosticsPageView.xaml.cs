using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.DiagnosticsPage
{

	/// <summary>
	/// Diagnostics page.
	/// </summary>
	public sealed partial class DiagnosticsPageView : Page
	{

		public DiagnosticsPageView()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var window = Window.Current;
			var frame = (Frame)window.Content;
			frame.Navigate(typeof(HomeView));
		}

	}

}
