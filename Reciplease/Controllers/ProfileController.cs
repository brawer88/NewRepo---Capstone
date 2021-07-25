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
    public class ProfileController : Controller {
        public ActionResult Index()
        {
            Models.User u = new Models.User();
            u = u.GetUserSession();
            
            return View(u);
        }

        [HttpPost]
        public ActionResult Index( FormCollection collfrmAttr)
        {
            try
            {
                Models.User u = new Models.User();
                u = u.GetUserSession();

                u.FirstName = collfrmAttr["FirstName"];
                u.LastName = collfrmAttr["LastName"];
                u.Email = collfrmAttr["Email"];
                u.Username = collfrmAttr["Username"];
                u.Password = collfrmAttr["Password"];

                if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.Username.Length == 0 || u.Password.Length == 0)
                {
                    u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
                    return View(u);
                }
                else
                {
                    if (collfrmAttr["btnSubmit"] == "update")
                    { //update button pressed
                        u.Save();
                        u.SaveUserSession();
                        return RedirectToAction("Index");
                    }
                    return View(u);
                }
            }
            catch (Exception)
            {
                Models.User u = new Models.User();
                return View(u);
            }

        }

        public ActionResult SignIn()
        {
            Models.User u = new Models.User();
            return View(u);
        }

        [HttpPost]
        public ActionResult SignIn(FormCollection col)
        {
            try
            {
                Models.User u = new Models.User();

                if (col["btnSubmit"] == "signin")
                {
                    u.Username = col["Username"];
                    u.Password = col["Password"];

                    if (u.Username.Length == 0 || u.Password.Length == 0)
                    {
                        u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
                        return View(u);
                    }
                    else
                    {
                        u = u.Login();
                        if (u != null && u.UID > 0)
                        {
                            u.SaveUserSession();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            u = new Models.User();
                            u.Username = col["Username"];
                            u.ActionType = Models.User.ActionTypes.LoginFailed;
                        }
                    }
                }
                return View(u);
            }
            catch (Exception)
            {
                Models.User u = new Models.User();
                return View(u);
            }
        }

        public ActionResult SignUp()
        {
            Models.User u = new Models.User();
            return View(u);
        }

        [HttpPost]
        public ActionResult SignUp(FormCollection col)
        {
            try
            {
                Models.User u = new Models.User();

                u.FirstName = col["FirstName"];
                u.LastName = col["LastName"];
                u.Email = col["Email"];
                u.Username = col["Username"];
                u.Password = col["Password"];

                if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.Username.Length == 0 || u.Password.Length == 0)
                {
                    u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
                    return View(u);
                }
                else
                {
                    if (col["btnSubmit"] == "signup")
                    { //sign up button pressed
                        Models.User.ActionTypes at = Models.User.ActionTypes.NoType;
                        at = u.Save();
                        switch (at)
                        {
                            case Models.User.ActionTypes.InsertSuccessful:
                                u.SaveUserSession();
                                return RedirectToAction("Index");
                            //break;
                            default:
                                return View(u);
                                //break;
                        }
                    }
                    else
                    {
                        return View(u);
                    }
                }
            }
            catch (Exception)
            {
                Models.User u = new Models.User();
                return View(u);
            }
        }

        public ActionResult SignOut()
        {
            Models.User u = new Models.User();
            u.RemoveUserSession();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Favorites()
        {
            FavoritesContent favorites = new FavoritesContent();
            Models.User u = new Models.User();
            Database db = new Database();
            favorites.user = u.GetUserSession();
            favorites.lstUserFavorites = db.GetUserFavorites(favorites.user.UID);

            return View(favorites);
        }


        public ActionResult UserRecipes()
        {
            UserRecipeContent MyRecipes = new UserRecipeContent();
            Models.User u = new Models.User();
            Database db = new Database();
            MyRecipes.user = u.GetUserSession();
            MyRecipes.lstUserRecipes = db.GetUserCreations(MyRecipes.user.UID);



            return View( MyRecipes );

        }



    }
}

