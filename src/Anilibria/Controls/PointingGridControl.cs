using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Anilibria.Controls {

	/// <summary>
	/// Grid with changing cursor to hand pointer if mouse over on this control.
	/// </summary>
	public class PointingGridControl : Grid {

		public PointingGridControl () {
			PointerEntered += PointingGridControl_PointerEntered;
			PointerExited += PointingGridControl_PointerExited;
			PointerMoved += PointingGridControl_PointerMoved;
		}

		private void PointingGridControl_PointerMoved ( object sender , PointerRoutedEventArgs e ) {
			if ( Window.Current.CoreWindow.PointerCursor != null && Window.Current.CoreWindow.PointerCursor.Type != CoreCursorType.Hand ) {
				Window.Current.CoreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Hand , 0 );
			}
		}

		private void PointingGridControl_PointerExited ( object sender , PointerRoutedEventArgs e ) {
			Window.Current.CoreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Arrow , 0 );
		}

		private void PointingGridControl_PointerEntered ( object sender , PointerRoutedEventArgs e ) {
			Window.Current.CoreWindow.PointerCursor = new CoreCursor ( CoreCursorType.Hand , 0 );
		}

	}

}
