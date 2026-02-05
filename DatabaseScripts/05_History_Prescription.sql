USE MediClinicDatabase;
GO

-- =========================================================
-- 9) PatientHistory
-- =========================================================
CREATE TABLE PatientHistory
(
    HistoryId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    VisitDate DATETIME NOT NULL DEFAULT GETDATE(),
    Diagnosis VARCHAR(500) NULL,
    Notes VARCHAR(1000) NULL,

    CONSTRAINT FK_PatientHistory_Patients
        FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),

    CONSTRAINT FK_PatientHistory_Doctors
        FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId)
);
GO


-- =========================================================
-- 10) Prescriptions
-- =========================================================
CREATE TABLE Prescriptions
(
    PrescriptionId INT IDENTITY(1,1) PRIMARY KEY,
    HistoryId INT NOT NULL UNIQUE, -- 1 prescription per history row (simple)
    AdviceText VARCHAR(2000) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Prescriptions_PatientHistory
        FOREIGN KEY (HistoryId) REFERENCES PatientHistory(HistoryId)
);
GO
