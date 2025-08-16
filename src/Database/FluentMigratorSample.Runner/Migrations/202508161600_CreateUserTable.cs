using FluentMigrator;

namespace FluentMigratorSample.Runner.Migrations;

[Migration(2025_08_16_16_00)]
public class CreateUserTable : Migration
{
    public override void Up()
    {
        Create.Table("Users");
    }

    public override void Down()
    {
        Delete.Table("Users");
    }
}