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
        public ActionResult Index(  )
        {
            CartContent cart = new CartContent( );
            Models.User u = new Models.User();

			cart.user = u.GetUserSession( );
			cart.cart = new Cart( );
			cart.cart = cart.cart.GetCartSession( );
			Database db = new Database( );

			cart.cart.list = db.GetShoppingList( cart.user.UID );

			cart.cart.ingredients = db.GetIngredients( cart.cart.list.intRecipeID.ToString( ) );

			cart.jsUrl = "";
			cart.jsMessage = "";

			string id = Convert.ToString( RouteData.Values["id"] );
			if ( id.Length > 0 )
			{
				string jsMessage = "";
				if ( id == "1" )
				{
					jsMessage = "Not all items were found";
				}

				string jsUrl = "http://itd1.cincinnatistate.edu/CPDM-WernkeB/Home";

				cart.jsUrl = jsUrl;
				cart.jsMessage = jsMessage;
			}

			return View(cart);
        }


		public ActionResult KrogerSignIn() {
			User u = new User( );
			u = u.GetUserSession( );

			//if ( u.KrogerAuthCode == null )
			//{
			//	u.KrogerAuthCode = "314u7WFP19xXPDATuvehlm911NyqAEn91tdwa2Rh";
			//	u.KrogerAuthTokens = new AuthCodes( );
			//	u.KrogerAuthTokens.access_token = "";
			//	u.KrogerAuthTokens.refresh_token = "GLaJNpkqq35QiqUP4Cq1G3ONwPaoOsRy0qWTO5IS";
			//}


			if (u.KrogerAuthCode == null)
			{
				string url = KrogerAPI.GetKrogerAuth( );

				return Redirect( url );
			}
			else
			{
				u.KrogerAuthTokens = KrogerAPI.RefreshKrogerToken( u.KrogerAuthTokens.refresh_token );

				// code to add to kroger cart
				Cart c = new Cart( );
				c = c.GetCartSession( );
				Database db = new Database( );

				c.list = db.GetShoppingList( u.UID );

				c.ingredients = db.GetIngredients( c.list.intRecipeID.ToString( ) );

				CartMappedToKrogerUPC upcs = KrogerAPI.GetKrogerUPCS( c.ingredients );
				KrogerAPI.AddToKrogerCart( upcs.convertToJson( ) );
				string id = "0";
				if ( upcs.dictItems["items"].Length != c.ingredients.Count )
				{
					id = "1";
				}
				upcs = new CartMappedToKrogerUPC( );
				c.EmptyCart( );

				return RedirectToAction( "Index", new { id } );
			}
			
			
		}

		public ActionResult AuthCode( ){
			string authcode = string.Empty;
		
			authcode = Request["code"];

			User user = new User( );
			user = user.GetUserSession( );

			user.KrogerAuthCode = authcode;
			user.KrogerAuthTokens = KrogerAPI.GetKrogerToken( authcode );
			user.SaveUserSession( );
			
			// code to add to kroger cart
			Cart c = new Cart( );
			c = c.GetCartSession( );
			Database db = new Database( );

			c.list = db.GetShoppingList( user.UID );

			c.ingredients = db.GetIngredients( c.list.intRecipeID.ToString( ) );

			CartMappedToKrogerUPC upcs = KrogerAPI.GetKrogerUPCS( c.ingredients );
			KrogerAPI.AddToKrogerCart( upcs.convertToJson( ) );
			
			string id = "0";
			if ( upcs.dictItems["items"].Length != c.ingredients.Count )
			{
				id = "1";
			}
			upcs = new CartMappedToKrogerUPC( );
			c.EmptyCart( );


			return RedirectToAction( "Index", new { id } );		
			
			
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



        [HandleError]
        public ActionResult ClearCartContents(int UID)
        {
            // code to delete cart
            Database db = new Database();
                       
            db.DropCart(UID);

			return Json( new
			{
				success = true
			}, JsonRequestBehavior.AllowGet );

		}


       


    }
}