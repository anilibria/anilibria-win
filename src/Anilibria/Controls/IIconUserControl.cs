using Windows.UI.Xaml.Media;

namespace Anilibria.Controls {

	/// <summary>
	/// Inteface for direct icon user controls.
	/// </summary>
	public interface IIconUserControl {

		/// <summary>
		/// Icon width.
		/// </summary>
		double IconWidth {
			get;
			set;
		}

		/// <summary>
		/// Icon height.
		/// </summary>
		double IconHeight {
			get;
			set;
		}

		/// <summary>
		/// Icon color.
		/// </summary>
		Brush IconColor {
			get;
			set;
		}

		/// <summary>
		/// Icon color.
		/// </summary>
		Brush IconStroke {
			get;
			set;
		}

	}

}
