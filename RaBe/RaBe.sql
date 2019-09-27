-- TABLE
CREATE TABLE Arbeitsplatz(
  id int not null primary key,
  name varchar(255) not null,
  raum_id int not null, position int not null default 0,
  FOREIGN KEY (raum_id) REFERENCES Raum(id)
);
CREATE TABLE Fehler(
  id int not null primary key,
  status int not null default 1,
  beschreibung varchar(5000) default NULL,
  titel varchar(255) not null,
  arbeitsplatz_id int not null,
  kategorie_id int not null,
  FOREIGN KEY (arbeitsplatz_id) REFERENCES Arbeitsplatz(id),
  FOREIGN KEY (kategorie_id) REFERENCES Kategorie(id)
);
CREATE TABLE Kategorie(
  id int not null primary key,
  name varchar(255) not null
);
CREATE TABLE Lehrer(
  id INT NOT NULL PRIMARY KEY,
  name varchar(255) NOT NULL,
  email varchar(255) NOT NULL,
  password varchar(255) not null,
  passwordGeaendert int not null default false,
  blocked int not null default false,
  administrator int not null default false,
  token varchar(255) default null
);
CREATE TABLE LehrerRaum(
  id int not null primary key,
  betreuer tinyint default false,
  lehrer_id int not null,
  raum_id int not null,
  FOREIGN KEY (lehrer_id) REFERENCES Lehrer(id),
  FOREIGN KEY (raum_id) REFERENCES Raum(id)
);
CREATE TABLE Raum(
  id int not null primary KEY,
  name varchar(255) not null
, vorlage int not null default 0);
CREATE TABLE StandardFehler(
  id int not null primary key,
  status int not null default 1,
  beschreibung varchar(5000) default NULL,
  titel varchar(255) not null,
  kategorie_id int not null,
  FOREIGN KEY (kategorie_id) REFERENCES Kategorie(id)
);
 
-- INDEX
 
-- TRIGGER
 
-- VIEW
 
