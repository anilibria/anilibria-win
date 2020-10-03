using System;
using System.Threading.Tasks;
using Anilibria.GlobalState;
using Anilibria.Pages.DiagnosticsPage;
using Anilibria.Pages.ProtocolsPages;
using Anilibria.Services.Implementations;
using Anilibria.Services.PresentationClasses;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#if !DEBUG
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
#endif

namespace Anilibria {

	/// <summary>
	/// AniLibria application.
	/// </summary>
	sealed partial class App : Application {

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App () {
			InitializeComponent ();
			Suspending += OnSuspending;
#if !DEBUG
			AppCenter.Start ( AnalyticsService.TrackKey , typeof ( Analytics ) , typeof ( Crashes ) );
#endif
		}

		protected override void OnActivated ( IActivatedEventArgs args ) {
			if ( args.Kind != ActivationKind.Protocol ) return;

			var isNotStarted = args.PreviousExecutionState == ApplicationExecutionState.NotRunning || args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser;
			if ( isNotStarted ) TransitionToFullScreen ();

			var eventArgs = args as ProtocolActivatedEventArgs;
			var uri = eventArgs.Uri;

			switch ( uri.Host.ToLowerInvariant () ) {
				case "diagnosticspage":
					TransitionToDiagnosticsPage ();
					break;
				case "changeapipath":
					var query = uri.PathAndQuery;

					var newUrl = query.Replace ( "/?path=" , "" );
					ApplicationData.Current.LocalSettings.Values[AnilibriaApiService.ApiPathSettingName] = newUrl;

					if ( isNotStarted ) {
						TransitionToChangeApiPath ( newUrl );
					} else {
						var frame = (Frame) Window.Current.Content;
						var homeView = ( frame.Content as HomeView );
						homeView.SetApiPath ( newUrl );
					}

					break;
				default:
					if ( isNotStarted ) Exit ();
					break;
			}
		}

		private void TransitionToChangeApiPath ( string url ) {
			if ( Window.Current.Content == null ) Window.Current.Content = new Frame ();

			var frame = (Frame) Window.Current.Content;
			frame.Navigate ( typeof ( ChangeApiView ) , url );
			Window.Current.Activate ();
		}

		private void TransitionToDiagnosticsPage () {
			if ( Window.Current.Content == null ) Window.Current.Content = new Frame ();

			var frame = (Frame) Window.Current.Content;
			frame.Navigate ( typeof ( DiagnosticsPageView ) );
			Window.Current.Activate ();
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override async void OnLaunched ( LaunchActivatedEventArgs e ) {
			//if app started on xbox then increase screen size on full screen.
			TransitionToFullScreen ();

			if ( e.PreviousExecutionState != ApplicationExecutionState.Suspended && e.PreviousExecutionState != ApplicationExecutionState.Running ) await PopulateFirstStartReleases ();

			LaunchParameters.SetArguments ( e.Arguments );

			var rootFrame = Window.Current.Content as Frame;

			if ( rootFrame == null ) {
				rootFrame = new Frame ();

				Window.Current.Content = rootFrame;
				await new JumpListService ().RefreshPagesGroup ();
			}

			if ( e.PrelaunchActivated == false ) {
				if ( rootFrame.Content == null ) rootFrame.Navigate ( typeof ( HomeView ) , e.Arguments );
				Window.Current.Activate ();
			}
		}

		private static void TransitionToFullScreen () {
			if ( SystemService.GetDeviceFamilyType () == DeviceFamilyType.Xbox ) ApplicationView.GetForCurrentView ().SetDesiredBoundsMode ( ApplicationViewBoundsMode.UseCoreWindow );
		}

		private async Task PopulateFirstStartReleases () {
			await ReleaseSingletonService.LoadReleases ();

			//don't wait for release sync because it may take longer than expected
#pragma warning disable CS4014
			new SynchronizeService ( ApiService.Current () , StorageService.Current () , ReleaseSingletonService.Current () ).SynchronizeReleases ();
#pragma warning restore CS4014
		}

		/// <summary>
		/// Invoked when application execution is being suspended. Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending ( object sender , SuspendingEventArgs e ) {
			var deferral = e.SuspendingOperation.GetDeferral ();
			//TODO: Save application state and stop any background activity
			deferral.Complete ();
		}

	}

}
