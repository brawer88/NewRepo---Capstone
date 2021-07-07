using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


//#################################################
// Models.HomeContent
//
// Contains the fields passed to the Home View
// 
//#################################################
namespace Reciplease.Models {
	public class HomeContent {
		public RecipesList RecipesToDisplay { get; internal set; }
		public SearchResultsList SearchResults { get; internal set; }
		public SearchItems searchItems;
		public User User { get; internal set; }

		public Recipe SingleRecipe { get; internal set; }
	}

	public struct SearchItems {
		public string cuisine;
		public string ingredients;
		public string diets;
		public string excludedIngredients;
		public string intolerances;
		public string type;
		public string query;
		public int index;
	}
}