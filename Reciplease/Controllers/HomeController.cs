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

			CartMappedToKrogerUPC cart = new CartMappedToKrogerUPC( );

			Item item = new Item( );
			item.upc = "0001111041700";
			item.quantity = 2;


			cart.addItem( item );
			item.upc = "0004900002891";
			item.quantity = 2;
			cart.addItem( item );

			item.upc = "0007283000100";
			item.quantity = 2;
			cart.addItem( item );


			item.upc = "0074236526480";
			item.quantity = 2;
			cart.addItem( item );


			item.upc = "0004310033106";
			item.quantity = 2;
			cart.addItem( item );



			KrogerAPI.AddToKrogerCart( cart.convertToJson() );


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