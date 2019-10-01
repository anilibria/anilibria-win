using System;
using System.IO;
using System.Net.Http;
using Anilibria.Services.Implementations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Anilibria.Converters {

	/// <summary>
	/// Caching images in a local database.
	/// </summary>
	public class CacheImageConverter {

		public static readonly DependencyProperty ImagePathProperty =
			DependencyProperty.RegisterAttached (
				"ImagePath" ,
				typeof ( Uri ) ,
				typeof ( CacheImageConverter ) ,
				new PropertyMetadata ( null , ImagePathPropertyChanged )
		);

		public static void SetImagePath ( DependencyObject dependencyProperty , Uri value ) => dependencyProperty.SetValue ( ImagePathProperty , value );

		public static Uri GetImagePath ( DependencyObject dependencyProperty ) => (Uri) dependencyProperty.GetValue ( ImagePathProperty );

		private static async void ImagePathPropertyChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			Image image = d as Image;
			if ( image == null ) return;
			if ( image.Tag == null ) return;

			var releaseId = (long) image.Tag;
			var group = GetImageGroup ( d );

			var uri = e.NewValue as Uri;
			if ( uri == null ) throw new InvalidCastException ( "Must be Uri class for image source." );

			var dataContext = StorageService.Current ();

			if ( !dataContext.IsFileExists ( group , releaseId ) ) {
				try {
					var byteArray = await new HttpClient ().GetByteArrayAsync ( uri );

					using ( Stream stream = new MemoryStream () ) {
						stream.Write ( byteArray , 0 , byteArray.Length );
						stream.Position = 0;

						dataContext.UploadFile ( group , releaseId , stream );

						stream.Position = 0;
						var bitmapImage = new BitmapImage ();
						await bitmapImage.SetSourceAsync ( stream.AsRandomAccessStream () );

						image.Source = bitmapImage;
					}
				} catch {
					image.Source = null;
				}
			}
			else {
				try {
					var stream = dataContext.DownloadFile ( group , releaseId );

					var bitmapImage = new BitmapImage ();
					await bitmapImage.SetSourceAsync ( stream.AsRandomAccessStream () );

					image.Source = bitmapImage;
				} catch {
					image.Source = null;
				}
			}
		}


		public static readonly DependencyProperty ImageGroupProperty =
			DependencyProperty.RegisterAttached (
				"ImageGroup" ,
				typeof ( string ) ,
				typeof ( CacheImageConverter ) ,
				new PropertyMetadata ( null )
		);

		public static void SetImageGroup ( DependencyObject dependencyProperty , string value ) => dependencyProperty.SetValue ( ImageGroupProperty , value );

		public static string GetImageGroup ( DependencyObject dependencyProperty ) => (string) dependencyProperty.GetValue ( ImageGroupProperty );



	}

}
