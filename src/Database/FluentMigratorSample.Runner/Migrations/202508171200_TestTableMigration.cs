using FluentMigrator;

namespace FluentMigratorSample.Runner.Migrations;

[Migration(2025_08_17_12_00)]
public class TestTableMigration: Migration
{
    public override void Up()
    {
        Create.Table("TestTable")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(255).NotNullable().WithDefaultValue("Anonymous");

        Create.Table("TestTable2")
            .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
            .WithColumn("Name").AsString(255).Nullable()
            .WithColumn("TestTableId").AsInt32().NotNullable();

        Create.Index("ix_Name").OnTable("TestTable2").OnColumn("Name").Ascending()
            .WithOptions().NonClustered();

        Create.Column("Name2").OnTable("TestTable2").AsBoolean().Nullable();

        Create.ForeignKey("fk_TestTable2_TestTableId_TestTable_Id")
            .FromTable("TestTable2").ForeignColumn("TestTableId")
            .ToTable("TestTable").PrimaryColumn("Id");

        Insert.IntoTable("TestTable").Row(new { Name = "Test" });
    }

    public override void Down()
    {
        Delete.Table("TestTable2");
        Delete.Table("TestTable");
    }
}