USE MediClinicDatabase;
GO

-- =========================================================
-- 3) Patients
-- =========================================================
CREATE TABLE Patients
(
    PatientId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    FullName VARCHAR(120) NOT NULL,
    DOB DATE NULL,
    Gender VARCHAR(10) NULL,
    Phone VARCHAR(20) NULL,
    Address VARCHAR(250) NULL,
    IsVerified BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Patients_Users
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO


-- =========================================================
-- 4) Doctors
-- =========================================================
CREATE TABLE Doctors
(
    DoctorId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    FullName VARCHAR(120) NOT NULL,
    Specialization VARCHAR(100) NULL,
    Phone VARCHAR(20) NULL,

    CONSTRAINT FK_Doctors_Users
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO


-- =========================================================
-- 5) Chemists
-- =========================================================
CREATE TABLE Chemists
(
    ChemistId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    FullName VARCHAR(120) NOT NULL,
    Phone VARCHAR(20) NULL,

    CONSTRAINT FK_Chemists_Users
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO


-- =========================================================
-- 6) Suppliers
-- =========================================================
CREATE TABLE Suppliers
(
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL UNIQUE,
    CompanyName VARCHAR(150) NOT NULL,
    Phone VARCHAR(20) NULL,

    CONSTRAINT FK_Suppliers_Users
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO


-- SELECT * FROM Patients;
-- SELECT * FROM Doctors;
-- SELECT * FROM Chemists;
-- SELECT * FROM Suppliers;