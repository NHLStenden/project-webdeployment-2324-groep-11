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
    ProductNaam VARCHAR(24) NOT NULL,
    ProductDesc VARCHAR(255),
    ProductPrijs INT NOT NULL,
    Voorraad INT,
    Supplier VARCHAR(48) NOT NULL DEFAULT 'Unknown',
    AddID INT,
    LactoseVrij BOOLEAN NOT NULL DEFAULT 0,
    Vegetarisch BOOLEAN NOT NULL DEFAULT 0,
    Veganistisch BOOLEAN NOT NULL DEFAULT 0,
    ProductAfbeelding VARCHAR(255) NOT NULL DEFAULT 'default.jpg',
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

ALTER TABLE Product
ADD FOREIGN KEY (AddID) REFERENCES Product(ProductID) ON DELETE SET NULL;

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
    (CategorieID, NaamCategorie, ParentID)
VALUES
    (1, 'Beverages', NULL),
    (2, 'Food', NULL),
    (3, 'Ingredients', NULL),
    (4, 'Hot Drinks', 1),
    (5, 'Cold Drinks', 1),
    (6, 'Alcoholic Drinks', 5),
    (7, 'Non-Alcoholic Drinks', 5),
    (8, 'Coffee', 4),
    (9, 'Tea', 4),
    (10, 'Sandwiches', 2),
    (11, 'Pasta', 2),
    (12, 'Salad', 2),
    (13, 'Pastry & Desserts', 2);




-- Insert data into Product
INSERT INTO Product
    (CategorieID, ProductNaam, ProductDesc, ProductPrijs, Voorraad, Supplier, AddID, LactoseVrij, Vegetarisch, Veganistisch, ProductAfbeelding)
VALUES
    (8, 'Espresso', 'Strong and bold coffee', 3, 100, 'Local Roasters', NULL, 1, 1, 1, 'espresso.jpg'),
    (8, 'Latte', 'Smooth coffee with milk', 4, 80, 'Local Roasters', NULL, 0, 1, 0, 'latte.jpg'),
    (8, 'Cappuccino', 'Coffee with steamed milk foam', 4, 90, 'Local Roasters', NULL, 0, 1, 0, 'cappuccino.jpg'),
    (7, 'Iced Tea', 'Refreshing iced tea', 3, 50, 'Tea Suppliers Inc.', NULL, 1, 1, 1, 'iced_tea.jpg'),
    (7, 'Smoothie', 'Fruit smoothie', 5, 40, 'Fruit Farmers', NULL, 1, 1, 1, 'smoothie.jpg'),
    (7, 'Lemonade', 'Fresh lemonade', 3, 60, 'Local Beverages', NULL, 1, 1, 1, 'lemonade.jpg'),
    (13, 'Muffin', 'Freshly baked muffin', 2, 30, 'Bakery Co.', NULL, 1, 1, 1, 'muffin.jpg'),
    (13, 'Croissant', 'Buttery French croissant', 3, 40, 'Bakery Co.', NULL, 1, 1, 0, 'croissant.jpg'),
    (13, 'Cookie', 'Chocolate chip cookie', 1, 50, 'Bakery Co.', NULL, 1, 1, 1, 'cookie.jpg'),
    (12, 'Caesar Salad', 'Mixed green salad', 7, 15, 'Farm Fresh', NULL, 1, 1, 1, 'salad.jpg'),
    (11, 'Pasta Bolognese', 'Italian pasta with tomato sauce', 8, 20, 'Pasta World', NULL, 0, 1, 0, 'pasta.jpg'),
    (10, 'Ham Sandwich', 'Ham and cheese sandwich', 5, 25, 'Deli Delights', NULL, 0, 0, 0, 'sandwich.jpg'),
    (3, 'Coffee Beans', 'Freshly ground beans', 5, 25, 'Coffee BV', NULL, 0, 0, 0, 'bean.jpg');


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
    (TafelID, TafelSectie, TafelAfbeelding)
VALUES
    (1, 'A', 'table1.jpg'),
    (2, 'B', 'table2.jpg'),
    (3, 'C', 'table3.jpg'),
    (4, 'D', 'table4.jpg');

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