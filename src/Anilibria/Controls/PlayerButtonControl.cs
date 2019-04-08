using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Anilibria.Controls {

	/// <summary>
	/// Base for all online player buttons.
	/// </summary>
	public class PlayerButtonControl : Button {

		protected override void OnProcessKeyboardAccelerators ( ProcessKeyboardAcceleratorEventArgs args ) {

			if ( args.Key == VirtualKey.Space ) {
				args.Handled = true;
			}
			base.OnProcessKeyboardAccelerators ( args );
		}

	}

}
