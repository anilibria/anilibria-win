using System;
using System.Collections.Generic;
using System.Linq;
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

		private const string m_SessionName = "PHPSESSID";

		/// <summary>
		/// Get page from releases.
		/// </summary>
		/// <param name="page">Page number.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Release's collection.</returns>
		public async Task<IEnumerable<Release>> GetPage ( int page , int pageSize , string name = default ( string ) ) {
			var cookieContainer = new CookieContainer ();
			var handler = new HttpClientHandler { CookieContainer = cookieContainer };

			var parameters = new List<KeyValuePair<string , string>> {
				new KeyValuePair<string , string> ( "query" , string.IsNullOrEmpty ( name ) ? "list" : "search" ),
				new KeyValuePair<string , string> ( "page" , page.ToString () ),
				new KeyValuePair<string , string> ( "perPage" , pageSize.ToString () )
			};
			if ( !string.IsNullOrEmpty ( name ) ) parameters.Add ( new KeyValuePair<string , string> ( "search" , name ) );

			var formContent = new FormUrlEncodedContent ( parameters );
			//cookieContainer.Add ( new Uri ( m_WebSiteUrl ) , new Cookie ( m_SessionName , "cookie_value" ) ); <-- user session
			var httpClient = new HttpClient ( handler );
			var result = await httpClient.PostAsync ( m_ApiReleasesUrl , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			IEnumerable<Release> releases = null;
			if ( string.IsNullOrEmpty ( name ) ) {
				var responseModel = JsonConvert.DeserializeObject<ApiResponse<PagingList>> ( content );
				if ( !responseModel.Status ) {
					//TODO: handle error
				}

				releases = responseModel.Data.Items;
			} else {
				var responseModel = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<Release>>> ( content );
				if ( !responseModel.Status ) {
					//TODO: handle error
				}

				releases = responseModel.Data;
			}

			foreach ( var item in releases ) item.Type = FormatHtml ( item.Type );

			return releases;
		}

		/// <summary>
		/// Authentification by email and password.
		/// </summary>
		/// <param name="email">User email.</param>
		/// <param name="password">User password.</param>
		public async Task<bool> Authentification ( string email , string password ) {
			var cookieContainer = new CookieContainer ();
			var handler = new HttpClientHandler { CookieContainer = cookieContainer };

			var formContent = new FormUrlEncodedContent (
				new[]
				{
					new KeyValuePair<string, string>("mail", email),
					new KeyValuePair<string, string>("passwd", password)
				}
			);

			var httpClient = new HttpClient ( handler );
			var result = await httpClient.PostAsync ( m_ApiLoginUrl , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			var cookies = cookieContainer.GetCookies ( new Uri ( m_WebSiteUrl ) ).Cast<Cookie> ();
			var sessionCookie = cookies.FirstOrDefault ( a => a.Name == m_SessionName );

			var sessionId = sessionCookie.Value;

			return true;
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
		private string FormatHtml ( string content ) => content.Replace ( "&gt;" , ">" ).Replace ( "&lt;" , "<" ).Replace ( "&quot;" , "\"" );

	}

}
