using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zoo.Infrastructure.Migrations
{
    public partial class UpdateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Previously this migration dropped the generated junction table "AnimalKeeper",
            // which removed the relationship table created by the AddKeepers migration.
            // Keep Up() empty to avoid deleting the automatically-generated junction table.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // У Down нічого не виконуємо або можна відновити стан за потреби
        }
    }
}
