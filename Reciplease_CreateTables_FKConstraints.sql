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
	,strName			VARCHAR(250)					NOT NULL
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
	 intShoppingListID		INTEGER			NOT NULL
	,intUserID				INTEGER			NOT NULL
	,intRecipeIngredientID	INTEGER			NOT NULL
	--,CONSTRAINT RecipeIngredientList_UQ UNIQUE ( intUserID, intRecipeIngredientID )
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

-- INSERT INTO TDifficulty ( intDifficultyRating )
-- VALUES					 (1)
--						,(2)
--						,(3)
--						,(4)
--						,(5)

-- INSERT INTO TTaste ( intTasteRating )
-- VALUES					 (1)
--						,(2)
--						,(3)
--						,(4)
--						,(5)

-- INSERT INTO TUsers		(strFirstName, strLastName, strEmail, strPassword, strUserName)
-- VALUES					 ('Isaak', 'Galle', 'isaakgalle@gmail.com', 'reciplease1', 'Falcawn')
--						,('Brandon', 'Wernke', 'brandon.wernke@gmail.com', 'reciplease2', 'Brawer')
--						,('Kaitlin', 'Cordell', 'Kaitlin.cordell@gmail.com', 'reciplease3', 'asapneko')
--						,('Omonigho', 'Odairi', 'obodairi@cincinnatistate.edu', 'reciplease4', 'Omoh')

--INSERT INTO TRecipes	(strName, strInstructions, intReadyInMins, intServings, intUserID)
--VALUES					 ('Rosemary Garlic Butter Steak', 'Step 1: Let steak rest to room temperature and pat dry before cooking to get a proper sear. Allow pan to get hot at a Medium High - High heat. Sear each side including the "rim" of the steak, about 2-3mins a side until golden brown.
--							   Step 2; Once the steak is seared reduce heat to Medium - Medium High add butter to the pan and let it melt. Once the butter has melted put in 2-3 "sticks" of rosemary and 3-4 garlic cloves quartered or halved in the butter. Cook the steak an additional 3 - 5 minutes depending on how rare youd like it, while cooking spoon the melted butter over the steak.
--							   Step 3; Enjoy :)', 25, 2, 1 )
--						,('Recipe Test 2', 'Recipe Test 2 Instructions', 30, 4, 1)
--						,('Recipe Test 3', 'Recipe Test 3 Instructions', 60, 4, 3)
--						,('Recipe Test 4', 'Recipe Test 4 Instructions',180, 6, 4)
--						,('Recipe Test 5', 'Recipe Test 4 Instructions',180, 6, 2)
--						,('Recipe Test 6', 'Recipe Test 4 Instructions',180, 6, 3)
--						,('Recipe Test 7', 'Recipe Test 4 Instructions',180, 6, 2)
--						,('Recipe Test 8', 'Recipe Test 4 Instructions',180, 6, 3)
--						,('Recipe Test 9', 'Recipe Test 4 Instructions',180, 6, 1)
--						,('Recipe Test 10', 'Recipe Test 4 Instructions',180, 6, 1)
--						,('Recipe Test 11', 'Recipe Test 11 Instructions',180, 6, 1)

-- INSERT INTO TRatings		(intRatingID, intUserID, intDifficultyID, intTasteID, intRecipeID)
-- VALUES					 (1, 1, 5, 5, 5000001)
--						,(2, 2, 2, 3, 5000001)
--						,(3, 3, 5, 2, 5000001)
--						,(4, 4, 5, 5, 5000001)
--						,(5, 2, 2, 3, 5000003)
--						,(6, 2, 5, 2, 5000004)
--						,(7, 1, 5, 5, 5000004)

-- INSERT INTO TUserFavorites( intUserID, intRecipeID )
-- VALUES					 (2, 5000002)
--						,(2, 5000001)
--						,(3, 5000001)
--						,(4, 5000003)
--						,(4, 5000001)
--						,(1, 5000002)
--						,(1, 5000003)

--INSERT INTO TIngredients ( intIngredientID, strIngredientName )
--VALUES					 (1, 'Sweet White Onion' )
--						,(2, 'Red Onion')
--						,(3, 'Chicken Breast Bone-In')
--						,(4, 'Chicken Breast Boneless')
--						,(5, 'Chicken Thigh')
--						,(6, 'Ribeye')
--						,(7, 'Cabbage')
--						,(8, 'Butter')
--						,(9, 'Salt')
--						,(10, 'Peper')
--						,(11, 'Garlic Clove')
--						,(12, 'Rosemary')
--						,(13, 'Potatoe')
--						,(14, 'Celery')
--						,(15, 'Red-Snapper')
--						,(16, 'Lemon')
--						,(17, 'All-Spice')

-- INSERT INTO TRecipeIngredients (intRecipeID, intIngredientID, dblIngredientQuantity, strUnitOfMeasurement )
-- VALUES					 (5000001, 6, 1.5, 'LB')
--						,(5000001, 8, 1.75, 'TSP')
--						,(5000001, 9, 1, 'TBSP')
--						,(5000001, 10, 0.75, 'OZ')
--						,(5000001, 11, 3, 'ML')
--						,(5000001, 12, 3, 'Gram')
--						,(5000002, 3, 2, 'Cup')
--						,(5000002, 9, 1, 'Quart')
--						,(5000002, 10, 1, 'Pint')
--						,(5000002, 8, 1, 'Gallon')
--						,(5000003, 8, 1.75, 'TSP')
--						,(5000003, 9, 1, 'TBSP')
--						,(5000003, 10, 0.75, 'OZ')
--						,(5000002, 11, 3, 'ML')
--						,(5000003, 12, 13, 'Gram')
--						,(5000004, 3, 2, 'Cup')
--						,(5000004, 9, 12, 'Quart')
--						,(5000004, 10, 17, 'Pint')
--						,(5000004, 8, 11, 'Gallon')


--INSERT INTO TShoppingList ( intShoppingListID, intUserID, intRecipeIngredientID )
--VALUES						 (1,1,1)
--							,(2,1,2)
--							,(3,1,3)
--							,(4,1,4)
--							,(5,1,5)
--							,(6,1,6)
--							,(7,2,8)

--INSERT INTO TLast10 (intLast10ID, intUserID, intRecipeID)
--VALUES						 (1, 1, 5000001)
--							,(2, 1, 5000002)
--							,(3, 1, 5000003)
--							,(4, 1, 5000004)
--							,(5, 1, 5000005)
--							,(6, 1, 5000006)
--							,(7, 1, 5000007)
--							,(8, 1, 5000008)
--							,(9, 1, 5000009)
--							,(10, 1, 5000010)