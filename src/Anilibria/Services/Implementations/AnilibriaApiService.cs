using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Anilibria.Services.PresentationClasses;
using Newtonsoft.Json;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Service for receiving data from the anilibria api site.
	/// </summary>
	public class AnilibriaApiService : IAnilibriaApiService {

		private const string m_ApiUrl = "https://dev.anilibria.tv/public/";

		private static bool IsBase64String ( string value ) {
			value = value.Trim ();
			return ( value.Length % 4 == 0 ) && Regex.IsMatch ( value , @"^[a-zA-Z0-9\+/]*={0,3}$" , RegexOptions.None );
		}

		private string ConvertFromBase64 ( string content ) {
			if ( !IsBase64String ( content ) ) return content;

			var bytes = Convert.FromBase64String ( content );
			return Encoding.UTF8.GetString ( bytes );
		}

		public async Task<IEnumerable<Release>> GetCatalog () {
			var formContent = new FormUrlEncodedContent (
				new[]
				{
					new KeyValuePair<string, string>("query", "info"),
					new KeyValuePair<string, string>("json", ""),
				}
			);

			var httpClient = new HttpClient ();
			var result = await httpClient.PostAsync ( m_ApiUrl + "/api/index.php" , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			var releases = JsonConvert.DeserializeObject<IEnumerable<Release>> ( content );

			foreach ( var release in releases ) {
				release.Description = ConvertFromBase64 ( release.Description );
				release.Type = ConvertFromBase64 ( release.Type );

				release.Name = release.Name.Select ( a => ConvertFromBase64 ( a ) );
			}

			return releases;
		}

	}

}
