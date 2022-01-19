CREATE DATABASE CigarShop

USE CigarShop

CREATE TABLE Sizes
(
	Id INT PRIMARY KEY IDENTITY(1, 1)
	,[Length] INT NOT NULL
	,RingRange DECIMAL NOT NULL
)

CREATE TABLE Tastes
(
	Id INT PRIMARY KEY IDENTITY(1, 1)
	,TasteType NVARCHAR(20) NOT NULL
	,TasteStrength NVARCHAR(15) NOT NULL
	,ImageURL NVARCHAR(100) NOT NULL
)

CREATE TABLE Brands
(
	Id INT PRIMARY KEY IDENTITY(1, 1)
	,BrandName NVARCHAR(30) UNIQUE NOT NULL
	,BrandDescription NVARCHAR(max) NULL
)

CREATE TABLE Cigars
(
	Id INT PRIMARY KEY IDENTITY(1, 1)
	,CigarName NVARCHAR(80) NOT NULL
	,BrandId INT NOT NULL
	CONSTRAINT FK_Cigars_Brands
	FOREIGN KEY (BrandId)
	REFERENCES Brands(Id)
	,TastId INT NOT NULL
	CONSTRAINT FK_Cigar_Tastes
	FOREIGN KEY (TastId)
	REFERENCES Tastes(Id)
	,SizeId INT NOT NULL
	CONSTRAINT FK_Cigars_Sizes
	FOREIGN KEY (SizeId)
	REFERENCES Sizes(Id)
	,PriceForSingleCigar DECIMAL NOT NULL
	,ImageURL NVARCHAR(100) NOT NULL
)

CREATE TABLE Addresses
(
	Id INT PRIMARY KEY IDENTITY(1, 1)
	,Town NVARCHAR(30) NOT NULL
	,Country NVARCHAR(30) NOT NULL
	,Streat NVARCHAR(100) NOT NULL
	,ZIP NVARCHAR(20) NOT NULL
)

CREATE TABLE Clients
(
	Id INT PRIMARY KEY IDENTITY(1, 1)
	,FirstName NVARCHAR(30) NOT NULL
	,LastName NVARCHAR(30) NOT NULL
	,Email NVARCHAR(50) NOT NULL
	,AddressId INT NOT NULL
	CONSTRAINT FK_Clients_Addresses
	FOREIGN KEY (AddressId)
	REFERENCES Addresses(Id)
)

CREATE TABLE ClientsCigars
(
	ClientId INT NOT NULL
	CONSTRAINT FK_ClientsCigars
	FOREIGN KEY (ClientId)
	REFERENCES Clients(Id)
	,CigarId INT NOT NULL
	CONSTRAINT FK_ClientsCigars_Cigars
	FOREIGN KEY (CigarId)
	REFERENCES Cigars(Id)
	,CONSTRAINT PK_ClientCigars
	PRIMARY KEY (ClientId, CigarId)
)

--2--
INSERT INTO Cigars(CigarName, BrandId, TastId, SizeId, PriceForSingleCigar, ImageURL)
VALUES
('COHIBA ROBUSTO', 9, 1, 5, 15.50, 'cohiba-robusto-stick_18.jpg')
,('COHIBA SIGLO I', 9, 1, 10, 410.00, 'cohiba-siglo-i-stick_12.jpg')
,('HOYO DE MONTERREY LE HOYO DU MAIRE', 14, 5, 11, 7.50, 'hoyo-du-maire-stick_17.jpg')
,('HOYO DE MONTERREY LE HOYO DE SAN JUAN', 14, 4, 15, 32.00, 'hoyo-de-san-juan-stick_20.jpg')
,('TRINIDAD COLONIALES', 2, 3, 8, 85.21, 'trinidad-coloniales-stick_30.jpg')

INSERT INTO Addresses(Town, Country, Streat, ZIP)
VALUES
('Sofia', 'Bulgaria', '18 Bul. Vasil levski', '1000')
,('Athens', 'Greece', '4342 McDonald Avenue', '10435')
,('Zagreb', 'Croatia', '4333 Lauren Drive', '10000')

--3--
UPDATE Cigars
SET PriceForSingleCigar += PriceForSingleCigar * 0.2
WHERE TastId = 1

UPDATE Brands
SET BrandDescription = 'New description'
WHERE BrandDescription IS NULL

--4--
DELETE FROM Clients WHERE AddressId IN 
(
	SELECT Id 
	FROM Addresses
	WHERE LEFT(Country, 1) = 'C'
)
DELETE FROM Addresses
WHERE LEFT(Country, 1) = 'C'

--5--
SELECT CigarName
,PriceForSingleCigar
,ImageURL
FROM Cigars
ORDER BY PriceForSingleCigar, CigarName DESC

--6--
SELECT c.Id
,c.CigarName
,c.PriceForSingleCigar
,t.TasteType
,t.TasteStrength
FROM Cigars AS c
JOIN Tastes AS t ON c.TastId = t.Id
WHERE t.TasteType IN ('Earthy', 'Woody')
ORDER BY c.PriceForSingleCigar DESC

--7--
SELECT c.Id
,c.FirstName + ' ' + LastName AS ClientName
,c.Email
FROM Clients AS c
WHERE c.Id NOT IN
(
	SELECT ClientId 
	FROM ClientsCigars
)
ORDER BY c.FirstName

--8--
SELECT TOP (5) c.CigarName
,c.PriceForSingleCigar
,c.ImageURL
--,s.[Length]
--,s.[RingRange]
FROM Cigars AS c
JOIN Sizes AS s ON c.SizeId = s.Id
WHERE s.[Length] >= 12 AND ((CHARINDEX('ci', CigarName) > 0) OR (PriceForSingleCigar > 50)) AND s.RingRange > 2.55
ORDER BY c.CigarName ASC, c.PriceForSingleCigar DESC

--9--
SELECT c.FirstName + ' ' + c.LastName AS FullName
,a.Country
,a.ZIP
,'$' + CAST(MAX(ci.PriceForSingleCigar) AS NVARCHAR(10)) AS CigarPrice
FROM Clients AS c
JOIN Addresses AS a ON c.AddressId = a.Id
JOIN ClientsCigars AS cc ON c.Id = cc.ClientId
JOIN Cigars AS ci ON cc.CigarId = ci.Id
WHERE a.ZIP NOT LIKE '%[^0-9]%' AND ZIP NOT LIKE '%.%.%'
GROUP BY c.FirstName, c.LastName, a.Country, a.ZIP
ORDER BY FullName

--10--
SELECT c.LastName
,CEILING(AVG(s.Length)) AS CigarLength
,CEILING(AVG(s.RingRange)) AS CigarRingRange
FROM Clients AS c
LEFT JOIN ClientsCigars AS cc ON cc.ClientId = c.Id
JOIN Cigars AS ci ON cc.CigarId = ci.Id
JOIN Sizes AS s ON ci.SizeId = s.Id
GROUP BY c.LastName
ORDER BY CigarLength DESC

--11--
CREATE FUNCTION udf_ClientWithCigars(@name NVARCHAR(20))
RETURNS INT
AS
BEGIN
	DECLARE @COUNT INT;
	
	SET @COUNT = (
	SELECT COUNT(CigarId) 
	FROM Clients AS c
	JOIN ClientsCigars AS cc ON c.Id = cc.ClientId 
	WHERE c.FirstName = @name
	)
	RETURN @COUNT;
END

--12--
CREATE PROC usp_SearchByTaste(@taste NVARCHAR(20))
AS
BEGIN
	SELECT CigarName
	,'$' + CAST(PriceForSingleCigar AS NVARCHAR(10))
	,t.TasteType
	,b.BrandName
	,CAST(s.[Length] AS NVARCHAR(10)) + ' cm'
	,CAST(s.RingRange AS NVARCHAR(10)) + ' cm'
	FROM Cigars AS c
	JOIN Tastes AS t ON c.TastId = t.Id
	JOIN Brands AS b ON c.BrandId = b.Id
	JOIN Sizes AS s ON c.SizeId = s.Id
	WHERE t.TasteType = @taste
	ORDER BY s.[Length], s.RingRange DESC
END