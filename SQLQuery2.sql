CREATE PROCEDURE SP_UpdatePatientProfile
(
    @PatientID INT,
    @PatientName VARCHAR(100),
    @DOB DATE,
    @Gender VARCHAR(10),
    @Address VARCHAR(200),
    @Phone VARCHAR(15),
    @Email VARCHAR(100),
    @Allergies VARCHAR(500),
    @ChronicDiseases VARCHAR(500),
    @PastIllnesses VARCHAR(500),
    @Notes VARCHAR(1000)
)
AS
BEGIN
    UPDATE Patient
    SET 
        PatientName = @PatientName,
        DOB = @DOB,
        Gender = @Gender,
        Address = @Address,
        Phone = @Phone,
        Email = @Email,
        Allergies = @Allergies,
        ChronicDiseases = @ChronicDiseases,
        PastIllnesses = @PastIllnesses,
        Notes = @Notes
    WHERE PatientID = @PatientID
END
