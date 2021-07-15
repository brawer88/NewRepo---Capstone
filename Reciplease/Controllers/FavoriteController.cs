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
     
