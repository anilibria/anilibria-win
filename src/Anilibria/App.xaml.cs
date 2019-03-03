using System;
using System.Threading.Tasks;
using Anilibria.Services.Implementations;
using Anilibria.Services.PresentationClasses;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
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

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override async void OnLaunched ( LaunchActivatedEventArgs e ) {
			//if app started on xbox then increase screen size on full screen.
			if (SystemService.GetDeviceFamilyType() == DeviceFamilyType.Xbox) ApplicationView.GetForCurrentView ().SetDesiredBoundsMode ( ApplicationViewBoundsMode.UseCoreWindow );

			await PopulateFirstStartReleases ();

			var rootFrame = Window.Current.Content as Frame;

			if ( rootFrame == null ) {
				rootFrame = new Frame ();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if ( e.PreviousExecutionState == ApplicationExecutionState.Terminated ) {
					//TODO: Load state from previously suspended application
				}

				Window.Current.Content = rootFrame;
			}

			if ( e.PrelaunchActivated == false ) {
				if ( rootFrame.Content == null ) rootFrame.Navigate ( typeof ( HomeView ) , e.Arguments );
				Window.Current.Activate ();
			}
		}

		private async Task PopulateFirstStartReleases () {
			await new SynchronizeService ( ApiService.Current () , StorageService.Current () ).SynchronizeReleases ();
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed ( object sender , NavigationFailedEventArgs e ) => throw new Exception ( "Failed to load Page " + e.SourcePageType.FullName );

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
