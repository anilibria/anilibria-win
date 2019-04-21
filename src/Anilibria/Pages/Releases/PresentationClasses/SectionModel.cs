namespace Anilibria.Pages.Releases.PresentationClasses {

	/// <summary>
	/// Section model.
	/// </summary>
	public class SectionModel {

		/// <summary>
		/// Title.
		/// </summary>
		public string Title
		{
			get;
			set;
		}

		/// <summary>
		/// Type.
		/// </summary>
		public SectionType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Sorting mode.
		/// </summary>
		public SortingItemType SortingMode {
			get;
			set;
		}

		/// <summary>
		/// Sorting direction.
		/// </summary>
		public SortingDirectionType SortingDirection {
			get;
			set;
		}

	}

}
