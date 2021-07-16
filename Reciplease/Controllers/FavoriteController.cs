using Reciplease.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Reciplease.Controllers
{
    public class FavoritesController : Controller
    {
        public ActionResult Add_Favorite()
        {
            return View();
        }

		public ActionResult Index() {
			FavoritesContent favorites = new FavoritesContent( );

			// need to fill this in with items from the favorites content
			// in the database object, we have a method GetUserFavorites that gets the list of recipes to display on the homepage
			// get the user object and pass that user in to this method

			return View( favorites );
		}
    }

    // [HttpPost]
    //  public ActionResult Add()
    // {

    // }


   // public ActionResult Remove_Favorite()
   // {
       // return RedirectToAction("Index", "Home");
  //  }



}
     
