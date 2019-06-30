using Anilibria.Controls;
using Anilibria.ThemeChanger;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
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
			var hyperlink = element as Hyperlink;
			if ( hyperlink != null ) {
				hyperlink.Foreground = brush;
				return;
			}

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

			SetMenuFlyoutStyle ( element , ControlsThemeChanger.CurrentTheme () );

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


		public static readonly DependencyProperty ActionButtonProperty =
			DependencyProperty.RegisterAttached (
				"ActionButton" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , ActionButtonChanged )
			);

		private static void SetActionButtonStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (Button) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.Style = (Style) App.Current.Resources["ActionButtonStyle"];
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.Style = (Style) App.Current.Resources["DarkActionButtonStyle"];
					break;
			}
		}

		private static void ActionButtonChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetActionButtonStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetActionButtonStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetActionButton ( DependencyObject actionButton , string value ) => actionButton.SetValue ( ActionButtonProperty , value );

		public static string GetActionButton ( DependencyObject actionButton ) => (string) actionButton.GetValue ( ActionButtonProperty );

		public static readonly DependencyProperty AnilibriaTextBoxProperty =
			DependencyProperty.RegisterAttached (
				"AnilibriaTextBox" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , AnilibriaTextBoxChanged )
			);

		private static void SetAnilibriaTextBoxStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (TextBox) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.Style = (Style) App.Current.Resources["AnilibriaTextBox"];
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.Style = (Style) App.Current.Resources["DarkAnilibriaTextBox"];
					break;
			}
		}

		private static void AnilibriaTextBoxChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetAnilibriaTextBoxStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetAnilibriaTextBoxStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetAnilibriaTextBox ( DependencyObject textbox , string value ) => textbox.SetValue ( AnilibriaTextBoxProperty , value );

		public static string GetAnilibriaTextBox ( DependencyObject textbox ) => (string) textbox.GetValue ( AnilibriaTextBoxProperty );

		public static readonly DependencyProperty CloseIconProperty =
			DependencyProperty.RegisterAttached (
				"CloseIcon" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , CloseIconChanged )
			);

		private static void SetCloseIconStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (CloseIcon) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 0 , 0 , 0 ) );
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) );
					break;
			}
		}

		private static void CloseIconChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetCloseIconStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetCloseIconStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetCloseIcon ( DependencyObject closeIcon , string value ) => closeIcon.SetValue ( CloseIconProperty , value );

		public static string GetCloseIcon ( DependencyObject closeIcon ) => (string) closeIcon.GetValue ( CloseIconProperty );

		public static readonly DependencyProperty FavoriteIconProperty =
			DependencyProperty.RegisterAttached (
				"FavoriteIcon" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , FavoriteIconChanged )
			);

		private static void SetFavoriteIconStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (FavoriteIcon) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 0 , 0 , 0 ) );
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) );
					break;
			}
		}

		private static void FavoriteIconChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetFavoriteIconStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetFavoriteIconStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetFavoriteIcon ( DependencyObject closeIcon , string value ) => closeIcon.SetValue ( FavoriteIconProperty , value );

		public static string GetFavoriteIcon ( DependencyObject closeIcon ) => (string) closeIcon.GetValue ( FavoriteIconProperty );

		public static readonly DependencyProperty RemoveFavoriteIconProperty =
			DependencyProperty.RegisterAttached (
				"RemoveFavoriteIcon" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , RemoveFavoriteIconChanged )
			);

		private static void SetRemoveFavoriteIconStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (FavoriteIcon) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 161 , 39 , 39 ) );
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 179 , 179 , 179 ) );
					break;
			}
		}

		private static void RemoveFavoriteIconChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetRemoveFavoriteIconStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetRemoveFavoriteIconStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetRemoveFavoriteIcon ( DependencyObject closeIcon , string value ) => closeIcon.SetValue ( RemoveFavoriteIconProperty , value );

		public static string GetRemoveFavoriteIcon ( DependencyObject closeIcon ) => (string) closeIcon.GetValue ( RemoveFavoriteIconProperty );


		public static readonly DependencyProperty ChatIconProperty =
			DependencyProperty.RegisterAttached (
				"ChatIcon" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , ChatIconChanged )
			);

		private static void SetChatIconStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (ChatIcon) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 0 , 0 , 0 ) );
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) );
					break;
			}
		}

		private static void ChatIconChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetChatIconStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetChatIconStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetChatIcon ( DependencyObject closeIcon , string value ) => closeIcon.SetValue ( ChatIconProperty , value );

		public static string GetChatIcon ( DependencyObject closeIcon ) => (string) closeIcon.GetValue ( ChatIconProperty );


		public static readonly DependencyProperty OpenEyeIconProperty =
			DependencyProperty.RegisterAttached (
				"OpenEyeIcon" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , OpenEyeIconChanged )
			);

		private static void SetOpenEyeIconStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (OpenEyeIcon) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 0 , 0 , 0 ) );
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.IconColor = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) );
					break;
			}
		}

		private static void OpenEyeIconChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetOpenEyeIconStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetOpenEyeIconStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetOpenEyeIcon ( DependencyObject closeIcon , string value ) => closeIcon.SetValue ( OpenEyeIconProperty , value );

		public static string GetOpenEyeIcon ( DependencyObject closeIcon ) => (string) closeIcon.GetValue ( OpenEyeIconProperty );

		public static readonly DependencyProperty AnilibriaComboBoxProperty =
			DependencyProperty.RegisterAttached (
				"AnilibriaComboBox" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , AnilibriaComboBoxChanged )
			);

		private static void SetAnilibriaComboBoxStyle ( DependencyObject element , string themeName ) {
			var menuFlyout = (ComboBox) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					menuFlyout.Style = (Style) App.Current.Resources["AnilibriaComboBox"];
					break;
				case ControlsThemeChanger.DarkTheme:
					menuFlyout.Style = (Style) App.Current.Resources["DarkAnilibriaComboBox"];
					break;
			}
		}

		private static void AnilibriaComboBoxChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetAnilibriaComboBoxStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetAnilibriaComboBoxStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetAnilibriaComboBox ( DependencyObject combobox , string value ) => combobox.SetValue ( AnilibriaComboBoxProperty , value );

		public static string GetAnilibriaComboBox ( DependencyObject combobox ) => (string) combobox.GetValue ( AnilibriaComboBoxProperty );

		public static readonly DependencyProperty AnilibriaSidebarProperty =
			DependencyProperty.RegisterAttached (
				"AnilibriaSidebar" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , AnilibriaSidebarChanged )
			);

		private static void SetAnilibriaSidebarStyle ( DependencyObject element , string themeName ) {
			var relativeElement = (RelativePanel) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					relativeElement.Style = (Style) App.Current.Resources["AnilibriaSidebarStyle"];
					break;
				case ControlsThemeChanger.DarkTheme:
					relativeElement.Style = (Style) App.Current.Resources["DarkAnilibriaSidebarStyle"];
					break;
			}
		}

		private static void AnilibriaSidebarChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetAnilibriaSidebarStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetAnilibriaSidebarStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetAnilibriaSidebar ( DependencyObject sidebar , string value ) => sidebar.SetValue ( AnilibriaSidebarProperty , value );

		public static string GetAnilibriaSidebar ( DependencyObject sidebar ) => (string) sidebar.GetValue ( AnilibriaSidebarProperty );

		public static readonly DependencyProperty AnilibriaSidebarListProperty =
			DependencyProperty.RegisterAttached (
				"AnilibriaSidebarList" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , AnilibriaSidebarListChanged )
			);

		private static void SetAnilibriaSidebarListStyle ( DependencyObject element , string themeName ) {
			var relativeElement = (ListView) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					relativeElement.Style = (Style) App.Current.Resources["AnilibriaSidebarList"];
					break;
				case ControlsThemeChanger.DarkTheme:
					relativeElement.Style = (Style) App.Current.Resources["DarkAnilibriaSidebarList"];
					break;
			}
		}

		private static void AnilibriaSidebarListChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetAnilibriaSidebarListStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetAnilibriaSidebarListStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetAnilibriaSidebarList ( DependencyObject sidebar , string value ) => sidebar.SetValue ( AnilibriaSidebarListProperty , value );

		public static string GetAnilibriaSidebarList ( DependencyObject sidebar ) => (string) sidebar.GetValue ( AnilibriaSidebarListProperty );

		public static readonly DependencyProperty AnilibriaPasswordBoxProperty =
			DependencyProperty.RegisterAttached (
				"AnilibriaPasswordBox" ,
				typeof ( string ) ,
				typeof ( BackgroundThemeConverter ) ,
				new PropertyMetadata ( null , AnilibriaPasswordBoxChanged )
			);

		private static void SetAnilibriaPasswordBoxStyle ( DependencyObject element , string themeName ) {
			var relativeElement = (PasswordBox) element;
			switch ( themeName ) {
				case ControlsThemeChanger.DefaultTheme:
					relativeElement.Style = (Style) App.Current.Resources["AnilibriaPasswordBox"];
					break;
				case ControlsThemeChanger.DarkTheme:
					relativeElement.Style = (Style) App.Current.Resources["DarkAnilibriaPasswordBox"];
					break;
			}
		}

		private static void AnilibriaPasswordBoxChanged ( DependencyObject element , DependencyPropertyChangedEventArgs e ) {
			var themeResourceName = e.NewValue.ToString ();

			SetAnilibriaPasswordBoxStyle ( element , ControlsThemeChanger.CurrentTheme () );

			ControlsThemeChanger.RegisterSubscriber (
				( string name ) => SetAnilibriaPasswordBoxStyle ( element , ControlsThemeChanger.CurrentTheme () )
			);
		}

		public static void SetAnilibriaPasswordBox ( DependencyObject sidebar , string value ) => sidebar.SetValue ( AnilibriaPasswordBoxProperty , value );

		public static string GetAnilibriaPasswordBox ( DependencyObject sidebar ) => (string) sidebar.GetValue ( AnilibriaPasswordBoxProperty );

	}

}
