using Anilibria.ThemeChanger;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Anilibria.Converters {

	/// <summary>
	/// Background theme converter.
	/// </summary>
	public class BackgroundThemeConverter {

		public static readonly DependencyProperty BackgroundMapperProperty =
			DependencyProperty.RegisterAttached (
				"BackgroundMapper" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , BackgroundMapperChanged )
		);

		private static void SetBackground ( DependencyObject element , Brush brush ) {
			var panel = element as Panel;
			if ( panel != null ) {
				panel.Background = brush;
				return;
			}

			var border = element as Border;
			if ( border != null ) {
				border.Background = brush;
				return;
			}
		}

		private static void BackgroundMapperChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetBackground ( element , ControlsThemeChanger.GetThemeResource ( themeResourceName ) );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => {
					SetBackground ( element , ControlsThemeChanger.GetThemeResource ( themeResourceName ) );
				}
			);
		}

		public static void SetBackgroundMapper ( DependencyObject textBlock , string value ) {
			textBlock.SetValue ( BackgroundMapperProperty , value );
		}

		public static string GetBackgroundMapper ( DependencyObject textBlock ) {
			return (string) textBlock.GetValue ( BackgroundMapperProperty );
		}

		private static void SetBorderBrush ( DependencyObject element , Brush brush ) {
			var border = element as Border;
			if ( border != null ) {
				border.BorderBrush = brush;
				return;
			}
		}

		public static readonly DependencyProperty BorderMapperProperty =
			DependencyProperty.RegisterAttached (
				"BorderMapper" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , BorderMapperChanged )
		);

		private static void BorderMapperChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetBorderBrush ( element , ControlsThemeChanger.GetThemeResource ( themeResourceName ) );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => {
					SetBorderBrush ( element , ControlsThemeChanger.GetThemeResource ( themeResourceName ) );
				}
			);
		}

		public static void SetBorderMapper ( DependencyObject textBlock , string value ) => textBlock.SetValue ( BorderMapperProperty , value );

		public static string GetBorderMapper ( DependencyObject textBlock ) => (string) textBlock.GetValue ( BorderMapperProperty );

		private static void SetTextColor ( DependencyObject element , Brush brush ) {
			var border = element as TextBlock;
			if ( border != null ) {
				border.Foreground = brush;
				return;
			}
		}

		public static readonly DependencyProperty TextMapperProperty =
			DependencyProperty.RegisterAttached (
				"TextMapper" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , TextMapperChanged )
		);

		private static void TextMapperChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetTextColor ( element , ControlsThemeChanger.GetThemeResource ( themeResourceName ) );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => {
					SetTextColor ( element , ControlsThemeChanger.GetThemeResource ( themeResourceName ) );
				}
			);
		}

		public static void SetTextMapper ( DependencyObject textBlock , string value ) => textBlock.SetValue ( TextMapperProperty , value );

		public static string GetTextMapper ( DependencyObject textBlock ) => (string) textBlock.GetValue ( TextMapperProperty );


	}

}
