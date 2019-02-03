namespace Anilibria.Pages {

	/// <summary>
	/// Navigation.
	/// </summary>
	public interface INavigation {

		/// <summary>
		/// Start navigate to page.
		/// </summary>
		/// <param name="parameter">Parameter.</param>
		void NavigateTo ( object parameter );

		/// <summary>
		/// End navigate to page.
		/// </summary>
		void NavigateFrom ();

	}

}
