using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Reciplease.Models {
	public class RecipeAPI {
		const string APIKEY = "42b1ebc198msh0a6b9caf8b93dc9p1e52ccjsn888e76e0bd8a";

		// Gets 5 random recipes from the api, this is very costly as it counts as 5 requests in the api
		public static RecipesList Get5RandomAPIRecipes( ) {


			RecipesList Recipes = new RecipesList( );

			// get 
			var client = new RestClient( "https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/random?number=5" );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "x-rapidapi-key", APIKEY );
			request.AddHeader( "x-rapidapi-host", "spoonacular-recipe-food-nutrition-v1.p.rapidapi.com" );
			IRestResponse response = client.Execute( request );

			// deserialize object
			if ( response.StatusCode == HttpStatusCode.OK )
			{
				Recipes = JsonConvert.DeserializeObject<RecipesList>( response.Content );
				Database db = new Database( );

				foreach( Recipe r in Recipes.GetEnumerator() )
				{
					if ( r.image == null )
					{
						r.image = Database.FailedImagePath;
					}
					r.dictRatings = db.GetRecipeRatings( int.Parse( r.id ) );
					r.title = Recipe.ToTitleCase(r.title);
				}
			}

			return Recipes;
			
		}


		public static Recipe GetRecipeById( string id ) {

			Recipe clsRecipe = new Recipe( );
			// get 
			var client = new RestClient( "https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/" + id + "/information?includeNutrition=true" );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "x-rapidapi-key", APIKEY );
			request.AddHeader( "x-rapidapi-host", "spoonacular-recipe-food-nutrition-v1.p.rapidapi.com" );
			IRestResponse response = client.Execute( request );

			// deserialize object
			if ( response.StatusCode == HttpStatusCode.OK )
			{
				clsRecipe = JsonConvert.DeserializeObject<Recipe>( response.Content );

				Database db = new Database( );
				if ( clsRecipe.image == null )
				{
					clsRecipe.image = Database.FailedImagePath;
				}
				clsRecipe.dictRatings = db.GetRecipeRatings( int.Parse( clsRecipe.id ) );
				clsRecipe.title = Recipe.ToTitleCase( clsRecipe.title );
			}
			

			return clsRecipe;

		}


		public static SearchResultsList RecipeSearch( string SearchQuery, string cuisine = "", string diets = "", string excludedIngredients ="", string intolerances = "", string type = "", int index = 0) {
			SearchResultsList Recipes = new SearchResultsList( );
			Regex RegexComma = new Regex( "," );
			Regex RegexSpace = new Regex( " " );

			// clean parts
			if ( SearchQuery != null ) SearchQuery = RegexComma.Replace( SearchQuery, "%2C%20" );
			if ( SearchQuery != null ) SearchQuery = RegexSpace.Replace( SearchQuery, "%20" );
			if ( cuisine != null ) cuisine = RegexComma.Replace( cuisine, "%2C%20" );
			if ( cuisine != null ) cuisine = RegexSpace.Replace( cuisine, "%20" );
			if ( excludedIngredients != null ) excludedIngredients = RegexComma.Replace( excludedIngredients, "%2C%20" );
			if ( excludedIngredients != null ) excludedIngredients = RegexSpace.Replace( excludedIngredients, "%20" );
			if ( intolerances != null ) intolerances = RegexComma.Replace( intolerances, "%2C%20" );
			if ( intolerances != null ) intolerances = RegexSpace.Replace( intolerances, "%20" );
			if ( type != null ) type = RegexComma.Replace( type, "%2C%20" );
			if ( type != null ) type = RegexSpace.Replace( type, "%20" );


			var client = new RestClient( "https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/search?query=" + SearchQuery +
									 "&diet=" +									diets +
									 "&excludeIngredients=" +					excludedIngredients +
									 "&intolerances=" +							intolerances +
									 "&number=20&offset=" +						index * 20 +
									 "&type=" +									type +
									 "&instructionsRequired=true&cuisine=" +	cuisine
									 );

			var request = new RestRequest( Method.GET );
			request.AddHeader( "x-rapidapi-key", APIKEY );
			request.AddHeader( "x-rapidapi-host", "spoonacular-recipe-food-nutrition-v1.p.rapidapi.com" );
			IRestResponse response = client.Execute( request );

			// deserialize object
			if ( response.StatusCode == HttpStatusCode.OK )
			{
				Recipes = JsonConvert.DeserializeObject<SearchResultsList>( response.Content );
				Database db = new Database( );

				foreach ( Result r in Recipes.GetEnumerator( ) )
				{
					if(r.image == null)
					{
						r.image = Database.FailedImagePath;
					}
					r.dictRatings = db.GetRecipeRatings( int.Parse( r.id ) );
					r.title = Recipe.ToTitleCase( r.title );
				}

			}
			else if ( response.StatusCode == HttpStatusCode.Unauthorized )
			{
				// quota is gone
			}
			return Recipes;
		}


		public static double ConvertAmounts( string targetUnit, string ingredientName, string sourceAmount, string sourceUnit ) {

			double amount = 0;
			// get 
			var client = new RestClient( "https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/convert?targetUnit=" + targetUnit + "&ingredientName=" + ingredientName + "&sourceAmount=" + sourceAmount +"&sourceUnit=" + sourceUnit );
			var request = new RestRequest( Method.GET );
			request.AddHeader( "x-rapidapi-key", APIKEY );
			request.AddHeader( "x-rapidapi-host", "spoonacular-recipe-food-nutrition-v1.p.rapidapi.com" );
			IRestResponse response = client.Execute( request );

			// deserialize object
			if ( response.StatusCode == HttpStatusCode.OK )
			{
				Conversion myConversion = JsonConvert.DeserializeObject<Conversion>( response.Content );
				amount = myConversion.targetAmount;
			}


			return amount;

		}
		
	}

	// Conversion myDeserializedClass = JsonConvert.DeserializeObject<Conversion>(myJsonResponse); 
	public class Conversion {
		public double sourceAmount { get; set; }
		public string sourceUnit { get; set; }
		public double targetAmount { get; set; }
		public string targetUnit { get; set; }
		public string answer { get; set; }
		public string type { get; set; }
	}
}