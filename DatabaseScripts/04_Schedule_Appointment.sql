USE MediClinicDatabase;
GO

-- =========================================================
-- 7) DoctorSchedules
-- =========================================================
CREATE TABLE DoctorSchedules
(
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    DoctorId INT NOT NULL,
    DayOfWeek VARCHAR(15) NOT NULL,  -- Monday, Tuesday, etc.
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,

    CONSTRAINT FK_DoctorSchedules_Doctors
        FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),

    CONSTRAINT CK_DoctorSchedules_Time
        CHECK (EndTime > StartTime)
);
GO


-- =========================================================
-- 8) Appointments
-- =========================================================
CREATE TABLE Appointments
(
    AppointmentId INT IDENTITY(1,1) PRIMARY KEY,
    PatientId INT NOT NULL,
    DoctorId INT NOT NULL,
    AppointmentDate DATE NOT NULL,
    AppointmentTime TIME NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Requested',
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Appointments_Patients
        FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),

    CONSTRAINT FK_Appointments_Doctors
        FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),

    CONSTRAINT CK_Appointments_Status
        CHECK (Status IN ('Requested','Approved','Rejected','Completed'))
);
GO


-- SELECT * FROM DoctorSchedules;
-- SELECT * FROM Appointments;