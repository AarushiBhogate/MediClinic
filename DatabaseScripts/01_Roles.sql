USE MediClinicDatabase;
GO

CREATE TABLE Roles
(
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE
);
GO

INSERT INTO Roles(RoleName)
VALUES 
('Admin'),
('Doctor'),
('Patient'),
('Chemist'),
('Supplier');
GO

SELECT * FROM Roles;
GO
