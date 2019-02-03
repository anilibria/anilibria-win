using System;
using System.Linq;
using HtmlAgilityPack;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Anilibria.Converters {

	/// <summary>
	/// Generate Xaml content based on Html.
	/// </summary>
	public class TextBlockHtmlConverter {

		public static readonly DependencyProperty HtmlContentProperty =
			DependencyProperty.RegisterAttached (
				"HtmlContent" ,
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
			textBlock.Inlines.Add (
				new Run {
					Text = "Описание " ,
					FontWeight = FontWeights.Bold
				}
			);

			string formattedText = (string) e.NewValue ?? string.Empty;
			if ( string.IsNullOrEmpty ( formattedText ) ) return;

			formattedText = HtmlEntity.DeEntitize ( formattedText );

			var doc = new HtmlDocument ();
			doc.LoadHtml ( formattedText );

			foreach ( var childNode in doc.DocumentNode.ChildNodes ) ProcessInline ( null , childNode , textBlock.Inlines );
		}

		private static void ProcessInline ( Span parent , HtmlNode node , InlineCollection collection ) {
			InlineCollection parentCollection = parent != null ? parent.Inlines : collection;
			switch ( node.NodeType ) {
				case HtmlNodeType.Text:
					parentCollection.Add (
						new Run {
							Text = HtmlEntity.DeEntitize ( node.InnerText )
						}
					);
					break;
				case HtmlNodeType.Element:
					switch ( node.Name ) {
						case "br":
							//don't need LineBreak because Inlines already breaking lines
							return;
						case "script":
							//Tag script don't supported.
							return;
						case "b":
						case "strong":
							var bold = new Bold ();
							parentCollection.Add ( bold );

							foreach ( var boldChild in node.ChildNodes ) ProcessInline ( bold , boldChild , collection );

							return;
						case "u":
							var underline = new Underline ();
							parentCollection.Add ( underline );

							foreach ( var underlineChild in node.ChildNodes ) ProcessInline ( underline , underlineChild , collection );

							return;
						case "a":
							var url = node.Attributes.FirstOrDefault ( a => a.Name.ToLower () == "href" );
							if ( url == null ) return;

							var hyperlink = new Hyperlink {
								NavigateUri = new Uri ( url.Value )
							};
							parentCollection.Add ( hyperlink );

							foreach ( var underlineChild in node.ChildNodes ) ProcessInline ( hyperlink , underlineChild , collection );

							return;
						default: return;
					}
				default: return;
			}


		}

	}

}
