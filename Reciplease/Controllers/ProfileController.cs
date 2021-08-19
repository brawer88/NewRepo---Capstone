using Newtonsoft.Json;
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
		[HandleError]
		public ActionResult Index()
        {
            Models.User u = new Models.User();
            u = u.GetUserSession();
            
            return View(u);
        }

		[HandleError]
		public ActionResult SignIn()
        {
            Models.User u = new Models.User();
			u.previousPage = System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            return View(u);
        }

		[HandleError]
		[HttpPost]
        public ActionResult SignIn(FormCollection col)
        {
            try
            {
                Models.User u = new Models.User();

				u.previousPage = col["previousPage"];

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
						User newUser = new User( );
						newUser = u.Login();
						newUser.previousPage = u.previousPage;
						u = newUser;
                        if (u != null && u.UID > 0)
                        {
                            u.SaveUserSession();
                            return Redirect( u.previousPage );
                        }
                        else
                        {
							u.RemoveUserSession( );
							u.Password = String.Empty;
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

		[HandleError]
		public ActionResult SignUp()
        {
            Models.User u = new Models.User();
            return View(u);
        }

		[HandleError]
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
								u = u.Login( );
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

		[HandleError]
		public ActionResult SignOut()
        {
            Models.User u = new Models.User();
            u.RemoveUserSession();
            return RedirectToAction("Index", "Home");
        }

		[HandleError]
		public ActionResult Favorites()
        {
            FavoritesContent favorites = new FavoritesContent();
            Models.User u = new Models.User();
            Database db = new Database();
            favorites.user = u.GetUserSession();
            favorites.lstUserFavorites = db.GetUserFavorites(favorites.user.UID);

            return View(favorites);
        }

		[HandleError]
		public ActionResult UserRecipes()
        {
            UserRecipeContent MyRecipes = new UserRecipeContent();
			Models.User u = new Models.User( );
            Database db = new Database();
            MyRecipes.user = u.GetUserSession();
            MyRecipes.lstUserRecipes = db.GetUserCreations(MyRecipes.user.UID);

            return View( MyRecipes );
        }



		[HandleError]
		public ActionResult CreateRecipe( ) {

			string strRecipeID = Convert.ToString( RouteData.Values["id"] );

			Models.UserRecipeContent recipeContent = new UserRecipeContent( );

			if(strRecipeID.Length > 0)
			{
				Database db = new Database( );
				recipeContent.SingleRecipe = db.LoadRecipe( strRecipeID );
			}
				

			User u = new User( );
			recipeContent.user = u.GetUserSession( );
			return View( recipeContent );
		}

		[HandleError]
		[HttpPost]
        public ActionResult CreateRecipe(FormCollection col)
        {
            try
            {
                Models.User u = new Models.User();
                Models.Recipe recipe = new Recipe();
                Models.UserRecipeContent recipeContent = new UserRecipeContent();
                recipeContent.user = u.GetUserSession();

				string diets = "-1";
				string cuisines = "-1";
				string dishTypes = "-1";
				recipe.readyInMinutes = "-1";
				recipe.servings = "-1";
				recipe.image = col["image"];

				if ( recipe.image == null )
				{
					recipe.image = "/Content/images/no-photo.jpg";
				}

				// example of getting data from the page in a post method
				recipe.title = col["RecipeName"];
                recipe.instructions = col["instructions"]; // required
				if ( col["diets"].Length > 0 )
				{
					diets = (col["diets"]); // optional default to "-1"
				}
				if ( col["cuisines"].Length > 0 )
				{
					cuisines = col["cuisines"]; // optional default to "-1"
				}
				if ( col["dishTypes"].Length > 0 )
				{
					dishTypes = col["dishTypes"]; // optional default to "-1"
				}
				if ( col["readyinMinutes"].Length > 0 )
				{
					recipe.readyInMinutes = col["readyinMinutes"]; // optional default to "-1"
				}
				if ( col["servings"].Length > 0 )
				{
					recipe.servings = col["servings"]; // optional default to "-1"
				}

				string[] ingredients = Request.Form.GetValues( "ingredients" );
				string[] amounts = Request.Form.GetValues( "amounts" );
				string[] measurements = Request.Form.GetValues( "measurements" );
				recipe.extendedIngredients = new List<Ingredient>( );

				for (int index=0; index < ingredients.Length; index += 1 )
				{
					Ingredient ing = new Ingredient( );
					ing.name = ingredients[index];
					ing.amount = amounts[index];
					ing.unit = measurements[index];
					recipe.extendedIngredients.Add( ing );
				}
				
                Database db = new Database();
                int intSavedID = db.SaveRecipe(recipe.title, recipe.instructions, int.Parse(recipe.readyInMinutes), recipe.image, int.Parse(recipe.servings), cuisines, diets, dishTypes, "-1", recipeContent.user.UID, -1);
				foreach ( Ingredient ingredient in recipe.extendedIngredients )
				{
					// now save ingredients
					int IngredientID = db.SaveIngredient( 0, ingredient.name );

					// trim 
					ingredient.amount = ingredient.amount.Trim( );


					try
					{
						db.AddRecipeIngredients( intSavedID, IngredientID, double.Parse(ingredient.amount), ingredient.unit );
					}
					catch (Exception)
					{
						char[] charSeparators = new char[] { '\\', '/', ' ' };
						string[] splitAmounts = ingredient.amount.Split( charSeparators );
						double dblAmount = 0;
						if (splitAmounts.Length > 2 && splitAmounts[0] != "")
						{
							splitAmounts[1] = (double.Parse( splitAmounts[1] ) + double.Parse( splitAmounts[0] ) * double.Parse( splitAmounts[2] )).ToString();
							dblAmount = double.Parse( splitAmounts[1] ) / double.Parse( splitAmounts[2] );
						}
						else
						{
							dblAmount = double.Parse( splitAmounts[0] ) / double.Parse( splitAmounts[1] );
						}

						db.AddRecipeIngredients( intSavedID, IngredientID, dblAmount, ingredient.unit );
					}
					
				}
				return RedirectToAction( "UserRecipes" );
                
            }
            catch (Exception)
            {
                Models.UserRecipeContent recipeContent = new UserRecipeContent();
                return View(recipeContent);
            }
        }

		[HandleError]
		public ActionResult EditRecipe( ) {
			Models.UserRecipeContent recipeContent = new Models.UserRecipeContent( );
			recipeContent.user = new Models.User( );
			recipeContent.user = recipeContent.user.GetUserSession( );

			Database DB = new Database( );

			string strRecipeID = Convert.ToString( RouteData.Values["id"] );
			if ( strRecipeID.Length > 0 )
			{
				// check if recipe id exists
				if ( DB.RecipeExists( strRecipeID ) == true )
				{
					recipeContent.SingleRecipe = DB.LoadRecipe( strRecipeID );
				}
				else
				{
					recipeContent.SingleRecipe = null;
				}
			}
			else
			{
				recipeContent.SingleRecipe = null;
			}

			return View( recipeContent );
		}


		[HandleError]
		[HttpPost]
		public ActionResult EditRecipe( FormCollection col ) {
			try
			{
				Models.User u = new Models.User( );
				Models.Recipe recipe = new Recipe( );
				Models.UserRecipeContent recipeContent = new UserRecipeContent( );
				recipeContent.user = u.GetUserSession( );


				string diets = "-1";
				string cuisines = "-1";
				string dishTypes = "-1";
				recipe.readyInMinutes = "-1";
				recipe.servings = "-1";


				// example of getting data from the page in a post method
				recipe.id = col["RecipeID"];
				recipe.title = col["RecipeName"];
				recipe.instructions = col["instructions"]; // required
				if ( col["diets"].Length > 0 )
				{
					diets = col["diets"]; // optional default to "-1"
				}
				if ( col["cuisines"].Length > 0 )
				{
					cuisines = col["cuisines"]; // optional default to "-1"
				}
				if ( col["dishTypes"].Length > 0 )
				{
					dishTypes = col["dishTypes"]; // optional default to "-1"
				}
				if ( col["readyinMinutes"].Length > 0 )
				{
					recipe.readyInMinutes = col["readyinMinutes"]; // optional default to "-1"
				}
				if ( col["servings"].Length > 0 )
				{
					recipe.servings = col["servings"]; // optional default to "-1"
				}

				string[] ingredients = Request.Form.GetValues( "ingredients" );
				string[] amounts = Request.Form.GetValues( "amounts" );
				string[] measurements = Request.Form.GetValues( "measurements" );
				recipe.extendedIngredients = new List<Ingredient>( );

				for ( int index = 0; index < ingredients.Length; index += 1 )
				{
					Ingredient ing = new Ingredient( );
					ing.name = ingredients[index];
					ing.amount = amounts[index];
					ing.unit = measurements[index];
					recipe.extendedIngredients.Add( ing );
				}

				Database db = new Database( );
				db.UpdateRecipe( recipe.title, recipe.instructions, int.Parse( recipe.readyInMinutes ), "/Content/images/no-photo.jpg", int.Parse( recipe.servings ), cuisines, diets, dishTypes, "-1", recipeContent.user.UID, int.Parse(recipe.id) );
				foreach ( Ingredient ingredient in recipe.extendedIngredients )
				{
					// now save ingredients
					int IngredientID = db.SaveIngredient( 0, ingredient.name );


					try
					{
						db.AddRecipeIngredients( int.Parse(recipe.id), IngredientID, double.Parse( ingredient.amount ), ingredient.unit );
					}
					catch ( Exception )
					{
						char[] charSeparators = new char[] { '\\', '/', ' ' };
						string[] splitAmounts = ingredient.amount.Split( charSeparators );
						double dblAmount = 0;
						if ( splitAmounts.Length > 2 && splitAmounts[0] != "" )
						{
							splitAmounts[1] = ( double.Parse( splitAmounts[1] ) + double.Parse( splitAmounts[0] ) * double.Parse( splitAmounts[2] ) ).ToString( );
							dblAmount = double.Parse( splitAmounts[1] ) / double.Parse( splitAmounts[2] );
						}
						else
						{
							dblAmount = double.Parse( splitAmounts[0] ) / double.Parse( splitAmounts[1] );
						}

						db.AddRecipeIngredients( int.Parse( recipe.id ), IngredientID, dblAmount, ingredient.unit );
					}

				}
				return RedirectToAction( "UserRecipes" );
			}
			catch ( Exception )
			{
				Models.UserRecipeContent recipeContent = new UserRecipeContent( );
				return View( recipeContent );
			}
		}


		[HandleError]
		public ActionResult DeleteUser() {
			User u = new User( );
			u = u.GetUserSession( );

			u.RemoveUserSession( );

			Database db = new Database( );
			db.DeleteAccount( u.UID );

			return RedirectToAction( "Index" );
		}


		[HandleError]
		public ActionResult UpdateUser( ) {
			User u = new Models.User( );

			u = u.GetUserSession( );
			return View( u );
		}

		[HandleError]
		[HttpPost]
		public ActionResult UpdateUser( FormCollection collfrmAttr ) {
			try
			{
				User u = new User( );

				u = u.GetUserSession( );

				string FirstName = collfrmAttr["FirstName"];
				string LastName = collfrmAttr["LastName"];
				string Email = collfrmAttr["Email"];
				string Username = collfrmAttr["Username"];
				string Password = collfrmAttr["Password"];

				if ( u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.Username.Length == 0 || u.Password.Length == 0 )
				{
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View( u );
				}
				else
				{
					if ( collfrmAttr["btnSubmit"] == "update" )
					{ //update button pressed
						u.ActionType = u.Save(  u.UID,  Username,  Password,  Email,  FirstName, LastName );
						if( u.ActionType == Models.User.ActionTypes.UpdateSuccessful )
						{
							u.SaveUserSession( );
						}

					}

					return View( u );
				}
			}
			catch ( Exception )
			{
				Models.User u = new Models.User( );
				return View( u );
			}

		}

		[HandleError]
		public ActionResult UserSingleRecipe( ) {
			Models.UserRecipeContent recipeContent = new Models.UserRecipeContent( );
			recipeContent.user = new Models.User( );
			recipeContent.user = recipeContent.user.GetUserSession( );

			Database DB = new Database( );

			string strRecipeID = Convert.ToString( RouteData.Values["id"] );

			// check if recipe id exists
			if ( DB.RecipeExists( strRecipeID ) == false )
			{
				// get recipes to display
				recipeContent.SingleRecipe = RecipeAPI.GetRecipeById( strRecipeID );
				// save recipe
				int intSavedID = DB.SaveRecipe( recipeContent.SingleRecipe.title, recipeContent.SingleRecipe.instructions, int.Parse( recipeContent.SingleRecipe.readyInMinutes ), recipeContent.SingleRecipe.image, int.Parse( recipeContent.SingleRecipe.servings ),
					String.Join( ", ", recipeContent.SingleRecipe.cuisines ), String.Join( ", ", recipeContent.SingleRecipe.diets ), String.Join( ", ", recipeContent.SingleRecipe.dishTypes ), JsonConvert.SerializeObject( recipeContent.SingleRecipe.nutrition ), -1, int.Parse( recipeContent.SingleRecipe.id ) );

				foreach ( Ingredient ingredient in recipeContent.SingleRecipe.extendedIngredients )
				{
					// now save ingredients
					int IngredientID = DB.SaveIngredient( int.Parse( ingredient.id ), ingredient.name );

					DB.AddRecipeIngredients( intSavedID, IngredientID, double.Parse( ingredient.amount ), ingredient.unit );
				}


			}
			else
			{
				// load from db
				recipeContent.SingleRecipe = DB.LoadRecipe( strRecipeID );
			}

			return View( recipeContent );
		}

		[HandleError]
		public ActionResult DeleteRecipe( ) {
			User u = new User( );
			u = u.GetUserSession( );
			string strRecipeID = Convert.ToString( RouteData.Values["id"] );
			Database db = new Database( );

			db.DeleteRecipe( strRecipeID, u.UID );

			return RedirectToAction( "UserRecipes" );
		}


        [HandleError]
        public ActionResult LastTen()
        {
            LastTenContent last10 = new LastTenContent( );
            Models.User u = new Models.User();
            Database db = new Database();
           
            last10.user = u.GetUserSession();
			last10.lstLastTen = db.GetLastTen( last10.user.UID );

			last10.lstLastTen.Reverse( );
           
            db.GetLastTen(last10.user.UID);
           
            return View( last10 );
        }

    }
}

