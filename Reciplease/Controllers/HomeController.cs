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
		public ActionResult Index( ) {

			Models.HomeContent h = new Models.HomeContent
			{
				// get recipes to display
				RecipesToDisplay = RecipeAPI.Get5RandomAPIRecipes( )

				// get the user object when we set up users
			};

			return View( h );
		}

		public ActionResult Search( ) {
			Models.HomeContent h = new Models.HomeContent();

			return View( h );
		}

		[HttpPost]
		public ActionResult Search( FormCollection col ) {

			SearchItems searchItems = default( SearchItems );

		// added this items to the model in a struct so we can continue using same search items
		// ternary : if hidden element is null, use original search page element, else use hidden element

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
				
				// get the user object when we set up users
			};

			return View( h );
		}

		public ActionResult Recipe(  ) {
			// update to search model when that is created
			Models.HomeContent h = new Models.HomeContent
			{
				// get recipes to display
				SingleRecipe = RecipeAPI.GetRecipeById( Convert.ToString(RouteData.Values["id"]) )

				// get the user object when we set up users
			};

			return View( h );
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