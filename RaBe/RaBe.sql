BEGIN TRANSACTION;
CREATE TABLE StandardFehler (
	id	integer,
	status	integer NOT NULL DEFAULT 1,
	beschreibung	varchar(5000) DEFAULT NULL,
	titel	varchar(255) NOT NULL,
	kategorie_id	integer NOT NULL,
	FOREIGN KEY(kategorie_id) REFERENCES Kategorie(id),
	PRIMARY KEY(id)
);
CREATE TABLE Raum (
	id	integer,
	name	varchar(255) NOT NULL,
	vorlage	integer NOT NULL DEFAULT 0,
	PRIMARY KEY(id)
);
CREATE TABLE LehrerRaum (
	id	integer,
	betreuer	bit DEFAULT false,
	lehrer_id	integer NOT NULL,
	raum_id	integer NOT NULL,
	FOREIGN KEY(raum_id) REFERENCES Raum(id),
	PRIMARY KEY(id),
	FOREIGN KEY(lehrer_id) REFERENCES Lehrer(id)
);
CREATE TABLE Lehrer (
	id	integer,
	name	varchar(255) NOT NULL,
	email	varchar(255) NOT NULL,
	password	varchar(255) NOT NULL,
	passwordGeaendert bit NOT NULL DEFAULT false,
	blocked	bit NOT NULL DEFAULT false,
	administrator bit NOT NULL DEFAULT false,
	token	varchar(255) DEFAULT null,
	PRIMARY KEY(id)
);
CREATE TABLE Kategorie (
	id	integer,
	name	varchar(255) NOT NULL,
	PRIMARY KEY(id)
);
CREATE TABLE Fehler (
	id	integer,
	status	integer NOT NULL DEFAULT 1,
	beschreibung	varchar(5000) DEFAULT NULL,
	titel	varchar(255) NOT NULL,
	arbeitsplatz_id	integer NOT NULL,
	kategorie_id	integer NOT NULL,
	FOREIGN KEY(kategorie_id) REFERENCES Kategorie(id),
	FOREIGN KEY(arbeitsplatz_id) REFERENCES Arbeitsplatz(id),
	PRIMARY KEY(id)
);
CREATE TABLE Arbeitsplatz (
	id	integer,
	name	varchar(255) NOT NULL,
	raum_id	integer NOT NULL,
	position	integer NOT NULL DEFAULT 0,
	FOREIGN KEY(raum_id) REFERENCES Raum(id),
	PRIMARY KEY(id)
);
INSERT INTO StandardFehler (id,status,beschreibung,titel,kategorie_id) VALUES (1,1,'Alle Tasten der Tastatur wurden zu Z geändert','Z-Attack',1);
INSERT INTO Raum (id,name,vorlage) VALUES (1,'C017',1),
 (2,'UA11',4),
 (3,'C005',3),
 (4,'C004',2);
INSERT INTO LehrerRaum (id,betreuer,lehrer_id,raum_id) VALUES (1,1,1,1),
 (2,0,2,1);
INSERT INTO Lehrer (id,name,email,password,passwordGeaendert,blocked,administrator,token) VALUES (1,'admin','tom.pauly.arbeit@gmail.com','1dq3QDIPs0s39iVyBXDElKxACMsShTO2MmoeVEIX0nk=',1,0,1,'{alg:http://www.w3.org/2001/04/xmldsig-more#hmac-sha256,typ:JWT}.{id:1,email:tom.pauly.arbeit@gmail.com,name:admin,nbf:0,exp:0,iss:http://localhost:80,aud:GSO}'),
 (2,'Gerald Körperich','kh@gso-koeln.de','RBbtez1O3w6QVynyhy5xNfxARvqDQFSug1uN1lyeSZc=',1,0,1,''),
 (3,'Florian Larue','le@gso-koeln.de','Gr5vkLStQyFyh1dUdJnu2J81VEfk1bIEFrdersHINbs=',1,0,0,NULL),
 (4,'Kein Password','kp@gso-koeln.de','5YA7+gqdhKvYi6cA9yTHpJj53Gg2/nnofGXUaTH4gn4=',0,0,0,NULL);
INSERT INTO Kategorie (id,name) VALUES (1,'Tastatur'),
 (2,'Maus'),
 (3,'Monitor'),
 (4,'Hardware'),
 (5,'Software'),
 (6,'Beamer'),
 (7,'Sound'),
 (8,'AccessPoint'),
 (9,'Stream Media Adapter');
INSERT INTO Fehler (id,status,beschreibung,titel,arbeitsplatz_id,kategorie_id) VALUES (1,1,'Einige Tasten sind vertauscht','Tastaturtasten vertauscht',1,1),
 (2,2,'PC startet nicht','PC kaputt',12,4);
INSERT INTO Arbeitsplatz (id,name,raum_id,position) VALUES (1,'p-0g-0c017-0001',1,0),
 (2,'p-0g-0c017-0002',1,1),
 (3,'p-0g-0c017-0003',1,2),
 (4,'p-0g-0c017-0004',1,3),
 (5,'p-0g-0c017-0005',1,4),
 (6,'p-0g-0c017-0006',1,5),
 (7,'p-0g-0c017-0007',1,6),
 (8,'p-0g-0c017-0008',1,7),
 (9,'p-0g-0c017-0009',1,8),
 (10,'p-0g-0c017-0010',1,9),
 (11,'p-0g-0c017-0011',1,10),
 (12,'p-0g-c005-0001',3,1);
COMMIT;
