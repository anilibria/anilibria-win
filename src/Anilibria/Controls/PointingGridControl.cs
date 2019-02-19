using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Controls {

	/// <summary>
	/// Grid with changing cursor to hand pointer if mouse over on this control.
	/// </summary>
	public class PointingGridControl : Grid {

		public PointingGridControl () {
			PointerEntered += PointingGridControl_PointerEntered;
			PointerExited += PointingGridControl_PointerExited;
		}

		private void PointingGridControl_PointerExited ( object sender , Windows.UI.Xaml.Input.PointerRoutedEventArgs e ) {
			Window.Current.CoreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Arrow , 0 );
		}

		private void PointingGridControl_PointerEntered ( object sender , Windows.UI.Xaml.Input.PointerRoutedEventArgs e ) {
			Window.Current.CoreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Hand , 0 );
		}

	}

}
