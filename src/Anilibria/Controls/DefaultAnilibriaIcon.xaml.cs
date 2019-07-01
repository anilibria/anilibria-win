using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Controls {
	public sealed partial class DefaultAnilibriaIcon : UserControl {

		public DefaultAnilibriaIcon () => InitializeComponent ();

		public static readonly DependencyProperty IconWidthProperty =
			DependencyProperty.Register (
				"IconWidth" ,
				typeof ( double ) ,
				typeof ( DefaultAnilibriaIcon ) ,
				new PropertyMetadata ( 0 , IconWidthChangedHandler )
		);

		private static void IconWidthChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is DefaultAnilibriaIcon iconControl ) ) return;

			iconControl.IconViewbox.Width = (double) e.NewValue;
		}

		public double IconWidth
		{
			get => (double) GetValue ( IconWidthProperty );
			set => SetValue ( IconWidthProperty , value );
		}

		public static readonly DependencyProperty IconHeightProperty =
					DependencyProperty.Register (
						"IconHeight" ,
						typeof ( double ) ,
						typeof ( DefaultAnilibriaIcon ) ,
						new PropertyMetadata ( 0 , IconHeightChangedHandler )
				);

		private static void IconHeightChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is DefaultAnilibriaIcon iconControl ) ) return;

			iconControl.IconViewbox.Height = (double) e.NewValue;
		}

		public double IconHeight
		{
			get => (double) GetValue ( IconWidthProperty );
			set => SetValue ( IconWidthProperty , value );
		}

	}
}
