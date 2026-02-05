USE MediClinicDatabase;
GO

CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(200) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    RoleId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);
GO


INSERT INTO Users (Username, PasswordHash, Email, RoleId)
VALUES ('admin', 'admin123', 'admin@gmail.com', 1);
GO

SELECT * FROM Users;
GO
