using Newtonsoft.Json;
using Reciplease.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	[JsonObjectAttribute] // need this attribute to allow deserialization of the api results
	public class RecipesList : IEnumerable {
		public List<Recipe> recipes;

		// Implementation for the GetEnumerator method.
		IEnumerator IEnumerable.GetEnumerator( ) {
			return (IEnumerator)GetEnumerator( );
		}

		public IEnumerable GetEnumerator( ) {
			return this.recipes;
		}
	}	
}