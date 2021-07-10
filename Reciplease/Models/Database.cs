using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;

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
					//SysLog.UpdateLogFile(this.ToString(), MethodBase.GetCurrentMethod().Name.ToString(), ex2.Message);
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
				System.Diagnostics.Debug.WriteLine( "getting ratings error: " +  ex.Message );
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

	}
}