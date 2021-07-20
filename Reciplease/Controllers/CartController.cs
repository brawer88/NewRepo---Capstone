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
            return View();
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