﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Reciplease.Models {
	//#################################################
	// Models.Recipe
	//
	// Contains the elements of a recipe returned by random recipes call to api
	// 
	//#################################################
	public class Recipe {
		public string id { get; set; }
		public string title { get; set; }
		public string readyInMinutes { get; set; }
		public string servings { get; set; }
		public string image { get; set; }
		public string instructions { get; set; }
		public List<String> cuisines { get; set; }
		public List<String> diets { get; set; }
		public List<String> dishTypes { get; set; }
		public List<Ingredient> extendedIngredients { get; set; }
		[JsonProperty( "nutrition" )]
		public Nutrition nutrition { get; set; }
		public ActionTypes ActionType = ActionTypes.NoType;
		public Dictionary<String, int> dictRatings { get; set; }


		public static string ToTitleCase( string title ) {
			string ret = "";

			var temp = title.Split( );

			for (int i = 0; i < temp.Length; i ++ )
			{
				if ( temp[i] != "and" )
				{
					temp[i] = Char.ToUpper( temp[i][0] ) + temp[i].Substring( 1 );
				}
			}

			ret = String.Join( " ", temp );

			return ret;
		}

		// return sorted and fractionized ingredients
		public List<Ingredient> GetFractionizedIngredients( ) {
			List<Ingredient> ingredients = new List<Ingredient>( );


			foreach (Ingredient ingredient in this.extendedIngredients){
				ingredients.Add( ingredient );

				string fraction = ConvertToFraction( ingredients[ingredients.Count-1].amount );

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

		public string[] SplitInstructions() {
			string pattern = @"(?<=[\.!\?])\s+";
			
			Regex spaceRegex = new Regex( @"\.(?! |$)", RegexOptions.Compiled );
			Regex tagStartRegex = new Regex( @"\<(?!.|$)", RegexOptions.Compiled );
			Regex _htmlRegex = new Regex( "<.*?>", RegexOptions.Compiled );
			if ( this.instructions != null )
			{
				string cleanedInstructions = spaceRegex.Replace( this.instructions, ". " );
				cleanedInstructions = tagStartRegex.Replace( cleanedInstructions, ". <" );
				cleanedInstructions = _htmlRegex.Replace( cleanedInstructions, String.Empty );
				string[] ainstructions = Regex.Split( cleanedInstructions, pattern );

				// check for empty line
				if(ainstructions[ainstructions.Length - 1].Length == 0)
				{
					var cleaned = new List<String>( ainstructions );
					cleaned.RemoveAt( ainstructions.Length - 1 );
					ainstructions = cleaned.ToArray( );
				}

				return ainstructions;
			}
			else
			{
				return new string[0];
			}
		}
	}

	// only supplied when you pull the recipe from the api directly, not in search results
	public class Nutrition {
		[JsonProperty( "nutrients" )]
		public List<Nutrient> nutrients { get; set; }
	}

	
	public class Nutrient {
		[JsonProperty( "name" )]
		public string Name { get; set; }

		[JsonProperty( "title" )]
		public string Title { get; set; }

		[JsonProperty( "amount" )]
		public double Amount { get; set; }

		[JsonProperty( "unit" )]
		public string Unit { get; set; }

		[JsonProperty( "percentOfDailyNeeds" )]
		public double PercentOfDailyNeeds { get; set; }
	}


	public class Ingredient {
		[JsonProperty( "id" )]
		public string id { get; set; }

		[JsonProperty( "name" )]
		public string name { get; set; }

		[JsonProperty( "amount" )]
		public string amount { get; set; }

		[JsonProperty( "unit" )]
		public string unit { get; set; }
	}


	public enum ActionTypes {
		NoType = 0,
		InsertSuccessful = 1,
		UpdateSuccessful = 2,
		DuplicateID = 4,
		Unknown = 5,
		RequiredFieldsMissing = 6
	}

}