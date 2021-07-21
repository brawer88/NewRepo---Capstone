﻿using Newtonsoft.Json;
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
		public ActionResult Index( ) {
			
			Models.HomeContent h = new Models.HomeContent
			{
				// get recipes to display
				RecipesToDisplay = RecipeAPI.Get5RandomAPIRecipes( )
				
			};

			h.User = new Models.User( );
			h.User = h.User.GetUserSession( );

			return View( h );
		}

		public ActionResult Search( ) {
			// testing testing testing
			//Database DB = new Database( );

			//User u = new User( );

			//u.Username = "Brawer";
			//u.Password = "reciplease2";

			//u = u.Login( );

			//string authcode = "bj0XRkqi8BMPfzs5BLm8c2gUp2jIgrkbrcApyYwF";

			//u.SaveUserSession( );
			//return RedirectToAction( "KrogerSignIn", "Cart" );
			//return RedirectToAction( "AuthCode", "Cart", new { id = authcode } );



			Models.HomeContent h = new Models.HomeContent
			{
				User = new Models.User( )
			};
			h.User = h.User.GetUserSession( );

			return View( h );
		}

		[HttpPost]
		public ActionResult Search( FormCollection col ) {

			SearchItems searchItems = default( SearchItems );

			// added this items to the model in a struct so we can continue using same search items
			// ternary : if hidden element is null, use original search page element, else use hidden element
			
			if (col["btnSubmit"] == "close")
			{
				return RedirectToAction( "index" );
			}
			else if ( col["btnSubmit"] == "submit" )
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

				h.User = new Models.User( );
				h.User = h.User.GetUserSession( );

				return View( h );
			}

			return View( );
		}



		public ActionResult Recipe(  ) {
			Models.HomeContent h = new Models.HomeContent( );
			h.User = new Models.User( );
			h.User = h.User.GetUserSession( );

			Database DB = new Database( );

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
					// now save ingredients
					int IngredientID = DB.SaveIngredient( int.Parse( ingredient.id ), ingredient.name );

					DB.AddRecipeIngredients( intSavedID, IngredientID, double.Parse( ingredient.amount ), ingredient.unit );
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
        [HttpPost]
		public JsonResult RateRecipe( int UID, int ID, int intDifficultyRating, int intTasteRating ) {
			try
			{
				Models.Database db = new Models.Database( );
				int intReturn = 0;
				intReturn = db.RateRecipe( UID, ID, intDifficultyRating, intTasteRating );
				return Json( new { Status = intReturn } );
			}
			catch ( Exception ex )
			{
				return Json( new { Status = -1 } ); //error
			}
		}


        public ActionResult About( ) {
			ViewBag.Message = "Your application description page.";

			return View( );
		}

		public ActionResult Contact( ) {
			ViewBag.Message = "Your contact page.";

			return View( );
		}

	}
}