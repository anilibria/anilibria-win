namespace Anilibria.Storage.Entities {

	/// <summary>
	/// Entity for keep video last position and watch mark.
	/// </summary>
	public class VideoStateEntity {

		/// <summary>
		/// Video identifier.
		/// </summary>
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Last position.
		/// </summary>
		public double LastPosition
		{
			get;
			set;
		}

		/// <summary>
		/// Already seen or not.
		/// </summary>
		public bool IsSeen
		{
			get;
			set;
		}

	}

}
