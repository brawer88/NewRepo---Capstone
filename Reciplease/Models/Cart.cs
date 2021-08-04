using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class Cart {
		public string RecipeName { get; set; }
		public int Servings { get; set; }

		private List<Ingredient> ingredients { get; set; }

		public void AddToCart( string strRecipeID ) {
			Database db = new Database( );
			User u = new User( );
			u = u.GetUserSession();

			ShoppingList list = new ShoppingList( );

			db.AddToShoppingList( int.Parse( strRecipeID ), u.UID );

			list = db.GetShoppingList( u.UID );

			ingredients = db.GetIngredients( strRecipeID );

			SaveCartSession( );
		}

		// method to update
		public void UpdateCart( int newServingSize ) {
			// will update when there is a usp to do this
		}

		// method to remove
		public void EmptyCart( string strRecipeID ) {
			// will update when there is a usp to do this
		}


		public bool SaveCartSession( ) {
		try
		{
			HttpContext.Current.Session.Timeout = 525600; 
			HttpContext.Current.Session["Cart"] = this;
			return true;
		}
		catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public Cart GetCartSession( ) {
			try
			{
				Cart cart= new Cart
				{
					ingredients = new List<Ingredient>()
				};
				if ( HttpContext.Current.Session["Cart"] == null )
				{
					return cart;
				}
				cart = (Cart)HttpContext.Current.Session["Cart"];
				return cart;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}

		// return sorted and fractionized ingredients
		public List<Ingredient> GetFractionizedIngredients( ) {
			List<Ingredient> ingredients = new List<Ingredient>( );


			foreach ( Ingredient ingredient in this.ingredients )
			{
				ingredients.Add( ingredient );

				string fraction = ConvertToFraction( ingredients[ingredients.Count - 1].amount );

				ingredients[ingredients.Count - 1].amount = fraction;
			}


			return ingredients;
		}



		// convert single to fraction string
		private string ConvertToFraction( string strUnit ) {
			string fraction = string.Empty;

			string[] numberParts = strUnit.Split( '.' );

			if ( numberParts.Length == 2 )
			{
				string wholePart = numberParts[0];
				string decPart = numberParts[1];
				if ( numberParts[1].Length > 3 )
				{
					decPart = decPart.Substring( 0, 3 );
				}


				// there will only be a few options for this, so will code those up
				string[] aDecPossinilities = { "0", "062", "125", "166", "187", "25", "312", "333", "375", "437", "5", "562", "625", "666", "6875", "75", "812", "833", "875", "9375" };
				string[] aFracPossinilities = { "", "1/16", "1/8", "1/6", "3/16", "1/4", "5/16", "1/3", "3/8", "7/16", "1/2", "9/16", "5/8", "2/3", "11/16", "3/4", "13/16", "5/6", "7/8", "15/16" };

				int index = Array.IndexOf( aDecPossinilities, decPart );

				if ( wholePart == "0" )
				{
					wholePart = "";
				}
				if ( index != -1 )
				{
					fraction = wholePart + " " + aFracPossinilities[index];
				}
				else
				{
					fraction = wholePart + decPart;
				}
				return fraction;
			}
			else
			{
				return strUnit;
			}
		}
	}

}