
CREATE TABLE Physician (
    PhysicianID INT IDENTITY PRIMARY KEY,
    PhysicianName VARCHAR(100) NOT NULL,
    Specialization VARCHAR(100),
    Address VARCHAR(200),
    Phone VARCHAR(15),
    Email VARCHAR(100),
    Summary VARCHAR(255),
    PhysicianStatus VARCHAR(20)
);

CREATE TABLE Supplier (
    SupplierID INT IDENTITY PRIMARY KEY,
    SupplierName VARCHAR(100),
    Address VARCHAR(200),
    Phone VARCHAR(15),
    Email VARCHAR(100),
    SupplierStatus VARCHAR(20)
);


CREATE TABLE Patient (
    PatientID INT IDENTITY PRIMARY KEY,
    PatientName VARCHAR(100),
    DOB DATE,
    Gender VARCHAR(10),
    Address VARCHAR(200),
    Phone VARCHAR(15),
    Email VARCHAR(100),
    Summary VARCHAR(255),
    PatientStatus VARCHAR(20)
);


CREATE TABLE Chemist (
    ChemistID INT IDENTITY PRIMARY KEY,
    ChemistName VARCHAR(100),
    Address VARCHAR(200),
    Phone VARCHAR(15),
    Email VARCHAR(100),
    Summary VARCHAR(255),
    ChemistStatus VARCHAR(20)
);


CREATE TABLE Drug (
    DrugID INT IDENTITY PRIMARY KEY,
    DrugTitle VARCHAR(100),
    Description VARCHAR(255),
    Expiry DATE,
    Dosage VARCHAR(50),
    DrugStatus VARCHAR(20)
);


CREATE TABLE [User] (
    UserID INT IDENTITY PRIMARY KEY,
    UserName VARCHAR(50),
    [Password] VARCHAR(100),
    Role VARCHAR(20),
    RoleReferenceID INT NULL,
    Status VARCHAR(20)
);

CREATE TABLE Appointment (
    AppointmentID INT IDENTITY PRIMARY KEY,
    PatientID INT,
    AppointmentDate DATETIME,
    Criticality VARCHAR(20),
    Reason VARCHAR(255),
    Note VARCHAR(255),
    ScheduleStatus VARCHAR(20),

    CONSTRAINT FK_Appointment_Patient
    FOREIGN KEY (PatientID) REFERENCES Patient(PatientID)
);


CREATE TABLE Schedule (
    ScheduleID INT IDENTITY PRIMARY KEY,
    PhysicianID INT,
    AppointmentID INT,
    ScheduleDate DATE,
    ScheduleTime VARCHAR(10),
    ScheduleStatus VARCHAR(20),

    CONSTRAINT FK_Schedule_Physician
    FOREIGN KEY (PhysicianID) REFERENCES Physician(PhysicianID),

    CONSTRAINT FK_Schedule_Appointment
    FOREIGN KEY (AppointmentID) REFERENCES Appointment(AppointmentID)
);
CREATE TABLE PhysicianAdvice (
    PhysicianAdviceID INT IDENTITY PRIMARY KEY,
    ScheduleID INT,
    Advice VARCHAR(500),
    Note VARCHAR(255),

    CONSTRAINT FK_PhysicianAdvice_Schedule
    FOREIGN KEY (ScheduleID) REFERENCES Schedule(ScheduleID)
);


CREATE TABLE PhysicianPrescrip (
    PrescriptionID INT IDENTITY PRIMARY KEY,
    PhysicianAdviceID INT,
    DrugID INT,
    Prescription VARCHAR(255),
    Dosage VARCHAR(50),

    CONSTRAINT FK_Prescrip_Advice
    FOREIGN KEY (PhysicianAdviceID) REFERENCES PhysicianAdvice(PhysicianAdviceID),

    CONSTRAINT FK_Prescrip_Drug
    FOREIGN KEY (DrugID) REFERENCES Drug(DrugID)
);


CREATE TABLE DrugRequest (
    DrugRequestID INT IDENTITY PRIMARY KEY,
    PhysicianID INT,
    DrugsInfoText VARCHAR(MAX),
    RequestDate DATETIME,
    RequestStatus VARCHAR(20),

    CONSTRAINT FK_DrugRequest_Physician
    FOREIGN KEY (PhysicianID) REFERENCES Physician(PhysicianID)
);


CREATE TABLE PurchaseOrderHeader (
    POID INT IDENTITY PRIMARY KEY,
    PONo VARCHAR(50),
    PODate DATETIME,
    SupplierID INT,

    CONSTRAINT FK_PO_Supplier
    FOREIGN KEY (SupplierID) REFERENCES Supplier(SupplierID)
);

INSERT INTO Physician
(PhysicianName, Specialization, Address, Phone, Email, Summary, PhysicianStatus)
VALUES
('Dr. Suresh Rao', 'Neurology', 'Bangalore', '9011111111', 'suresh@medicure.com', 'Neuro Specialist', 'Active'),
('Dr. Kavya Nair', 'Dermatology', 'Kochi', '9011111112', 'kavya@medicure.com', 'Skin Care Expert', 'Active');

INSERT INTO Supplier
(SupplierName, Address, Phone, Email, SupplierStatus)
VALUES
('MedLife Distributors', 'Hyderabad', '9022222221', 'contact@medlife.com', 'Active'),
('CarePlus Pharma', 'Chennai', '9022222222', 'sales@careplus.com', 'Active');

INSERT INTO Patient
(PatientName, DOB, Gender, Address, Phone, Email, Summary, PatientStatus)
VALUES
