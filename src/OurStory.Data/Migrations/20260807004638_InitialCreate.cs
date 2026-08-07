using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurStory.Data.Migrations {
    /// <inheritdoc />
    public partial class InitialCreate : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            _ = migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new {
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table => {
                    _ = table.PrimaryKey("PK_settings", x => x.Key);
                });

            _ = migrationBuilder.CreateTable(
                name: "users",
                columns: table => new {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false, collation: "NOCASE"),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    LastLoginAt = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table => {
                    _ = table.PrimaryKey("PK_users", x => x.Id);
                });

            _ = migrationBuilder.CreateTable(
                name: "heartbeats",
                columns: table => new {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    VisitorHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    ClickDay = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false)
                },
                constraints: table => {
                    _ = table.PrimaryKey("PK_heartbeats", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_heartbeats_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            _ = migrationBuilder.CreateTable(
                name: "moments",
                columns: table => new {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    ContentHtml = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    CoverUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Mood = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    MomentDate = table.Column<long>(type: "INTEGER", nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowComment = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table => {
                    _ = table.PrimaryKey("PK_moments", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_moments_users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            _ = migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MomentId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: true),
                    AuthorName = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    AuthorMail = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    AuthorUrl = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IsApproved = table.Column<bool>(type: "INTEGER", nullable: false),
                    VisitorHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table => {
                    _ = table.PrimaryKey("PK_comments", x => x.Id);
                    _ = table.ForeignKey(
                        name: "FK_comments_comments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    _ = table.ForeignKey(
                        name: "FK_comments_moments_MomentId",
                        column: x => x.MomentId,
                        principalTable: "moments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    _ = table.ForeignKey(
                        name: "FK_comments_users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            _ = migrationBuilder.CreateIndex(
                name: "IX_comments_AuthorId",
                table: "comments",
                column: "AuthorId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_comments_MomentId_CreatedAt",
                table: "comments",
                columns: ["MomentId", "CreatedAt"]);

            _ = migrationBuilder.CreateIndex(
                name: "IX_comments_ParentId",
                table: "comments",
                column: "ParentId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_heartbeats_Role_ClickDay",
                table: "heartbeats",
                columns: ["Role", "ClickDay"]);

            _ = migrationBuilder.CreateIndex(
                name: "IX_heartbeats_UserId_ClickDay",
                table: "heartbeats",
                columns: ["UserId", "ClickDay"]);

            _ = migrationBuilder.CreateIndex(
                name: "IX_heartbeats_VisitorHash_ClickDay",
                table: "heartbeats",
                columns: ["VisitorHash", "ClickDay"]);

            _ = migrationBuilder.CreateIndex(
                name: "IX_moments_AuthorId",
                table: "moments",
                column: "AuthorId");

            _ = migrationBuilder.CreateIndex(
                name: "IX_moments_Slug",
                table: "moments",
                column: "Slug",
                unique: true);

            _ = migrationBuilder.CreateIndex(
                name: "IX_moments_Status_MomentDate",
                table: "moments",
                columns: ["Status", "MomentDate"]);

            _ = migrationBuilder.CreateIndex(
                name: "IX_users_UserName",
                table: "users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            _ = migrationBuilder.DropTable(
                name: "comments");

            _ = migrationBuilder.DropTable(
                name: "heartbeats");

            _ = migrationBuilder.DropTable(
                name: "settings");

            _ = migrationBuilder.DropTable(
                name: "moments");

            _ = migrationBuilder.DropTable(
                name: "users");
        }
    }
}
