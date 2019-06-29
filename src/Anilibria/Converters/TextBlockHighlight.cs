using System.Collections.Generic;
using Anilibria.ThemeChanger;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Anilibria.Converters {
	public class TextBlockHighlight {

		public static readonly DependencyProperty HyperLinkHightlightProperty =
			DependencyProperty.RegisterAttached (
				"HyperLinkHightlight" ,
				typeof ( string ) ,
				typeof ( TextBlockHtmlConverter ) ,
				new PropertyMetadata ( null , HyperLinkHightlightChanged )
		);

		private static void HyperLinkHightlightChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			var textBlock = d as TextBlock;
			if ( textBlock == null ) return;

			if ( (bool) e.NewValue ) {
				textBlock.PointerEntered += TextBlock_PointerEntered;
				textBlock.PointerExited += TextBlock_PointerExited;
				textBlock.PointerMoved += TextBlock_PointerMoved;
			}
			else {
				textBlock.PointerEntered -= TextBlock_PointerEntered;
				textBlock.PointerExited -= TextBlock_PointerExited;
				textBlock.PointerMoved -= TextBlock_PointerMoved;
			}
		}

		private static void FillHyperlinks ( IList<Hyperlink> hyperlinks , InlineCollection inlines ) {
			if ( inlines == null || inlines.Count == 0 ) return;

			foreach ( var inline in inlines ) {
				var hyperlink = inline as Hyperlink;
				if ( hyperlink != null ) hyperlinks.Add ( hyperlink );

				var span = inline as Span;
				if ( span != null ) FillHyperlinks ( hyperlinks , span.Inlines );
			}
		}

		private static void TextBlock_PointerMoved ( object sender , PointerRoutedEventArgs e ) {
			var textBlock = sender as TextBlock;
			if ( textBlock == null ) return;

			ChangeColorForHyperlinks ( textBlock );
		}

		private static void ChangeColorForHyperlinks ( TextBlock textBlock ) {
			Brush brush = null;
			switch ( ControlsThemeChanger.CurrentTheme () ) {
				case ControlsThemeChanger.DefaultTheme:
					brush = new SolidColorBrush ( Color.FromArgb ( 255 , 163 , 39 , 39 ) );
					break;
				case ControlsThemeChanger.DarkTheme:
					brush = new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) );
					break;
			}

			var hyperlinks = new List<Hyperlink> ();
			FillHyperlinks ( hyperlinks , textBlock.Inlines );
			foreach ( var hyperlink in hyperlinks ) hyperlink.Foreground = brush;
		}

		private static void TextBlock_PointerExited ( object sender , PointerRoutedEventArgs e ) {
			var textBlock = sender as TextBlock;
			if ( textBlock == null ) return;

			ChangeColorForHyperlinks ( textBlock );
		}

		private static void TextBlock_PointerEntered ( object sender , PointerRoutedEventArgs e ) {
			var textBlock = sender as TextBlock;
			if ( textBlock == null ) return;

			ChangeColorForHyperlinks ( textBlock );
		}

		public static void SetHyperLinkHightlight ( DependencyObject textBlock , bool value ) {
			textBlock.SetValue ( HyperLinkHightlightProperty , value );
		}

		public static bool GetHyperLinkHightlight ( DependencyObject textBlock ) {
			return (bool) textBlock.GetValue ( HyperLinkHightlightProperty );
		}

	}
}
