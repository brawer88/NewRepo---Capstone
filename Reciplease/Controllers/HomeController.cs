using Newtonsoft.Json;
using Reciplease.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Reciplease.Controllers {
	public class HomeController : Controller {
		[HandleError]
		public ActionResult Index( ) {

			//CartMappedToKrogerUPC cart = new CartMappedToKrogerUPC( );

			//User u = new User( );
			//u.Username = "Brawer";
			//u.Password = "reciplease2";
			//u = u.Login( );
			//u.KrogerAuthTokens = new AuthCodes( );
			//u.KrogerAuthTokens.access_token = "eyJhbGciOiJSUzI1NiIsImprdSI6Imh0dHBzOi8vYXBpLmtyb2dlci5jb20vdjEvLndlbGwta25vd24vandrcy5qc29uIiwia2lkIjoiWjRGZDNtc2tJSDg4aXJ0N0xCNWM2Zz09IiwidHlwIjoiSldUIn0.eyJhdWQiOiJyZWNpcGxlYXNlLWNmZjk0ZjRmNjA0MWRlYTEyOTA5NWQxYjkzNjE4MDAxNjI5MDQzOTg2MjY0OTk0ODEzOCIsImV4cCI6MTYyODM0MzU5NiwiaWF0IjoxNjI4MzQxNzkxLCJpc3MiOiJhcGkua3JvZ2VyLmNvbSIsInN1YiI6ImQ5NzhlOGYyLTA5Y2MtNDk5My04NWRhLTQ5Njc3YzU3ZmViNCIsInNjb3BlIjoiY2FydC5iYXNpYzp3cml0ZSIsImF1dGhBdCI6MTYyODM0MTc5NjgwNjAwMjcwNSwicGZjeCI6InVybjprcm9nZXJjbzpwcm9maWxlOnNob3BwZXI6M2Q2OTJmMDctNWZlMy05MTc1LTcxNzYtZDYzZDI0MDgyNjFhIiwiYXpwIjoicmVjaXBsZWFzZS1jZmY5NGY0ZjYwNDFkZWExMjkwOTVkMWI5MzYxODAwMTYyOTA0Mzk4NjI2NDk5NDgxMzgifQ.Dc8JCEhmO_M5BPBncnUsGtlJ7mchYBGAlpFVbo11VfS7XDwrw6C1Oec21fGIRPV-gplqeybFHvf9FQky6bl0dtR_VkAdtUnkMIPJ2aQ9Z-IODaBZk4H-0NRXd0zT9pctUYpYg197rw6CGP0pJj2QFzhsaonzH63tE3STX8LZ_RL7aIEvM-GFk7VZCLhyPBUxJr9K4ruZYuVHPkHPEeZfvi_cdieVNdr2kUeksvrxhOS_WI7y9uO-2lyZSShuDi0vGcqH6vDRkHeG1yPFuisPSEQsU4S4Q5oxGMTbEvstT-UboaTHUjL4c0NU6ekJ5o8OkBQuILO3_Bjs886etwBRsw";
			//u.SaveUserSession( );
			//Cart c = new Cart( );
			//c = c.GetCartSession( );
			//Database db = new Database( );

			//c.list = db.GetShoppingList( u.UID );

			//c.ingredients = db.GetIngredients( c.list.intRecipeID.ToString( ) );

			//CartMappedToKrogerUPC upcs = KrogerAPI.GetKrogerUPCS( c.ingredients );
			//KrogerAPI.AddToKrogerCart( upcs.convertToJson( ) );

			//return Content( "<script language='javascript' type='text/javascript'>window.open('https://www.kroger.com/cart');location.reload();</script>" );

			RecipesList recipes = new RecipesList( );
			Database DB = new Database( );

			recipes.recipes = DB.GetTopDifficultyRatedRecipes( );
			
			Models.HomeContent h = new Models.HomeContent
			{
				// get recipes to display
				RecipesToDisplay = recipes
				
			};

			h.user = new Models.User( );
			h.user = h.user.GetUserSession( );


			return View( h );
		}

		[HandleError]
		public ActionResult Search( ) {

			Models.HomeContent h = new Models.HomeContent
			{
				user = new Models.User( )
			};
			h.user = h.user.GetUserSession( );

			return View( h );
		}

		[HandleError]
		[HttpPost]
		public ActionResult Search( FormCollection col ) {

			SearchItems searchItems = default( SearchItems );

			// added this items to the model in a struct so we can continue using same search items
			// ternary : if hidden element is null, use original search page element, else use hidden element
			
			if (col["btnSubmit"] == "close")
			{
				return RedirectToAction( "index" );
			}
			else // is the submit button or the next page button which does the same thing
			{
				searchItems.query = col["hquery"] == null ? col["SearchQuery"] : col["hquery"];
				searchItems.cuisine = col["hcuisine"] == null ? col["Cuisine"] : col["hcuisine"];
				searchItems.ingredients = col["hingredients"] == null ? col["Ingredients"] : col["hingredients"];
				searchItems.diets = col["hdiets"] == null ? col["Diets"] : col["hdiets"];
				searchItems.excludedIngredients = col["hexcludedIngredients"] == null ? col["ExcludedIngredients"] : col["hexcludedIngredients"];
				searchItems.intolerances = col["hintolerances"] == null ? col["Intolerances"] : col["hintolerances"];
				searchItems.type = col["htype"] == null ? col["Type"] : col["htype"];
				searchItems.index = col["hindex"] == null ? 0 : Convert.ToInt32( col["hindex"] ) + 1;

				// update to search model when that is created
				Models.HomeContent h = new Models.HomeContent
				{
					searchItems = searchItems,
					// get recipes to display
					SearchResults = RecipeAPI.RecipeSearch( searchItems.query, searchItems.cuisine, searchItems.ingredients,
															searchItems.diets, searchItems.excludedIngredients, searchItems.intolerances, searchItems.type, searchItems.index )
				
				};

				h.user = new Models.User( );
				h.user = h.user.GetUserSession( );

				return View( h );
			}

		}

		[HandleError]
		public ActionResult Recipe(  ) {
			Models.HomeContent h = new Models.HomeContent( );
			h.user = new Models.User( );
			h.user = h.user.GetUserSession( );
			

			Database DB = new Database( );

			h.user.Ratings = DB.GetUserRatings( h.user.UID );

			string strRecipeID = Convert.ToString( RouteData.Values["id"] );

			// check if recipe id exists
			if ( DB.RecipeExists( strRecipeID ) == false )
			{
				// get recipes to display
				h.SingleRecipe = RecipeAPI.GetRecipeById( strRecipeID );
				// save recipe
				int intSavedID = DB.SaveRecipe( h.SingleRecipe.title, h.SingleRecipe.instructions, int.Parse( h.SingleRecipe.readyInMinutes ), h.SingleRecipe.image, int.Parse( h.SingleRecipe.servings ),
					String.Join( ", ", h.SingleRecipe.cuisines ), String.Join( ", ", h.SingleRecipe.diets ), String.Join( ", ", h.SingleRecipe.dishTypes ), JsonConvert.SerializeObject( h.SingleRecipe.nutrition ), -1, int.Parse( h.SingleRecipe.id ) );

				foreach ( Ingredient ingredient in h.SingleRecipe.extendedIngredients )
				{
					if ( ingredient.name != null )
					{
						// now save ingredients
						int IngredientID = DB.SaveIngredient( int.Parse( ingredient.id ), ingredient.name );

						DB.AddRecipeIngredients( intSavedID, IngredientID, double.Parse( ingredient.amount ), ingredient.unit );
					}
				}


			}
			else
			{
				// load from db
				h.SingleRecipe = DB.LoadRecipe( strRecipeID );
			}

			return View( h );
		}

		// will update when procedures are in place
		[HandleError]
		[HttpPost]
		public JsonResult RateRecipe( int UID, string RecipeID, int intDifficultyRating, int intTasteRating ) {
			try
			{
				Models.Database db = new Models.Database( );
				int intReturn = 0;
				intReturn = db.RateRecipe( UID, int.Parse(RecipeID), intDifficultyRating, intTasteRating );
				return Json( new { Status = intReturn } );
			}
			catch ( Exception ex )
			{
				return Json( new { Status = -1 } ); //error
			}
		}

		[HandleError]
		[HttpPost]
		public ActionResult ToggleFavorite() {
			User u = new Models.User( );
			u = u.GetUserSession( );

			int result = u.ToggleFavorite( Convert.ToInt32( RouteData.Values["id"] ) );

			

			// update favorites list
				Database db = new Database( );

			u.Favorites = db.GetUserFavorites( u.UID );

			u.SaveUserSession( );
			if(result != -1 )
			{
				return Json( new
				{
					success = true,
					responseText = result
				}, JsonRequestBehavior.AllowGet );
			}
			else
			{
				return Json( new
				{
					success = false,
					responseText = result
				}, JsonRequestBehavior.AllowGet );
			}

		}

		[HandleError]
		public ActionResult About( ) {
			ViewBag.Message = "Your application description page.";

			return View( );
		}

		[HandleError]
		public ActionResult Contact( ) {
			ViewBag.Message = "Your contact page.";

			return View( );
		}

	}
}