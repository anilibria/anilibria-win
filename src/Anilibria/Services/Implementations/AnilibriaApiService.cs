using System;
using System.Collections.Generic;
using System.IO;
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

		private const string m_WebSiteUrl = "https://www.anilibria.tv";

		private const string m_ImageUploadUrl = m_WebSiteUrl + "/upload/release/";

		private const string m_ApiIndexUrl = m_WebSiteUrl + "/public/api/index.php";

		private const string m_ApiLoginUrl = m_WebSiteUrl + "/public/login.php";

		private const string m_ApiLogoutUrl = m_WebSiteUrl + "/public/logout.php";

		private const string m_SessionName = "PHPSESSID";

		private const string SessionIdName = "SessionId";

		private HttpClient m_HttpClient;

		private HttpClientHandler m_HttpHandler;

		private string m_SessionId = null;

		private UserModel m_UserModel = null;


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
				var responseModel = JsonConvert.DeserializeObject<ApiResponse<PagingList<Release>>> ( content );
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
		/// Get youtube videos.
		/// </summary>
		/// <param name="page">Page number.</param>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Youtube videos.</returns>
		public async Task<IEnumerable<YoutubeModel>> GetYoutubeVideosPage ( int page , int pageSize ) {
			var parameters = new List<KeyValuePair<string , string>> {
				new KeyValuePair<string , string> ( "query" , "youtube" ),
				new KeyValuePair<string , string> ( "page" , page.ToString () ),
				new KeyValuePair<string , string> ( "perPage" , pageSize.ToString () )
			};

			var formContent = new FormUrlEncodedContent ( parameters );
			var httpClient = new HttpClient ();
			var result = await httpClient.PostAsync ( m_ApiIndexUrl , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			var responseModel = JsonConvert.DeserializeObject<ApiResponse<PagingList<YoutubeModel>>> ( content );
			if ( !responseModel.Status ) {
				//TODO: handle error
			}

			foreach ( var item in responseModel.Data.Items ) item.Title = HtmlEntity.DeEntitize ( item.Title ); //Type can be contains html special characters

			return responseModel.Data.Items;
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

			m_UserModel = userModel.Data;

			return m_UserModel;
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
		public Uri GetUrl ( string relativeUrl ) => new Uri ( m_WebSiteUrl + relativeUrl );

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
			if ( sessionId == null ) m_UserModel = null;
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

		/// <summary>
		/// Get user favorites.
		/// </summary>
		/// <returns>Favorites releases collection.</returns>
		public async Task<IEnumerable<long>> GetUserFavorites () {
			var parameters = new List<KeyValuePair<string , string>> {
				new KeyValuePair<string , string> ( "query" , "favorites" ),
				new KeyValuePair<string , string> ( "filter" , "id" ),
				new KeyValuePair<string , string> ( "page" , "1" ),
				new KeyValuePair<string , string> ( "perPage" , "1000" ) // I guess it enough :)
			};

			var formContent = new FormUrlEncodedContent ( parameters );
			var result = await m_HttpClient.PostAsync ( m_ApiIndexUrl , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			var responseModel = JsonConvert.DeserializeObject<ApiResponse<PagingList<FavoriteModel>>> ( content );
			if ( !responseModel.Status ) {
				//TODO: handle error
			}

			return responseModel.Data.Items?.Select ( a => a.Id ).ToList () ?? Enumerable.Empty<long> ();
		}

		/// <summary>
		/// Add to user favorites.
		/// </summary>
		public async Task AddUserFavorites ( long id ) {
			await PerformActionFavorite ( id , "add" );
		}

		/// <summary>
		/// Delete from user favorites.
		/// </summary>
		public async Task RemoveUserFavorites ( long id ) {
			await PerformActionFavorite ( id , "delete" );
		}

		/// <summary>
		/// Perform action on favorite.
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="action">Action.</param>
		private async Task PerformActionFavorite ( long id , string action ) {
			var parameters = new List<KeyValuePair<string , string>> {
				new KeyValuePair<string , string> ( "query" , "favorites" ),
				new KeyValuePair<string , string> ( "id" , id.ToString() ),
				new KeyValuePair<string , string> ( "action" , action )
			};

			var formContent = new FormUrlEncodedContent ( parameters );
			var result = await m_HttpClient.PostAsync ( m_ApiIndexUrl , formContent );
			var content = await result.Content.ReadAsStringAsync ();

			var responseModel = JsonConvert.DeserializeObject<ApiResponse<PagingList<FavoriteModel>>> ( content );
			if ( !responseModel.Status ) {
				//TODO: handle error
			}
		}

		/// <summary>
		/// Get user model.
		/// </summary>
		public UserModel GetUserModel () => m_UserModel;

		/// <summary>
		/// Download torrent.
		/// </summary>
		/// <param name="torrentUri">Torrent uri.</param>
		/// <returns>Torrent path.</returns>
		public async Task<StorageFile> DownloadTorrent ( string torrentUri ) {
			var storageFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync ( "release.torrent" , CreationCollisionOption.GenerateUniqueName );
			var byteArray = await m_HttpClient.GetByteArrayAsync ( m_WebSiteUrl + torrentUri );

			using ( Stream stream = await storageFile.OpenStreamForWriteAsync () ) {
				stream.Write ( byteArray , 0 , byteArray.Length );
			}

			return storageFile;
		}

	}

}
