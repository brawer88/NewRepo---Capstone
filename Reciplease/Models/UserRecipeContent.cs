using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class UserRecipeContent {
		public User User { get; internal set; }
		public Recipe SingleRecipe { get; internal set; }
	}
}