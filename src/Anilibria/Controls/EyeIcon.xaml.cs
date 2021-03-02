using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Anilibria.Controls {

	public sealed partial class EyeIcon : UserControl, IIconUserControl {

		public EyeIcon () => InitializeComponent ();

		public static readonly DependencyProperty IconWidthProperty =
					DependencyProperty.Register (
						"IconWidth" ,
						typeof ( double ) ,
						typeof ( EyeIcon ) ,
						new PropertyMetadata ( 0 , IconWidthChangedHandler )
				);

		private static void IconWidthChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is EyeIcon iconControl ) ) return;

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
						typeof ( EyeIcon ) ,
						new PropertyMetadata ( 0 , IconHeightChangedHandler )
				);

		private static void IconHeightChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is EyeIcon iconControl ) ) return;

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
								typeof ( EyeIcon ) ,
								new PropertyMetadata ( 0 , IconColorChangedHandler )
						);

		private static void IconColorChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is EyeIcon iconControl ) ) return;

			iconControl.IconViewboxPath.Fill = (Brush) e.NewValue;
		}

		public Brush IconColor
		{
			get => (Brush) GetValue ( IconColorProperty );
			set => SetValue ( IconColorProperty , value );
		}

		public static readonly DependencyProperty IconStrokeProperty =
							DependencyProperty.Register (
								"IconStroke" ,
								typeof ( Brush ) ,
								typeof ( EyeIcon ) ,
								new PropertyMetadata ( 0 , IconStrokeChangedHandler )
						);

		private static void IconStrokeChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is EyeIcon iconControl ) ) return;

			iconControl.IconViewboxPath.Stroke = (Brush) e.NewValue;
		}

		public Brush IconStroke
		{
			get => (Brush) GetValue ( IconStrokeProperty );
			set => SetValue ( IconStrokeProperty , value );
		}

		public static readonly DependencyProperty IconStrokeThicknessProperty =
							DependencyProperty.Register (
								"IconStrokeThickness" ,
								typeof ( double ) ,
								typeof ( EyeIcon ) ,
								new PropertyMetadata ( 0 , IconStrokeThicknessChangedHandler )
						);

		private static void IconStrokeThicknessChangedHandler ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			if ( !( d is EyeIcon iconControl ) ) return;

			iconControl.IconViewboxPath.StrokeThickness = (double) e.NewValue;
		}

		public double IconStrokeThickness
		{
			get => (double) GetValue ( IconStrokeThicknessProperty );
			set => SetValue ( IconStrokeThicknessProperty , value );
		}

	}

}
