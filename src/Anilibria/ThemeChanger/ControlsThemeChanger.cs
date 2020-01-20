using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Anilibria.ThemeChanger
{

	/// <summary>
	/// Controls theme changer.
	/// </summary>
	public static class ControlsThemeChanger
	{

		public const string DefaultTheme = "default";

		public const string DarkTheme = "dark";

		private static string m_ThemeName = DefaultTheme;

		private static List<SubscribeItemModel> m_Subscribers = new List<SubscribeItemModel> ();

		private static Dictionary<string , Brush> m_DefaultBrushes = new Dictionary<string , Brush> {
			["TextBlockForeground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 0 , 0 , 0 ) ) ,
			["MainBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 230 , 230 , 230 ) ) ,
			["PanelBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 158 , 35 , 35 ) ) ,
			["UpperPanel"] = new SolidColorBrush ( Color.FromArgb ( 255 , 179 , 179 , 179 ) ) ,
			["HumburgerBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 186 , 45 , 45 ) ) ,
			["ReleaseGridBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 242 , 242 , 242 ) ) ,
			["ImageBorderColor"] = new SolidColorBrush ( Color.FromArgb ( 255 , 173 , 173 , 173 ) ) ,
			["TextBlock"] = new SolidColorBrush ( Color.FromArgb ( 255 , 0 , 0 , 0 ) ) ,
			["TextBlockAccent"] = new SolidColorBrush ( Color.FromArgb ( 255 , 179 , 33 , 33 ) ) ,
			["TextBlockMenuOption"] = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) ) ,
			["TextBlockHighlight"] = new SolidColorBrush ( Color.FromArgb ( 255 , 163 , 39 , 39 ) ) ,
			["ReleaseCardBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 242 , 242 , 242 ) ) ,
			["AccentBorderColor"] = new SolidColorBrush ( Color.FromArgb ( 255 , 179 , 33 , 33 ) ) ,
		};

		private static Dictionary<string , Brush> m_DarkBrushes = new Dictionary<string , Brush> {
			["TextBlockForeground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) ) ,
			["MainBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 31 , 31 , 31 ) ) ,
			["PanelBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 38 , 38 , 38 ) ) ,
			["UpperPanel"] = new SolidColorBrush ( Color.FromArgb ( 255 , 61 , 61 , 61 ) ) ,
			["HumburgerBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 84 , 84 , 84 ) ) ,
			["ReleaseGridBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 71 , 71 , 71 ) ) ,
			["ImageBorderColor"] = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) ) ,
			["TextBlock"] = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) ) ,
			["TextBlockAccent"] = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) ) ,
			["TextBlockMenuOption"] = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) ) ,
			["TextBlockHighlight"] = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) ) ,
			["ReleaseCardBackground"] = new SolidColorBrush ( Color.FromArgb ( 255 , 84 , 84 , 84 ) ) ,
			["AccentBorderColor"] = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) ) ,
		};

		/// <summary>
		/// Change theme
		/// </summary>
		/// <param name="themeName">Theme name.</param>
		public static void ChangeTheme ( string themeName ) {
			m_ThemeName = themeName;

			foreach ( var subscriber in m_Subscribers ) subscriber.Handler ( themeName , subscriber.Element );
		}

		/// <summary>
		/// Current theme.
		/// </summary>
		public static string CurrentTheme () => m_ThemeName;

		/// <summary>
		/// Get theme resource.
		/// </summary>
		/// <param name="name">Name.</param>
		public static Brush GetThemeResource ( string name ) {
			switch ( m_ThemeName ) {
				case "default":
					return m_DefaultBrushes[name];
				case "dark":
					return m_DarkBrushes[name];
			}

			return new SolidColorBrush ( Color.FromArgb ( 255 , 0 , 0 , 0 ) );
		}

		/// <summary>
		/// Register new subscriber.
		/// </summary>
		/// <param name="handler">Subscriber.</param>
		public static void RegisterSubscriber ( string name , DependencyObject element , Action<string , DependencyObject> handler ) {
			if ( m_Subscribers.Any ( a => a.Element == element && a.Name == name ) ) return;

			m_Subscribers.Add (
				new SubscribeItemModel {
					Name = name ,
					Element = element ,
					Handler = handler
				}
			);
		}

		/// <summary>
		/// Unregister subscriber.
		/// </summary>
		/// <param name="subscriber">Subscriber.</param>
		public static void UnRegisterSubscriber ( DependencyObject element ) {
			var allSubscribes = m_Subscribers
				.Where ( a => a.Element == element )
				.ToList ();
			foreach ( var subscribe in allSubscribes ) {
				m_Subscribers.Remove ( subscribe );
			}
		}

	}

}
