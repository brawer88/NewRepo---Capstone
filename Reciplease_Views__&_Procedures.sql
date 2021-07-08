USE [CPDM-GroupA]   -- Get out of the master database
SET NOCOUNT ON;		-- Report only errors

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

	DECLARE @UserNameExists as INTEGER
	DECLARE @PasswordMatches as INTEGER

	DECLARE ifUserNameExists CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TUsers WHERE strUserName = @strUsername -- Returns 1 if username exists, 0 if it doesn't

		OPEN ifUserNameExists

		FETCH FROM ifUserNameExists
		INTO @UserNameExists

		CLOSE ifUserNameExists

	DECLARE ifPasswordMatchesUser CURSOR LOCAL FOR
	SELECT COUNT(1) FROM TUsers WHERE strUserName = @strUsername AND strPassword = @strPassword -- Returns 1 if password matches username, 0 if it doesn't

		OPEN ifPasswordMatchesUser

		FETCH FROM ifPasswordMatchesUser
		INTO @PasswordMatches

		CLOSE ifPasswordMatchesUser
	
	IF @UserNameExists = 1
		BEGIN
			IF @PasswordMatches = 1
				BEGIN
					SELECT * From TUsers WHERE @strUsername = strUserName AND @strPassword = strPassword
					COMMIT
					RETURN 1 -- Returns 1 and selects User Info if username and password match
				END
			ELSE
				BEGIN
					COMMIT
					RETURN -1 -- Returns -1 if password doesnt match username
				END
		END
	ELSE
		COMMIT
		RETURN 0 -- returns 0 if Username doesnt exist
	
COMMIT TRANSACTION



--GO
--DECLARE @strUserName as VARCHAR(50) = ''
--DECLARE @strPassword as VARCHAR(50) = ''
--DECLARE @int		as int
--EXECUTE @int = uspLogin 'Falcawn', 'reciplease1'
--PRINT @int

-- --------------------------------------------------------------------------------------------

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
				--SELECT * FROM TRatings WHERE @intUserID = intUserID AND @intRecipeID = intRecipeID
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