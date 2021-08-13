USE [CPDM-GroupA]   -- Get out of the master database
SET NOCOUNT ON;		-- Report only errors

-- ---------------------------------------------------------------------------------
-- Drop Views
-- ---------------------------------------------------------------------------------

IF OBJECT_ID('VRecipeIngredients')				IS NOT NULL DROP VIEW VRecipeIngredients
IF OBJECT_ID('VUserFavorites')					IS NOT NULL DROP VIEW VUserFavorites
IF OBJECT_ID('VUserShoppingList')				IS NOT NULL DROP VIEW VUserShoppingList
IF OBJECT_ID('VRecipeRatings')					IS NOT NULL DROP VIEW VRecipeRatings 
IF OBJECT_ID('vUserRating')						IS NOT NULL DROP VIEW vUserRating
IF OBJECT_ID('vUserLast10')						IS NOT NULL DROP VIEW vUserLast10
IF OBJECT_ID('vEarliestLast10')					IS NOT NULL DROP VIEW vEarliestLast10

-- ---------------------------------------------------------------------------------
-- Drop Procedures
-- ---------------------------------------------------------------------------------

IF OBJECT_ID('uspAddNewUser')					IS NOT NULL DROP PROCEDURE uspAddNewUser
IF OBJECT_ID('uspLogin')						IS NOT NULL DROP PROCEDURE uspLogin
IF OBJECT_ID('uspRateRecipe')					IS NOT NULL DROP PROCEDURE uspRateRecipe 
IF OBJECT_ID('uspUpdateUser')					IS NOT NULL DROP PROCEDURE uspUpdateUser 
IF OBJECT_ID('uspRateRecipe')					IS NOT NULL DROP PROCEDURE uspRateRecipe 
IF OBJECT_ID('uspFavoriteUnfavorite')			IS NOT NULL DROP PROCEDURE uspFavoriteUnfavorite 
IF OBJECT_ID('uspAddRecipe')					IS NOT NULL DROP PROCEDURE uspAddRecipe 
IF OBJECT_ID('uspAddIngredient')				IS NOT NULL DROP PROCEDURE uspAddIngredient 
IF OBJECT_ID('uspAddRecipeIngredients')			IS NOT NULL DROP PROCEDURE uspAddRecipeIngredients 
IF OBJECT_ID('uspDeleteUserAccount')			IS NOT NULL DROP PROCEDURE uspDeleteUserAccount 
IF OBJECT_ID('uspDeleteUserRecipe')				IS NOT NULL DROP PROCEDURE uspDeleteUserRecipe 
IF OBJECT_ID('uspUpdateRecipe')					IS NOT NULL DROP PROCEDURE uspUpdateRecipe 
IF OBJECT_ID('uspAddToShoppingList')			IS NOT NULL DROP PROCEDURE uspAddToShoppingList 
IF OBJECT_ID('uspDropUserShoppingList')			IS NOT NULL DROP PROCEDURE uspDropUserShoppingList 
IF OBJECT_ID('uspUserLast10')					IS NOT NULL DROP PROCEDURE uspUserLast10 

-- --------------------------------------------------------------------------------------------
-- # VIEWS #
-- --------------------------------------------------------------------------------------------

GO

Create View VRecipeIngredients
AS
Select	 TRI.intRecipeIngredientID
		,TR.intRecipeID
		,TR.strName
		,TRI.dblIngredientQuantity
		,TRI.strUnitOfMeasurement
		,TI.strIngredientName
	 

FROM	TRecipes as TR JOIN TRecipeIngredients as TRI
		ON TR.intRecipeID = TRI.intRecipeID

		JOIN TIngredients as TI
		ON TI.intIngredientID = TRI.intIngredientID

GO

SELECT * FROM VRecipeIngredients

GO

-- --------------------------------------------------------------------------------------------

Create View VUserFavorites
AS
Select	 TU.intUserID
		,TR.intRecipeID
		,TR.strName
		,TR.intReadyInMins
		,TR.strRecipeImage

FROM	TUsers as TU JOIN TUserFavorites as TUF
		ON TU.intUserID = TUF.intUserID

		LEFT JOIN TRecipes as TR
		ON TR.intRecipeID = TUF.intRecipeID

GO

SELECT * FROM VUserFavorites

GO

--------------------------------------------------------------------------------------------

Create View VUserShoppingList
AS
SELECT	 TU.intUserID
		,TR.intRecipeID
		,TR.strName
		,TR.intServings
		--,TSL.intRecipeIngredientID
		,TRI.intIngredientID
		,TI.strIngredientName
		,TRI.dblIngredientQuantity
		,TRI.strUnitOfMeasurement

FROM	TUsers as TU JOIN TShoppingList TSL
		ON TU.intUserID = TSL.intUserID

		JOIN TRecipeIngredients as TRI
		ON TRI.intRecipeIngredientID = TSL.intRecipeIngredientID

		JOIN TRecipes as TR
		ON TR.intRecipeID = TRI.intRecipeID

		JOIN TIngredients as TI
		ON TI.intIngredientID = TRI.intIngredientID

		GROUP BY TU.intUserID, TR.intRecipeID, TR.strName, TR.intServings, TRI.intIngredientID, TI.strIngredientName, TRI.dblIngredientQuantity, TRI.strUnitOfMeasurement

		--JOIN TMeasurementUnits as TMU
		--ON TMU.intMeasurementUnitID = TRI.intMeasurementUnitID	

GO

SELECT * FROM VUserShoppingList WHERE intUserID = 1

GO

-- --------------------------------------------------------------------------------------------

Create View VRecipeRatings
AS
SELECT		 TR.intRecipeID
			,TR.strName
			,AVG(TD.intDifficultyRating) as AverageDifficulty
			,COUNT(TD.intDifficultyID) as DifficultyVotes
			,AVG(TT.intTasteRating) as AverageTaste
			,COUNT(TT.intTasteRating) as TasteVotes

FROM		TDifficulty as TD JOIN TRatings as TRa
			ON TD.intDifficultyID = TRa.intDifficultyID

			JOIN TTaste as TT
			ON TT.intTasteID = TRa.intTasteID

			JOIN TRecipes as TR
			ON TR.intRecipeID = TRa.intRecipeID

GROUP BY TR.intRecipeID, TR.strName

GO

SELECT * FROM VRecipeRatings

GO

-- --------------------------------------------------------------------------------------------

Create View vUserRating
AS
SELECT		 TRe.intRecipeID
			,TU.intUserID
			,TD.intDifficultyID
			,TT.intTasteID

FROM		TUsers as TU JOIN TRatings as TR
			ON TU.intUserID = TR.intUserID

			JOIN TDifficulty as TD
			ON TD.intDifficultyID = TR.intDifficultyID

			JOIN TTaste as TT
			ON TT.intTasteID = TR.intTasteID

			JOIN TRecipes as TRe
			ON TRe.intRecipeID = TR.intRecipeID

GO

--SELECT * FROM vUserRating WHERE intRecipeID = 1

GO

-- --------------------------------------------------------------------------------------------

Create View vUserLast10
AS
SELECT		 TU.intUserID
			,TL10.intLast10ID
			,TR.intRecipeID
			,TR.strName
			,TR.intReadyInMins
			,TR.strRecipeImage

FROM		TUsers as TU JOIN TLast10 as TL10
			ON TU.intUserID = TL10.intUserID

			JOIN TRecipes as TR
			ON TR.intRecipeID = TL10.intRecipeID

GO

SELECT * FROM vUserLast10

--------------------------------------------------------------------------------------------

GO

Create View vEarliestLast10
AS
SELECT		 TU.intUserID
			,MIN(TL10.intLast10ID) as intLast10ID

FROM		TUsers as TU JOIN TLast10 as TL10
			ON TU.intUserID = TL10.intUserID

			JOIN TRecipes as TR
			ON TR.intRecipeID = TL10.intRecipeID

GROUP BY TU.intUserID

GO

SELECT * FROM vEarliestLast10 WHERE intUserID = 1
-- --------------------------------------------------------------------------------------------
-- # PROCEDURES #
-- --------------------------------------------------------------------------------------------

GO
Create Procedure uspUserLast10
					 @intUserID		AS INTEGER OUTPUT
					,@intRecipeID	AS INTEGER OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	DECLARE @Exists as INTEGER
	DECLARE @intLast10ID as INTEGER
	DECLARE @Count as INTEGER

		-- Returns 1 if username exists, 0 if it doesn't
	DECLARE ifInLast10 CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TLast10 WHERE intRecipeID = @intRecipeID AND intUserID = @intUserID -- Returns 1 if exists, 0 if it doesn't

	DECLARE if10Saved CURSOR LOCAL FOR
	SELECT COUNT(*) FROM TLast10 WHERE intUserID = 1

		OPEN ifInLast10

		FETCH FROM ifInLast10
		INTO @Exists

		CLOSE ifInLast10

		OPEN if10Saved

		FETCH FROM if10Saved
		INTO @Count

		CLOSE if10Saved

	IF @Exists = 1 -- if the recipe already exists in the users last10, its removed and replaced so its the 'newest' last 10
		BEGIN
			DELETE FROM TLast10 WHERE intRecipeID = @intRecipeID AND intUserID = @intUserID

			-- Gets next Last10ID
			SELECT @intLast10ID = MAX(intLast10ID) + 1
			FROM Tlast10 (TABLOCKX) -- Locks table till end of transaction

			-- Defaults to 1 if no ID
			SELECT @intLast10ID = COALESCE ( @intLast10ID, 1)

			INSERT INTO TLast10	(intLast10ID, intUserID, intRecipeID)
			VALUES				(@intLast10ID, @intUserID, @intRecipeID)

		END
	ELSE IF @Exists = 0 -- if user has not favorited the favorite will be added
		BEGIN
			IF @Count = 10 -- if a user has 10 last10 recipes, deletes the earliest recipe (minimumID for user) and adds a new one.
				BEGIN
					DELETE FROM TLast10 WHERE intLast10ID = (SELECT COUNT(1) FROM vEarliestLast10 WHERE intUserID = @intUserID)

					-- Gets next Last10ID
					SELECT @intLast10ID = MAX(intLast10ID) + 1
					FROM Tlast10 (TABLOCKX) -- Locks table till end of transaction

					-- Defaults to 1 if no Users
					SELECT @intLast10ID = COALESCE ( @intLast10ID, 1)

					INSERT INTO TLast10	(intLast10ID, intUserID, intRecipeID)
					VALUES				(@intLast10ID, @intUserID, @intRecipeID)
			
				END
			ELSE IF @Count < 10 -- If user has less than 10 last10 recipes, a new one will be added with next ID
				BEGIN
					-- Gets next Last10ID
					SELECT @intLast10ID = MAX(intLast10ID) + 1
					FROM Tlast10 (TABLOCKX) -- Locks table till end of transaction

					-- Defaults to 1 if no Users
					SELECT @intLast10ID = COALESCE ( @intLast10ID, 1)

					INSERT INTO TLast10	(intLast10ID, intUserID, intRecipeID)
					VALUES				(@intLast10ID, @intUserID, @intRecipeID)
			
				END
		END
COMMIT TRANSACTION	

GO


--SELECT * FROM vUserLast10 WHERE intUserID = 1
--EXECUTE uspUserLast10 1, 5000008
--SELECT * FROM vUserLast10 WHERE intUserID = 1

--SELECT * FROM vUserLast10 WHERE intUserID = 2
--EXECUTE uspUserLast10 2, 5000003
--SELECT * FROM vUserLast10 WHERE intUserID = 2


-- --------------------------------------------------------------------------------------------
GO

Create Procedure uspAddNewUser -- Adds new user to DB
		 @intUserID			AS INTEGER OUTPUT
		,@strFirstName		as VARCHAR(50) OUTPUT
		,@strLastName		as VARCHAR(50) OUTPUT
		,@strEmail			as VARCHAR(50) OUTPUT
		,@strPassword		as VARCHAR(50) OUTPUT
		,@strUsername		as VARCHAR(50) OUTPUT

AS
SET XACT_ABORT ON -- Terminates and rolls back if any errors are encountered

BEGIN TRANSACTION

	DECLARE @UserNameExists INTEGER
	DECLARE @EmailExists	INTEGER

	-- Returns 1 if username exists, 0 if it doesn't
	DECLARE ifUserNameExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TUsers WHERE strUserName = @strUsername -- Returns 1 if username exists, 0 if it doesn't

		OPEN ifUserNameExists

		FETCH FROM ifUserNameExists
		INTO @UserNameExists

		CLOSE ifUserNameExists

	DECLARE ifEmailExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TUsers WHERE strEmail = @strEmail -- Returns 1 if email exists, 0 if it doesn't

		OPEN ifEmailExists

		FETCH FROM ifEmailExists
		INTO @EmailExists

		CLOSE ifEmailExists

	IF @UserNameExists = 0 AND @EmailExists = 0
		BEGIN

			-- Gets next UserID
			SELECT @intUserID = MAX(intUserID) + 1
			FROM TUsers (TABLOCKX) -- Locks table till end of transaction

			-- Defaults to 1 if no Users
			SELECT @intUserID = COALESCE ( @intUserID, 1)

			SET IDENTITY_INSERT TUsers ON

			INSERT INTO TUsers	(intUserID, strFirstName, strLastName, strEmail, strPassword, strUserName)
			VALUES				(@intUserID, @strFirstName, @strLastName, @strEmail, @strPassword, @strUsername)

			SET IDENTITY_INSERT TUsers OFF

		END

	ELSE IF @UserNameExists = 1

		BEGIN
			COMMIT
			RETURN @UserNameExists -- Will return 1 to signal that the Username is already used
			PRINT @strUserName
		END

	ELSE IF @EmailExists = 1

		BEGIN
			COMMIT
			RETURN @EmailExists + 1 -- Will return 2 to signal that the EMAIL is already used
			PRINT @strEmail
		END
		
COMMIT TRANSACTION

GO	

--SELECT * FROM TUsers
--DECLARE @intUserID as INTEGER = 0
--DECLARE @MSG as INTEGER = 0
--EXECUTE @MSG = uspAddNewUser @intUserID OUTPUT, 'Sofie', 'Tcilina', 'st@gmail.com', '55555', 'Falcawn' -- Test by uncommenting, if username exists will return a 1, if email exists will return a 2, if none exists return 0
--PRINT 'USER ID = ' + CONVERT(VARCHAR, @intUserID)
--PRINT @MSG
--SELECT * FROM TUsers


--GO

-- --------------------------------------------------------------------------------------------

Create Procedure uspLogin	-- Login for users based on Username and password input
			 @strUsername		as VARCHAR(50) OUTPUT
			,@strPassword		as VARCHAR(50) OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	-- Returns entire table if user exists, returns nothing if they don't exist
	SELECT * FROM TUsers WHERE @strUsername = strUserName AND @strPassword = strPassword
	
COMMIT TRANSACTION



--GO

--EXECUTE uspLogin 'Falcawn', 'reciplease1'

-- --------------------------------------------------------------------------------------------

GO

Create Procedure uspRateRecipe
				 @intUserID			as INTEGER OUTPUT
				,@intTasteID		as INTEGER OUTPUT
				,@intDifficultyID	as INTEGER OUTPUT
				,@intRecipeID		as INTEGER OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION	
	DECLARE @Exists as INTEGER = 0
	DECLARE @intRatingID as INTEGER = 0

	DECLARE ifExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TRatings WHERE @intUserID = intUserID AND @intRecipeID = intRecipeID -- checks if user has rated this recipe yet

		OPEN ifExists

		FETCH FROM ifExists
		INTO @Exists
		
		CLOSE ifExists
	
		IF @Exists = 1
			BEGIN
				UPDATE TRatings
				SET intTasteID = @intTasteID, intDifficultyID = @intDifficultyID
				WHERE @intUserID = intUserID AND @intRecipeID = intRecipeID
				COMMIT
				RETURN @Exists
			END
		ELSE IF @Exists = 0
			BEGIN
				
				-- Gets next RatingID
				SELECT @intRatingID = MAX(intRatingID) + 1
				FROM TRatings (TABLOCKX) -- Locks table till end of transaction

				-- Defaults to 1 if no Users
				SELECT @intRatingID = COALESCE ( @intRatingID, 1)
				
				INSERT INTO TRatings (intRatingID, intUserID, intDifficultyID, intTasteID, intRecipeID)
				VALUES				 (@intRatingID, @intUserID, @intDifficultyID, @intTasteID, @intRecipeID)

				COMMIT
				RETURN @Exists

			END
COMMIT TRANSACTION

GO

--DECLARE @Exists as INTEGER
--SELECT * FROM VRecipeRatings
--EXECUTE @Exists = uspRateRecipe 1, 4, 4, 2
--SELECT * FROM VRecipeRatings
--PRINT @Exists

GO
-- --------------------------------------------------------------------------------------------

Create Procedure uspUpdateUser
			 @intUserID			as INTEGER OUTPUT
			,@strFirstName		as VARCHAR(50) OUTPUT
			,@strLastName		as VARCHAR(50) OUTPUT
			,@strEmail			as VARCHAR(50) OUTPUT
			,@strPassword		as VARCHAR(50) OUTPUT
			,@strUsername		as VARCHAR(50) OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	DECLARE @UserNameExists INTEGER
	DECLARE @EmailExists	INTEGER

	-- Returns 1 if username exists, 0 if it doesn't
	DECLARE ifUserNameExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TUsers WHERE strUserName = @strUsername AND intUserID != @intUserID -- Returns 1 if username exists, 0 if it doesn't

		OPEN ifUserNameExists

		FETCH FROM ifUserNameExists
		INTO @UserNameExists

		CLOSE ifUserNameExists

	DECLARE ifEmailExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TUsers WHERE strEmail = @strEmail AND intUserID != @intUserID -- Returns 1 if email exists, 0 if it doesn't

		OPEN ifEmailExists

		FETCH FROM ifEmailExists
		INTO @EmailExists

		CLOSE ifEmailExists	
	
	IF @UserNameExists = 0 AND @EmailExists = 0
		BEGIN
			UPDATE TUsers  
			SET strFirstName = @strFirstName, strLastName = @strLastName, strEmail = @strEmail, strPassword = @strPassword, strUserName = @strUsername
			WHERE @intUserID = intUserID 
			COMMIT
			RETURN 0
		END

	ELSE IF @UserNameExists = 1
		BEGIN
			COMMIT
			RETURN 1
		END
	ELSE IF @EmailExists = 1
		BEGIN
			COMMIT
			RETURN 2
		END

COMMIT TRANSACTION	

GO

--SELECT * FROM TUsers
--DECLARE @error as int
--EXECUTE @error =  uspUpdateUser 1, 'Ike', 'Galle', 'Kaitlin.cordelsl@gmail.com', 'reciplease2', 'Falcawn'
--PRINT @error
--SELECT * FROM TUsers

GO
-- --------------------------------------------------------------------------------------------

Create Procedure uspFavoriteUnfavorite
					 @intUserID		AS INTEGER OUTPUT
					,@intRecipeID	AS INTEGER OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	DECLARE @Exists as INTEGER

		-- Returns 1 if username exists, 0 if it doesn't
	DECLARE ifFavorited CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TUserFavorites WHERE intRecipeID = @intRecipeID AND intUserID = @intUserID -- Returns 1 if exists, 0 if it doesn't

		OPEN ifFavorited

		FETCH FROM ifFavorited
		INTO @Exists

		CLOSE ifFavorited

	IF @Exists = 1 -- if user has favorited dish, the favorite will be removed
		BEGIN
			DELETE FROM TUserFavorites WHERE intRecipeID = @intRecipeID AND intUserID = @intUserID
			COMMIT
			RETURN @Exists
		END
	ELSE IF @Exists = 0 -- if user has not favorited the favorite will be added
		BEGIN
			INSERT INTO TUserFavorites	(intUserID, intRecipeID)
			VALUES						(@intUserID, @intRecipeID)
			
			COMMIT
			RETURN @Exists
		END

COMMIT TRANSACTION	

GO

--SELECT * FROM VUserFavorites
--EXECUTE uspFavoriteUnfavorite 1, 1
--SELECT * FROM VUserFavorites

GO
-- --------------------------------------------------------------------------------------------

Create Procedure uspAddRecipe
					 @intRecipeID		AS INTEGER OUTPUT
					,@strName			AS VARCHAR(50) OUTPUT
					,@strInstructions	AS VARCHAR(3000) OUTPUT
					,@strRecipeImage	AS VARCHAR(255) OUTPUT		-- OPTIONAL
					,@intReadyInMins	AS INTEGER OUTPUT			-- OPTIONAL
					,@intServings		AS INTEGER OUTPUT			-- OPTIONAL
					,@strCuisines		AS VARCHAR(255) OUTPUT		-- OPTIONAL
					,@strDiets			AS VARCHAR(255) OUTPUT		-- OPTIONAL
					,@strDishTypes		AS VARCHAR(255) OUTPUT		-- OPTIONAL
					,@strNutrition		AS VARCHAR(3000) OUTPUT		-- OPTIONAL
					,@intUserID			AS INTEGER OUTPUT			-- OPTIONAL

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	DECLARE @Exists as INTEGER

			-- Returns 1 if username exists, 0 if it doesn't
	DECLARE ifExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TRecipes WHERE intRecipeID = @intRecipeID -- Returns 1 if exists, 0 if it doesn't

		OPEN ifExists

		FETCH FROM ifExists
		INTO @Exists

		CLOSE ifExists

	IF @Exists = 1 -- Recipe already exists in Database, returns 0
		BEGIN
			COMMIT 
			RETURN 0
		END
	ELSE IF @Exists = 0
		BEGIN

			-- ALL OPTIONAL OUTPUTS will be given a -1 for NULL or another value if not null
			IF @strRecipeImage = '-1'
				BEGIN
					SET @strRecipeImage = NULL
				END
			IF @intReadyInMins = -1
				BEGIN
					SET @intReadyInMins = NULL
				END

			IF @intServings = -1
				BEGIN
					SET @intServings = NULL
				END
				
			IF @strCuisines = '-1'
				BEGIN
					SET @strCuisines = NULL
				END
				
			IF @strDiets = '-1'
				BEGIN
					SET @strDiets = NULL
				END			

			IF @strDishTypes = '-1'
				BEGIN
					SET @strDishTypes = NULL
				END
				
			IF @strNutrition = '-1'
				BEGIN
					SET @strNutrition = NULL
				END
				
			IF @intUserID = -1
				BEGIN
					SET @intUserID = NULL
				END			

			-- Gets next UserID
			IF @intRecipeID = -1
				BEGIN
					SELECT @intRecipeID = MAX(intRecipeID) + 1
					FROM TRecipes (TABLOCKX) -- Locks table till end of transaction

					-- Defaults to 1 if no Users
					SELECT @intRecipeID = COALESCE ( @intRecipeID, 5000001)
				END

			SET IDENTITY_INSERT TRecipes ON

			INSERT INTO TRecipes (intRecipeID, strName, strInstructions, intReadyInMins, intServings, strCuisines, strDiets, strDishTypes, strNutrition, intUserID, strRecipeImage)
			VALUES				(@intRecipeID, @strName, @strInstructions, @intReadyInMins, @intServings, @strCuisines, @strDiets, @strDishTypes, @strNutrition, @intUserID, @strRecipeImage)

			SET IDENTITY_INSERT TRecipes OFF
			COMMIT
			RETURN @intRecipeID
		END

COMMIT TRANSACTION	

GO 

--SELECT * FROM TRecipes
--DECLARE @RecipeID as int = 5000005
--EXECUTE @RecipeID = uspAddRecipe @RecipeID OUTPUT, 'Delicious Beef Stew', 'LARGE LIST OF INSTRUCTIONS HERE', -1, -1, 'American', '-1', '-1', '-1', -1
--PRINT @RecipeID
--SELECT * FROM TRecipes
--GO 

-- --------------------------------------------------------------------------------------------

Create Procedure uspAddIngredient  -- Checks if IngredientID and IngredientName being added exist. If they exist it returns the ID for the ingredient. If it does not the Ingredient is added.
								   -- If the Ingredient added has no ID then it is defaulted to the next value.
					 @intIngredientID		AS INTEGER OUTPUT
					,@strIngredientName		AS VARCHAR(50) OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION
	DECLARE @IDExists as INTEGER
	DECLARE @NameExists as INTEGER = 0

			-- Returns 1 if username exists, 0 if it doesn't
	DECLARE ifIDExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TIngredients WHERE intIngredientID = @intIngredientID OR strIngredientName = @strIngredientName -- Returns 1 if exists, 0 if it doesn't

		OPEN ifIDExists

		FETCH FROM ifIDExists
		INTO @IDExists

		CLOSE ifIDExists

	DECLARE ifNameExists CURSOR LOCAL FOR
	SELECT intIngredientID FROM TIngredients WHERE strIngredientName = @strIngredientName  -- Gets ID for Ingredient if the name was already found to avoid duplicates

		OPEN ifNameExists

		FETCH FROM ifNameExists
		INTO @NameExists

		CLOSE ifNameExists


	IF @IDExists = 1 AND @NameExists > 0 -- Ingredient already exists in Database, returns 1

		BEGIN
			COMMIT
			RETURN @NameExists -- Finds correct ID for Ingredeient and returns the ID, will return 0 if ID exists and Name Does not.
		END

	ELSE IF @IDExists = 0 -- Ingredient doesn't exist, adds to DB

		BEGIN
						-- Gets next ingredientID
			IF @intIngredientID = 0
				BEGIN
					SELECT @intIngredientID = MAX(intIngredientID) + 1
					FROM TIngredients (TABLOCKX) -- Locks table till end of transaction

					-- Defaults to 1 if no Users
					SELECT @intIngredientID = COALESCE ( @intIngredientID, 1)
				END
			
			INSERT INTO TIngredients (intIngredientID, strIngredientName)
			VALUES					(@intIngredientID, @strIngredientName)
			COMMIT
			RETURN @intIngredientID -- Returns intIngredientID to be added to an array to then be added to uspAddRecipeIngredient
		END

COMMIT TRANSACTION
	
GO
--SELECT * FROM TIngredients
--DECLARE @IngredientID as Int
--EXECUTE @IngredientID = uspAddIngredient 12, 'Pork'
--SELECT * FROM TIngredients
--PRINT @IngredientID
--GO

-- --------------------------------------------------------------------------------------------

Create Procedure uspAddRecipeIngredients
				 @intRecipeID				AS INTEGER		OUTPUT
				,@intIngredientID			AS INTEGER		OUTPUT
				,@dblIngredientQuantity		AS FLOAT		OUTPUT
				,@strUnitOfMeasurement		AS VARCHAR(50)	OUTPUT
AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	DECLARE @Exists as INTEGER

	-- Returns 1 if recipe + ingredient exists, 0 if it doesn't
	DECLARE ifExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TRecipeIngredients WHERE intRecipeID = @intRecipeID and intIngredientID = @intIngredientID -- Returns 1 if exists, 0 if it doesn't

		OPEN ifExists

		FETCH FROM ifExists
		INTO @Exists

		CLOSE ifExists

	IF @Exists = 1
		BEGIN
			COMMIT
			RETURN -1 -- Returns -1 because ingredient is already in recipe
		END
	ELSE
		BEGIN
			-- intRecipeIngredeintID is set to INTEGER IDENTITY, ID will be grabbed from next available ID
			INSERT INTO TRecipeIngredients	(intRecipeID, intIngredientID, dblIngredientQuantity, strUnitOfMeasurement)
			VALUES							(@intRecipeID, @intIngredientID, @dblIngredientQuantity, @strUnitOfMeasurement)
			COMMIT
			RETURN 0 -- Returns 0, code ran successfully
		END

COMMIT TRANSACTION

GO

--SELECT * FROM TRecipeIngredients
--EXECUTE uspAddRecipeIngredients 5000004, 1, 20, 'OZ'
--SELECT * FROM TRecipeIngredients

--GO

--IF OBJECT_ID('uspDeleteUserRecipe')				IS NOT NULL DROP PROCEDURE uspDeleteUserRecipe 
--IF OBJECT_ID('uspUpdateUserRecipe')				IS NOT NULL DROP PROCEDURE uspUpdateUserRecipe 

-- --------------------------------------------------------------------------------------------

Create Procedure uspDeleteUserAccount
					 @intUserID as INTEGER OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	-- user id is deleted from UserFavorites
	DELETE FROM TUserFavorites WHERE intUserID = @intUserID

	-- deletes ratings done by user
	DELETE FROM TRatings WHERE intUserID = @intUserID

	-- deletes user shopping list
	DELETE FROM TShoppingList WHERE intUserID = @intUserID

	-- userID is delted from TUsers table.
	DELETE FROM TUsers WHERE intUserID = @intUserID

	-- If user had created recipes, their ID is removed so errors do not occur.
	UPDATE TRecipes
	SET intUserID = NULL
	WHERE intUserID = @intUserID
		
COMMIT TRANSACTION

GO

--SELECT * FROM vUserRating WHERE intUserID = 1
--Select * FROM vUserFavorites WHERE intUserID = 1
--Select * FROM vUserShoppingList WHERE intUserID = 1
--EXECUTE uspDeleteUserAccount 1
--SELECT * FROM vUserRating
--Select * FROM vUserFavorites
--Select * FROM vUserShoppingList

-- --------------------------------------------------------------------------------------------

Create Procedure uspDeleteUserRecipe
				 @intUserID as INTEGER OUTPUT
				,@intRecipeID as INTEGER OUTPUT 

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	DECLARE @Count as Int
	DECLARE @intRecipeIngredientID as INT

	-- gets COUNT of instances where an Ingredient in the recipe to be removed exists
	DECLARE IngredientCount CURSOR LOCAL FOR
		SELECT COUNT(intRecipeIngredientID) as COUNT
		FROM TRecipeIngredients WHERE intRecipeID = @intRecipeID
	-- Gets a list of RecipeIngredientIDs that need to be removed from shopping lists
	DECLARE getRecipeIngredientID CURSOR LOCAL FOR
		SELECT intRecipeIngredientID
		FROM TRecipeIngredients WHERE intRecipeID = @intRecipeID

	OPEN IngredientCount
	FETCH FROM IngredientCount
	INTO @Count
	CLOSE IngredientCount

	OPEN getRecipeIngredientID
	WHILE (@Count > 0)
	BEGIN
		
			FETCH FROM getRecipeIngredientID
			INTO @intRecipeIngredientID
		
		-- Deletes Ingredients from shopping list that are tied to the intRecipeID in TRecipeIngredients
		DELETE FROM TShoppingList WHERE intRecipeIngredientID = @intRecipeIngredientID
		PRINT @intRecipeIngredientID
		SET @Count -= 1
	END

	CLOSE getRecipeIngredientID
	-- Deletes all instance of RecipeID from TRecipeIngredients
	DELETE FROM TRecipeIngredients WHERE intRecipeID = @intRecipeID
	-- Deletes all instances of RecipeID from TUserFavorites
	DELETE FROM TUserFavorites WHERE intRecipeID = @intRecipeID
	-- Deletes all instance of ratings for the removed recipe
	DELETE FROM TRatings WHERE intRecipeID = @intRecipeID
	-- Deletes Recipe where RecipeID and UserID match.
	DELETE FROM TRecipes WHERE intRecipeID = @intRecipeID AND intUserID = @intUserID

COMMIT TRANSACTION

GO

--SELECT * FROM VUserFavorites
--SELECT * FROM TShoppingList
--SELECT * FROM VRecipeIngredients
--EXECUTE uspDeleteUserRecipe 1, 5000001
--SELECT * FROM VUserFavorites
--SELECT * FROM TShoppingList
--SELECT * FROM VRecipeIngredients
GO
-- --------------------------------------------------------------------------------------------

Create Procedure uspUpdateRecipe
					 @intRecipeID		AS INTEGER OUTPUT
					,@strName			AS VARCHAR(50) OUTPUT
					,@strInstructions	AS VARCHAR(3000) OUTPUT
					,@strRecipeImage	AS VARCHAR(255) OUTPUT		-- OPTIONAL
					,@intReadyInMins	AS INTEGER OUTPUT			-- OPTIONAL
					,@intServings		AS INTEGER OUTPUT			-- OPTIONAL
					,@strCuisines		AS VARCHAR(255) OUTPUT		-- OPTIONAL
					,@strDiets			AS VARCHAR(255) OUTPUT		-- OPTIONAL
					,@strDishTypes		AS VARCHAR(255) OUTPUT		-- OPTIONAL
					,@strNutrition		AS VARCHAR(3000) OUTPUT		-- OPTIONAL
					,@intUserID			AS INTEGER OUTPUT			-- OPTIONAL
AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	-- ALL OPTIONAL OUTPUTS will be given a -1 for NULL or another value if not null
	IF @strRecipeImage = '-1'
		BEGIN
			SET @strRecipeImage = NULL
		END
	IF @intReadyInMins = -1
		BEGIN
			SET @intReadyInMins = NULL
		END

	IF @intServings = -1
		BEGIN
			SET @intServings = NULL
		END
				
	IF @strCuisines = '-1'
		BEGIN
			SET @strCuisines = NULL
		END
				
	IF @strDiets = '-1'
		BEGIN
			SET @strDiets = NULL
		END			

	IF @strDishTypes = '-1'
		BEGIN
			SET @strDishTypes = NULL
		END
				
	IF @strNutrition = '-1'
		BEGIN
			SET @strNutrition = NULL
		END
				
	IF @intUserID = -1
		BEGIN
			SET @intUserID = NULL
		END			

	UPDATE TRecipes
	SET strName = @strName, strInstructions = @strInstructions, strRecipeImage = @strRecipeImage, intReadyInMins = @intReadyInMins, intServings = @intServings, strCuisines = @strCuisines, strDiets = @strDiets, strDishTypes = @strDishTypes, strNutrition = @strNutrition, intUserID = @intUserID
	WHERE intRecipeID = @intRecipeID

	-- Clears Ingredients for Recipe so the edited list can be added with ASP.NET
	DELETE FROM TRecipeIngredients
	WHERE intRecipeID = @intRecipeID

COMMIT TRANSACTION

GO

--SELECT * FROM TRecipes
--EXECUTE uspUpdateRecipe 5000001, 'Rosemary Garlic Butter Steak', 'Step 1: Let steak rest to room temperature and pat dry before cooking to get a proper sear. Allow pan to get hot at a Medium High - High heat. Sear each side including the "rim" of the steak, about 2-3mins a side until golden brown.
--									Step 2; Once the steak is seared reduce heat to Medium - Medium High add butter to the pan and let it melt. Once the butter has melted put in 2-3 "sticks" of rosemary and 3-4 garlic cloves quartered or halved in the butter. Cook the steak an additional 3 - 5 minutes 
--									depending on how rare youd like it, while cooking spoon the melted butter over the steak. Step 3; Enjoy :)'
--									, '-1', 40, 4, 'American', '-1', 'Dinner', '-1', 1
--SELECT * FROM TRecipes
		
GO						 

-- --------------------------------------------------------------------------------------------

Create Procedure uspAddToShoppingList
				 @intRecipeID	as INTEGER OUTPUT
				,@intUserID		as INTEGER OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	DECLARE @intShoppingListID		as INTEGER
	DECLARE @intRecipeIngredientID	as INTEGER

	-- Gets a list of RecipeIngredientIDs that do not already exist in the users shoppinglist
	DECLARE getRecipeIngredientID CURSOR LOCAL FOR
	SELECT intRecipeIngredientID
	FROM TRecipeIngredients WHERE intRecipeID = @intRecipeID

	--intRecipeIngredientID NOT IN (SELECT intRecipeIngredientID FROM TShoppingList WHERE intUserID = @intUserID) AND intRecipeID = @intRecipeID
	OPEN getRecipeIngredientID

	-- fetches first value from the cursor, then runs WHILE LOOP
	FETCH NEXT FROM getRecipeIngredientID INTO @intRecipeIngredientID

	-- If shopping list contains a recipes ingredients it will delete it before populating it with desired recipe ingredients
	IF @@ROWCOUNT > 0
		BEGIN
			DELETE FROM TShoppingList WHERE @intUserID = intUserID
		END

	WHILE @@FETCH_STATUS = 0
	BEGIN	
		
		SELECT @intShoppingListID = MAX(intShoppingListID) + 1
		FROM TShoppingList (TABLOCKX) -- Locks table till end of transaction

		-- Defaults to 1 if none exist
		SELECT @intShoppingListID = COALESCE ( @intShoppingListID, 1)

		-- Inserts Ingredients into Shopping List with USERID where the RecipeIngredientID is tied to the RECIPEID OUTPUT
		INSERT INTO TShoppingList	(intShoppingListID, intRecipeIngredientID, intUserID)
		VALUES						(@intShoppinglistID, @intRecipeIngredientID, @intUserID) 

		FETCH NEXT FROM getRecipeIngredientID INTO @intRecipeIngredientID
	END
	

COMMIT TRANSACTION

GO

-- --------------------------------------------------------------------------------------------

--Select * from VUserShoppingList
--EXECUTE uspAddToShoppingList 5000003, 1
--Select * from VUserShoppingList
-- --------------------------------------------------------------------------------------------
GO

-- simple USP to delete a users shopping list
Create Procedure uspDropUserShoppingList
				@intUserID as INTEGER OUTPUT

AS
SET XACT_ABORT ON

BEGIN TRANSACTION

	DELETE FROM TShoppingList WHERE @intUserID = intUserID

COMMIT TRANSACTION

GO

-- --------------------------------------------------------------------------------------------

--SELECT * FROM TShoppingList
--EXECUTE uspDropUserShoppingList 1
--SELECT * FROM TShoppingList
--GO