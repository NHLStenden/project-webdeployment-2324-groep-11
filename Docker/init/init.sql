DROP DATABASE IF EXISTS DieselDatabaseV1;
CREATE DATABASE DieselDatabaseV1;
USE DieselDatabaseV1;

DROP TABLE IF EXISTS Categorie;
CREATE TABLE Categorie
(
    CategorieID INT,
    NaamCategorie VARCHAR(100),
    ParentID INT,
    PRIMARY KEY (CategorieID)
);

DROP TABLE IF EXISTS Product;
CREATE TABLE Product
(
    ProductID INT NOT NULL,
    CategorieID INT NOT NULL,
    ProductNaam VARCHAR(24) NOT NULL,
    ProductPrijs INT NOT NULL,
    Voorraad INT,
    AddID INT,
    PRIMARY KEY (ProductID)
);

DROP TABLE IF EXISTS Bestelronde;
CREATE TABLE Bestelronde
(
    BestelrondeID INT NOT NULL,
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
    PRIMARY KEY (ProductID, BestelrondeID)
);

DROP TABLE IF EXISTS Ober;
CREATE TABLE Ober
(
    OberID INT NOT NULL,
    MedewerkerID INT NOT NULL,
    OberNaam VARCHAR(48) NOT NULL,
    Inlognaam VARCHAR(28) NOT NULL,
    Wachtwoord VARCHAR(48) NOT NULL,
    PRIMARY KEY (OberID)
);

DROP TABLE IF EXISTS Medewerker;
CREATE TABLE Medewerker
(
    MedewerkerID INT NOT NULL,
    MedewerkerNaam VARCHAR(48) NOT NULL,
    Telefoonnummer VARCHAR(15) NOT NULL,
    EmailMedewerker VARCHAR(48) NOT NULL,
    SupervisorID INT,
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
    BestellingID INT NOT NULL,
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
    TafelID INT NOT NULL,
    Sectie VARCHAR(24),
    PRIMARY KEY (TafelID)
);

DROP TABLE IF EXISTS Overzicht;
CREATE TABLE Overzicht
(
    OverzichtID INT NOT NULL,
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

ALTER TABLE Categorie
ADD FOREIGN KEY (ParentID) REFERENCES Categorie(CategorieID);

ALTER TABLE Product
ADD FOREIGN KEY (CategorieID) REFERENCES Categorie(CategorieID);

ALTER TABLE Product
ADD FOREIGN KEY (AddID) REFERENCES Product(ProductID);

ALTER TABLE Bestelronde
ADD FOREIGN KEY (OberID) REFERENCES Ober(OberID);

ALTER TABLE Product_per_Bestelronde
ADD FOREIGN KEY (ProductID) REFERENCES Product(ProductID);

ALTER TABLE Product_per_Bestelronde
ADD FOREIGN KEY (BestelrondeID) REFERENCES Bestelronde(BestelrondeID);

ALTER TABLE Ober
ADD FOREIGN KEY (MedewerkerID) REFERENCES Medewerker(MedewerkerID);

ALTER TABLE Medewerker
ADD FOREIGN KEY (SupervisorID) REFERENCES Medewerker(MedewerkerID);

ALTER TABLE Medewerker_Ober_koppel
ADD FOREIGN KEY (MedewerkerID) REFERENCES Medewerker(MedewerkerID);

ALTER TABLE Medewerker_Ober_koppel
ADD FOREIGN KEY (OberID) REFERENCES Ober(OberID);

ALTER TABLE Bestelling
ADD FOREIGN KEY (TafelID) REFERENCES Tafel(TafelID);

ALTER TABLE Bestelling
ADD FOREIGN KEY (BestelrondeID) REFERENCES Bestelronde(BestelrondeID);

ALTER TABLE Overzicht
ADD FOREIGN KEY (BestellingID) REFERENCES Bestelling(BestellingID);

ALTER TABLE Overzicht_per_product
ADD FOREIGN KEY (ProductID) REFERENCES Product(ProductID);

ALTER TABLE Overzicht_per_product
ADD FOREIGN KEY (OverzichtID) REFERENCES Overzicht(OverzichtID);

-- Insert dummy data

INSERT INTO Categorie
    (CategorieID, NaamCategorie, ParentID)
VALUES
    (1, 'Beverages', NULL),
    (2, 'Alcoholic Beverages', 1),
    (3, 'Non-Alcoholic Beverages', 1),
    (4, 'Food', NULL),
    (5, 'Snacks', 4),
    (6, 'Dairy Products', 4),
    (7, 'Bakery', 4);

INSERT INTO Medewerker
    (MedewerkerID, MedewerkerNaam, Telefoonnummer, EmailMedewerker, SupervisorID)
VALUES
    (1, 'John Doe', '1234567890', 'john@example.com', NULL),
    (2, 'Jane Smith', '0987654321', 'jane@example.com', 1),
    (3, 'Alice Johnson', '1111111111', 'alice@example.com', 2),
    (4, 'Bob Brown', '2222222222', 'bob@example.com', 1);

INSERT INTO Ober
    (OberID, MedewerkerID, OberNaam, Inlognaam, Wachtwoord)
VALUES
    (1, 1, 'John Doe', 'john', 'password1'),
    (2, 2, 'Jane Smith', 'jane', 'password2'),
    (3, 3, 'Alice Johnson', 'alice', 'password3'),
    (4, 4, 'Bob Brown', 'bob', 'password4');

INSERT INTO Product
    (ProductID, CategorieID, ProductNaam, ProductPrijs, Voorraad, AddID)
VALUES
    (1, 2, 'Beer', 150, 100, NULL),
    (2, 3, 'Soda', 50, 200, NULL),
    (3, 5, 'Chips', 30, 150, NULL),
    (4, 6, 'Milk', 20, 80, NULL),
    (5, 7, 'Bread', 10, 50, NULL),
    (6, 2, 'Wine', 200, 90, NULL),
    (7, 3, 'Juice', 70, 120, NULL),
    (8, 5, 'Cookies', 40, 140, NULL);

INSERT INTO Bestelronde
    (BestelrondeID, OberID, StatusBestelling, Tijd)
VALUES
    (1, 1, 'Pending', NOW()),
    (2, 2, 'Completed', NOW()),
    (3, 3, 'Pending', NOW()),
    (4, 4, 'Completed', NOW());

INSERT INTO Product_per_Bestelronde
    (ProductID, BestelrondeID, AantalProduct, AantalBetaald)
VALUES
    (1, 1, 2, 2),
    (2, 2, 3, 3),
    (3, 3, 1, 1),
    (4, 4, 2, 2),
    (5, 1, 3, 3),
    (6, 2, 1, 1),
    (7, 3, 2, 2),
    (8, 4, 1, 1);

INSERT INTO Tafel
    (TafelID, Sectie)
VALUES
    (1, 'A'),
    (2, 'B'),
    (3, 'C'),
    (4, 'D');

INSERT INTO Bestelling
    (BestellingID, TafelID, BestelrondeID, StatusBestelling, TijdBestelling, KostenplaatsnummerID, TotaalPrijs)
VALUES
    (1, 1, 1, 'Pending', NOW(), NULL, 300),
    (2, 2, 2, 'Completed', NOW(), NULL, 150),
    (3, 3, 3, 'Pending', NOW(), NULL, 450),
    (4, 4, 4, 'Completed', NOW(), NULL, 200);

INSERT INTO Overzicht
    (OverzichtID, BestellingID, ProductVerkocht, ProductVoorraad)
VALUES
    (1, 1, 2, 98),
    (2, 2, 3, 197),
    (3, 3, 1, 149),
    (4, 4, 2, 48);

INSERT INTO Overzicht_per_product
    (ProductID, OverzichtID, VoorraadPP)
VALUES
    (1, 1, 98),
    (2, 2, 197),
    (3, 3, 149),
    (4, 4, 48),
    (5, 1, 47),
    (6, 2, 89),
    (7, 3, 118),
    (8, 4, 139);

INSERT INTO Medewerker_Ober_koppel
    (MedewerkerID, OberID, Inkloktijd, Uitkloktijd)
VALUES
    (1, 1, NOW(), NOW()),
    (2, 2, NOW(), NOW()),
    (3, 3, NOW(), NOW()),
    (4, 4, NOW(), NOW());