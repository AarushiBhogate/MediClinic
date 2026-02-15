using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediClinic.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chemist",
                columns: table => new
                {
                    ChemistID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChemistName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Summary = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    ChemistStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Chemist__C0D5B7B4C293BF6D", x => x.ChemistID);
                });

            migrationBuilder.CreateTable(
                name: "Drug",
                columns: table => new
                {
                    DrugID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DrugTitle = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Expiry = table.Column<DateOnly>(type: "date", nullable: true),
                    Dosage = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    DrugStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Drug__908D66F624B6FE4D", x => x.DrugID);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderUserId = table.Column<int>(type: "int", nullable: false),
                    ReceiverUserId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    PatientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    DOB = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Address = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Summary = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    PatientStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Patient__970EC34688E81DE8", x => x.PatientID);
                });

            migrationBuilder.CreateTable(
                name: "Physician",
                columns: table => new
                {
                    PhysicianID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicianName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Summary = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    PhysicianStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Physicia__DFF5ED73B26EE8C2", x => x.PhysicianID);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    SupplierStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Supplier__4BE666949E56E28B", x => x.SupplierID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    RoleReferenceID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__1788CCACC63841C7", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    AppointmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientID = table.Column<int>(type: "int", nullable: true),
                    AppointmentDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Criticality = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Reason = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    RequiredSpecialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduleStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Appointm__8ECDFCA22DB0FA67", x => x.AppointmentID);
                    table.ForeignKey(
                        name: "FK_Appointment_Patient",
                        column: x => x.PatientID,
                        principalTable: "Patient",
                        principalColumn: "PatientID");
                });

            migrationBuilder.CreateTable(
                name: "DrugRequest",
                columns: table => new
                {
                    DrugRequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicianID = table.Column<int>(type: "int", nullable: true),
                    DrugsInfoText = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RequestStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DrugRequ__AEE9D65075240E95", x => x.DrugRequestID);
                    table.ForeignKey(
                        name: "FK_DrugRequest_Physician",
                        column: x => x.PhysicianID,
                        principalTable: "Physician",
                        principalColumn: "PhysicianID");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderHeader",
                columns: table => new
                {
                    POID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PONo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PODate = table.Column<DateTime>(type: "datetime", nullable: true),
                    SupplierID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__5F02A2F47E24C1E8", x => x.POID);
                    table.ForeignKey(
                        name: "FK_PO_Supplier",
                        column: x => x.SupplierID,
                        principalTable: "Supplier",
                        principalColumn: "SupplierID");
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    ScheduleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicianID = table.Column<int>(type: "int", nullable: true),
                    AppointmentID = table.Column<int>(type: "int", nullable: true),
                    ScheduleDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ScheduleTime = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    ScheduleStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__9C8A5B69A01FC4E3", x => x.ScheduleID);
                    table.ForeignKey(
                        name: "FK_Schedule_Appointment",
                        column: x => x.AppointmentID,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentID");
                    table.ForeignKey(
                        name: "FK_Schedule_Physician",
                        column: x => x.PhysicianID,
                        principalTable: "Physician",
                        principalColumn: "PhysicianID");
                });

            migrationBuilder.CreateTable(
                name: "PurchaseProductLine",
                columns: table => new
                {
                    POLineID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POID = table.Column<int>(type: "int", nullable: true),
                    DrugID = table.Column<int>(type: "int", nullable: true),
                    SlNo = table.Column<int>(type: "int", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Purchase__07B9D3425619C648", x => x.POLineID);
                    table.ForeignKey(
                        name: "FK_PurchaseLine_Drug",
                        column: x => x.DrugID,
                        principalTable: "Drug",
                        principalColumn: "DrugID");
                    table.ForeignKey(
                        name: "FK_PurchaseLine_PO",
                        column: x => x.POID,
                        principalTable: "PurchaseOrderHeader",
                        principalColumn: "POID");
                });

            migrationBuilder.CreateTable(
                name: "PhysicianAdvice",
                columns: table => new
                {
                    PhysicianAdviceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleID = table.Column<int>(type: "int", nullable: true),
                    Advice = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    Note = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Physicia__82C62610A3F67EE2", x => x.PhysicianAdviceID);
                    table.ForeignKey(
                        name: "FK_PhysicianAdvice_Schedule",
                        column: x => x.ScheduleID,
                        principalTable: "Schedule",
                        principalColumn: "ScheduleID");
                });

            migrationBuilder.CreateTable(
                name: "PhysicianPrescrip",
                columns: table => new
                {
                    PrescriptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhysicianAdviceID = table.Column<int>(type: "int", nullable: true),
                    DrugID = table.Column<int>(type: "int", nullable: true),
                    Prescription = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Dosage = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Physicia__40130812C544648D", x => x.PrescriptionID);
                    table.ForeignKey(
                        name: "FK_Prescrip_Advice",
                        column: x => x.PhysicianAdviceID,
                        principalTable: "PhysicianAdvice",
                        principalColumn: "PhysicianAdviceID");
                    table.ForeignKey(
                        name: "FK_Prescrip_Drug",
                        column: x => x.DrugID,
                        principalTable: "Drug",
                        principalColumn: "DrugID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientID",
                table: "Appointment",
                column: "PatientID");

            migrationBuilder.CreateIndex(
                name: "IX_DrugRequest_PhysicianID",
                table: "DrugRequest",
                column: "PhysicianID");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicianAdvice_ScheduleID",
                table: "PhysicianAdvice",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicianPrescrip_DrugID",
                table: "PhysicianPrescrip",
                column: "DrugID");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicianPrescrip_PhysicianAdviceID",
                table: "PhysicianPrescrip",
                column: "PhysicianAdviceID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeader_SupplierID",
                table: "PurchaseOrderHeader",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseProductLine_DrugID",
                table: "PurchaseProductLine",
                column: "DrugID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseProductLine_POID",
                table: "PurchaseProductLine",
                column: "POID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_AppointmentID",
                table: "Schedule",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_PhysicianID",
                table: "Schedule",
                column: "PhysicianID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chemist");

            migrationBuilder.DropTable(
                name: "DrugRequest");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "PhysicianPrescrip");

            migrationBuilder.DropTable(
                name: "PurchaseProductLine");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "PhysicianAdvice");

            migrationBuilder.DropTable(
                name: "Drug");

            migrationBuilder.DropTable(
                name: "PurchaseOrderHeader");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Physician");

            migrationBuilder.DropTable(
                name: "Patient");
        }
    }
}
