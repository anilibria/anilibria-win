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

	}

}
