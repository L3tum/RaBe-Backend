-- TABLE
CREATE TABLE Arbeitsplatz(
  id integer primary key,
  name varchar(255) not null,
  raum_id integer not null, position integer not null default 0,
  FOREIGN KEY (raum_id) REFERENCES Raum(id)
);
CREATE TABLE Fehler(
  id integer primary key,
  status integer not null default 1,
  beschreibung varchar(5000) default NULL,
  titel varchar(255) not null,
  arbeitsplatz_id integer not null,
  kategorie_id integer not null,
  FOREIGN KEY (arbeitsplatz_id) REFERENCES Arbeitsplatz(id),
  FOREIGN KEY (kategorie_id) REFERENCES Kategorie(id)
);
CREATE TABLE Kategorie(
  id integer primary key,
  name varchar(255) not null
);
CREATE TABLE Lehrer(
  id integer PRIMARY KEY,
  name varchar(255) NOT NULL,
  email varchar(255) NOT NULL,
  password varchar(255) not null,
  passwordGeaendert integer not null default false,
  blocked integer not null default false,
  administrator integer not null default false,
  token varchar(255) default null
);
CREATE TABLE LehrerRaum(
  id integer primary key,
  betreuer tinyint default false,
  lehrer_id integer not null,
  raum_id integer not null,
  FOREIGN KEY (lehrer_id) REFERENCES Lehrer(id),
  FOREIGN KEY (raum_id) REFERENCES Raum(id)
);
CREATE TABLE Raum(
  id integer primary KEY,
  name varchar(255) not null
, vorlage integer not null default 0);

CREATE TABLE StandardFehler(
  id integer primary key,
  status integer not null default 1,
  beschreibung varchar(5000) default NULL,
  titel varchar(255) not null,
  kategorie_id integer not null,
  FOREIGN KEY (kategorie_id) REFERENCES Kategorie(id)
);
 
-- INDEX
 
-- TRIGGER
 
-- VIEW
 
-- DATA
INSERT INTO Lehrer (name, email, password, passwordGeaendert, administrator) VALUES('admin', 'tom.pauly.arbeit@gmail.com', 'c37a0ae3a5d46dba0c8f3c60fb036c8a0fb873101fc4de12f3cd0290d44f0668', 1, 1);
