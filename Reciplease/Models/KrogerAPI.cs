using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Reciplease.Models {
	public class KrogerAPI {
		const String client_secret = APIConfig.KrogerSecret;
		const String client_id = "reciplease-cff94f4f6041dea129095d1b936180016290439862649948138";

		public static string GetKrogerAuth() {

			var client = new RestClient( "https://api.kroger.com/v1/connect/oauth2/authorize?scope=profile.compact&response_type=code&client_id=" + client_id + "&redirect_uri=http://itd1.cincinnatistate.edu/cpdm-wernkeb/Cart/AuthCode" );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "Accept", "application/json" );
			IRestResponse response = client.Execute( request );

			string url = response.ResponseUri.AbsoluteUri.ToString( );

			return url;
		}

		public static AuthCodes GetKrogerToken( string strAuthCode ) {
			string AuthToken = client_id + ":" + client_secret;

			AuthToken = Base64Encode( AuthToken );

			var client = new RestClient( "https://api.kroger.com/v1/connect/oauth2/token" );
			var request = new RestRequest( Method.POST );
			request.AddParameter( "grant_type", "authorization_code" );
			request.AddParameter( "scope", "cart.basic:write" );
			request.AddParameter( "code", strAuthCode );
			request.AddParameter( "redirect_uri", "http://itd1.cincinnatistate.edu/cpdm-wernkeb/Cart/AuthCode" );
			request.AddHeader( "Content-Type", "application/x-www-form-urlencoded" );
			request.AddHeader( "Authorization", "Basic " + AuthToken );
			IRestResponse response = client.Post( request );

			AuthCodes auths = null;
			// deserialize object
			if ( response.StatusCode == HttpStatusCode.OK )
			{
				auths = JsonConvert.DeserializeObject<AuthCodes>( response.Content );
			}

			return auths;
		}


		public static string Base64Encode( string plainText ) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes( plainText );
			return System.Convert.ToBase64String( plainTextBytes );
		}

		public static string GetKrogerUserID( String authCode ) {

			var client = new RestClient( "https://api.kroger.com/v1/identity/profile" );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "Accept", "application/json" );
			request.AddHeader( "Authorization", authCode );
			IRestResponse response = client.Execute( request );

			return response.Content;
		}


	}

	public class CartItems {
		public List<CartItem> cartItems { get; set; }
	}

	public class CartItem {
		public int quantity { get; set; }
		public string upc { get; set; }
	}

	public class AuthCodes {
		public string access_token { get; set; }
		public string refresh_token { get; set; }
	}
}