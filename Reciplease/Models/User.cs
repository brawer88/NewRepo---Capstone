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
		public long UID = 0;
		public string UserID = string.Empty;
		public string FirstName = string.Empty;
		public string LastName = string.Empty;
		public string Password = string.Empty;
		public ActionTypes ActionType = ActionTypes.NoType;
		public string Email = string.Empty;

		public enum ActionTypes {
			NoType = 0,
			InsertSuccessful = 1,
			UpdateSuccessful = 2,
			DuplicateEmail = 3,
			DuplicateUserID = 4,
			Unknown = 5,
			RequiredFieldsMissing = 6,
			LoginFailed = 7
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
				HttpContext.Current.Session["CurrentUser"] = this;
				return true;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}

	}


}