using System;
using System.Linq;
using Anilibria.Services.PresentationClasses;
using Windows.Devices.Input;
using Windows.Gaming.Input;
using Windows.System.Profile;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Service for access OS routines.
	/// </summary>
	public static class SystemService {

		public static DeviceFamilyType GetDeviceFamilyType () {
			var deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;

			switch ( deviceFamily ) {
				case "Windows.Desktop":
					return DeviceFamilyType.WindowsDesktop;
				case "Windows.Mobile":
					return DeviceFamilyType.WindowsMobile;
				case "Windows.IoT":
					return DeviceFamilyType.IoT;
				case "Windows.Xbox":
					return DeviceFamilyType.Xbox;
				case "Windows.Holographic":
					return DeviceFamilyType.Holographic;
				default: throw new NotSupportedException ( $"Device family {deviceFamily} not supported." );
			}
		}

		/// <summary>
		/// Get keyboard allowed.
		/// </summary>
		private static bool IsKeyboardAllowed () => new KeyboardCapabilities ().KeyboardPresent != 0;

		/// <summary>
		/// Is gamepad allowed.
		/// </summary>
		private static bool IsGamePadAllowed () => Gamepad.Gamepads.Count > 0;

		/// <summary>
		/// Is mouse allowed.
		/// </summary>
		private static bool IsMouseAllowed () {
			var mouseCapabilities = new MouseCapabilities ();
			return mouseCapabilities.MousePresent != 0 && mouseCapabilities.NumberOfButtons >= 2;
		}

		/// <summary>
		/// Is touch input allowed.
		/// </summary>
		/// <returns></returns>
		private static bool IsTouchAllowed () => new TouchCapabilities ().TouchPresent != 0;

		/// <summary>
		/// Is pen allowed.
		/// </summary>
		private static bool IsPenAllowed () {
			var pointerDevices = PointerDevice.GetPointerDevices ();
			return pointerDevices.Any ( a => a.PointerDeviceType == PointerDeviceType.Pen );
		}

		/// <summary>
		/// Get input devices.
		/// </summary>
		public static DeviceInputType GetInputDevices () {
			DeviceInputType inputDevices = DeviceInputType.Unknown;

			if ( IsKeyboardAllowed () ) inputDevices = inputDevices | DeviceInputType.Keyboard;
			if ( IsGamePadAllowed () ) inputDevices = inputDevices | DeviceInputType.Gamepad;
			if ( IsMouseAllowed () ) inputDevices = inputDevices | DeviceInputType.Mouse;
			if ( IsTouchAllowed () ) inputDevices = inputDevices | DeviceInputType.Touch;
			if ( IsPenAllowed () ) inputDevices = inputDevices | DeviceInputType.Pen;

			return inputDevices;
		}

	}

}
