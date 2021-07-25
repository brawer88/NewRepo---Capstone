using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class Cart {
		// "milk" : "1 gal"
		Dictionary<string, string> ingredients { get; set; }


		public void AddToCart( string ingredient, string quantity, string measurement ) {
			if (this.ingredients.ContainsKey( ingredient ))
			{
				// combine ingredients

			}
			else
			{
				this.ingredients[ingredient] = quantity + " " + measurement;
			}
		}

		// method to update


		// method to remove



		public bool SaveCartSession( ) {
			try
			{
				HttpContext.Current.Session["Cart"] = this;
				return true;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public Cart GetCartSession( ) {
			try
			{
				Cart cart= new Cart( );
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