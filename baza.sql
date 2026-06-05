-- Rekonstrukcija baze na osnovu klasa iz projekta i GenericBroker-a.
-- Napomena:
-- 1. Redosled kolona je namerno postavljen tako da odgovara INSERT pozivima
--    koji ne navode listu kolona.
-- 2. Naziv baze je uzet iz BrokerBazePodataka/GenericBroker.cs:
--    Initial Catalog=chatapp_db

IF DB_ID(N'chatapp_db') IS NULL
BEGIN
    CREATE DATABASE [chatapp_db];
END
GO

USE [chatapp_db];
GO

IF OBJECT_ID(N'dbo.Poruka', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Poruka;
END
GO

IF OBJECT_ID(N'dbo.prijateljstvo', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.prijateljstvo;
END
GO

IF OBJECT_ID(N'dbo.Korisnik', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Korisnik;
END
GO

CREATE TABLE dbo.Korisnik
(
    Id INT IDENTITY(1,1) NOT NULL,
    korisnicko_ime NVARCHAR(100) NOT NULL,
    lozinka NVARCHAR(255) NOT NULL,

    CONSTRAINT PK_Korisnik PRIMARY KEY (Id),
    CONSTRAINT UQ_Korisnik_korisnicko_ime UNIQUE (korisnicko_ime)
);
GO

CREATE TABLE dbo.prijateljstvo
(
    id_prijateljstva INT IDENTITY(1,1) NOT NULL,
    korisnik1_id INT NOT NULL,
    korisnik2_id INT NOT NULL,
    status NVARCHAR(20) NOT NULL,

    CONSTRAINT PK_prijateljstvo PRIMARY KEY (id_prijateljstva),
    CONSTRAINT FK_prijateljstvo_korisnik1 FOREIGN KEY (korisnik1_id)
        REFERENCES dbo.Korisnik(Id),
    CONSTRAINT FK_prijateljstvo_korisnik2 FOREIGN KEY (korisnik2_id)
        REFERENCES dbo.Korisnik(Id),
    CONSTRAINT CK_prijateljstvo_status
        CHECK (status IN (N'ceka se', N'prihvacen')),
    CONSTRAINT CK_prijateljstvo_razliciti_korisnici
        CHECK (korisnik1_id <> korisnik2_id)
);
GO

CREATE TABLE dbo.Poruka
(
    Id INT IDENTITY(1,1) NOT NULL,
    primalac INT NOT NULL,
    posiljalac INT NOT NULL,
    poruka_text NVARCHAR(MAX) NOT NULL,
    datum DATETIME2(0) NOT NULL,

    CONSTRAINT PK_Poruka PRIMARY KEY (Id),
    CONSTRAINT FK_Poruka_primalac FOREIGN KEY (primalac)
        REFERENCES dbo.Korisnik(Id),
    CONSTRAINT FK_Poruka_posiljalac FOREIGN KEY (posiljalac)
        REFERENCES dbo.Korisnik(Id)
);
GO

CREATE INDEX IX_Poruka_Primalac_Posiljalac_Datum
    ON dbo.Poruka(primalac, posiljalac, datum);
GO

CREATE INDEX IX_Poruka_Posiljalac_Primalac_Datum
    ON dbo.Poruka(posiljalac, primalac, datum);
GO

CREATE INDEX IX_Prijateljstvo_Korisnik1_Status
    ON dbo.prijateljstvo(korisnik1_id, status);
GO

CREATE INDEX IX_Prijateljstvo_Korisnik2_Status
    ON dbo.prijateljstvo(korisnik2_id, status);
GO

-- Opcioni test podaci:
-- INSERT INTO dbo.Korisnik VALUES (N'domagoj', N'domagoj');
-- INSERT INTO dbo.Korisnik VALUES (N'marko', N'123');
-- INSERT INTO dbo.prijateljstvo VALUES (1, 2, N'prihvacen');
-- INSERT INTO dbo.Poruka VALUES (2, 1, N'Zdravo', '2026-05-21 12:00:00');
