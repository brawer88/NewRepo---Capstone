using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class KrogerAPI {
		const String client_secret = "FZ0osbSCdA7k00PGmMzHXOetq8cA0BjIIBNQg7pn";
		const String client_id = "reciplease-cff94f4f6041dea129095d1b936180016290439862649948138";

		public static string GetKrogerAuth() {
			string AuthToken = client_id + ":" + client_secret;

			AuthToken = Base64Encode( AuthToken );

			var client = new RestClient( "https://api.kroger.com/v1/connect/oauth2/authorize?scope=profile.compact,cart.basic:write,product.compact&response_type=code&client_id=" + client_id + "&redirect_uri=http://itd1.cincinnatistate.edu/cpdm-wernkeb/Cart/AuthCode" );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "Accept", "application/json" );
			request.AddHeader( "Authorization", AuthToken );
			IRestResponse response = client.Execute( request );

			return response.Content;
		}

		public static string Base64Encode( string plainText ) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes( plainText );
			return System.Convert.ToBase64String( plainTextBytes );
		}

		public String GetKrogerUserID( String authCode ) {

			var client = new RestClient( "https://api.kroger.com/v1/identity/profile" );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "Accept", "application/json" );
			request.AddHeader( "Authorization", "{{TOKEN}}" );
			IRestResponse response = client.Execute( request );

			return response.Content;
		}


	}
}