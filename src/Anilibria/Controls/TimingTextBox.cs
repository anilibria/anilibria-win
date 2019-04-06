using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Anilibria.Controls {

	/// <summary>
	/// Timing text box.
	/// </summary>
	public class TimingTextBox : TextBox {

		DispatcherTimer m_Timer = new DispatcherTimer ();

		public TimingTextBox () {
			m_Timer.Interval = TimeSpan.FromMilliseconds ( 500 );
			m_Timer.Tick += CommandTimerTick;

			TextChanging += TimingTextBox_TextChanging;
		}

		private void TimingTextBox_TextChanging ( TextBox sender , TextBoxTextChangingEventArgs args ) {
			//refresh timer
			if ( m_Timer.IsEnabled ) {
				m_Timer.Stop ();
				m_Timer.Start ();
			}
			else {
				m_Timer.Start ();
			}
		}

		private void CommandTimerTick ( object sender , object e ) {
			m_Timer.Stop ();

			GetTimeoutCommand ( this )?.Execute ( null );
		}

		/// <summary>
		/// Is open property.
		/// </summary>
		public static readonly DependencyProperty TimeoutCommandProperty =
			DependencyProperty.RegisterAttached (
				"TimeoutCommand" ,
				typeof ( ICommand ) ,
				typeof ( TimingTextBox ) ,
				new PropertyMetadata ( true )
			);

		public static void SetTimeoutCommand ( DependencyObject element , ICommand value ) => element.SetValue ( TimeoutCommandProperty , value );

		public static ICommand GetTimeoutCommand ( DependencyObject element ) => (ICommand) element.GetValue ( TimeoutCommandProperty );

	}

}
