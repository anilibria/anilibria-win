using Anilibria.ThemeChanger;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

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

		public static readonly DependencyProperty LinkCommandProperty =
			DependencyProperty.RegisterAttached (
				"LinkCommand" ,
				typeof ( ICommand ) ,
				typeof ( TextBlockHtmlConverter ) ,
				new PropertyMetadata ( null )
		);

		public static void SetLinkCommand ( DependencyObject textBlock , ICommand value ) => textBlock.SetValue ( LinkCommandProperty , value );

		public static ICommand GetLinkCommand ( DependencyObject textBlock ) => (ICommand) textBlock.GetValue ( LinkCommandProperty );

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

			foreach ( var childNode in doc.DocumentNode.ChildNodes ) ProcessInline ( null , childNode , textBlock.Inlines , textBlock );
		}

		private static Brush GetThemeBrush () {
			switch ( ControlsThemeChanger.CurrentTheme () ) {
				case ControlsThemeChanger.DefaultTheme:
					return new SolidColorBrush ( Color.FromArgb ( 255 , 163 , 39 , 39 ) );
				case ControlsThemeChanger.DarkTheme:
					return new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) );
				default:
					return new SolidColorBrush ( Color.FromArgb ( 255 , 255 , 255 , 255 ) );
			}
		}

		private static void ProcessInline ( Span parent , HtmlNode node , InlineCollection collection , TextBlock root ) {
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

							foreach ( var boldChild in node.ChildNodes ) ProcessInline ( bold , boldChild , collection , root );

							return;
						case "u":
							var underline = new Underline ();
							parentCollection.Add ( underline );

							foreach ( var underlineChild in node.ChildNodes ) ProcessInline ( underline , underlineChild , collection , root );

							return;
						case "a":
							var url = node.Attributes.FirstOrDefault ( a => a.Name.ToLower () == "href" );
							if ( url == null ) return;

							Hyperlink hyperlink = null;

							if ( !string.IsNullOrEmpty ( url.Value ) && ( url.Value.StartsWith ( "https://www.anilibria.tv/release/" ) || url.Value.StartsWith ( "http://www.anilibria.tv/release/" ) ) ) {
								var urlValue = url.Value;
								hyperlink = new Hyperlink {
									Foreground = GetThemeBrush ()
								};
								hyperlink.Click += ( sender , args ) => {
									var linkCommand = GetLinkCommand ( root );
									if ( linkCommand != null ) linkCommand.Execute ( urlValue );
								};
							}
							else {
								hyperlink = new Hyperlink {
									NavigateUri = new Uri ( url.Value ) ,
									Foreground = GetThemeBrush ()
								};
							}
							parentCollection.Add ( hyperlink );

							foreach ( var underlineChild in node.ChildNodes ) ProcessInline ( hyperlink , underlineChild , collection , root );

							return;
						default: return;
					}
				default: return;
			}


		}

	}

}
