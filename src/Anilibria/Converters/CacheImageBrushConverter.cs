using System;
using System.IO;
using System.Net.Http;
using Anilibria.Services.Implementations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Anilibria.Converters {

	/// <summary>
	/// Caching images in a local database and display on image brush canvas.
	/// </summary>
	public class CacheImageBrushConverter {

		public static readonly DependencyProperty ImagePathProperty =
			DependencyProperty.RegisterAttached (
				"ImagePath" ,
				typeof ( Uri ) ,
				typeof ( CacheImageBrushConverter ) ,
				new PropertyMetadata ( null , ImagePathPropertyChanged )
			);

		public static void SetImagePath ( DependencyObject dependencyProperty , Uri value ) => dependencyProperty.SetValue ( ImagePathProperty , value );

		public static Uri GetImagePath ( DependencyObject dependencyProperty ) => (Uri) dependencyProperty.GetValue ( ImagePathProperty );

		private static async void ImagePathPropertyChanged ( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
			ImageBrush image = d as ImageBrush;
			if ( image == null ) return;

			var releaseId = GetImageId ( d );
			var group = GetImageGroup ( d );

			var uri = e.NewValue as Uri;
			if ( uri == null ) return;

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

						image.ImageSource = bitmapImage;
					}
				}
				catch {
					image.ImageSource = null;
				}
			}
			else {
				try {
					var stream = dataContext.DownloadFile ( group , releaseId );

					var bitmapImage = new BitmapImage ();
					await bitmapImage.SetSourceAsync ( stream.AsRandomAccessStream () );

					image.ImageSource = bitmapImage;
				}
				catch {
					image.ImageSource = null;
				}
			}
		}


		public static readonly DependencyProperty ImageGroupProperty =
			DependencyProperty.RegisterAttached (
				"ImageGroup" ,
				typeof ( string ) ,
				typeof ( CacheImageBrushConverter ) ,
				new PropertyMetadata ( null )
		);

		public static void SetImageGroup ( DependencyObject dependencyProperty , string value ) => dependencyProperty.SetValue ( ImageGroupProperty , value );

		public static string GetImageGroup ( DependencyObject dependencyProperty ) => (string) dependencyProperty.GetValue ( ImageGroupProperty );

		public static readonly DependencyProperty ImageIdProperty =
			DependencyProperty.RegisterAttached (
				"ImageId" ,
				typeof ( long ) ,
				typeof ( CacheImageBrushConverter ) ,
				new PropertyMetadata ( -1L )
		);

		public static void SetImageId ( DependencyObject dependencyProperty , long value ) => dependencyProperty.SetValue ( ImageIdProperty , value );

		public static long GetImageId ( DependencyObject dependencyProperty ) => (long) dependencyProperty.GetValue ( ImageIdProperty );

	}

}
