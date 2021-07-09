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

-- ---------------------------------------------------------------------------------
-- Drop Procedures
-- ---------------------------------------------------------------------------------

IF OBJECT_ID('uspAddNewUser')					IS NOT NULL DROP PROCEDURE uspAddNewUser
IF OBJECT_ID('uspLogin')						IS NOT NULL DROP PROCEDURE uspLogin
IF OBJECT_ID('uspRateRecipe')					IS NOT NULL DROP PROCEDURE uspRateRecipe 
IF OBJECT_ID('uspUpdateUser')					IS NOT NULL DROP PROCEDURE uspUpdateUser
                                                                                                                                                                                                                                                                                                                                                                                                
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
	 intRecipeID		INTEGER	  IDENTITY	NOT NULL
	,strName			VARCHAR(50)			NOT NULL
	,strDescription		VARCHAR(300)		NOT NULL
	,strInstructions	VARCHAR(3000)		NOT NULL
	,intReadyInMins		INTEGER				NOT NULL
	,intServings		INTEGER				NOT NULL
	,strCuisines		VARCHAR(255)		
	,strDiets			VARCHAR(255)		
	,strDishTypes		VARCHAR(255)
	,strNutrition		VARCHAR(3000)		
	,intUserID			INTEGER				
	,CONSTRAINT TRecipes_PK PRIMARY KEY ( intRecipeID )
)

CREATE TABLE TRecipeIngredients
(
	 intRecipeIngredientID	INTEGER IDENTITY	NOT NULL
	,intRecipeID			INTEGER				NOT NULL
	,intIngredientID		INTEGER				NOT NULL
	,intIngredientQuantity	INTEGER				NOT NULL
	,intMeasurementUnitID	INTEGER				NOT	NULL
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

CREATE TABLE TMeasurementUnits
(
	 intMeasurementUnitID				INTEGER				NOT NULL
	,strUnit							VARCHAR(50)			NOT NULL
	,CONSTRAINT TUnitsOfMeasurement		PRIMARY KEY ( intMeasurementUnitID )
)

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
ALTER TABLE TRecipeIngredients ADD CONSTRAINT TRecipeIngredients_TMeasurementUnits_FK
FOREIGN KEY ( intMeasurementUnitID ) REFERENCES TMeasurementUnits ( intMeasurementUnitID )

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

 INSERT INTO TMeasurementUnits( intMeasurementUnitID, strUnit )
 VALUES					 (1, ' ')
						,(2, 'Tsp')
						,(3, 'TBSP')
						,(4, 'Cup')
						,(5, 'Quart')
						,(6, 'Pint')
						,(7, 'Gallon')
						,(8, 'Liter')
						,(9, 'ML')
						,(10, 'OZ')
						,(11, 'LB')
						,(12, 'Grams')

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

INSERT INTO TRecipes	(strName, strDescription, strInstructions, intReadyInMins, intServings, intUserID)
VALUES					 ('Rosemary Garlic Butter Steak', 'Ribeye Steak pan seared in a cast iron with butter allowing the rosemary and garlic to infuse into the butter and steak itself.'
							, 'Step 1: Let steak rest to room temperature and pat dry before cooking to get a proper sear. Allow pan to get hot at a Medium High - High heat. Sear each side including the "rim" of the steak, about 2-3mins a side until golden brown.
							   Step 2; Once the steak is seared reduce heat to Medium - Medium High add butter to the pan and let it melt. Once the butter has melted put in 2-3 "sticks" of rosemary and 3-4 garlic cloves quartered or halved in the butter. Cook the steak an additional 3 - 5 minutes depending on how rare youd like it, while cooking spoon the melted butter over the steak.
							   Step 3; Enjoy :)', 25, 2, 1 )
						,('Recipe Test 2', 'Recipe Test 2 Desc', 'Recipe Test 2 Instructions', 30, 4, 1)
						,('Recipe Test 3', 'Recipe Test 3 Desc', 'Recipe Test 3 Instructions', 60, 4, 3)
						,('Recipe Test 4', 'Recipe Test 4 Desc', 'Recipe Test 4 Instructions',180, 6, 4)

 INSERT INTO TRatings		(intRatingID, intUserID, intDifficultyID, intTasteID, intRecipeID)
 VALUES					 (1, 1, 5, 5, 1)
						,(2, 2, 2, 3, 1)
						,(3, 3, 5, 2, 1)
						,(4, 4, 5, 5, 1)
						,(5, 2, 2, 3, 3)
						,(6, 2, 5, 2, 4)
						,(7, 1, 5, 5, 3)

 INSERT INTO TUserFavorites( intUserID, intRecipeID )
 VALUES					 (2, 2)
						,(2, 1)
						,(3, 1)
						,(4, 3)
						,(4, 1)
						,(3, 2)
						,(3, 3)

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

 INSERT INTO TRecipeIngredients (intRecipeID, intIngredientID, intIngredientQuantity, intMeasurementUnitID )
 VALUES					 (1, 6, 1, 1)
						,(1, 8, 1, 2)
						,(1, 9, 6, 2)
						,(1, 10, 1, 4)
						,(1, 11, 3, 1)
						,(1, 12, 3, 1)
						,(2, 3, 2, 7)
						,(2, 9, 1, 8)
						,(2, 10, 1, 9)
						,(2, 8, 1, 10)

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
		,TR.strName
		,TRI.intIngredientQuantity
		,TMU.strUnit
		,TI.strIngredientName
	 

FROM	TRecipes as TR JOIN TRecipeIngredients as TRI
		ON TR.intRecipeID = TRI.intRecipeID

		JOIN TIngredients as TI
		ON TI.intIngredientID = TRI.intIngredientID

		JOIN TMeasurementUnits as TMU
		ON TMU.intMeasurementUnitID = TRI.intMeasurementUnitID

GO

SELECT * FROM VRecipeIngredients

GO

-- --------------------------------------------------------------------------------------------

Create View VUserFavorites
AS
Select	 TU.intUserID
		,TU.strFirstName + ' ' + TU.strLastName as USERNAME
		,TR.intRecipeID
		,TR.strName
		,TR.strDescription		 

FROM	TUsers as TU JOIN TUserFavorites as TUF
		ON TU.intUserID = TUF.intUserID

		JOIN TRecipes as TR
		ON TR.intRecipeID = TUF.intRecipeID

GO

SELECT * FROM VUserFavorites

GO

--------------------------------------------------------------------------------------------

Create View VUserShoppingList
AS
SELECT	 TU.intUserID as USERID
		,TRI.intIngredientID as INGREDIENTID
		,TRI.intIngredientQuantity
		,TMU.strUnit
		,TI.strIngredientName

FROM	TUsers as TU JOIN TShoppingList TSL
		ON TU.intUserID = TSL.intUserID

		JOIN TRecipeIngredients as TRI
		ON TRI.intRecipeIngredientID = TSL.intRecipeIngredientID

		JOIN TRecipes as TR
		ON TR.intRecipeID = TRI.intRecipeID

		JOIN TIngredients as TI
		ON TI.intIngredientID = TRI.intIngredientID

		JOIN TMeasurementUnits as TMU
		ON TMU.intMeasurementUnitID = TRI.intMeasurementUnitID	

GO

SELECT * FROM VUserShoppingList

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
--EXECUTE @Exists = uspRateRecipe 1, 5, 5, 2
--SELECT * FROM VRecipeRatings
--PRINT @Exists

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

-- --------------------------------------------------------------------------------------------



--DECLARE @intVisitID AS INTEGER = 0 
--EXECUTE uspRandomizationInsertVisit @intVisitID OUTPUT, 4
--PRINT 'VISIT ID = ' + CONVERT(VARCHAR, @intVisitID)
--SELECT * FROM TVisits

 -- TEST
--SELECT * FROM vPatientVisits
--DECLARE @intVisitID as INTEGER = 0
--EXECUTE uspScreening @intVisitID OUTPUT, '12/12/19', 2, '09/10/1992', 1, 178
--PRINT 'VISIT ID = ' + CONVERT(VARCHAR, @intVisitID)
--SELECT * FROM vPatientVisits
--SELECT * FROM vPatientSites


--Create Procedure uspInsertPatient
--		 @intPatientID		as INTEGER OUTPUT
--		,@intSiteID			as INTEGER
--		,@dtmDOB			as DATE
--		,@intGenderID		as INTEGER
--		,@intWeight			as INTEGER
--AS
--SET XACT_ABORT ON	--- Terminate and roll back if any errors

--BEGIN TRANSACTION

--	SELECT @intPatientID = MAX(intPatientID) + 1
--	FROM TPatients	(TABLOCKX)	-- Locks table until the end of the transaction

--	-- Default to 1 if the table is empty
--	SELECT @intPatientID = COALESCE( @intPatientID, 1 )

--	-- Gets next number from patients at selected site
--	DECLARE @intPatientNumber as INTEGER
--		BEGIN
--			DECLARE PatientNumCursor CURSOR LOCAL FOR
--			SELECT MAX(intPatientNumber) + 1 FROM vPatientSites
--			WHERE intSiteID = @intSiteID
			
--			OPEN PatientNumCursor
			
--			FETCH FROM PatientNumCursor
--			INTO @intPatientNumber
--		END			

--		-- INSERT data into the table
--	INSERT INTO TPatients(intPatientID, intPatientNumber, intSiteID, dtmDOB, intGenderID, intWeight)
--	VALUES (@intPatientID, @intPatientNumber, @intSiteID, @dtmDOB, @intGenderID, @intWeight)
		
--COMMIT TRANSACTION

--GO