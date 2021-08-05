using System;
using System.Collections.Generic;

namespace Reciplease.Models {
	public class ShoppingList {
		public int intRecipeID { get; set; }
		public int intRecipeIngredientID { get; set; }
		public string RecipeName { get; set; }
		public string RecipeServings { get; set; }
		private List<Ingredient> ingredients { get; set; }

		public ShoppingList( ) {
			ingredients = new List<Ingredient>( );
		}

		internal void AddIngredient( Ingredient ingredient ) {
			ingredients.Add( ingredient );
		}
	}
}