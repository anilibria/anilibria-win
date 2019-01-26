using System;
using System.Collections.Generic;
using System.Net;
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

		private const string m_WebSiteUrl = "https://test.anilibria.tv";

		private const string m_ContentWebSiteUrl = "https://dev.anilibria.tv";

		private const string m_ImageUploadUrl = m_WebSiteUrl + "/upload/release/";

		private const string m_ApiReleasesUrl = m_WebSiteUrl + "/public/api/index.php";

		private const string m_ApiLoginUrl = m_WebSiteUrl + "/public/login.php";

		private static bool IsBase64String ( string value ) {
			value = value.Trim ();
			return ( value.Length % 4 == 0 ) && Regex.IsMatch ( value , @"^[a-zA-Z0-9\+/]*={0,3}$" , RegexOptions.None );
		}

		private string ConvertFromBase64 ( string content ) {
			if ( !IsBase64String ( content ) ) return content;

			var bytes = Convert.FromBase64String ( content );
			return Encoding.UTF8.GetString ( bytes );
		}

		/// <summary>
		/// Get page from releases.
		/// </summary>
		/// <param name="page">Page number.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Release's collection.</returns>
		public async Task<IEnumerable<Release>> GetPage ( int page , int pageSize ) {
			var cookieContainer = new CookieContainer ();
			var handler = new HttpClientHandler { CookieContainer = cookieContainer };

			var formContent = new FormUrlEncodedContent (
				new[]
				{
					new KeyValuePair<string, string>("query", "list"),
					new KeyValuePair<string, string>("page", page.ToString()),
					new KeyValuePair<string, string>("perPage", pageSize.ToString()),
				}
			);
			//cookieContainer.Add ( new Uri ( m_WebSiteUrl ) , new Cookie ( "PHPSESSID" , "cookie_value" ) ); <-- user session
			var httpClient = new HttpClient ( handler );
			var result = await httpClient.PostAsync ( m_ApiReleasesUrl , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			var releases = JsonConvert.DeserializeObject<ApiResponse<PagingList>> ( content );

			if ( !releases.Status ) {
				//TODO: handle error
			}

			foreach ( var item in releases.Data.Items ) {
				item.Type = FormatHtml ( item.Type );
			}

			return releases.Data.Items;
		}

		/// <summary>
		/// Get url.
		/// </summary>
		/// <param name="relativeUrl">Relative url.</param>
		/// <returns>Full url.</returns>
		public Uri GetUrl ( string relativeUrl ) => new Uri ( m_ContentWebSiteUrl + relativeUrl );

		/// <summary>
		/// Format html content.
		/// </summary>
		/// <param name="content">Content.</param>
		/// <returns>Formatted content.</returns>
		private string FormatHtml ( string content ) => content.Replace ( "&gt;" , ">" ).Replace ( "&lt;" , "<" );

	}

}
