using System;
using Anilibria.Services.Implementations;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Pages.ProtocolsPages {

	/// <summary>
	/// Change api control.
	/// </summary>
	public sealed partial class ChangeApiControl : UserControl {

		private bool m_NeedCloseApplicationOnCancel = false;

		public ChangeApiControl () => InitializeComponent ();

		private async void Apply ( object sender , RoutedEventArgs e ) {
			var result = await new ContentDialog {
				Title = "Смена адреса api" ,
				Content = "Изменения этой настройки будет применено только после перезапуска приложения, если Вы смените настройку приложение закроется и его надо заново запустить, Вы уверены?" ,
				PrimaryButtonText = "Сменить" ,
				CloseButtonText = "Отмена"
			}.ShowAsync ();

			if ( result != ContentDialogResult.Primary ) return;

			ApplicationData.Current.LocalSettings.Values[AnilibriaApiService.ApiPathSettingName] = ApiAddress.Text;

			Application.Current.Exit ();
		}

		private void Cancel ( object sender , RoutedEventArgs e ) {
			if ( m_NeedCloseApplicationOnCancel ) {
				Application.Current.Exit ();
			} else {
				Visibility = Visibility.Collapsed;
			}
		}

		public void SetApiAddress ( string url ) {
			ApiAddress.Text = url;
		}

		public void SetNeedCloseApplicationOnCancel ( bool isNeed ) {
			m_NeedCloseApplicationOnCancel = isNeed;
		}

	}

}
