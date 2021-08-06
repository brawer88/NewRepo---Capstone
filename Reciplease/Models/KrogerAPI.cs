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
			string scope = HttpUtility.UrlEncode( "cart.basic:write" );
			var client = new RestClient( "https://api.kroger.com/v1/connect/oauth2/authorize?scope="+ scope + "&response_type=code&client_id=" + client_id + "&redirect_uri=http://itd1.cincinnatistate.edu/cpdm-wernkeb/Cart/AuthCode" );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "Accept", "application/json" );
			IRestResponse response = client.Execute( request );

			string url = response.ResponseUri.AbsoluteUri.ToString( );

			return url;
		}

		public static AuthCodes GetKrogerToken( string strAuthCode ) {
			string AuthToken = client_id + ":" + client_secret;
			string scope = HttpUtility.UrlEncode( "cart.basic:write" );
			AuthToken = Base64Encode( AuthToken );

			var client = new RestClient( "https://api.kroger.com/v1/connect/oauth2/token" );
			var request = new RestRequest( Method.POST );
			request.AddParameter( "grant_type", "authorization_code" );
			request.AddParameter( "scope", scope );
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


		public static void AddToKrogerCart( string items ) {
			// get user auth token
			
			
			User user = new User( );
			
			user = user.GetUserSession( );

			string accessToken = "eyJhbGciOiJSUzI1NiIsImprdSI6Imh0dHBzOi8vYXBpLmtyb2dlci5jb20vdjEvLndlbGwta25vd24vandrcy5qc29uIiwia2lkIjoiWjRGZDNtc2tJSDg4aXJ0N0xCNWM2Zz09IiwidHlwIjoiSldUIn0.eyJhdWQiOiJyZWNpcGxlYXNlLWNmZjk0ZjRmNjA0MWRlYTEyOTA5NWQxYjkzNjE4MDAxNjI5MDQzOTg2MjY0OTk0ODEzOCIsImV4cCI6MTYyODI3MjMxMCwiaWF0IjoxNjI4MjcwNTA1LCJpc3MiOiJhcGkua3JvZ2VyLmNvbSIsInN1YiI6ImQ5NzhlOGYyLTA5Y2MtNDk5My04NWRhLTQ5Njc3YzU3ZmViNCIsInNjb3BlIjoiY2FydC5iYXNpYzp3cml0ZSIsImF1dGhBdCI6MTYyODI3MDUxMDI3NzMyOTgxMCwicGZjeCI6InVybjprcm9nZXJjbzpwcm9maWxlOnNob3BwZXI6M2Q2OTJmMDctNWZlMy05MTc1LTcxNzYtZDYzZDI0MDgyNjFhIiwiYXpwIjoicmVjaXBsZWFzZS1jZmY5NGY0ZjYwNDFkZWExMjkwOTVkMWI5MzYxODAwMTYyOTA0Mzk4NjI2NDk5NDgxMzgifQ.PiuDtiOWeMADmLQEl_-XtnKAzr0T5zMF_aaa7e8OQzx-CjtnJYMSCpLOpr1HsaB56dXJmX4SVuLTGjjqaM0i1-k26j-EQ9-RxE0y49kTuR0h_w3ulI_D9Fk6fPmmQHR0Jxv5Ygt5z0NWDtqQZLkhRUcywYlEyLOZ5nqN3FmQN_j6TE7q2tHXz8DlcLsNmAd48xfY9trsN2mRcfvW6_V4br6jfSJUDfoyTlH6n6QUvIQVG-gwO4tOXWDlDpBe1kdqJnTutv7Ff6rrHiGRLCu1TW5KtHvQrN3oB7940wngWSdFjo8gIUROjB9B1FXRmOeocSIm9JnbIdb6dZbl4f57dQ";

			var client = new RestClient( "https://api.kroger.com/v1/cart/add" );			
			var request = new RestRequest( Method.PUT );

			request.AddJsonBody( items );
			request.AddHeader( "Accept", "application/json" );
			
			request.AddHeader( "Authorization", "Bearer " + accessToken );

			

			IRestResponse response = client.Execute( request );

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