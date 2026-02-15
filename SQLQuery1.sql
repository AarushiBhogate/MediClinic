
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
('Arjun Mehta', '1997-03-12', 'Male', 'Pune', '9033333331', 'arjun@gmail.com', 'Routine Checkup', 'Active'),
('Sneha Kulkarni', '2000-11-25', 'Female', 'Nagpur', '9033333332', 'sneha@gmail.com', 'New Patient', 'Active');

INSERT INTO Chemist
(ChemistName, Address, Phone, Email, Summary, ChemistStatus)
VALUES
('Mahesh Patil', 'Pune', '9044444441', 'mahesh@pharmacy.com', 'Wholesale medicine dealer', 'Active');

INSERT INTO Drug
(DrugTitle, Description, Expiry, Dosage, DrugStatus)
VALUES
('Ibuprofen', 'Anti-inflammatory painkiller', '2027-01-15', '400mg', 'Active'),
('Cefixime', 'Antibiotic medicine', '2026-08-10', '200mg', 'Active'),
('Metformin', 'Diabetes control drug', '2027-04-22', '500mg', 'Active'),
('Losartan', 'Blood pressure medicine', '2026-12-05', '50mg', 'Active'),
('Cetirizine', 'Allergy relief', '2026-06-18', '10mg', 'Active');

INSERT INTO [User]
(UserName, [Password], Role, RoleReferenceID, Status)
VALUES
-- Admin
('systemadmin', 'admin@123', 'Admin', NULL, 'Active'),

-- Physicians
('drsuresh', 'doc@123', 'Physician', 1, 'Active'),
('drkavya', 'doc@123', 'Physician', 2, 'Active'),

-- Patients
('arjun.m', 'user@123', 'Patient', 1, 'Active'),
('sneha.k', 'user@123', 'Patient', 2, 'Active'),

-- Suppliers
('medlife', 'supp@123', 'Supplier', 1, 'Active'),
('careplus', 'supp@123', 'Supplier', 2, 'Active'),

-- Chemist
('mahesh.p', 'chem@123', 'Chemist', 1, 'Active');

INSERT INTO Appointment
(PatientID, AppointmentDate, Criticality, Reason, Note, ScheduleStatus)
VALUES
(1, GETDATE(), 'Low', 'Headache', 'MRI not required', 'Scheduled');
INSERT INTO Schedule
(PhysicianID, AppointmentID, ScheduleDate, ScheduleTime, ScheduleStatus)
VALUES
(1, 1, CAST(GETDATE() AS DATE), '11:00', 'Confirmed');

INSERT INTO PhysicianAdvice
(ScheduleID, Advice, Note)
VALUES
(1, 'Adequate sleep and hydration', 'Review after 5 days');

INSERT INTO PhysicianPrescrip
(PhysicianAdviceID, DrugID, Prescription, Dosage)
VALUES
(1, 1, 'After meals twice daily', '400mg'),
(1, 5, 'Once at night', '10mg'),
(1, 4, 'Morning dose', '50mg');

INSERT INTO DrugRequest
(PhysicianID, DrugsInfoText, RequestDate, RequestStatus)
VALUES
(1, 'Requesting medicines for neurology department', GETDATE(), 'Pending');

INSERT INTO PurchaseOrderHeader
(PONo, PODate, SupplierID)
VALUES
('PO-101', GETDATE(), 2);

INSERT INTO PurchaseProductLine
(POID, DrugID, SlNo, Qty, Note)
VALUES
(1, 2, 1, 250, 'Antibiotic stock'),
(1, 3, 2, 400, 'Diabetes medication'),
(1, 5, 3, 150, 'Allergy medicine');
CREATE TABLE PurchaseProductLine (
    POLineID INT IDENTITY PRIMARY KEY,
    POID INT,
    DrugID INT,
    SlNo INT,
    Qty INT,
    Note VARCHAR(255),

    CONSTRAINT FK_PurchaseLine_PO
    FOREIGN KEY (POID) REFERENCES PurchaseOrderHeader(POID),

    CONSTRAINT FK_PurchaseLine_Drug
    FOREIGN KEY (DrugID) REFERENCES Drug(DrugID)
);