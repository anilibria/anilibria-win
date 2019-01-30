using HtmlAgilityPack;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Anilibria.Converters {

	public class TextBlockHtmlConverter {

		public static readonly DependencyProperty HtmlContentProperty =
			DependencyProperty.RegisterAttached (
				"FormattedText" ,
				typeof ( string ) ,
				typeof ( TextBlockHtmlConverter ) ,
				new PropertyMetadata ( null , HtmlContentPropertyChanged )
		);

		public static void SetHtmlContent ( DependencyObject textBlock , string value ) {
			textBlock.SetValue ( HtmlContentProperty , value );
		}

		public static string GetHtmlContent ( DependencyObject textBlock ) {
			return (string) textBlock.GetValue ( HtmlContentProperty );
		}

		private static void HtmlContentPropertyChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {

			TextBlock textBlock = d as TextBlock;
			textBlock.ClearValue ( TextBlock.TextProperty );
			textBlock.Inlines.Clear ();

			string formattedText = (string) e.NewValue ?? string.Empty;
			if ( string.IsNullOrEmpty ( formattedText ) ) return;

			var doc = new HtmlDocument ();
			doc.LoadHtml ( formattedText );

			foreach ( var childNode in doc.DocumentNode.ChildNodes ) {
				textBlock.Inlines.Add ( new Run () );
			}
		}

	}

}
