﻿using System;
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
		public string Password = string.Empty;





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



	}


}