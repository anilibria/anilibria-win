using System.Collections.Generic;
using System.Collections.ObjectModel;
using Anilibria.Pages.Releases.PresentationClasses;

namespace Anilibria.Pages.Releases {

	/// <summary>
	/// Releases items for comboboxes and so on.
	/// </summary>
	public static class ReleasesItems {

		public static ObservableCollection<FavoriteMarkItem> GetFavoriteMarkItems () {
			var items = new List<FavoriteMarkItem> {
				new FavoriteMarkItem {
					Title = "Не используется",
					Type = FavoriteMarkType.NotUsed
				},
				new FavoriteMarkItem {
					Title = "В избранном",
					Type = FavoriteMarkType.Favorited
				},
				new FavoriteMarkItem {
					Title = "Не в избранном",
					Type = FavoriteMarkType.NotFavorited
				}
			};
			return new ObservableCollection<FavoriteMarkItem> ( items );
		}

		public static ObservableCollection<SeenMarkItem> GetSeenMarkItems () {
			var items = new List<SeenMarkItem> {
				new SeenMarkItem {
					Title = "Не используется",
					Type = SeenMarkType.NotUsed
				},
				new SeenMarkItem {
					Title = "Просмотренные",
					Type = SeenMarkType.Seen
				},
				new SeenMarkItem {
					Title = "Просматриваемые",
					Type = SeenMarkType.SeenNow
				},
				new SeenMarkItem {
					Title = "Не просмотренные",
					Type = SeenMarkType.NotSeen
				}
			};

			return new ObservableCollection<SeenMarkItem> ( items );
		}

	}

}
