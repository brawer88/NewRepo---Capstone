using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class Cart {
		// "milk" : "1 gal"
		public List<Ingredient> ingredients { get; set; }


		public void AddToCart( string strRecipeID ) {
			Database db = new Database( );

			List<Ingredient> incomingIngredients = db.GetIngredients( strRecipeID );

			foreach (Ingredient i in incomingIngredients)
			{
				Ingredient found = null;
				foreach ( Ingredient c in ingredients )
				{
					if ( i.name == c.name )
					{
						found = c;
					}
				}

				if ( found != null )
				{
					int foundIndex = ingredients.IndexOf( found );
					found.amount = (double.Parse(found.amount) + double.Parse( i.amount)).ToString();
					ingredients[foundIndex] = found;
				}
				else
				{
					ingredients.Add( i );
				}				
			}
			User u = new User( );
			u = u.GetUserSession();

			SaveCartSession( );
		}

		// method to update


		// method to remove



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
	}
}