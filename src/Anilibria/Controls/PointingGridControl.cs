using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Anilibria.Controls {

	/// <summary>
	/// Grid with changing background color if cursor over grid.
	/// </summary>
	public class PointingGridControl : Grid {

		public PointingGridControl () {
			PointerEntered += PointingGridControl_PointerEntered;
			PointerExited += PointingGridControl_PointerExited;
			CornerRadius = new CornerRadius ( 2 );
		}

		private void PointingGridControl_PointerExited ( object sender , PointerRoutedEventArgs e ) {
			Background = new SolidColorBrush ( Color.FromArgb ( 0 , 148 , 184 , 184 ) );
		}

		private void PointingGridControl_PointerEntered ( object sender , PointerRoutedEventArgs e ) {
			Background = new SolidColorBrush ( Color.FromArgb ( 80 , 148 , 184 , 184 ) );
		}

	}

}
