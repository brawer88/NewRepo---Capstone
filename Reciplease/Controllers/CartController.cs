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
		public ActionResult CreatRecipe( FormCollection col ) {
			try
			{
				Models.User u = new Models.User( );
				Models.Recipe recipe = new Recipe( );

				// example of getting data from the page in a post method
				recipe.title = col["RecipeName"];
				recipe.instructions = col["Instructions"];
				recipe.diets = new List<string> { col["Diet"] };
				recipe.cuisines = new List<string> { col["Cusines"] };
				recipe.dishTypes = new List<string> { col["dishTypes"] };
				//recipe.extendedIngredients = new List<Ingredient> { col["Diets"] };

				if ( u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.Username.Length == 0 || u.Password.Length == 0 )
				{
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View( u );
				}
				else
				{
					return View( u );					
				}
			}
			catch ( Exception )
			{
				Models.User u = new Models.User( );
				return View( u );
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
    }
}