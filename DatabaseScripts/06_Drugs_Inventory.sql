USE MediClinicDatabase;
GO

-- =========================================================
-- 11) Drugs
-- =========================================================
CREATE TABLE Drugs
(
    DrugId INT IDENTITY(1,1) PRIMARY KEY,
    DrugName VARCHAR(150) NOT NULL,
    Description VARCHAR(500) NULL,
    Price DECIMAL(10,2) NOT NULL DEFAULT 0
);
GO


-- =========================================================
-- 12) Inventory
-- =========================================================
CREATE TABLE Inventory
(
    InventoryId INT IDENTITY(1,1) PRIMARY KEY,
    DrugId INT NOT NULL UNIQUE,
    AvailableQty INT NOT NULL DEFAULT 0,
    ReorderLevel INT NOT NULL DEFAULT 10,

    CONSTRAINT FK_Inventory_Drugs
        FOREIGN KEY (DrugId) REFERENCES Drugs(DrugId),

    CONSTRAINT CK_Inventory_Qty
        CHECK (AvailableQty >= 0 AND ReorderLevel >= 0)
);
GO
