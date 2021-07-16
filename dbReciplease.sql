-- --------------------------------------------------------------------------------
-- Name: Reciplease Database
-- Class: CPDM-290
-- --------------------------------------------------------------------------------

-- --------------------------------------------------------------------------------
-- Options
-- --------------------------------------------------------------------------------
USE [CPDM-GroupA]   -- Get out of the master database
SET NOCOUNT ON;		-- Report only errors
-- --------------------------------------------------------------------------------
-- Drop Tables
-- --------------------------------------------------------------------------------
IF OBJECT_ID('TUserFavorites')					IS NOT NULL DROP TABLE TUserFavorites
IF OBJECT_ID('TShoppingList')					IS NOT NULL DROP TABLE TShoppingList
IF OBJECT_ID('TRecipeIngredients')				IS NOT NULL DROP TABLE TRecipeIngredients
IF OBJECT_ID('TRatings')						IS NOT NULL DROP TABLE TRatings
IF OBJECT_ID('TMeasurementUnits')				IS NOT NULL DROP TABLE TMeasurementUnits
IF OBJECT_ID('TLast10')							IS NOT NULL DROP TABLE TLast10
IF OBJECT_ID('TRecipes')						IS NOT NULL DROP TABLE TRecipes
IF OBJECT_ID('TUsers')							IS NOT NULL DROP TABLE TUsers
IF OBJECT_ID('TTaste')							IS NOT NULL DROP TABLE TTaste
IF OBJECT_ID('TDifficulty')						IS NOT NULL DROP TABLE TDifficulty
IF OBJECT_ID('TIngredients')					IS NOT NULL DROP TABLE TIngredients

-- ---------------------------------------------------------------------------------
-- Drop Views
-- ---------------------------------------------------------------------------------

IF OBJECT_ID('VRecipeIngredients')				IS NOT NULL DROP VIEW VRecipeIngredients
IF OBJECT_ID('VUserFavorites')					IS NOT NULL DROP VIEW VUserFavorites
IF OBJECT_ID('VUserShoppingList')				IS NOT NULL DROP VIEW VUserShoppingList
IF OBJECT_ID('VRecipeRatings')					IS NOT NULL DROP VIEW VRecipeRatings 
IF OBJECT_ID('vUserRating')						IS NOT NULL DROP VIEW vUserRating

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
                                                                                                                                                                                                                                                                                                                                                                                               
-- --------------------------------------------------------------------------------
-- Step #1: Create Tables
-- --------------------------------------------------------------------------------
CREATE TABLE TUsers
(
	 intUserID			INTEGER	IDENTITY	NOT NULL
	,strFirstName		VARCHAR(50)			NOT NULL
	,strLastName		VARCHAR(50)			NOT NULL 
	,strEmail			VARCHAR(50)			NOT NULL
	,strPassword		VARCHAR(50)			NOT NULL
	,strUserName		VARCHAR(50)			NOT NULL
	,CONSTRAINT TUsers_PK PRIMARY KEY ( intUserID )
)

CREATE TABLE TRecipes
(
	 intRecipeID		INTEGER	IDENTITY(5000001, 1)	NOT NULL
	,strName			VARCHAR(50)						NOT NULL
	,strInstructions	VARCHAR(3000)					NOT NULL
	,intReadyInMins		INTEGER				
	,intServings		INTEGER				
	,strCuisines		VARCHAR(255)		
	,strDiets			VARCHAR(255)		
	,strDishTypes		VARCHAR(255)
	,strNutrition		VARCHAR(3000)		
	,intUserID			INTEGER		
	,strRecipeImage		VARCHAR(500)		
	,CONSTRAINT TRecipes_PK PRIMARY KEY ( intRecipeID )
)

CREATE TABLE TRecipeIngredients
(
	 intRecipeIngredientID	INTEGER IDENTITY	NOT NULL
	,intRecipeID			INTEGER				NOT NULL
	,intIngredientID		INTEGER				NOT NULL
	,dblIngredientQuantity	FLOAT				NOT NULL
	--,intIngredientQuantity	INTEGER				NOT NULL
	,strUnitOfMeasurement	VARCHAR(50)			NOT	NULL
	,CONSTRAINT RecipeIngredient_UQ UNIQUE ( intRecipeID, intIngredientID )
	,CONSTRAINT TRecipeIngredients_PK PRIMARY KEY ( intRecipeIngredientID )
)

CREATE TABLE TIngredients
(
	 intIngredientID	INTEGER				NOT NULL
	,strIngredientName	VARCHAR(50)			NOT NULL
	,CONSTRAINT TIngredients_PK PRIMARY KEY ( intIngredientID )
)

CREATE TABLE TShoppingList
(
	 intShoppingListID	INTEGER			NOT NULL
	,intUserID			INTEGER			NOT NULL
	,intRecipeIngredientID INTEGER		NOT NULL
	,CONSTRAINT RecipeIngredientList_UQ UNIQUE ( intUserID, intRecipeIngredientID )
	,CONSTRAINT TShoppingList_PK PRIMARY KEY ( intShoppingListID )
)

CREATE TABLE TUserFavorites
(
	 intUserFavoriteID	INTEGER IDENTITY	NOT NULL
	,intUserID			INTEGER				NOT NULL
	,intRecipeID		INTEGER				NOT NULL
	,CONSTRAINT UserID_RecipeID_UQ UNIQUE ( intUserID, intRecipeID )
	,CONSTRAINT TUserFavorites_PK PRIMARY KEY ( intUserFavoriteID )
)

CREATE TABLE TRatings
(
	 intRatingID		INTEGER				NOT NULL
	,intUserID			INTEGER				NOT NULL
	,intDifficultyID	INTEGER				NOT NULL
	,intTasteID			INTEGER				NOT NULL
	,intRecipeID		INTEGER				NOT NULL
	,CONSTRAINT RecipeUser_UQ UNIQUE ( intRecipeID, intUserID )
	,CONSTRAINT TRatings_PK PRIMARY KEY ( intRatingID )
)

CREATE TABLE TTaste
(
	 intTasteID			INTEGER	IDENTITY	NOT NULL
	,intTasteRating		INTEGER				NOT NULL
	,CONSTRAINT TTaste_PK PRIMARY KEY ( intTasteID )
)

CREATE TABLE TDifficulty
(
	 intDifficultyID		INTEGER	IDENTITY NOT NULL
	,intDifficultyRating	INTEGER			 NOT NULL
	,CONSTRAINT TDifficulty_PK PRIMARY KEY ( intDifficultyID )
)

CREATE TABLE TLast10
(
	 intLast10ID			INTEGER				NOT NULL
	,intUserID				INTEGER				NOT NULL
	,intRecipeID			INTEGER				NOT NULL
	,CONSTRAINT TLast10_PK PRIMARY KEY ( intLast10ID )
)

--CREATE TABLE TMeasurementUnits
--(
--	 intMeasurementUnitID				INTEGER				NOT NULL
--	,strUnit							VARCHAR(50)			NOT NULL
--	,CONSTRAINT TUnitsOfMeasurement		PRIMARY KEY ( intMeasurementUnitID )
--)

-- --------------------------------------------------------------------------------
-- Step #2: Identify and Create Foreign Keys
-- --------------------------------------------------------------------------------
--
-- #	Child								Parent						Column(s)
-- -	-----								------						---------
-- 1	TUserFavorites						TUsers						intUserID
-- 2	TUserFavorites						TRecipes					intRecipeID
-- 5	TShoppingList						TUsers						intUserID
-- 6	TShoppingList						TRecipeIngredients			intRecipeIngredientID
-- 3	TRecipeIngredients					TRecipes					intRecipeID
-- 4	TRecipeIngredients					TIngredients				intIngredientID
-- 7	TRatings							TTaste						intTasteID						
-- 8	TRatings							TDifficulty					intDifficultyID
-- 9	TRatings							TUsers						intUserID
-- 10	TRatings							TRecipe						intRecipeID
-- 11	TUsers								TShoppingList				intShoppingListID
-- 12	TLast10								TUsers						intUserID
-- 13	TRecipes							TUsers						intUserID
-- 14	TLast10								TRecipes					intRecipeID



---- 1
ALTER TABLE TUserFavorites ADD CONSTRAINT TUserFavorites_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID )

-- 2
ALTER TABLE TUserFavorites ADD CONSTRAINT TUserFavorites_TRecipes_FK
FOREIGN KEY ( intRecipeID ) REFERENCES TRecipes ( intRecipeID )

ALTER TABLE TShoppingList ADD CONSTRAINT TShoppingList_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID )

ALTER TABLE TShoppingList ADD CONSTRAINT TShoppingList_TRecipeIngredients_FK
FOREIGN KEY ( intRecipeIngredientID ) REFERENCES TRecipeIngredients ( intRecipeIngredientID )

-- 3
ALTER TABLE TRecipeIngredients ADD CONSTRAINT TRecipeIngredients_TRecipes_FK
FOREIGN KEY ( intRecipeID ) REFERENCES TRecipes ( intRecipeID )

-- 4
ALTER TABLE TRecipeIngredients ADD CONSTRAINT TRecipeIngredients_TIngredients_FK
FOREIGN KEY ( intIngredientID ) REFERENCES TIngredients ( intIngredientID )

-- 15
----ALTER TABLE TRecipeIngredients ADD CONSTRAINT TRecipeIngredients_TMeasurementUnits_FK
----FOREIGN KEY ( intMeasurementUnitID ) REFERENCES TMeasurementUnits ( intMeasurementUnitID )

-- 7
ALTER TABLE TRatings ADD CONSTRAINT TRatings_TTaste_FK
FOREIGN KEY ( intTasteID ) REFERENCES TTaste ( intTasteID )

-- 8
ALTER TABLE TRatings ADD CONSTRAINT TRatings_TDifficulty_FK
FOREIGN KEY ( intDifficultyID ) REFERENCES TDifficulty ( intDifficultyID )

-- 9
ALTER TABLE TRatings ADD CONSTRAINT TRatings_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID )

-- 10
ALTER TABLE TRatings ADD CONSTRAINT TRatings_TRecipe_FK
FOREIGN KEY ( intRecipeID ) REFERENCES TRecipes ( intRecipeID )

-- 12
ALTER TABLE TLast10 ADD CONSTRAINT TLast10_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID )

-- 13
ALTER TABLE TUsers ADD CONSTRAINT TRecipes_TUsers_FK
FOREIGN KEY ( intUserID ) REFERENCES TUsers ( intUserID )

-- 14
ALTER TABLE TLast10 ADD CONSTRAINT TLast10_TRecipes_FK
FOREIGN KEY ( intRecipeID ) REFERENCES TRecipes ( intRecipeID )



 --------------------------------------------------------------------------------
 --Step #3: INSERT INTO TABLES
 --------------------------------------------------------------------------------

 --INSERT INTO TMeasurementUnits( intMeasurementUnitID, strUnit )
 --VALUES					 (1, ' ')
	--					,(2, 'Tsp')
	--					,(3, 'TBSP')
	--					,(4, 'Cup')
	--					,(5, 'Quart')
	--					,(6, 'Pint')
	--					,(7, 'Gallon')
	--					,(8, 'Liter')
	--					,(9, 'ML')
	--					,(10, 'OZ')
	--					,(11, 'LB')
	--					,(12, 'Grams')

 INSERT INTO TDifficulty ( intDifficultyRating )
 VALUES					 (1)
						,(2)
						,(3)
						,(4)
						,(5)

 INSERT INTO TTaste ( intTasteRating )
 VALUES					 (1)
						,(2)
						,(3)
						,(4)
						,(5)

 INSERT INTO TUsers		(strFirstName, strLastName, strEmail, strPassword, strUserName)
 VALUES					 ('Isaak', 'Galle', 'isaakgalle@gmail.com', 'reciplease1', 'Falcawn')
						,('Brandon', 'Wernke', 'brandon.wernke@gmail.com', 'reciplease2', 'Brawer')
						,('Kaitlin', 'Cordell', 'Kaitlin.cordell@gmail.com', 'reciplease3', 'asapneko')
						,('Omonigho', 'Odairi', 'obodairi@cincinnatistate.edu', 'reciplease4', 'Omoh')

INSERT INTO TRecipes	(strName, strInstructions, intReadyInMins, intServings, intUserID, strRecipeImage)
VALUES					 ('Rosemary Garlic Butter Steak', 'Step 1: Let steak rest to room temperature and pat dry before cooking to get a proper sear. Allow pan to get hot at a Medium High - High heat. Sear each side including the "rim" of the steak, about 2-3mins a side until golden brown.
							   Step 2; Once the steak is seared reduce heat to Medium - Medium High add butter to the pan and let it melt. Once the butter has melted put in 2-3 "sticks" of rosemary and 3-4 garlic cloves quartered or halved in the butter. Cook the steak an additional 3 - 5 minutes depending on how rare youd like it, while cooking spoon the melted butter over the steak.
							   Step 3; Enjoy :)', 25, 2, 1, '')
						,('Recipe Test 2', 'Recipe Test 2 Instructions', 30, 4, 1, '')
						,('Recipe Test 3', 'Recipe Test 3 Instructions', 60, 4, 3, '')
						,('Recipe Test 4', 'Recipe Test 4 Instructions',180, 6, 4, '')

 INSERT INTO TRatings		(intRatingID, intUserID, intDifficultyID, intTasteID, intRecipeID)
 VALUES					 (1, 1, 5, 5, 5000001)
						,(2, 2, 2, 3, 5000001)
						,(3, 3, 5, 2, 5000001)
						,(4, 4, 5, 5, 5000001)
						,(5, 2, 2, 3, 5000003)
						,(6, 2, 5, 2, 5000004)
						,(7, 1, 5, 5, 5000004)

 INSERT INTO TUserFavorites( intUserID, intRecipeID )
 VALUES					 (2, 5000002)
						,(2, 5000001)
						,(3, 5000001)
						,(4, 5000003)
						,(4, 5000001)
						,(3, 5000002)
						,(3, 5000003)

INSERT INTO TIngredients ( intIngredientID, strIngredientName )
VALUES					 (1, 'Sweet White Onion' )
						,(2, 'Red Onion')
						,(3, 'Chicken Breast Bone-In')
						,(4, 'Chicken Breast Boneless')
						,(5, 'Chicken Thigh')
						,(6, 'Ribeye')
						,(7, 'Cabbage')
						,(8, 'Butter')
						,(9, 'Salt')
						,(10, 'Peper')
						,(11, 'Garlic Clove')
						,(12, 'Rosemary')
						,(13, 'Potatoe')
						,(14, 'Celery')
						,(15, 'Red-Snapper')
						,(16, 'Lemon')
						,(17, 'All-Spice')

 INSERT INTO TRecipeIngredients (intRecipeID, intIngredientID, dblIngredientQuantity, strUnitOfMeasurement )
 VALUES					 (5000001, 6, 1.5, 'LB')
						,(5000001, 8, 1.75, 'TSP')
						,(5000001, 9, 1, 'TBSP')
						,(5000001, 10, 0.75, 'OZ')
						,(5000001, 11, 3, 'ML')
						,(5000001, 12, 3, 'Gram')
						,(5000002, 3, 2, 'Cup')
						,(5000002, 9, 1, 'Quart')
						,(5000002, 10, 1, 'Pint')
						,(5000002, 8, 1, 'Gallon')

INSERT INTO TShoppingList ( intShoppingListID, intUserID, intRecipeIngredientID )
VALUES						 (1,1,1)
							,(2,1,2)
							,(3,1,3)
							,(4,1,4)
							,(5,1,5)
							,(6,1,6)



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
-- # PROCEDURES #
-- --------------------------------------------------------------------------------------------

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

			INSERT INTO TRecipes (intRecipeID, strName, strInstructions, intReadyInMins, intServings, strCuisines, strDiets, strDishTypes, strNutrition, intUserID)
			VALUES				(@intRecipeID, @strName, @strInstructions, @intReadyInMins, @intServings, @strCuisines, @strDiets, @strDishTypes, @strNutrition, @intUserID)

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
GO

-- --------------------------------------------------------------------------------------------

Create Procedure uspAddRecipeIngredients
				 @intRecipeID				AS INTEGER		OUTPUT
				,@intIngredientID			AS INTEGER		OUTPUT
				,@dblIngredientQuantity		AS INTEGER		OUTPUT
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

GO


-- ADD RECIPE AND ALL ITS INGREDIENTS TEST --

--SELECT * FROM TRecipes
--SELECT * FROM TIngredients
--SELECT * FROM TRecipeIngredients
--SELECT * FROM VRecipeIngredients
--DECLARE @RecipeID as INTEGER = 4000
--DECLARE @IngredientID as INT = 0
--EXECUTE @RecipeID = uspAddRecipe  @RecipeID OUTPUT, 'Bagel Bites', 'Put the cheese on bagel, then the pepperonie, bake at high temp for a medium amount of time', 30, 1, 'American', '', '', 'BAD FOR YOU', 1
--PRINT @RecipeID
--EXECUTE @IngredientID = uspAddIngredient @IngredientID OUTPUT, 'Bacon' 
--PRINT @IngredientID
--EXECUTE uspAddRecipeIngredients @RecipeID OUTPUT, @IngredientID, 4, 'Grams'
--EXECUTE @IngredientID = uspAddIngredient 24, 'Butter' 
--EXECUTE uspAddRecipeIngredients @RecipeID OUTPUT, @IngredientID, 4, 'TBSP'
--EXECUTE uspAddRecipeIngredients @RecipeID OUTPUT, 2, 2, 'Cups'
--EXECUTE uspAddRecipeIngredients @RecipeID OUTPUT, 3, 3, 9
--EXECUTE uspAddRecipeIngredients @RecipeID OUTPUT, 4, 4, 10
--SELECT * FROM TRecipes
--SELECT * FROM TIngredients
--SELECT * FROM TRecipeIngredients
--SELECT * FROM VRecipeIngredients








--SELECT * FROM TRecipeIngredients
--DECLARE @Exists as int
--EXECUTE @Exists = uspAddRecipeIngredients 5000002, 6, 2, 2
--PRINT @Exists
--SELECT * FROM TRecipeIngredients
-- --------------------------------------------------------------------------------------------

--Create Procedure uspCreateCompleteRecipe
--					 @intRecipeID			AS INTEGER OUTPUT
--					,@strName				AS VARCHAR(50) OUTPUT
--					,@strDescription		AS VARCHAR(300) OUTPUT
--					,@strInstructions		AS VARCHAR(3000) OUTPUT
--					,@intReadyInMins		AS INTEGER OUTPUT			-- OPTIONAL
--					,@intServings			AS INTEGER OUTPUT			-- OPTIONAL
--					,@strCuisines			AS VARCHAR(255) OUTPUT		-- OPTIONAL
--					,@strDiets				AS VARCHAR(255) OUTPUT		-- OPTIONAL
--					,@strDishTypes			AS VARCHAR(255) OUTPUT		-- OPTIONAL
--					,@strNutrition			AS VARCHAR(3000) OUTPUT		-- OPTIONAL
--					,@intUserID				AS INTEGER OUTPUT			-- OPTIONAL
--					,@intIngredientID		AS INTEGER OUTPUT
--					,@strIngredientName		AS VARCHAR(50) OUTPUT
--					,@intIngredientQuantity	AS INTEGER OUTPUT
--					,@intMeasurementUnitID	AS INTEGER OUTPUT

--					 intRecipeID and intIngredientID must be 0 unless entered from the API. 
--					 intRecipeID will return 0 if Recipe was added succesfully, will return 1 if the Recipe already exists. RecipeID will COALESCE from 5000001 going up by 1 
--							for user created recipes. API recipes added to be able to be favorited will be the RecipeID used in the API.
--					 intIngredientID will return THE ID of the ingredient

--AS
--SET XACT_ABORT ON

--BEGIN TRANSACTION
	
--	DECLARE @Exists as INT

--	EXECUTE @Exists = uspAddRecipe @intRecipeID OUTPUT, @strName, @strDescription, @strInstructions, @intReadyInMins, @intServings, @strCuisines, @strDiets, @strDishTypes, @strNutrition, @intUserID

--	IF @Exists = 1

--		BEGIN
--		COMMIT
--		RETURN -1
--		END

--	ELSE 

--		EXECUTE @Exists = uspAddIngredient @intIngredientID OUTPUT, @strIngredientName

--		IF @Exists > 0

--			BEGIN
				
--				EXECUTE @Exists = uspAddRecipeIngredients


--COMMIT TRANSACTION
