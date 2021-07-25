using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class UserRecipeContent {
		public User user { get; internal set; }
		public Recipe SingleRecipe { get; internal set; }
		public List<Recipe> lstUserRecipes { get; set; }
	}
}