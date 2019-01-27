using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anilibria.Collections;
using Anilibria.MVVM;
using Anilibria.Pages.Releases.PresentationClasses;
using Anilibria.Services;
using Anilibria.Services.PresentationClasses;

namespace Anilibria.Pages.Releases {

	/// <summary>
	/// Release view model.
	/// </summary>
	public class ReleasesViewModel : ViewModel {

		private bool m_IsMultipleSelect;

		private IncrementalLoadingCollection<ReleaseModel> m_Collection;

		private readonly IAnilibriaApiService m_AnilibriaApiService;

		/// <summary>
		/// Constructor injection.
		/// </summary>
		/// <param name="anilibriaApiService">Anilibria Api Service.</param>
		public ReleasesViewModel ( IAnilibriaApiService anilibriaApiService ) {
			m_AnilibriaApiService = anilibriaApiService ?? throw new ArgumentNullException ( nameof ( anilibriaApiService ) );
		}

		/// <summary>
		/// Initialize view model.
		/// </summary>
		public void Initialize () {

			RefreshGroups ();
		}

		/// <summary>
		/// Refresh groups.
		/// </summary>
		private void RefreshGroups () {
			m_Collection = new IncrementalLoadingCollection<ReleaseModel> {
				PageSize = 20 ,
				GetPageFunction = GetItemsPageAsync
			};
			RaisePropertyChanged ( () => Collection );

			//RaiseSelectableCommands ();
		}

		private string GetStatus ( StatusType statusType ) {
			switch ( statusType ) {
				case StatusType.Unknown:
					return "Неизвестно";
				case StatusType.InWorking:
					return "В работе";
				case StatusType.Finished:
					return "Завершен";
				default: throw new NotSupportedException ( $"Status type {statusType} not supported." );
			}
		}

		/// <summary>
		/// Get items page.
		/// </summary>
		/// <param name="page">Page.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Items on current page.</returns>
		private async Task<IEnumerable<ReleaseModel>> GetItemsPageAsync ( int page , int pageSize ) {
			//TODO: network error handling
			var releases = await m_AnilibriaApiService.GetPage ( page , pageSize );

			return releases.Select (
				a => new ReleaseModel {
					Id = a.Id ,
					AddToFavorite = a.Favorite?.Added ?? false ,
					Code = a.Code ,
					Description = a.Description ,
					Genres = string.Join ( ", " , a.Genres ) ,
					Title = a.Names.FirstOrDefault () ,
					Names = a.Names ,
					Poster = m_AnilibriaApiService.GetUrl ( a.Poster.Replace ( "default" , a.Id.ToString () ) ) ,
					PosterFull = m_AnilibriaApiService.GetUrl ( a.PosterFull.Replace ( "default" , a.Id.ToString () ) ) ,
					Rating = a.Favorite.Rating ,
					Series = a.Series ,
					Status = GetStatus ( a.Status ) ,
					Type = a.Type ,
					Voices = string.Join ( ", " , a.Voices ) ,
					Year = a.Year
				}
			);
		}

		/// <summary>
		/// Collection.
		/// </summary>
		public IncrementalLoadingCollection<ReleaseModel> Collection
		{
			get => m_Collection;
			set => Set ( ref m_Collection , value );
		}

		/// <summary>
		/// Is multiple select.
		/// </summary>
		public bool IsMultipleSelect
		{
			get => m_IsMultipleSelect;
			set => Set ( ref m_IsMultipleSelect , value );
		}

	}

}
