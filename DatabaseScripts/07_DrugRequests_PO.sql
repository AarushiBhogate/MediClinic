USE MediClinicDatabase;
GO

-- =========================================================
-- 13) DrugRequests
-- =========================================================
CREATE TABLE DrugRequests
(
    DrugRequestId INT IDENTITY(1,1) PRIMARY KEY,
    DoctorId INT NOT NULL,
    DrugId INT NOT NULL,
    Quantity INT NOT NULL,
    RequestDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) NOT NULL DEFAULT 'Pending',

    CONSTRAINT FK_DrugRequests_Doctors
        FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),

    CONSTRAINT FK_DrugRequests_Drugs
        FOREIGN KEY (DrugId) REFERENCES Drugs(DrugId),

    CONSTRAINT CK_DrugRequests_Status
        CHECK (Status IN ('Pending','Approved','Ordered')),

    CONSTRAINT CK_DrugRequests_Qty
        CHECK (Quantity > 0)
);
GO


-- =========================================================
-- 14) PurchaseOrders
-- =========================================================
CREATE TABLE PurchaseOrders
(
    POId INT IDENTITY(1,1) PRIMARY KEY,
    SupplierId INT NOT NULL,
    RaisedByChemistId INT NOT NULL,
    PODate DATETIME NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) NOT NULL DEFAULT 'Pending',

    CONSTRAINT FK_PurchaseOrders_Suppliers
        FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId),

    CONSTRAINT FK_PurchaseOrders_Chemists
        FOREIGN KEY (RaisedByChemistId) REFERENCES Chemists(ChemistId),

    CONSTRAINT CK_PurchaseOrders_Status
        CHECK (Status IN ('Pending','Approved','Delivered'))
);
GO


-- =========================================================
-- 15) PurchaseOrderItems
-- =========================================================
CREATE TABLE PurchaseOrderItems
(
    POItemId INT IDENTITY(1,1) PRIMARY KEY,
    POId INT NOT NULL,
    DrugId INT NOT NULL,
    Quantity INT NOT NULL,
    PriceAtOrder DECIMAL(10,2) NOT NULL DEFAULT 0,

    CONSTRAINT FK_PurchaseOrderItems_PurchaseOrders
        FOREIGN KEY (POId) REFERENCES PurchaseOrders(POId),

    CONSTRAINT FK_PurchaseOrderItems_Drugs
        FOREIGN KEY (DrugId) REFERENCES Drugs(DrugId),

    CONSTRAINT CK_PurchaseOrderItems_Qty
        CHECK (Quantity > 0)
);
GO
