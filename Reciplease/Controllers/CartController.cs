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

		public String AuthCode( ){
			string authcode = string.Empty;
		
			authcode = Convert.ToString( RouteData.Values["code"] );

			return authcode;
		}
    }
}