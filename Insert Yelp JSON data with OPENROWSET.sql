USE YelpDB
GO

DELETE Tips
DELETE Checkins
DELETE UserFriends
DELETE Users
DELETE BusinessAttributes -- insert not implemented
DELETE Attributes         -- insert not implemented
DELETE BusinessCategories
DELETE Categories
DELETE Businesses

DECLARE @YelpUsers TABLE (json_data varchar(max));
DECLARE @YelpBusinesses TABLE (json_data varchar(max));
DECLARE @YelpTips TABLE (json_data varchar(max));
DECLARE @YelpCheckins TABLE (json_data varchar(max));

INSERT INTO @YelpUsers
  SELECT json_data
  FROM OPENROWSET(BULK 'C:\Users\dmfab\Documents\Visual Studio Projects\WSU\WSU-CS451\YelpJSON-BulkCopy\yelp_user.JSON',
                  FORMATFILE = 'C:\Users\dmfab\Documents\Visual Studio Projects\WSU\WSU-CS451\YelpJSON-BulkCopy\yelp_format.xml') U;
INSERT INTO @YelpBusinesses
  SELECT json_data
  FROM OPENROWSET(BULK 'C:\Users\dmfab\Documents\Visual Studio Projects\WSU\WSU-CS451\YelpJSON-BulkCopy\yelp_business.JSON',
                  FORMATFILE = 'C:\Users\dmfab\Documents\Visual Studio Projects\WSU\WSU-CS451\YelpJSON-BulkCopy\yelp_format.xml') B;
INSERT INTO @YelpTips
  SELECT json_data
  FROM OPENROWSET(BULK 'C:\Users\dmfab\Documents\Visual Studio Projects\WSU\WSU-CS451\YelpJSON-BulkCopy\yelp_tip.JSON',
                  FORMATFILE = 'C:\Users\dmfab\Documents\Visual Studio Projects\WSU\WSU-CS451\YelpJSON-BulkCopy\yelp_format.xml') T;
INSERT INTO @YelpCheckins
  SELECT json_data
  FROM OPENROWSET(BULK 'C:\Users\dmfab\Documents\Visual Studio Projects\WSU\WSU-CS451\YelpJSON-BulkCopy\yelp_checkin.JSON',
                  FORMATFILE = 'C:\Users\dmfab\Documents\Visual Studio Projects\WSU\WSU-CS451\YelpJSON-BulkCopy\yelp_format.xml') C;

INSERT INTO Users (UserID, Name, YelpingSince, AverageStars, Fans, Cool, Funny, Useful, TipCount) 
SELECT JsonUsers.*
FROM @YelpUsers U
  CROSS APPLY
  OPENJSON (U.json_data) WITH (
    [user_id] varchar(50),
    [name] varchar(max),
    [yelping_since] datetime,
    [average_stars] float,
    [fans] int,
    [cool] int,
    [funny] int,
    [useful] int,
    [tipcount] int
  ) AS JsonUsers;

INSERT INTO UserFriends(UserID, FriendID) 
SELECT JsonUsers.user_id, JsonFriends.value
FROM @YelpUsers U
  CROSS APPLY
  OPENJSON (U.json_data) WITH (
    [user_id] varchar(50),
    [friends] nvarchar(max) AS JSON
  ) AS JsonUsers
  CROSS APPLY
  OPENJSON (JsonUsers.friends) AS JsonFriends;

INSERT INTO Businesses (BusinessID, Name, Address, City, State, PostalCode, Latitude, Longitude, Stars, ReviewCount, IsOpen)
SELECT JsonBusinesses.*
FROM @YelpBusinesses B
  CROSS APPLY
  OPENJSON (B.json_data) WITH (
    [business_id] varchar(50),
    [name] varchar(max),
    [address] varchar(max),
    [city] varchar(max),
    [state] varchar(max),
    [postal_code] varchar(max),
    [latitude] float,
    [longitude] float,
    [stars] float,
    [review_count] int,
    [is_open] int
  ) AS JsonBusinesses;

INSERT INTO Categories (Name)
SELECT DISTINCT Category=LTRIM(RTRIM(CategoryList.value))
FROM @YelpBusinesses B
  CROSS APPLY
  OPENJSON (B.json_data) WITH (
    [categories] varchar(max)
  ) AS JsonCategories
  CROSS APPLY
  STRING_SPLIT(JsonCategories.categories, ',') CategoryList;

INSERT INTO BusinessCategories (BusinessID, CategoryID)
SELECT JsonBusinessCategories.business_id, C.CategoryID
FROM @YelpBusinesses B
  CROSS APPLY
  OPENJSON (B.json_data) WITH (
    [business_id] varchar(50),
    [categories] varchar(max)
  ) AS JsonBusinessCategories
  CROSS APPLY
  STRING_SPLIT(JsonBusinessCategories.categories, ',') CategoryList
  INNER JOIN Categories C ON C.Name=LTRIM(RTRIM(CategoryList.value));

INSERT INTO Checkins(BusinessID, CheckinDate)
SELECT JsonCheckins.business_id, CheckinDates.value
FROM @YelpCheckins C
  CROSS APPLY
  OPENJSON (C.json_data) WITH (
    [business_id] varchar(50),
    [date] varchar(max)
  ) AS JsonCheckins
  CROSS APPLY
  STRING_SPLIT(JsonCheckins.[date], ',') CheckinDates;

INSERT INTO Tips (BusinessID, UserID, TipDate, Likes, [Text]) 
SELECT JsonTips.*
FROM @YelpTips T
  CROSS APPLY
  OPENJSON (T.json_data) WITH (
    [business_id] varchar(50),
    [user_id] varchar(50),
    [date] datetime,
    [likes] int,
    [text] varchar(max)
  ) AS JsonTips;
