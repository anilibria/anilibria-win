using System;

namespace Anilibria.Services.PresentationClasses {

	/// <summary>
	/// Device input type.
	/// </summary>
	[Flags]
	public enum DeviceInputType {

		/// <summary>
		/// Unknown device type.
		/// </summary>
		Unknown = 0x0,

		/// <summary>
		/// Keyboard or similar analog.
		/// </summary>
		Keyboard = 0x1,

		/// <summary>
		/// Classic mouse device.
		/// </summary>
		Mouse = 0x2,

		/// <summary>
		/// Pen device.
		/// </summary>
		Pen = 0x4,

		/// <summary>
		/// Gamepad.
		/// </summary>
		Gamepad = 0x8,

		/// <summary>
		/// Touch.
		/// </summary>
		Touch = 0x10,

		/// <summary>
		/// Holographic controller?
		/// </summary>
		Holographic = 0x20,

	};

}
