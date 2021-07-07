using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reciplease.Models {
	[JsonObjectAttribute] // need this attribute to allow deserialization of the api results
	public class SearchResultsList : IEnumerable{
		public List<Result> results;

		// Implementation for the GetEnumerator method.
		IEnumerator IEnumerable.GetEnumerator( ) {
			return (IEnumerator)GetEnumerator( );
		}

		public IEnumerable GetEnumerator( ) {
			return this.results;
		}
	}


	public class Result {
		public string id = string.Empty;
		public string title = string.Empty;
		public string readyInMinutes = string.Empty;
		public string servings = string.Empty;
		public string image = string.Empty;
	}
}