DROP DATABASE IF EXISTS DieselDatabaseV8;
CREATE DATABASE DieselDatabaseV8;
USE DieselDatabaseV8;

DROP TABLE IF EXISTS Categorie;
CREATE TABLE Categorie
(
    CategorieID INT NOT NULL AUTO_INCREMENT,
    NaamCategorie VARCHAR(100),
    ParentID INT,
    PRIMARY KEY (CategorieID)
);

DROP TABLE IF EXISTS Product;
CREATE TABLE Product
(
    ProductID INT NOT NULL AUTO_INCREMENT,
    CategorieID INT NOT NULL,
    ProductNaam VARCHAR(255) NOT NULL,
    ProductDesc VARCHAR(255),
    ProductPrijs DECIMAL(4,2) NOT NULL,
    Voorraad INT,
    Supplier VARCHAR(48),
    LactoseVrij BOOLEAN DEFAULT 0,
    Vegetarisch BOOLEAN DEFAULT 0,
    Veganistisch BOOLEAN DEFAULT 0,
    ProductAfbeelding VARCHAR(255) DEFAULT 'default.jpg',
    PRIMARY KEY (ProductID)
);

DROP TABLE IF EXISTS Bestelronde;
CREATE TABLE Bestelronde
(
    BestelrondeID INT NOT NULL AUTO_INCREMENT,
    OberID INT,
    StatusBestelling VARCHAR(24) NOT NULL,
    Tijd DATETIME NOT NULL,
    PRIMARY KEY (BestelrondeID)
);

DROP TABLE IF EXISTS Product_per_Bestelronde;
CREATE TABLE Product_per_Bestelronde
(
    ProductID INT NOT NULL,
    BestelrondeID INT NOT NULL,
    AantalProduct INT,
    AantalBetaald INT,
    StatusBesteldeProduct VARCHAR(24) NOT NULL,
    PRIMARY KEY (ProductID, BestelrondeID)
);

DROP TABLE IF EXISTS Ober;
CREATE TABLE Ober
(
    OberID INT NOT NULL AUTO_INCREMENT,
    MedewerkerID INT NOT NULL,
    OberNaam VARCHAR(48) NOT NULL,
    PRIMARY KEY (OberID)
);

DROP TABLE IF EXISTS Medewerker;
CREATE TABLE Medewerker
(
    MedewerkerID INT NOT NULL AUTO_INCREMENT,
    MedewerkerNaam VARCHAR(48) NOT NULL,
    Telefoonnummer VARCHAR(15) NOT NULL,
    EmailMedewerker VARCHAR(48) NOT NULL,
    MedewerkerInlognaam VARCHAR(28) NOT NULL,
    MedewerkerWachtwoord VARCHAR(48) NOT NULL,  
    MedewerkerType VARCHAR(24) NOT NULL DEFAULT 'Medewerker',
    SupervisorID INT, 
    MedewerkerStatus VARCHAR(24) NOT NULL DEFAULT 'Actief',
    MedewerkerGeboortedatum DATE NOT NULL DEFAULT '2000-01-01',
    MedewerkerAdres VARCHAR(255) NOT NULL DEFAULT 'Straatnaam 1',
    MedewerkerPostcode VARCHAR(7) NOT NULL DEFAULT '1000AA',
    MedewerkerWoonplaats VARCHAR(48) NOT NULL DEFAULT 'Amsterdam',
    MedewerkerRol VARCHAR(24) NOT NULL DEFAULT 'Cook',
    MedewerkerContracturen INT NOT NULL DEFAULT 40,
    MedewerkerContracttype VARCHAR(24) NOT NULL DEFAULT 'Fulltime',
    MedewerkerSalaris INT NOT NULL DEFAULT 2000,
    MedewerkerContractBegin DATE NOT NULL DEFAULT '2020-01-01',
    MedewerkerContractEinde DATE NOT NULL DEFAULT '2021-01-01',
    MedewerkerBankrekening VARCHAR(24) NOT NULL DEFAULT 'NL00ABNA0123456789',
    MedewerkerAfbeelding VARCHAR(255) NOT NULL DEFAULT 'default.jpg',
    PRIMARY KEY (MedewerkerID)
);

DROP TABLE IF EXISTS Medewerker_Ober_koppel;
CREATE TABLE Medewerker_Ober_koppel
(
    MedewerkerID INT NOT NULL,
    OberID INT NOT NULL,
    Inkloktijd DATETIME NOT NULL,
    Uitkloktijd DATETIME NOT NULL,
    PRIMARY KEY (MedewerkerID, OberID)
);

DROP TABLE IF EXISTS Bestelling;
CREATE TABLE Bestelling
(
    BestellingID INT NOT NULL AUTO_INCREMENT,
    TafelID INT,
    BestelrondeID INT,
    StatusBestelling VARCHAR(24) NOT NULL,
    TijdBestelling DATETIME NOT NULL,
    KostenplaatsnummerID INT,
    TotaalPrijs INT,
    PRIMARY KEY (BestellingID)
);

DROP TABLE IF EXISTS Tafel;
CREATE TABLE Tafel
(
    TafelID INT NOT NULL AUTO_INCREMENT,
    TafelSectie VARCHAR(24),
    TafelAfbeelding VARCHAR(255) NOT NULL DEFAULT 'defaultTable.jpg',
    PRIMARY KEY (TafelID)
);

DROP TABLE IF EXISTS Overzicht;
CREATE TABLE Overzicht
(
    OverzichtID INT NOT NULL AUTO_INCREMENT,
    BestellingID INT,
    ProductVerkocht INT,
    ProductVoorraad INT,
    PRIMARY KEY (OverzichtID)
);

DROP TABLE IF EXISTS Overzicht_per_product;
CREATE TABLE Overzicht_per_product
(
    ProductID INT NOT NULL,
    OverzichtID INT NOT NULL,
    VoorraadPP INT,
    PRIMARY KEY (ProductID, OverzichtID)
);

DROP TABLE IF EXISTS FAQ;
CREATE TABLE FAQ
(
    FAQID INT NOT NULL AUTO_INCREMENT,
    FAQVraag VARCHAR(255) NOT NULL,
    FAQAntwoord VARCHAR(255) NOT NULL,
    PRIMARY KEY (FAQID)
);

ALTER TABLE Categorie
ADD FOREIGN KEY (ParentID) REFERENCES Categorie(CategorieID) ON DELETE SET NULL;

ALTER TABLE Product
ADD FOREIGN KEY (CategorieID) REFERENCES Categorie(CategorieID) ON DELETE CASCADE;

ALTER TABLE Bestelronde
ADD FOREIGN KEY (OberID) REFERENCES Ober(OberID) ON DELETE CASCADE;

ALTER TABLE Product_per_Bestelronde
ADD FOREIGN KEY (ProductID) REFERENCES Product(ProductID) ON DELETE CASCADE;

ALTER TABLE Product_per_Bestelronde
ADD FOREIGN KEY (BestelrondeID) REFERENCES Bestelronde(BestelrondeID) ON DELETE CASCADE;

ALTER TABLE Ober
ADD FOREIGN KEY (MedewerkerID) REFERENCES Medewerker(MedewerkerID) ON DELETE CASCADE;

ALTER TABLE Medewerker
ADD FOREIGN KEY (SupervisorID) REFERENCES Medewerker(MedewerkerID) ON DELETE SET NULL;

ALTER TABLE Medewerker_Ober_koppel
ADD FOREIGN KEY (MedewerkerID) REFERENCES Medewerker(MedewerkerID) ON DELETE CASCADE;

ALTER TABLE Medewerker_Ober_koppel
ADD FOREIGN KEY (OberID) REFERENCES Ober(OberID) ON DELETE CASCADE;

ALTER TABLE Bestelling
ADD FOREIGN KEY (TafelID) REFERENCES Tafel(TafelID) ON DELETE CASCADE;

ALTER TABLE Bestelling
ADD FOREIGN KEY (BestelrondeID) REFERENCES Bestelronde(BestelrondeID) ON DELETE CASCADE;

ALTER TABLE Overzicht
ADD FOREIGN KEY (BestellingID) REFERENCES Bestelling(BestellingID) ON DELETE CASCADE;

ALTER TABLE Overzicht_per_product
ADD FOREIGN KEY (ProductID) REFERENCES Product(ProductID) ON DELETE CASCADE;

ALTER TABLE Overzicht_per_product
ADD FOREIGN KEY (OverzichtID) REFERENCES Overzicht(OverzichtID) ON DELETE CASCADE;

-- Insert dummy data

-- Insert data into Categorie
INSERT INTO Categorie
    (NaamCategorie, ParentID)
VALUES
    ('Beverages', NULL),
    ('Food', NULL),
    ('Re-usables / disposables', NULL),
    ('Hot Drinks', 1),
    ('Cold Drinks', 1),
    ('Non-Alcoholic Drinks', 1),
    ('Snacks', 2),
    ('Lunch', 2),
    ('Pastry''s & Desserts', 2);

-- Insert data into Product
INSERT INTO Product
    (CategorieID, ProductNaam, ProductDesc, ProductPrijs, Voorraad, Supplier, LactoseVrij, Vegetarisch, Veganistisch, ProductAfbeelding)
VALUES
    (4, 'Coffee Creme', 'A smooth coffee with cream, perfect for a light coffee break.', 1.22, 100, 'Default Supplier', 0, 0, 0, 'Coffee Creme.png'),
    (4, 'Coffee Creme Large', 'A larger serving of smooth coffee with cream for those who need a bit more.', 1.88, 100, 'Default Supplier', 0, 0, 0, 'Coffee Creme Large.png'),
    (4, 'Cappuccino', 'Classic Italian coffee with steamed milk and a thick layer of foam.', 1.5, 100, 'Default Supplier', 0, 0, 0, 'Cappuccino.png'),
    (4, 'Cappuccino Large', 'A bigger version of the classic Italian coffee with extra steamed milk and foam.', 2.16, 100, 'Default Supplier', 0, 0, 0, 'Cappuccino Large.png'),
    (4, 'Espresso', 'A strong, concentrated coffee served in a small cup.', 1.22, 100, 'Default Supplier', 0, 0, 0, 'Espresso.png'),
    (4, 'Double Espresso', 'Twice the amount of strong, concentrated coffee for an extra caffeine boost.', 1.88, 100, 'Default Supplier', 0, 0, 0, 'Double Espresso.png'),
    (4, 'Latte Macchiato', 'Layers of steamed milk and espresso with a touch of foam.', 1.5, 100, 'Default Supplier', 0, 0, 0, 'Latte Macchiato.png'),
    (4, 'Latte Macchiato Large', 'A larger serving of layered steamed milk and espresso with a touch of foam.', 2.16, 100, 'Default Supplier', 0, 0, 0, 'Latte Macchiato Large.png'),
    (4, 'Flat White', 'A rich, velvety coffee with steamed milk and a thin layer of microfoam.', 1.88, 100, 'Default Supplier', 0, 0, 0, 'Flat White.png'),
    (4, 'Tea', 'A standard serving of black, green, or herbal tea.', 1.17, 100, 'Default Supplier', 0, 0, 0, 'Tea.png'),
    (4, 'Fresh Tea', 'A freshly brewed tea with vibrant flavors.', 1.3, 100, 'Default Supplier', 0, 0, 0, 'Fresh Tea.png'),
    (4, 'Hot Chocolate Dark', 'A rich and intense dark chocolate drink.', 2.1, 100, 'Default Supplier', 0, 0, 0, 'Hot Chocolate Dark.png'),
    (4, 'Syrup For Coffee', 'A variety of flavored syrups to add a sweet touch to your coffee.', 0.39, 100, 'Default Supplier', 0, 0, 0, 'Syrup For Coffee.png'),
    (4, 'Chai Latte', 'A spiced tea blend mixed with steamed milk.', 2.6, 100, 'Default Supplier', 0, 0, 0, 'Chai Latte.png'),
    (5, 'Ice Latte', 'A cold coffee drink with milk, perfect for a refreshing break.', 2.59, 100, 'Default Supplier', 0, 0, 0, 'Ice Latte.png'),
    (4, 'Dirty Chai Latte', 'A chai latte with an added shot of espresso for an extra kick.', 3.25, 100, 'Default Supplier', 0, 0, 0, 'Dirty Chai Latte.png'),
    (4, 'Coffee Special', 'A specialty coffee drink, ask for the flavor of the day.', 3, 100, 'Default Supplier', 0, 0, 0, 'Coffee Special.png'),
    (5, 'Smoothie', 'A blended fruit drink, available in various flavors.', 2.5, 100, 'Default Supplier', 0, 0, 0, 'Smoothie.png'),
    (5, 'Homemade Ice Tea', 'Refreshing homemade iced tea, perfect for a hot day.', 2.75, 100, 'Default Supplier', 0, 0, 0, 'Homemade Ice Tea.png'),
    (5, 'Water Still', 'Pure, still drinking water.', 2.1, 100, 'Default Supplier', 0, 0, 0, 'Water Still.png'),
    (5, 'Water Sparkling', 'Sparkling water with a refreshing fizz.', 2.1, 100, 'Default Supplier', 0, 0, 0, 'Water Sparkling.png'),
    (5, 'Lemon Lime Soda', 'A citrus-flavored soda, refreshing and bubbly.', 2.75, 100, 'Default Supplier', 0, 0, 0, 'Lemon Lime Soda.png'),
    (5, 'Tonic Bottle', 'Classic tonic water, great on its own or as a mixer.', 3.2, 100, 'Default Supplier', 0, 0, 0, 'Tonic Bottle.png'),
    (5, 'Fritz Cola', 'A popular cola drink with a unique flavor.', 2.6, 100, 'Default Supplier', 0, 0, 0, 'Fritz Cola.png'),
    (5, 'Fritz Rhubarb', 'A refreshing rhubarb-flavored soda.', 2.6, 100, 'Default Supplier', 0, 0, 0, 'Fritz Rhubarb.png'),
    (5, 'Fritz Limo Sinas', 'A tangy orange-flavored soda.', 2.6, 100, 'Default Supplier', 0, 0, 0, 'Fritz Limo Sinas.png'),
    (5, 'Fritz Lemon', 'A zesty lemon-flavored soda.', 2.6, 100, 'Default Supplier', 0, 0, 0, 'Fritz Lemon.png'),
    (5, 'Fritz Apple', 'A crisp apple-flavored soda.', 2.6, 100, 'Default Supplier', 0, 0, 0, 'Fritz Apple.png'),
    (6, 'Virgin Gin Tonic', 'A non-alcoholic version of the classic gin and tonic.', 5.5, 100, 'Default Supplier', 0, 0, 0, 'Virgin Gin Tonic.png'),
    (6, 'Van De Streek IPA', 'A flavorful non-alcoholic IPA beer.', 4.4, 100, 'Default Supplier', 0, 0, 0, 'Van De Streek IPA.png'),
    (6, 'Heineken Draft 0.0%', 'Non-alcoholic draft beer with the classic Heineken taste.', 3.2, 100, 'Default Supplier', 0, 0, 0, 'Heineken Draft 0.0%.png'),
    (9, 'Pie Of The Day', 'A daily selection of freshly baked pie.', 2.25, 100, 'Default Supplier', 0, 0, 0, 'Pie Of The Day.png'),
    (9, 'Homemade Apple Traybake', 'A delightful apple dessert baked in a tray, homemade style.', 3, 100, 'Default Supplier', 0, 0, 0, 'Homemade Apple Traybake.png'),
    (7, 'Bitterbal Per Piece', 'A single serving of a Dutch meat-based snack.', 1.1, 100, 'Default Supplier', 0, 0, 0, 'Bitterbal Per Piece.png'),
    (7, 'Vegetarian Springroll', 'A crispy roll filled with vegetables, served hot.', 4.5, 100, 'Default Supplier', 0, 0, 0, 'Vegetarian Springroll.png'),
    (8, '12-Uurtje', 'A traditional Dutch lunch plate with various small dishes.', 6.6, 100, 'Default Supplier', 0, 0, 0, '12-Uurtje.png'),
    (8, 'Croquette On Sourdough Bread', 'A Dutch croquette served on hearty sourdough bread.', 6.5, 100, 'Default Supplier', 0, 0, 0, 'Croquette On Sourdough Bread.png'),
    (8, 'Pulled Chicken', 'Tender, shredded chicken served with sauce.', 6.5, 100, 'Default Supplier', 0, 0, 0, 'Pulled Chicken.png'),
    (8, 'Pot Of The Season', 'A seasonal dish made with fresh ingredients.', 7.2, 100, 'Default Supplier', 0, 0, 0, 'Pot Of The Season.png'),
    (8, 'Grandma''s Meatball', 'A comforting, homemade-style meatball.', 6.2, 100, 'Default Supplier', 0, 0, 0, 'Grandma''s Meatball.png'),
    (8, 'Wentelteefjes', 'Traditional Dutch French toast, served sweet.', 5.1, 100, 'Default Supplier', 0, 0, 0, 'Wentelteefjes.png'),
    (8, 'Pasta Pesto', 'Pasta served with a fresh, basil pesto sauce.', 5.85, 100, 'Default Supplier', 0, 0, 0, 'Pasta Pesto.png'),
    (8, 'Surprise Menu Café Brandstof', 'A chef''s choice menu, offering a delightful surprise.', 6, 100, 'Default Supplier', 0, 0, 0, 'Surprise Menu Café Brandstof.png'),
    (6, 'Mocktail Of The Day', 'A daily selection of a non-alcoholic mixed drink.', 5.5, 100, 'Default Supplier', 0, 0, 0, 'Mocktail Of The Day.png'),
    (8, 'High Tea', 'An assortment of teas, finger sandwiches, and pastries, perfect for an afternoon treat.', 14.95, 100, 'Default Supplier', 0, 0, 0, 'High Tea.png'),
    (3, 'LBS Circulware Cup', 'A reusable cup for drinks, promoting sustainability.', 2, 100, 'Default Supplier', 0, 0, 0, 'LBS Circulware Cup.png'),
    (3, 'LBS Circulware Lid', 'A matching lid for the reusable cup.', 2, 100, 'Default Supplier', 0, 0, 0, 'LBS Circulware Lid.png'),
    (3, 'Re-Usable Cutlery Set', 'A set of reusable cutlery for meals on the go.', 4.5, 100, 'Default Supplier', 0, 0, 0, 'Re-Usable Cutlery Set.png');

-- Insert data into Medewerker
INSERT INTO Medewerker
    (MedewerkerNaam, Telefoonnummer, EmailMedewerker, SupervisorID, MedewerkerInlognaam, MedewerkerWachtwoord, MedewerkerType, MedewerkerStatus, MedewerkerGeboortedatum, MedewerkerAdres, MedewerkerPostcode, MedewerkerWoonplaats, MedewerkerRol, MedewerkerContracturen, MedewerkerContracttype, MedewerkerSalaris, MedewerkerContractBegin, MedewerkerContractEinde, MedewerkerBankrekening, MedewerkerAfbeelding)
VALUES
    ('John Doe', '0612345678', 'john.doe@cafe.com', NULL, 'johndoe', 'password123', 'Medewerker', 'Actief', '1990-05-15', 'Street 1', '1000AA', 'Amsterdam', 'Barista', 40, 'Fulltime', 2500, '2020-01-01', '2022-01-01', 'NL00ABNA0123456789', 'john.jpg'),
    ('Jane Smith', '0698765432', 'jane.smith@cafe.com', 1, 'janesmith', 'securepassword', 'Medewerker', 'Actief', '1985-08-20', 'Street 2', '2000BB', 'Rotterdam', 'Waiter', 32, 'Parttime', 1800, '2019-03-01', '2021-03-01', 'NL00INGB0987654321', 'jane.jpg'),
    ('Alice Johnson', '0611223344', 'alice.johnson@cafe.com', 1, 'alicej', 'alicepass', 'Medewerker', 'Actief', '1992-11-10', 'Street 3', '3000CC', 'The Hague', 'Cook', 40, 'Fulltime', 2300, '2021-02-01', '2023-02-01', 'NL00RABO0234567890', 'alice.jpg'),
    ('Bob Brown', '0688776655', 'bob.brown@cafe.com', 2, 'bobb', 'bobpassword', 'Medewerker', 'Actief', '1988-03-25', 'Street 4', '4000DD', 'Utrecht', 'Waiter', 20, 'Parttime', 1600, '2018-05-15', '2020-05-15', 'NL00SNSB0345678901', 'bob.jpg');

-- Insert data into Ober
INSERT INTO Ober
    (MedewerkerID, OberNaam)
VALUES
    (2, 'Jane Smith'),
    (4, 'Bob Brown');

-- Insert data into Bestelronde
INSERT INTO Bestelronde
    (OberID, StatusBestelling, Tijd)
VALUES
    (1, 'In Progress', '2024-06-01 12:30:00'),
    (1, 'Pending', '2024-06-01 13:00:00'),
    (2, 'In Progress', '2024-06-01 14:00:00'),
    (2, 'Pending', '2024-06-01 14:30:00');

-- Insert data into Product_per_Bestelronde
INSERT INTO Product_per_Bestelronde
    (ProductID, BestelrondeID, AantalProduct, AantalBetaald, StatusBesteldeProduct)
VALUES
    (1, 1, 2, 2, 'Pending'),
    (4, 1, 1, 1, 'Pending'),
    (7, 2, 3, 3, 'Pending'),
    (10, 2, 1, 1, 'In Progress'),
    (2, 3, 1, 1, 'Served'),
    (3, 3, 2, 2, 'Served'),
    (8, 4, 1, 1, 'Served'),
    (9, 4, 2, 2, 'Served');

-- Insert data into Tafel
INSERT INTO Tafel
    (TafelSectie, TafelAfbeelding)
VALUES
    ('A', 'table1.jpg'),
    ('B', 'table2.jpg'),
    ('C', 'table3.jpg'),
    ('D', 'table4.jpg');

-- Insert data into Bestelling
INSERT INTO Bestelling
    (TafelID, BestelrondeID, StatusBestelling, TijdBestelling, KostenplaatsnummerID, TotaalPrijs)
VALUES
    (1, 1, 'Pending', '2024-06-01 12:30:00', NULL, 9),
    (2, 2, 'Pending', '2024-06-01 13:00:00', NULL, 16),
    (3, 3, 'In Progress', '2024-06-01 14:00:00', NULL, 7),
    (4, 4, 'Completed', '2024-06-01 14:30:00', NULL, 8);

-- Insert data into Overzicht
INSERT INTO Overzicht
    (BestellingID, ProductVerkocht, ProductVoorraad)
VALUES
    (1, 3, 30),
    (2, 4, 40),
    (3, 3, 25),
    (4, 3, 20);

-- Insert data into Overzicht_per_product
INSERT INTO Overzicht_per_product
    (ProductID, OverzichtID, VoorraadPP)
VALUES
    (1, 1, 98),
    (4, 1, 49),
    (7, 2, 27),
    (10, 2, 14),
    (2, 3, 79),
    (3, 3, 88),
    (8, 4, 39),
    (9, 4, 48);

-- Insert data into Medewerker_Ober_koppel
INSERT INTO Medewerker_Ober_koppel
    (MedewerkerID, OberID, Inkloktijd, Uitkloktijd)
VALUES
    (1, 2, '2024-06-01 08:00:00', '2024-06-01 16:00:00'),
    (2, 1, '2024-06-01 09:00:00', '2024-06-01 17:00:00'),
    (3, 2, '2024-06-01 10:00:00', '2024-06-01 18:00:00'),
    (4, 1, '2024-06-01 11:00:00', '2024-06-01 19:00:00');

-- Insert data into FAQ
INSERT INTO FAQ
    (FAQVraag, FAQAntwoord)
VALUES
    ('What are your opening hours?', 'We are open from 8 AM to 8 PM every day.'),
    ('Do you offer vegan options?', 'Yes, we have several vegan options available.'),
    ('Do you have WiFi?', 'Yes, we offer free WiFi for our customers.');