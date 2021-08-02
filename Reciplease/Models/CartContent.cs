using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	public class CartContent {
		public Cart cart { get; set; }
		public User user { get; set; }
		public List<Ingredient> ingredients {get; set;}
	}
}