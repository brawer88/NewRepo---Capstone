using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



//#################################################
// Models.User
//
// Contains the elements of a user and will have methods
// needed to log user in and out when we get to it
// 
//#################################################
namespace Reciplease.Models {
	public class User {
		public int UID = 0;
		public string Username { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Password { get; set; }
		public ActionTypes ActionType = ActionTypes.NoType;
		public string Email { get; set; }
		public List<Rating> Ratings;
		public string KrogerAuthCode { get; set; }
		public AuthCodes KrogerAuthTokens { get; set; }
		public string previousPage { get; set; }


		public enum ActionTypes {
			NoType = 0,
			InsertSuccessful = 1,
			UpdateSuccessful = 2,
			DuplicateEmail = 3,
			DuplicateUsername = 4,
			Unknown = 5,
			RequiredFieldsMissing = 6,
			LoginFailed = 7,
			RecipeFavorited = 8,
			RecipeUnfavorited = 9
		}


		public bool IsAuthenticated {
			get
			{
				if ( UID > 0 ) return true;
				return false;
			}
		}

		public List<Recipe> Favorites { get; internal set; }

		public Tuple<int, int> GetUserRatings( int RecipeID ) {

			Tuple<int, int> ZeroRatings = Tuple.Create(0,0);

			try
			{
				foreach ( Rating r in this.Ratings )
				{
					if ( r.intRecipeID == RecipeID )
					{
						return Tuple.Create( r.intTasteRating, r.intDifficultyRating );
					}
				}
				return ZeroRatings;
			}
			catch ( Exception ) { return ZeroRatings; }
		}


		public User Login( ) {
			try
			{
				Database db = new Database( );
				return db.Login( this );
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public bool RemoveUserSession( ) {
			try
			{
				HttpContext.Current.Session["CurrentUser"] = null;
				return true;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}

		public User GetUserSession( ) {
			try
			{
				User u = new User( );
				if ( HttpContext.Current.Session["CurrentUser"] == null )
				{
					return u;
				}
				u = (User)HttpContext.Current.Session["CurrentUser"];
				return u;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}

		public bool SaveUserSession( ) {
			try
			{
				HttpContext.Current.Session.Timeout = 525600;
				HttpContext.Current.Session["CurrentUser"] = this;
				return true;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public User.ActionTypes Save( ) {
			try
			{
				Database db = new Database( );
				if ( UID == 0 )
				{ //insert new user
					this.ActionType = db.InsertUser( this );
				}

				return this.ActionType;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}

		public User.ActionTypes Save( int UID, string Username, string Password, string Email, string FirstName, string LastName ) {
			try
			{
				Database db = new Database( );
				
				this.ActionType = db.UpdateUser( UID, Username, Password, Email, FirstName, LastName );
				return this.ActionType;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public int RateRecipe( int RecipeID, int intDifficultyRating, int intTasteRating ) {
			try
			{
				Database db = new Database( );
				return db.RateRecipe( this.UID, RecipeID, intDifficultyRating, intTasteRating );
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public int ToggleFavorite( int RecipeID ) {
			try
			{
				Database db = new Database( );
				return db.ToggleFavorite( this.UID, RecipeID );
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public bool IsAFavorite( string RecipeID ) {
			bool blnFavorite = false;
				
			foreach (Recipe r in this.Favorites)
			{
				if (r.id == RecipeID)
				{
					blnFavorite = true;
					break;
				}
			}

			return blnFavorite;
		}
	}


}