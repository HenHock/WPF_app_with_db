create database  Seabattle

use Seabattle

create table Classes(Cod_cl int primary key,
	 Name_cl varchar(30), 
	 Type_cl char(2),
	 Country varchar(20), 
	 Numguns int,
	 bore int, 
	 displacement int
 )

 Create table Ship( Cod_ships int primary key,
	 name varchar(20),
	 cod_cl int foreign key references Classes(Cod_cl) ON DELETE CASCADE, 
	 launched int,
	 imgShip VARBINARY(MAX) DEFAULT NULL
 )
 go
 
 create table Battles( cod_battles int primary key, name varchar (20) , datas date)
 go

 create table Outcomes(
 Cod_ships int foreign key references Ship(Cod_ships ) ON DELETE CASCADE, 
cod_batles int foreign key references Battles(cod_battles) ON DELETE CASCADE, 
 Result varchar(10))
 go 

 CREATE TABLE Users(
	idUser INT PRIMARY KEY,
	loginUser VARCHAR NOT NULL,
	passwordUser VARCHAR NOT NULL,
	emailUser VARCHAR NOT NULL,
	isAdmin BIT
 )

INSERT Classes Values(1,'Bismarck', 'bb', 'Germany', 8, 15, 42000)
INSERT Classes Values(2, 'Iowa', 'bb', 'USA', 9, 16, 46000)
INSERT Classes Values(3, 'Kongo', 'bc', 'Japan', 8, 14, 32000)
INSERT Classes Values(4, 'North California', 'bb', 'USA', 9, 16, 37000)
INSERT Classes Values(5, 'Renown', 'bc', 'Gt.Britain', 6, 15, 32000)
INSERT Classes Values(6, 'Revenge', 'bb', 'Gr.Britain', 8, 15, 29000)
INSERT Classes Values(7, 'Tennessee', 'bb', 'USA', 12, 14, 32000)
INSERT Classes Values(8, 'Yamato', 'bb', 'Japan', 9, 18, 65000)

 INSERT Ship VALUES(1,'California',7, 1921, NULL)
 INSERT Ship VALUES(2,'Haruna',3, 1915, NULL)
 INSERT Ship VALUES(3, 'Hiei',3, 1914, NULL)
 INSERT Ship VALUES(4, 'Iowa',2, 1943, NULL)
 INSERT Ship VALUES(5,'Kirishima',3, 1915, NULL)
 INSERT Ship VALUES(6, 'Kongo',3, 1913, NULL)
 INSERT Ship VALUES(7, 'Missouri',2, 1944, NULL)
 INSERT Ship VALUES(8, 'Musashi',8, 1942, NULL)
 INSERT Ship VALUES(9, 'New Jersey',2, 1943, NULL)
 INSERT Ship VALUES(10, 'North Carolina',4, 1941, NULL)
 INSERT Ship VALUES(11, 'Ramillies',6, 1917, NULL)
 INSERT Ship VALUES(12, 'Renown',5, 1916, NULL)
 INSERT Ship VALUES(13, 'Repulse',5, 1916, NULL)
 INSERT Ship VALUES(14, 'Resolution',6, 1916, NULL)
 INSERT Ship VALUES(15, 'Revenge',6, 1916, NULL)
 INSERT Ship VALUES(16, 'Royal Oak',6, 1916, NULL)
 INSERT Ship VALUES(17, 'Royal Sovereign',6, 1916, NULL)
 INSERT Ship VALUES(18, 'Tennessee',7, 1920, NULL)
 INSERT Ship VALUES(19, 'Washington',4, 1941, NULL)
 INSERT Ship VALUES(20, 'Wisconsin',2, 1924, NULL)
 INSERT Ship VALUES(21, 'Yamato',8, 1941, NULL)
 INSERT Ship VALUES(22, 'Bismarck', 1, 1939, NULL)

INSERT Battles VALUES(1, 'North Atlantic', '5-24-41')
INSERT Battles VALUES(2, 'Guadalcanal', '11-15-42')
INSERT Battles VALUES(3, 'North Cape', '12-26-43')
INSERT Battles VALUES(4, 'Surigao Strait', '10-25-44')

INSERT Outcomes VALUES (22, 1, ' Sunk')
INSERT Outcomes VALUES (1, 4, 'Ok')
INSERT Outcomes VALUES(11, 3, 'Ok')
INSERT Outcomes VALUES(8, 4, 'Sunk')
INSERT Outcomes VALUES(12, 1, 'Sunk')
INSERT Outcomes VALUES(16, 1, 'Ok')
INSERT Outcomes VALUES(2, 2, 'Sunk')
INSERT Outcomes VALUES(17, 1, 'Damaged')
INSERT Outcomes VALUES(15, 1, 'Ok')
INSERT Outcomes VALUES(11, 3, 'Sunk')
INSERT Outcomes VALUES(20, 2, 'Damaged')
INSERT Outcomes VALUES(18, 4, 'Ok')
INSERT Outcomes VALUES(19, 2, 'Ok')
INSERT Outcomes VALUES(10, 4, 'Ok')
INSERT Outcomes VALUES(21, 4, 'Sunk')

INSERT Users VALUES(0,1,1,1,1)
INSERT Users VALUES(1, 2, 2, 2, 0)

SELECT * FROM Outcomes

BACKUP DATABASE SeaBattle TO DISK = N'C:\Users\User\source\repos\HenHock' WITH NOFORMAT, NOINIT, NAME = N'SeaBattle-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10BACKUP DATABASE SeaBattle TO DISK = N'" + FilePath + "' WITH NOFORMAT, NOINIT, NAME = N'SeaBattle-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10