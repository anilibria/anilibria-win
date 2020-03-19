using System;
using Anilibria.Services.Implementations;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Anilibria.Pages.AuthorizePage {

	/// <summary>
	/// Page contains form for authorize user.
	/// </summary>
	public sealed partial class AuthorizeView : UserControl {

		public AuthorizeView () {
			InitializeComponent ();

			DataContext = new AuthorizeViewModel ( ApiService.Current () , new AnalyticsService () );
		}

		private async void GridBackground_Loaded(object sender, RoutedEventArgs e)
		{
			var folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
			var file = await folder.GetFileAsync("authbackground.jpg");
			using (var stream = await file.OpenAsync(FileAccessMode.Read))
			{
				var bitmapImage = new BitmapImage();
				await bitmapImage.SetSourceAsync(stream);

				var decoder = await BitmapDecoder.CreateAsync(stream);

				GridBackground.Background = new ImageBrush
				{

					ImageSource = bitmapImage
				};
			}
		}
	}

}
