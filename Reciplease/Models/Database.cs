using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Reciplease.Models {
	public class Database {		

		private bool GetDBConnection( ref SqlConnection SQLConn ) {
			try
			{
				if ( SQLConn == null ) SQLConn = new SqlConnection( );
				if ( SQLConn.State != ConnectionState.Open )
				{
					SQLConn.ConnectionString = ConfigurationManager.AppSettings["AppDBConnect"];
					SQLConn.Open( );
				}
				return true;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}

		private bool CloseDBConnection( ref SqlConnection SQLConn ) {
			try
			{
				if ( SQLConn.State != ConnectionState.Closed )
				{
					SQLConn.Close( );
					SQLConn.Dispose( );
					SQLConn = null;
				}
				return true;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}

		private int SetParameter( ref SqlCommand cm, string ParameterName, Object Value
			, SqlDbType ParameterType, int FieldSize = -1
			, ParameterDirection Direction = ParameterDirection.Input
			, Byte Precision = 0, Byte Scale = 0 ) {
			try
			{
				cm.CommandType = CommandType.StoredProcedure;
				if ( FieldSize == -1 )
					cm.Parameters.Add( ParameterName, ParameterType );
				else
					cm.Parameters.Add( ParameterName, ParameterType, FieldSize );

				if ( Precision > 0 ) cm.Parameters[cm.Parameters.Count - 1].Precision = Precision;
				if ( Scale > 0 ) cm.Parameters[cm.Parameters.Count - 1].Scale = Scale;

				cm.Parameters[cm.Parameters.Count - 1].Value = Value;
				cm.Parameters[cm.Parameters.Count - 1].Direction = Direction;

				return 0;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		private int SetParameter( ref SqlDataAdapter cm, string ParameterName, Object Value
			, SqlDbType ParameterType, int FieldSize = -1
			, ParameterDirection Direction = ParameterDirection.Input
			, Byte Precision = 0, Byte Scale = 0 ) {
			try
			{
				cm.SelectCommand.CommandType = CommandType.StoredProcedure;
				if ( FieldSize == -1 )
					cm.SelectCommand.Parameters.Add( ParameterName, ParameterType );
				else
					cm.SelectCommand.Parameters.Add( ParameterName, ParameterType, FieldSize );

				if ( Precision > 0 ) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Precision = Precision;
				if ( Scale > 0 ) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Scale = Scale;

				cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Value = Value;
				cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Direction = Direction;

				return 0;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public User.ActionTypes InsertUser( User u ) {
			try
			{
				SqlConnection cn = null;
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlCommand cm = new SqlCommand( "uspAddNewUser", cn );
				int intReturnValue = -1;

				SetParameter( ref cm, "@intUserID", u.UID, SqlDbType.Int, Direction: ParameterDirection.Output );
				SetParameter( ref cm, "@strUsername", u.Username, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strPassword", u.Password, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strEmail", u.Email, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strFirstName", u.FirstName, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strLastName", u.LastName, SqlDbType.NVarChar );
				SetParameter( ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue );

				cm.ExecuteReader( );

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection( ref cn );

				switch ( intReturnValue )
				{
					case 0: // new user created
						u.UID = (int)cm.Parameters["@intUserID"].Value;
						return User.ActionTypes.InsertSuccessful;
					case 1:
						return User.ActionTypes.DuplicateUsername;
					case 2:
						return User.ActionTypes.DuplicateEmail;
					default:
						return User.ActionTypes.Unknown;
				}
			}
			catch ( Exception ex ) {
				System.Diagnostics.Debug.WriteLine( ex.ToString( ) );
				return User.ActionTypes.Unknown;
			}
		}




		internal Recipe LoadRecipe( string strRecipeID ) {
			try
			{
				DataSet ds = new DataSet( );
				SqlConnection cn = new SqlConnection( );
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				Recipe recipe = new Recipe( );

				SqlCommand selectCMD = new SqlCommand( "SELECT * FROM TRecipes WHERE intRecipeID=" + strRecipeID, cn );
				SqlDataAdapter da = new SqlDataAdapter( );
				da.SelectCommand = selectCMD;


				try
				{
					da.Fill( ds );
				}
				catch ( Exception ex2 )
				{
					System.Diagnostics.Debug.WriteLine( "getting user ratings error: " + ex2.Message );
				}
				finally { CloseDBConnection( ref cn ); }

				if ( ds.Tables[0].Rows.Count != 0 )
				{
					foreach ( DataRow dr in ds.Tables[0].Rows )
					{
						recipe.id = strRecipeID;
						recipe.title = (string)dr["strName"];
						recipe.image = (string)dr["strRecipeImage"];
						recipe.readyInMinutes = ( (int)dr["intReadyInMins"] ).ToString( );
						recipe.servings = ( (int)dr["intServings"] ).ToString( );
						recipe.instructions = (string)dr["strInstructions"];
						recipe.dishTypes = ( (string)dr["strName"] ).Split( ',' ).ToList( );
						recipe.cuisines = ( (string)dr["strCuisines"] ).Split( ',' ).ToList( );
						recipe.diets = ( (string)dr["strDiets"] ).Split( ',' ).ToList( );
						try
						{
							recipe.nutrition = JsonConvert.DeserializeObject<Nutrition>( (string)dr["strNutrition"] );
						}
						catch ( Exception ex )
						{
							recipe.nutrition = new Nutrition( );
						}
					}
					recipe.extendedIngredients = GetIngredients( strRecipeID );

					if (recipe.image.Length < 5)
					{
						recipe.image = "/Content/images/no-photo.jpg";
					}

					return recipe;
				}
				return null;
			}
				
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}

		private List<Ingredient> GetIngredients( string strRecipeID ) {
			List<Ingredient> ingredients = new List<Ingredient>( );
			DataSet ds = new DataSet( );
			SqlConnection cn = new SqlConnection( );
			if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
			Recipe recipe = new Recipe( );

			SqlCommand selectCMD = new SqlCommand( "SELECT * FROM VRecipeIngredients WHERE intRecipeID=" + strRecipeID, cn );
			SqlDataAdapter da = new SqlDataAdapter( );
			da.SelectCommand = selectCMD;


			try
			{
				da.Fill( ds );
			}
			catch ( Exception ex2 )
			{
				System.Diagnostics.Debug.WriteLine( "getting user ratings error: " + ex2.Message );
			}
			finally { CloseDBConnection( ref cn ); }

			if ( ds.Tables[0].Rows.Count != 0 )
			{
				foreach ( DataRow dr in ds.Tables[0].Rows )
				{
					Ingredient ingredient = new Ingredient( );
					ingredient.amount = ((double)dr["dblIngredientQuantity"]).ToString();
					ingredient.unit = (string)dr["strUnitOfMeasurement"];
					ingredient.name = (string)dr["strIngredientName"];
					ingredients.Add( ingredient );

				}
			}

			return ingredients;

		}

		internal bool RecipeExists( string recipeID ) {
			bool exists = false;
			DataSet ds = new DataSet( );
			SqlConnection cn = new SqlConnection( );
			if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
			List<Rating> ratings = new List<Rating>( );



			SqlCommand selectCMD = new SqlCommand( "SELECT * FROM TRecipes WHERE intRecipeID=" + recipeID, cn );
			SqlDataAdapter da = new SqlDataAdapter( );
			da.SelectCommand = selectCMD;


			try
			{
				da.Fill( ds );
			}
			catch ( Exception ex2 )
			{
				System.Diagnostics.Debug.WriteLine( "checking for recipe id error: " + ex2.Message );
			}
			finally { CloseDBConnection( ref cn ); }

			if ( ds.Tables[0].Rows.Count != 0 )
			{
				exists = true;
			}

			return exists;
		}





		public User Login( User u ) {
			try
			{
				SqlConnection cn = new SqlConnection( );
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlDataAdapter da = new SqlDataAdapter( "uspLogin", cn );
				DataSet ds;
				User newUser = new User{
					ActionType = User.ActionTypes.LoginFailed
				};
				da.SelectCommand.CommandType = CommandType.StoredProcedure;

				SetParameter( ref da, "@strUsername", u.Username, SqlDbType.NVarChar );
				SetParameter( ref da, "@strPassword", u.Password, SqlDbType.NVarChar );


				try
				{
					ds = new DataSet( );
					da.Fill( ds );
					if ( ds.Tables[0].Rows.Count > 0 )
					{
						newUser.ActionType = User.ActionTypes.NoType;
						DataRow dr = ds.Tables[0].Rows[0];
						newUser.UID = (int)dr["intUserID"];
						newUser.Username = u.Username;
						newUser.Password = u.Password;
						newUser.FirstName = (string)dr["strFirstName"];
						newUser.LastName = (string)dr["strLastName"];
						newUser.Email = (string)dr["strEmail"];
						// get user ratings list
						newUser.Ratings = GetUserRatings( newUser.UID );
						newUser.Favorites = GetUserFavorites( newUser.UID );
					}
				}
				catch ( Exception ex ) { throw new Exception( ex.Message ); }
				finally
				{
					CloseDBConnection( ref cn );
				}

				return newUser; //alls well in the world
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}


		public User.ActionTypes UpdateUser( User u ) {
			try
			{
				SqlConnection cn = null;
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlCommand cm = new SqlCommand( "uspUpdateUser", cn );
				int intReturnValue = -1;

				SetParameter( ref cm, "@intUserID", u.UID, SqlDbType.Int );
				SetParameter( ref cm, "@strUsername", u.Username, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strPassword", u.Password, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strEmail", u.Email, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strFirstName", u.FirstName, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strLastName", u.LastName, SqlDbType.NVarChar );
				SetParameter( ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue );

				cm.ExecuteReader( );

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection( ref cn );

				switch ( intReturnValue )
				{
					case 0: //new updated
						return User.ActionTypes.UpdateSuccessful;
					case 1:
						return User.ActionTypes.DuplicateUsername;
					case 2:
						return User.ActionTypes.DuplicateEmail;
					default:
						return User.ActionTypes.Unknown;
				}
			}
			catch ( Exception ex ) {
				System.Diagnostics.Debug.WriteLine( ex.ToString() );
				return User.ActionTypes.Unknown; }
		}




		public int RateRecipe( int UID, int RecipeID, int intDifficultyRating, int intTasteRating ) {
			try
			{
				SqlConnection cn = null;
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlCommand cm = new SqlCommand( "uspRateRecipe", cn );
				int intReturnValue = -1;

				
				SetParameter( ref cm, "@intUserID", UID, SqlDbType.BigInt );
				SetParameter( ref cm, "@intTasteID", intTasteRating, SqlDbType.BigInt );
				SetParameter( ref cm, "@intDifficultyID", intDifficultyRating, SqlDbType.BigInt );
				SetParameter( ref cm, "@intRecipeID", RecipeID, SqlDbType.BigInt );
				
				SetParameter( ref cm, "ReturnValue", 0, SqlDbType.Int, Direction: ParameterDirection.ReturnValue );

				cm.ExecuteReader( );

				//0 = new rate added
				//1 = existing rate updated
				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection( ref cn );
				return intReturnValue;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}



		public List<Rating> GetUserRatings( int intUserID ) {
			try
			{
				DataSet ds = new DataSet( );
				SqlConnection cn = new SqlConnection( );
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				List<Rating> ratings = new List<Rating>( );



				SqlCommand selectCMD = new SqlCommand( "SELECT * FROM VUserRating WHERE intUserID=" + intUserID, cn );
				SqlDataAdapter da = new SqlDataAdapter( );
				da.SelectCommand = selectCMD;


				try
				{
					da.Fill( ds );
				}
				catch ( Exception ex2 )
				{
					System.Diagnostics.Debug.WriteLine( "getting user ratings error: " + ex2.Message );
				}
				finally { CloseDBConnection( ref cn ); }
			
				if ( ds.Tables[0].Rows.Count != 0 )
				{
					foreach ( DataRow dr in ds.Tables[0].Rows )
					{
						Rating r = new Rating( );
						r.intRecipeID = (int)dr["intRecipeID"];
						r.intTasteRating = (int)dr["intTasteID"];
						r.intDifficultyRating = (int)dr["intDifficultyID"];
						ratings.Add( r );
					}
				}
				return ratings;
			}
			catch ( Exception ex ) { throw new Exception( ex.Message ); }
		}




		public Dictionary<String, int> GetRecipeRatings( int intRecipeID ) {
			Dictionary<String, int> ratings = new Dictionary<String, int>
				{
					{"AverageDifficulty", 0 },
					{"DifficultyCount", 0 },
					{"AverageTaste", 0 },
					{"TasteCount", 0 }
				};
			try
			{
				DataSet ds = new DataSet( );
				SqlConnection cn = new SqlConnection( );
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlCommand command = cn.CreateCommand();
				command.CommandText = "SELECT * FROM VRecipeRatings WHERE intRecipeID=" + intRecipeID;
				

				using ( SqlDataReader reader = command.ExecuteReader( ) )
				{
					while ( reader.Read( ) )
					{
						ratings["AverageDifficulty"] = reader.GetInt32( 2 );
						ratings["DifficultyCount"] = reader.GetInt32( 3 );
						ratings["AverageTaste"] = reader.GetInt32( 4 );
						ratings["TasteCount"] = reader.GetInt32( 5 );
					}
				}

				return ratings;
			}
			catch ( Exception ex ) {
				System.Diagnostics.Debug.WriteLine( "getting recipe ratings error: " +  ex.Message );
				return ratings; }
		}




		public void TestDBConnection() {
			bool blnResult = true;
			SqlConnection cn = new SqlConnection( );
			if ( !GetDBConnection( ref cn ) )
			{
				blnResult = false;
			}
			

			if (blnResult == true )
			{
				System.Diagnostics.Debug.WriteLine( "Db Connection Successful" );
			}
			else
			{
				System.Diagnostics.Debug.WriteLine( "Db Connection failed" );
			}
		}


		public int ToggleFavorite( int UID, int RecipeID ) {
			try
			{
				SqlConnection cn = null;
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlCommand cm = new SqlCommand( "uspFavoriteUnfavorite", cn );
				int intReturnValue = -1;

				SetParameter( ref cm, "@intUserID", UID, SqlDbType.Int );
				SetParameter( ref cm, "@intRecipeID", RecipeID, SqlDbType.BigInt );
				SetParameter( ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue );

				cm.ExecuteReader( );

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection( ref cn );

				return intReturnValue; // 0 is favorited, 1 is unfavorited
			}
			catch ( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( ex.ToString( ) );
				return -1; // somethind went wrong, check debug
			}
		}

		public int SaveRecipe( string strName, string strInstructions, int intReadyInMins = -1, string strRecipeImage = "'/Content/images/no-photo.jpg'",
			int intServings = -1, string strCuisines = "-1", string strDiets = "-1", string strDishTypes = "-1", string strNutrition = "-1", int UID = -1, int intRecipeID = 0) {
			try
			{
				SqlConnection cn = null;
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlCommand cm = new SqlCommand( "uspAddRecipe", cn );
				int intReturnValue = -1;

				SetParameter( ref cm, "@intRecipeID", intRecipeID, SqlDbType.NVarChar);
				SetParameter( ref cm, "@strName", strName, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strInstructions", strInstructions, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strRecipeImage", strRecipeImage, SqlDbType.NVarChar );
				SetParameter( ref cm, "@intReadyInMins", intReadyInMins, SqlDbType.BigInt );
				SetParameter( ref cm, "@intServings", intServings, SqlDbType.BigInt );
				SetParameter( ref cm, "@strCuisines", strCuisines, SqlDbType.NVarChar ); 
				SetParameter( ref cm, "@strDiets", strDiets, SqlDbType.NVarChar ); 
				SetParameter( ref cm, "@strDishTypes", strDishTypes, SqlDbType.NVarChar ); 
				SetParameter( ref cm, "@strNutrition", strNutrition, SqlDbType.NVarChar ); 
				SetParameter( ref cm, "@intUserID", UID, SqlDbType.Int ); 
				
				SetParameter( ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue );

				cm.ExecuteReader( );

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;
				CloseDBConnection( ref cn );

				return intReturnValue;
			}
			catch ( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( ex.ToString( ) );
				return -1; // something went wrong
			}
		}


		public int SaveIngredient( int intIngredientID, string strIngredientName ) {
			try
			{
				SqlConnection cn = null;
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlCommand cm = new SqlCommand( "uspAddIngredient", cn );
				int intReturnValue = -1;

				SetParameter( ref cm, "@intIngredientID", intIngredientID, SqlDbType.NVarChar );
				SetParameter( ref cm, "@strIngredientName", strIngredientName, SqlDbType.NVarChar );

				SetParameter( ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue );

				cm.ExecuteReader( );

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;

				CloseDBConnection( ref cn );

				return intReturnValue;
			}
			catch ( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( ex.ToString( ) );
				return -1; // something went wrong
			}
		}


		public int AddRecipeIngredients( int inRecipeID, int intIngredientID, double dblIngredientQuantity, string strUnit ) {
			try
			{
				SqlConnection cn = null;
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				SqlCommand cm = new SqlCommand( "uspAddRecipeIngredients", cn );
				int intReturnValue = -1;

				SetParameter( ref cm, "@intRecipeID", inRecipeID, SqlDbType.NVarChar );
				SetParameter( ref cm, "@intIngredientID", intIngredientID, SqlDbType.NVarChar );
				SetParameter( ref cm, "@dblIngredientQuantity", dblIngredientQuantity, SqlDbType.Float );
				SetParameter( ref cm, "@strUnitOfMeasurement", strUnit, SqlDbType.NVarChar );

				SetParameter( ref cm, "ReturnValue", 0, SqlDbType.TinyInt, Direction: ParameterDirection.ReturnValue );

				cm.ExecuteReader( );

				intReturnValue = (int)cm.Parameters["ReturnValue"].Value;

				CloseDBConnection( ref cn );

				return intReturnValue;
			}
			catch ( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( ex.ToString( ) );
				return -1; // something went wrong
			}
		}



		public List<Recipe> GetUserFavorites( int intUserID ) {
			try
			{
				Database db = new Database( );
				DataSet ds = new DataSet( );
				SqlConnection cn = new SqlConnection( );
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				List<Recipe> recipes = new List<Recipe>( );



				SqlCommand selectCMD = new SqlCommand( "SELECT * FROM VUserFavorites WHERE intUserID=" + intUserID, cn );
				SqlDataAdapter da = new SqlDataAdapter( );
				da.SelectCommand = selectCMD;


				try
				{
					da.Fill( ds );
				}
				catch ( Exception ex2 )
				{
					System.Diagnostics.Debug.WriteLine( "getting user ratings error: " + ex2.Message );
				}
				finally { CloseDBConnection( ref cn ); }

				if ( ds.Tables[0].Rows.Count != 0 )
				{
					foreach ( DataRow dr in ds.Tables[0].Rows )
					{
						Recipe r = new Recipe( );
						r.id = ((int)dr["intRecipeID"]).ToString();
						r.image = (string)dr["strRecipeImage"];
						r.readyInMinutes = ((int)dr["intReadyInMins"]).ToString();
						r.dictRatings = db.GetRecipeRatings( int.Parse(r.id) );
						if ( r.image.Length < 5 )
						{
							r.image = "/Content/images/no-photo.jpg";
						}
						recipes.Add( r );
					}
				}
				
				return recipes;
			}
			catch ( Exception ex ) { System.Diagnostics.Debug.WriteLine( ex.ToString( ) ); return null; }
		}


		public List<Recipe> GetUserCreations( int intUserID ) {
			try
			{
				Database db = new Database( );
				DataSet ds = new DataSet( );
				SqlConnection cn = new SqlConnection( );
				if ( !GetDBConnection( ref cn ) ) throw new Exception( "Database did not connect" );
				List<Recipe> recipes = new List<Recipe>( );



				SqlCommand selectCMD = new SqlCommand( "SELECT * FROM TRecipes WHERE intUserID=" + intUserID, cn );
				SqlDataAdapter da = new SqlDataAdapter( );
				da.SelectCommand = selectCMD;


				try
				{
					da.Fill( ds );
				}
				catch ( Exception ex2 )
				{
					System.Diagnostics.Debug.WriteLine( "getting user recipes error: " + ex2.Message );
				}
				finally { CloseDBConnection( ref cn ); }

				if ( ds.Tables[0].Rows.Count != 0 )
				{
					foreach ( DataRow dr in ds.Tables[0].Rows )
					{
						Recipe r = new Recipe( );
						r.id = ( (int)dr["intRecipeID"] ).ToString( );
						r.image = (string)dr["strRecipeImage"];
						r.readyInMinutes = ( (int)dr["intReadyInMinutes"] ).ToString( );
						r.dictRatings = db.GetRecipeRatings( int.Parse( r.id ) );
						recipes.Add( r );
					}
				}
				return recipes;
			}
			catch ( Exception ex ) { System.Diagnostics.Debug.WriteLine( ex.ToString( ) ); return null; }
		}
	}
}