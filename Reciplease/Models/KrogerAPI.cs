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
			string scope = HttpUtility.UrlEncode( "cart.basic:write product.compact" );
			var client = new RestClient( "https://api.kroger.com/v1/connect/oauth2/authorize?scope="+ scope + "&response_type=code&client_id=" + client_id + "&redirect_uri=http://itd1.cincinnatistate.edu/cpdm-wernkeb/Cart/AuthCode" );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "Accept", "application/json" );
			IRestResponse response = client.Execute( request );

			string url = response.ResponseUri.AbsoluteUri.ToString( );

			return url;
		}

		public static AuthCodes GetKrogerToken( string strAuthCode ) {
			string AuthToken = client_id + ":" + client_secret;
			string scope = HttpUtility.UrlEncode( "cart.basic:write product.compact" );
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

		public static AuthCodes RefreshKrogerToken( string refresh ) {
			string AuthToken = client_id + ":" + client_secret;
			string scope = HttpUtility.UrlEncode( "cart.basic:write product.compact" );
			AuthToken = Base64Encode( AuthToken );

			var client = new RestClient( "https://api.kroger.com/v1/connect/oauth2/token" );
			var request = new RestRequest( Method.POST );
			request.AddParameter( "grant_type", "refresh_token" );
			request.AddParameter( "scope", scope );
			request.AddParameter( "refresh_token", refresh );
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

		internal static CartMappedToKrogerUPC GetKrogerUPCS( List<Ingredient> list ) {
			CartMappedToKrogerUPC upcs = new CartMappedToKrogerUPC( );

			foreach ( Ingredient i in list )
			{
				clsItem item = new clsItem( );
				item = GetItemFromKroger( i.name, i.amount, i.unit );
				if (item.quantity != 0 )
				{
					upcs.addItem( item );
				}
				
			}

			return upcs;
					   
		}

		private static clsItem GetItemFromKroger( string name, string amount, string quant ) {
			User user = new User( );

			user = user.GetUserSession( );

			string accessToken = user.KrogerAuthTokens.access_token;

			var client = new RestClient( "https://api.kroger.com/v1/products?filter.term=" + name );
			var request = new RestRequest( Method.GET );
			
			request.AddHeader( "Accept", "application/json" );

			request.AddHeader( "Authorization", "Bearer " + accessToken );

			IRestResponse response = client.Execute( request );			

			KrogerSearchResults mySearchItems = JsonConvert.DeserializeObject<KrogerSearchResults>( response.Content );

			clsItem item = new clsItem( );

			double dblFinalDifference = 1000;
			if ( mySearchItems.data != null )
			{
				foreach ( Datum d in mySearchItems.data )
				{
					Item i = d.items[0];

					if ( quant == "pinch" || quant == "pinches" )
					{
						quant = "teaspoon";
						amount = ( double.Parse( amount ) * .0125 ).ToString( );
					}

					// split amount
					string[] amounts = i.size.Split( ' ' );

					if ( amounts[0].Contains( '-' ) )
					{
						string[] newAmount = amounts[0].Split( '-' );

						if ( double.TryParse( newAmount[1], out _ ) )
						{
							amounts[0] = ( double.Parse( newAmount[0] ) * double.Parse( newAmount[1] ) ).ToString( );
						}
						else
						{
							amounts = newAmount;
						}

					}

					if ( amounts[0].Contains( '/' ) )
					{
						string[] newAmount = amounts[0].Split( '/' );

						amounts[0] = ( double.Parse( newAmount[0] ) / double.Parse( newAmount[1] ) ).ToString( );
					}

					if ( amounts[0].Contains( '\\' ) )
					{
						string[] newAmount = amounts[0].Split( '\\' );

						amounts[0] = ( double.Parse( newAmount[0] ) / double.Parse( newAmount[1] ) ).ToString( );
					}
					if ( amounts.Length > 1 )
					{
						if ( amounts[1].Equals( "gal" ) )
						{
							amounts[1] = "gallon";
						}
					}

					if ( mySearchItems.data.Count == 1 )
					{
						item.upc = d.upc;
						item.quantity = 1;
					}

					else if ( amounts.Length == 2 || amounts.Length == 3 )
					{
						if ( amounts.Length == 3 )
						{
							if ( amounts[0] == "case" )
							{
								string[] newAmounts = amounts[1].Split( );
								amounts[0] = amounts[2];
								amounts[1] = "piece";

							}
							else
							{
								amounts[1] = amounts[1] + " " + amounts[2];
							}
							
						}

						if ( quant.Length == 0 )
						{
							quant = "piece";
						}
						if ( quant == "bottles" )
						{
							quant = "ml";
							amount = ( double.Parse( amount ) * 750 ).ToString( );
						}

						// convert ingredient to kroger amount
						double newAmount = RecipeAPI.ConvertAmounts( quant, name, amounts[0], amounts[1] );

						double dblDifference = 0;
						dblDifference = newAmount - double.Parse( amount );

						//if ( dblFinalDifference > dblDifference && dblDifference > -.005)
						if ( dblFinalDifference > dblDifference )
						{
							if ( double.Parse( amount ) <= newAmount )
							{
								item.upc = d.upc;
								item.quantity = 1;
								dblFinalDifference = dblDifference;
							}
							else if ( double.Parse( amounts[0] ) * 2 >= newAmount )
							{
								item.upc = d.upc;
								item.quantity = 2;
								dblFinalDifference = double.Parse( amounts[0] ) * 2;
							}
							else if ( double.Parse( amounts[0] ) * 3 >= newAmount )
							{
								item.upc = d.upc;
								item.quantity = 3;
								dblFinalDifference = double.Parse( amounts[0] ) * 3;
							}
							else if ( double.Parse( amounts[0] ) * 4 >= newAmount )
							{
								item.upc = d.upc;
								item.quantity = 4;
								dblFinalDifference = double.Parse( amounts[0] ) * 4;
							}
							else if ( double.Parse( amounts[0] ) * 5 >= newAmount )
							{
								item.upc = d.upc;
								item.quantity = 5;
								dblFinalDifference = double.Parse( amounts[0] ) * 5;
							}
							else if ( double.Parse( amounts[0] ) * 6 >= newAmount )
							{
								item.upc = d.upc;
								item.quantity = 6;
								dblFinalDifference = double.Parse( amounts[0] ) * 6;
							}
							else if ( double.Parse( amounts[0] ) * 7 >= newAmount )
							{
								item.upc = d.upc;
								item.quantity = 7;
								dblFinalDifference = double.Parse( amounts[0] ) * 7;
							}
							else if ( double.Parse( amounts[0] ) * 8 >= newAmount )
							{
								item.upc = d.upc;
								item.quantity = 8;
								dblFinalDifference = double.Parse( amounts[0] ) * 8;
							}
						}


					}

				}
			}

			return item;

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

			string accessToken = user.KrogerAuthTokens.access_token;

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