using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class FavoritesContent {
		public List<Recipe> lstUserFavorites { get; set; }
		public User user { get; set; }
		public Recipe SingleRecipe { get; set; }
	}
}