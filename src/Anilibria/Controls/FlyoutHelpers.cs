using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Anilibria.Controls {
	/// <summary>
	/// Flyout helper.
	/// </summary>
	public static class FlyoutHelper {

		/// <summary>
		/// Is open property.
		/// </summary>
		public static readonly DependencyProperty IsOpenProperty =
			DependencyProperty.RegisterAttached (
				"IsOpen" , typeof ( bool ) ,
				typeof ( FlyoutHelper ) ,
				new PropertyMetadata ( true , IsOpenChangedCallback )
			);

		/// <summary>
		/// Paretn element property.
		/// </summary>
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.RegisterAttached (
				"Parent" ,
				typeof ( FrameworkElement ) ,
				typeof ( FlyoutHelper ) ,
				null
			);

		/// <summary>
		/// Is only hide property.
		/// </summary>
		public static readonly DependencyProperty IsOnlyHideProperty =
			DependencyProperty.RegisterAttached (
				"IsOnlyHide" ,
				typeof ( bool ) ,
				typeof ( FlyoutHelper ) ,
				new PropertyMetadata ( true )
			);

		public static void SetIsOpen ( DependencyObject element , bool value ) {
			element.SetValue ( IsOpenProperty , value );
		}

		public static bool GetIsOpen ( DependencyObject element ) {
			return (bool) element.GetValue ( IsOpenProperty );
		}

		public static bool GetIsOnlyHide ( DependencyObject element ) {
			return (bool) element.GetValue ( IsOnlyHideProperty );
		}

		public static void SetIsOnlyHide ( DependencyObject element , bool value ) {
			element.SetValue ( IsOnlyHideProperty , value );
		}

		private static void flyout_Closed ( object sender , object e ) {
			SetIsOpen ( sender as DependencyObject , false );
		}

		public static void SetParent ( DependencyObject element , FrameworkElement value ) {
			element.SetValue ( ParentProperty , value );
		}

		public static FrameworkElement GetParent ( DependencyObject element ) {
			return (FrameworkElement) element.GetValue ( ParentProperty );
		}

		private static void IsOpenChangedCallback ( DependencyObject d ,
			DependencyPropertyChangedEventArgs e ) {
			var fb = d as FlyoutBase;
			if ( fb == null ) return;

			if ( (bool) e.NewValue ) {
				if ( !GetIsOnlyHide ( d ) ) {
					fb.Closed += flyout_Closed;
					fb.ShowAt ( GetParent ( d ) );
				}
			}
			else {
				fb.Closed -= flyout_Closed;
				fb.Hide ();
			}
		}

	}
}
