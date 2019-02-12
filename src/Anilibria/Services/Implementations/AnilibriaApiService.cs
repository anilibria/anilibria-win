using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Anilibria.Services.Exceptions;
using Anilibria.Services.PresentationClasses;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Windows.Storage;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Service for receiving data from the anilibria api site.
	/// </summary>
	public class AnilibriaApiService : IAnilibriaApiService {

		private const string m_WebSiteUrl = "https://new.anilibria.tv";

		private const string m_ContentWebSiteUrl = "https://new.anilibria.tv"; //"https://dev.anilibria.tv";

		private const string m_ImageUploadUrl = m_WebSiteUrl + "/upload/release/";

		private const string m_ApiIndexUrl = m_WebSiteUrl + "/public/api/index.php";

		private const string m_ApiLoginUrl = m_WebSiteUrl + "/public/login.php";

		private const string m_ApiLogoutUrl = m_WebSiteUrl + "/public/logout.php";

		private const string m_SessionName = "PHPSESSID";

		private const string SessionIdName = "SessionId";

		private HttpClient m_HttpClient;

		private HttpClientHandler m_HttpHandler;

		private string m_SessionId = null;


		public AnilibriaApiService () {
			m_HttpHandler = new HttpClientHandler { CookieContainer = new CookieContainer () };
			m_HttpClient = new HttpClient ( m_HttpHandler );

			//restore session identifier.
			var settings = ApplicationData.Current.LocalSettings;
			m_SessionId = settings.Values[SessionIdName] as string;
			if ( !string.IsNullOrEmpty ( m_SessionId ) ) m_HttpHandler.CookieContainer.Add ( new Uri ( m_WebSiteUrl ) , new Cookie ( m_SessionName , m_SessionId ) );
		}

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
			var httpClient = new HttpClient ( handler );
			var result = await httpClient.PostAsync ( m_ApiIndexUrl , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			IEnumerable<Release> releases = null;
			if ( string.IsNullOrEmpty ( name ) ) {
				var responseModel = JsonConvert.DeserializeObject<ApiResponse<PagingList>> ( content );
				if ( !responseModel.Status ) {
					//TODO: handle error
				}

				releases = responseModel.Data.Items;
			}
			else {
				var responseModel = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<Release>>> ( content );
				if ( !responseModel.Status ) {
					//TODO: handle error
				}

				releases = responseModel.Data;
			}

			foreach ( var item in releases ) item.Type = HtmlEntity.DeEntitize ( item.Type ); //Type can be contains html special characters

			return releases;
		}

		/// <summary>
		/// Authentification by email and password.
		/// </summary>
		/// <param name="email">User email.</param>
		/// <param name="password">User password.</param>
		public async Task<bool> Authentification ( string email , string password ) {
			var formContent = new FormUrlEncodedContent (
				new[]
				{
					new KeyValuePair<string, string>("mail", email),
					new KeyValuePair<string, string>("passwd", password)
				}
			);

			var result = await m_HttpClient.PostAsync ( m_ApiLoginUrl , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			var model = JsonConvert.DeserializeObject<AuthorizationModel> ( content );

			if ( model.Err != "ok" ) return false;

			var cookies = m_HttpHandler.CookieContainer.GetCookies ( new Uri ( m_WebSiteUrl ) ).Cast<Cookie> ();
			var sessionCookie = cookies.FirstOrDefault ( a => a.Name == m_SessionName );

			SetSession ( sessionCookie.Value );

			return true;
		}

		/// <summary>
		/// Get user data.
		/// </summary>
		/// <returns>Information about user profile.</returns>
		public async Task<UserModel> GetUserData () {
			if ( m_SessionId == null ) return null; // this request only if user authorized!

			var formContent = new FormUrlEncodedContent (
				new[]
				{
					new KeyValuePair<string, string>("query", "user")
				}
			);

			var result = await m_HttpClient.PostAsync ( m_ApiIndexUrl , formContent );

			CheckSession ();

			var content = await result.Content.ReadAsStringAsync ();

			var userModel = JsonConvert.DeserializeObject<ApiResponse<UserModel>> ( content );

			return userModel.Data;
		}

		/// <summary>
		/// Is authorized.
		/// </summary>
		/// <returns></returns>
		public bool IsAuthorized () => !string.IsNullOrEmpty ( m_SessionId );

		/// <summary>
		/// Get url.
		/// </summary>
		/// <param name="relativeUrl">Relative url.</param>
		/// <returns>Full url.</returns>
		public Uri GetUrl ( string relativeUrl ) => new Uri ( m_ContentWebSiteUrl + relativeUrl );

		private void CheckSession () {
			var cookies = m_HttpHandler.CookieContainer.GetCookies ( new Uri ( m_WebSiteUrl ) ).Cast<Cookie> ();

			var sessionCookie = cookies.FirstOrDefault ( a => a.Name == m_SessionName );

			if ( sessionCookie == null || sessionCookie.Value == "deleted" ) {
				SetSession ( null );
				throw new AuthorizeDeletedException ();
			}
		}

		private void SetSession ( string sessionId ) {
			var settings = ApplicationData.Current.LocalSettings;
			settings.Values[SessionIdName] = sessionId;
			m_SessionId = sessionId;
		}

		/// <summary>
		/// Logout.
		/// </summary>
		public async Task Logout () {
			var formContent = new FormUrlEncodedContent (
				new[]
				{
					new KeyValuePair<string, string>("query", "-")
				}
			);

			await m_HttpClient.PostAsync ( m_ApiLogoutUrl , formContent );

			SetSession ( null );
		}

	}

}
