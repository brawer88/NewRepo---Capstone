using Reciplease.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reciplease.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            CartContent cart = new CartContent( );
            Models.User u = new Models.User();
            
            return View(cart);
        }


		[HttpPost]
		public ActionResult CreateRecipe( FormCollection col ) {
			try
			{
				Models.User u = new Models.User( );
				Models.Recipe recipe = new Recipe( );
                Models.UserRecipeContent recipeContent = new UserRecipeContent();
                recipeContent.user = u.GetUserSession();


                // example of getting data from the page in a post method
                recipe.title = col["RecipeName"];
				recipe.instructions = col["Instructions"];
				recipe.diets = new List<string> { col["Diet"] };
				recipe.cuisines = new List<string> { col["Cusines"] };
				recipe.dishTypes = new List<string> { col["dishTypes"] };
				//recipe.extendedIngredients = new List<Ingredient> { col["Diets"] };

				if ( recipe.title.Length == 0)
				{
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View( u );
				}
				else
				{
                    Database db = new Database();
                    db.SaveRecipe(recipe.title, recipe.instructions, int.Parse(recipe.readyInMinutes), "'/Content/images/no-photo.jpg", int.Parse(recipe.servings), String.Join(",",recipe.cuisines), String.Join(",", recipe.diets), String.Join(",", recipe.dishTypes), "-1", recipeContent.user.UID, 0);
					return View( recipeContent );					
				}
			}
			catch ( Exception )
			{
                Models.UserRecipeContent recipeContent = new UserRecipeContent();
                return View(recipeContent);
			}
		}


		public ActionResult KrogerSignIn() {

			string url = KrogerAPI.GetKrogerAuth( );

			return Redirect( url );
		}

		public ActionResult GetKrogerAuthToken( ) {
			User user = new User( );
			user = user.GetUserSession( );

			user.KrogerAuthTokens = KrogerAPI.GetKrogerToken( user.KrogerAuthCode );

			user.SaveUserSession( );

			return RedirectToAction( "index" );
		}


		public ActionResult AuthCode( ){
			string authcode = string.Empty;
		
			authcode = Convert.ToString( RouteData.Values["id"] );

			User user = new User( );
			user = user.GetUserSession( );

			user.KrogerAuthCode = authcode;

			user.SaveUserSession( );

			return RedirectToAction( "GetKrogerAuthToken" );
		}

        public ActionResult RecipeID()
        {
            Models.HomeContent h = new Models.HomeContent();
            h.user = new Models.User();
            h.user = h.user.GetUserSession();

            Database DB = new Database();

            string RecipeID = Convert.ToString(RouteData.Values["id"]);


            string recipeID = string.Empty;

            Database DB = new Database();

           // recipeContent.SingleRecipe = DB.LoadRecipe(strRecipeID);

            recipeID = Convert.ToString(RouteData.Values["id"]);

            User user = new User();
            user = user.GetUserSession();

            // user.ShoppingCart = recipeID;

            user.SaveUserSession();

            return RedirectToAction("GetShoppingCart");

        }


    }
}