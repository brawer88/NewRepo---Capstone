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

			cart.user = u.GetUserSession( );
			cart.cart = new Cart( );
			cart.cart = cart.cart.GetCartSession( );
			Database db = new Database( );

			cart.cart.list = db.GetShoppingList( cart.user.UID );

			cart.cart.ingredients = db.GetIngredients( cart.cart.list.intRecipeID.ToString( ) );

			return View(cart);
        }


		public ActionResult KrogerSignIn() {

			string url = KrogerAPI.GetKrogerAuth( );

			return Redirect( url );
		}

		public ActionResult AuthCode( ){
			string authcode = string.Empty;
		
			authcode = Request["code"];

			User user = new User( );
			user = user.GetUserSession( );

			user.KrogerAuthCode = authcode;
			user.KrogerAuthTokens = KrogerAPI.GetKrogerToken( authcode );
			user.SaveUserSession( );

			//// code to add to kroger cart
			//Cart c = new Cart( );
			//c = c.GetCartSession( );

			//CartMappedToKrogerUPC upcs = KrogerAPI.GetKrogerUPCS( c.ingredients );
			//KrogerAPI.AddToKrogerCart( upcs.convertToJson() );
			//c.EmptyCart( );

			//return Content( "<script language='javascript' type='text/javascript'>window.open('https://www.kroger.com/cart');location.reload();</script>" );

			return RedirectToAction( "Index" );
		}

        public ActionResult AddToCart()
        {

            string RecipeID = Convert.ToString(RouteData.Values["id"]);

			User u = new User( );
			u = u.GetUserSession( );

            if (RecipeID.Length > 0)
            {
				Cart cart = new Cart();
				cart = cart.GetCartSession( );

				cart.AddToCart( RecipeID );
            }
          

            return RedirectToAction("Index");

        }


    }
}