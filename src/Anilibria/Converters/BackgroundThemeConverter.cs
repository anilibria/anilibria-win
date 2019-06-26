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

		public static readonly DependencyProperty MenuFlyoutProperty =
			DependencyProperty.RegisterAttached (
				"MenuFlyout" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , MenuFlyoutChanged )
			);

		private static void SetMenuFlyoutStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (MenuFlyout) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.MenuFlyoutPresenterStyle = (Style) App.Current.Resources["AnilibriaMenuFlyout"];
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.MenuFlyoutPresenterStyle = (Style) App.Current.Resources["DarkAnilibriaMenuFlyout"];
					break;
			}
			
		}

		private static void MenuFlyoutChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetMenuFlyoutStyle ( element , ControlsThemeChanger.CurrentTheme() );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetMenuFlyoutStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetMenuFlyout ( DependencyObject menuflyout , string value ) => menuflyout.SetValue ( MenuFlyoutProperty , value );

		public static string GetMenuFlyout ( DependencyObject menuflyout ) => (string) menuflyout.GetValue ( MenuFlyoutProperty );

		public static readonly DependencyProperty CommonFlyoutProperty =
			DependencyProperty.RegisterAttached (
				"CommonFlyout" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , CommonFlyoutChanged )
			);

		private static void SetFlyoutStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (Flyout) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.FlyoutPresenterStyle = (Style) App.Current.Resources["AnilibriaFlyout"];
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.FlyoutPresenterStyle = (Style) App.Current.Resources["DarkAnilibriaFlyout"];
					break;
			}
		}

		private static void CommonFlyoutChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetFlyoutStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetFlyoutStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetCommonFlyout ( DependencyObject flyout , string value ) => flyout.SetValue ( CommonFlyoutProperty , value );

		public static string GetCommonFlyout ( DependencyObject flyout ) => (string) flyout.GetValue ( CommonFlyoutProperty );


		public static readonly DependencyProperty MenuFlyoutItemProperty =
			DependencyProperty.RegisterAttached (
				"MenuFlyoutItem" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , MenuFlyoutItemChanged )
			);

		private static void SetMenuFlyoutItemStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (MenuFlyoutItem) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.Style = (Style) App.Current.Resources["AnilibriaMenuFlyoutItem"];
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.Style = (Style) App.Current.Resources["DarkAnilibriaMenuFlyoutItem"];
					break;
			}
		}

		private static void MenuFlyoutItemChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetMenuFlyoutItemStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetMenuFlyoutItemStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetMenuFlyoutItem ( DependencyObject flyout , string value ) => flyout.SetValue ( MenuFlyoutItemProperty , value );

		public static string GetMenuFlyoutItem ( DependencyObject flyout ) => (string) flyout.GetValue ( MenuFlyoutItemProperty );

		public static readonly DependencyProperty ToggleSwitchProperty =
			DependencyProperty.RegisterAttached (
				"ToggleSwitch" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , ToggleSwitchChanged )
			);

		private static void SetToggleSwitchStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (ToggleSwitch) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.Style = (Style) App.Current.Resources["AnilibriaToggleSwitch"];
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.Style = (Style) App.Current.Resources["DarkAnilibriaToggleSwitch"];
					break;
			}
		}

		private static void ToggleSwitchChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetToggleSwitchStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetToggleSwitchStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetToggleSwitch ( DependencyObject toggleSwitch , string value ) => toggleSwitch.SetValue ( ToggleSwitchProperty , value );

		public static string GetToggleSwitch ( DependencyObject toggleSwitch ) => (string) toggleSwitch.GetValue ( ToggleSwitchProperty );


	}

}
