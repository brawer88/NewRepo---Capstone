using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class UserRecipesContent {
		public List<Recipe> lstUserRecipes { get; set; }
		public User user { get; set; }
	}
}