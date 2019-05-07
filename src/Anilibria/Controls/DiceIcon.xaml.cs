using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Anilibria.Controls {

	public sealed partial class DiceIcon : UserControl {
		
		public DiceIcon () => InitializeComponent ();

		public static readonly DependencyProperty IconWidthProperty =
			DependencyProperty.Register (
				"IconWidth" ,
				typeof ( double ) ,
				typeof ( DiceIcon ) ,
				new PropertyMetadata ( 0 , IconWidthChangedHandler )
		);

		private static void IconWidthChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is DiceIcon iconControl ) ) return;

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
						typeof ( DiceIcon ) ,
						new PropertyMetadata ( 0 , IconHeightChangedHandler )
				);

		private static void IconHeightChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is DiceIcon iconControl ) ) return;

			iconControl.IconViewbox.Height = (double) e.NewValue;
		}

		public double IconHeight
		{
			get => (double) GetValue ( IconWidthProperty );
			set => SetValue ( IconWidthProperty , value );
		}

		public static readonly DependencyProperty IconColorProperty =
							DependencyProperty.Register (
								"IconColor" ,
								typeof ( Brush ) ,
								typeof ( DiceIcon ) ,
								new PropertyMetadata ( 0 , IconColorChangedHandler )
						);

		private static void IconColorChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is DiceIcon iconControl ) ) return;

			iconControl.IconViewboxPath.Foreground = (Brush) e.NewValue;
		}

		public Brush IconColor
		{
			get => (Brush) GetValue ( IconColorProperty );
			set => SetValue ( IconColorProperty , value );
		}

	}

}
