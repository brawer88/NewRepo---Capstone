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

			return RedirectToAction( "index" );
		}

        public ActionResult AddToCart()
        {

            string RecipeID = Convert.ToString(RouteData.Values["id"]);

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