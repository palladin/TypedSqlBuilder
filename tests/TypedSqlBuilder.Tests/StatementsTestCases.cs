using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.Tests;


public static class StatementsTestCases
{
    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InsertBasic_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "InsertBasic_GeneratesCorrectSql"), ("""
        INSERT INTO [customers] (
            [Id],
            [Age],
            [Name]
        )
        VALUES 
            (@p0, @p1, @p2)
        """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "InsertBasic_GeneratesCorrectSql"), ("""
        INSERT INTO "customers" (
            "Id",
            "Age",
            "Name"
        )
        VALUES 
            (:p0, :p1, :p2)
        """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "InsertBasic_GeneratesCorrectSql"), ("""
        INSERT INTO "customers" (
            "Id",
            "Age",
            "Name"
        )
        VALUES 
            (:p0, :p1, :p2)
        """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DeleteAll_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "DeleteAll_GeneratesCorrectSql"), ("""
        DELETE FROM [customers]
        """, [])),
        new((DatabaseType.SQLite, "DeleteAll_GeneratesCorrectSql"), ("""
        DELETE FROM "customers"
        """, [])),
        new((DatabaseType.PostgreSQL, "DeleteAll_GeneratesCorrectSql"), ("""
        DELETE FROM "customers"
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DeleteBasic_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "DeleteBasic_GeneratesCorrectSql"), ("""
        DELETE FROM [customers]
        WHERE 
            [customers].[Id] = @p0
        """, ["@p0"])),
        new((DatabaseType.SQLite, "DeleteBasic_GeneratesCorrectSql"), ("""
        DELETE FROM "customers"
        WHERE 
            "customers"."Id" = :p0
        """, [":p0"])),
        new((DatabaseType.PostgreSQL, "DeleteBasic_GeneratesCorrectSql"), ("""
        DELETE FROM "customers"
        WHERE 
            "customers"."Id" = :p0
        """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateBasic_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateBasic_GeneratesCorrectSql"), ("""
        UPDATE [customers]
        SET 
            [Age] = @p0
        WHERE 
            [customers].[Id] = @p1
        """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "UpdateBasic_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Age" = :p0
        WHERE 
            "customers"."Id" = :p1
        """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "UpdateBasic_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Age" = :p0
        WHERE 
            "customers"."Id" = :p1
        """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] DeleteConditional_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "DeleteConditional_GeneratesCorrectSql"), ("""
        DELETE FROM [customers]
        WHERE 
            [customers].[Age] < @p0 OR [customers].[Name] = @p1
        """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "DeleteConditional_GeneratesCorrectSql"), ("""
        DELETE FROM "customers"
        WHERE 
            "customers"."Age" < :p0 OR "customers"."Name" = :p1
        """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "DeleteConditional_GeneratesCorrectSql"), ("""
        DELETE FROM "customers"
        WHERE 
            "customers"."Age" < :p0 OR "customers"."Name" = :p1
        """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InsertWithNewColumnsNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "InsertWithNewColumnsNull_GeneratesCorrectSql"), ("""
        INSERT INTO [products] (
            [Id],
            [ProductName],
            [Price],
            [CreatedDate],
            [UniqueId]
        )
        VALUES 
            (@p0, @p1, NULL, NULL, NULL)
        """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "InsertWithNewColumnsNull_GeneratesCorrectSql"), ("""
        INSERT INTO "products" (
            "Id",
            "ProductName",
            "Price",
            "CreatedDate",
            "UniqueId"
        )
        VALUES 
            (:p0, :p1, NULL, NULL, NULL)
        """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "InsertWithNewColumnsNull_GeneratesCorrectSql"), ("""
        INSERT INTO "products" (
            "Id",
            "ProductName",
            "Price",
            "CreatedDate",
            "UniqueId"
        )
        VALUES 
            (:p0, :p1, NULL, NULL, NULL)
        """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InsertWithNewColumns_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "InsertWithNewColumns_GeneratesCorrectSql"), ("""
        INSERT INTO [products] (
            [Id],
            [ProductName],
            [Price],
            [CreatedDate],
            [UniqueId]
        )
        VALUES 
            (@p0, @p1, @p2, @p3, @p4)
        """, ["@p0", "@p1", "@p2", "@p3", "@p4"])),
        new((DatabaseType.SQLite, "InsertWithNewColumns_GeneratesCorrectSql"), ("""
        INSERT INTO "products" (
            "Id",
            "ProductName",
            "Price",
            "CreatedDate",
            "UniqueId"
        )
        VALUES 
            (:p0, :p1, :p2, :p3, :p4)
        """, [":p0", ":p1", ":p2", ":p3", ":p4"])),
        new((DatabaseType.PostgreSQL, "InsertWithNewColumns_GeneratesCorrectSql"), ("""
        INSERT INTO "products" (
            "Id",
            "ProductName",
            "Price",
            "CreatedDate",
            "UniqueId"
        )
        VALUES 
            (:p0, :p1, :p2, :p3, :p4)
        """, [":p0", ":p1", ":p2", ":p3", ":p4"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InsertWithNullInt_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "InsertWithNullInt_GeneratesCorrectSql"), ("""
        INSERT INTO [customers] (
            [Id],
            [Name],
            [Age]
        )
        VALUES 
            (@p0, @p1, NULL)
        """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "InsertWithNullInt_GeneratesCorrectSql"), ("""
        INSERT INTO "customers" (
            "Id",
            "Name",
            "Age"
        )
        VALUES 
            (:p0, :p1, NULL)
        """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "InsertWithNullInt_GeneratesCorrectSql"), ("""
        INSERT INTO "customers" (
            "Id",
            "Name",
            "Age"
        )
        VALUES 
            (:p0, :p1, NULL)
        """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] InsertWithNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "InsertWithNull_GeneratesCorrectSql"), ("""
        INSERT INTO [customers] (
            [Id],
            [Name],
            [Age]
        )
        VALUES 
            (@p0, NULL, @p1)
        """, ["@p0", "@p1"])),
        new((DatabaseType.SQLite, "InsertWithNull_GeneratesCorrectSql"), ("""
        INSERT INTO "customers" (
            "Id",
            "Name",
            "Age"
        )
        VALUES 
            (:p0, NULL, :p1)
        """, [":p0", ":p1"])),
        new((DatabaseType.PostgreSQL, "InsertWithNull_GeneratesCorrectSql"), ("""
        INSERT INTO "customers" (
            "Id",
            "Name",
            "Age"
        )
        VALUES 
            (:p0, NULL, :p1)
        """, [":p0", ":p1"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateConditional_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateConditional_GeneratesCorrectSql"), ("""
        UPDATE [customers]
        SET 
            [Age] = [customers].[Age] + @p0
        WHERE 
            [customers].[Age] >= @p1 AND [customers].[Name] != @p2
        """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "UpdateConditional_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Age" = "customers"."Age" + :p0
        WHERE 
            "customers"."Age" >= :p1 AND "customers"."Name" != :p2
        """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "UpdateConditional_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Age" = "customers"."Age" + :p0
        WHERE 
            "customers"."Age" >= :p1 AND "customers"."Name" != :p2
        """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateMultiple_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateMultiple_GeneratesCorrectSql"), ("""
        UPDATE [customers]
        SET 
            [Age] = @p0,
            [Name] = @p1
        WHERE 
            [customers].[Id] = @p2
        """, ["@p0", "@p1", "@p2"])),
        new((DatabaseType.SQLite, "UpdateMultiple_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Age" = :p0,
            "Name" = :p1
        WHERE 
            "customers"."Id" = :p2
        """, [":p0", ":p1", ":p2"])),
        new((DatabaseType.PostgreSQL, "UpdateMultiple_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Age" = :p0,
            "Name" = :p1
        WHERE 
            "customers"."Id" = :p2
        """, [":p0", ":p1", ":p2"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateSetNewColumnsNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateSetNewColumnsNull_GeneratesCorrectSql"), ("""
        UPDATE [products]
        SET 
            [Price] = NULL,
            [CreatedDate] = NULL,
            [UniqueId] = NULL
        WHERE 
            [products].[Id] = @p0
        """, ["@p0"])),
        new((DatabaseType.SQLite, "UpdateSetNewColumnsNull_GeneratesCorrectSql"), ("""
        UPDATE "products"
        SET 
            "Price" = NULL,
            "CreatedDate" = NULL,
            "UniqueId" = NULL
        WHERE 
            "products"."Id" = :p0
        """, [":p0"])),
        new((DatabaseType.PostgreSQL, "UpdateSetNewColumnsNull_GeneratesCorrectSql"), ("""
        UPDATE "products"
        SET 
            "Price" = NULL,
            "CreatedDate" = NULL,
            "UniqueId" = NULL
        WHERE 
            "products"."Id" = :p0
        """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateSetNullInt_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateSetNullInt_GeneratesCorrectSql"), ("""
        UPDATE [customers]
        SET 
            [Age] = NULL
        """, [])),
        new((DatabaseType.SQLite, "UpdateSetNullInt_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Age" = NULL
        """, [])),
        new((DatabaseType.PostgreSQL, "UpdateSetNullInt_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Age" = NULL
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateSetNullMixed_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateSetNullMixed_GeneratesCorrectSql"), ("""
        UPDATE [customers]
        SET 
            [Name] = @p0,
            [Age] = NULL
        """, ["@p0"])),
        new((DatabaseType.SQLite, "UpdateSetNullMixed_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Name" = :p0,
            "Age" = NULL
        """, [":p0"])),
        new((DatabaseType.PostgreSQL, "UpdateSetNullMixed_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Name" = :p0,
            "Age" = NULL
        """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateSetNullWhere_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateSetNullWhere_GeneratesCorrectSql"), ("""
        UPDATE [customers]
        SET 
            [Name] = NULL
        WHERE 
            [customers].[Id] = @p0
        """, ["@p0"])),
        new((DatabaseType.SQLite, "UpdateSetNullWhere_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Name" = NULL
        WHERE 
            "customers"."Id" = :p0
        """, [":p0"])),
        new((DatabaseType.PostgreSQL, "UpdateSetNullWhere_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Name" = NULL
        WHERE 
            "customers"."Id" = :p0
        """, [":p0"]))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateSetNull_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateSetNull_GeneratesCorrectSql"), ("""
        UPDATE [customers]
        SET 
            [Name] = NULL
        """, [])),
        new((DatabaseType.SQLite, "UpdateSetNull_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Name" = NULL
        """, [])),
        new((DatabaseType.PostgreSQL, "UpdateSetNull_GeneratesCorrectSql"), ("""
        UPDATE "customers"
        SET 
            "Name" = NULL
        """, []))
    ];

    private static readonly KeyValuePair<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)>[] UpdateWithNewColumns_GeneratesCorrectSqlPair =
    [
        new((DatabaseType.SqlServer, "UpdateWithNewColumns_GeneratesCorrectSql"), ("""
        UPDATE [products]
        SET 
            [Price] = @p0,
            [CreatedDate] = @p1,
            [UniqueId] = @p2
        WHERE 
            [products].[Id] = @p3
        """, ["@p0", "@p1", "@p2", "@p3"])),
        new((DatabaseType.SQLite, "UpdateWithNewColumns_GeneratesCorrectSql"), ("""
        UPDATE "products"
        SET 
            "Price" = :p0,
            "CreatedDate" = :p1,
            "UniqueId" = :p2
        WHERE 
            "products"."Id" = :p3
        """, [":p0", ":p1", ":p2", ":p3"])),
        new((DatabaseType.PostgreSQL, "UpdateWithNewColumns_GeneratesCorrectSql"), ("""
        UPDATE "products"
        SET 
            "Price" = :p0,
            "CreatedDate" = :p1,
            "UniqueId" = :p2
        WHERE 
            "products"."Id" = :p3
        """, [":p0", ":p1", ":p2", ":p3"]))
    ];

    public static readonly Dictionary<(DatabaseType, string TestName), (string Sql, string[] ParameterNames)> TestCases =

            InsertBasic_GeneratesCorrectSqlPair
            .Concat(DeleteAll_GeneratesCorrectSqlPair)
            .Concat(DeleteBasic_GeneratesCorrectSqlPair)
            .Concat(UpdateBasic_GeneratesCorrectSqlPair)
            .Concat(DeleteConditional_GeneratesCorrectSqlPair)
            .Concat(InsertWithNewColumnsNull_GeneratesCorrectSqlPair)
            .Concat(InsertWithNewColumns_GeneratesCorrectSqlPair)
            .Concat(InsertWithNullInt_GeneratesCorrectSqlPair)
            .Concat(InsertWithNull_GeneratesCorrectSqlPair)
            .Concat(UpdateConditional_GeneratesCorrectSqlPair)
            .Concat(UpdateMultiple_GeneratesCorrectSqlPair)
            .Concat(UpdateSetNewColumnsNull_GeneratesCorrectSqlPair)
            .Concat(UpdateSetNullInt_GeneratesCorrectSqlPair)
            .Concat(UpdateSetNullMixed_GeneratesCorrectSqlPair)
            .Concat(UpdateSetNullWhere_GeneratesCorrectSqlPair)
            .Concat(UpdateSetNull_GeneratesCorrectSqlPair)
            .Concat(UpdateWithNewColumns_GeneratesCorrectSqlPair)
            .ToDictionary(kv => kv.Key, kv => kv.Value);


}

