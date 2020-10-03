using System.Threading.Tasks;

namespace Anilibria.Services.Implementations {

	public static class ReleaseSingletonService {

		private static ReleasesService m_ReleasesService = new ReleasesService ();

		public static Task LoadReleases () => m_ReleasesService.LoadReleases ();

		public static IReleasesService Current () => m_ReleasesService;
	}
}
