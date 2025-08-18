using System.Threading.Tasks;

namespace TypedSqlBuilder.TestModels;

/// <summary>
/// Complete contract interface ensuring consistent query test coverage across all database dialects
/// This represents all 87 query test methods that must be implemented by all dialect-specific test classes
/// Uses Task return type to support both sync (unit tests) and async (integration tests) implementations
/// </summary>
public interface IQueryTestContract
{
    // All 87 query test methods from SQL Server (baseline)
    Task From_GeneratesCorrectSql();
    Task FromStatic_GeneratesCorrectSql();
    Task FromSelect_GeneratesCorrectSql();
    Task FromSelectSingle_GeneratesCorrectSql();
    Task FromWhereInt_GeneratesCorrectSql();
    Task FromWhereString_GeneratesCorrectSql();
    Task FromWhereMultiple_GeneratesCorrectSql();
    Task FromOrderByAsc_GeneratesCorrectSql();
    Task FromOrderByDesc_GeneratesCorrectSql();
    Task FromWhereSelectOrderBy_GeneratesCorrectSql();
    Task FromWhereSelect_GeneratesCorrectSql();
    Task FromWhereOr_GeneratesCorrectSql();
    Task FromSelectExpression_GeneratesCorrectSql();
    Task FromWhereOrderBy_GeneratesCorrectSql();
    Task FromWhereOrderBySelect_GeneratesCorrectSql();
    Task FromWhereSelectNamed_GeneratesCorrectSql();
    Task FromWhereFusionTwo_GeneratesCorrectSql();
    Task FromWhereFusionThree_GeneratesCorrectSql();
    Task FromWhereFusionWithSelect_GeneratesCorrectSql();
    Task FromWhereFusionWithOrderBy_GeneratesCorrectSql();
    Task FromOrderByThenBy_GeneratesCorrectSql();
    Task FromOrderByThenByDescending_GeneratesCorrectSql();
    Task FromOrderByDescendingThenBy_GeneratesCorrectSql();
    Task FromOrderByMultiple_GeneratesCorrectSql();
    Task FromWhereOrderByThenBy_GeneratesCorrectSql();
    Task FromOrderByThenBySelect_GeneratesCorrectSql();
    Task FromWhereIsNull_GeneratesCorrectSql();
    Task FromWhereIsNotNull_GeneratesCorrectSql();
    Task FromWhereIsNullCombined_GeneratesCorrectSql();
    Task SumAges_GeneratesCorrectSql();
    Task CountCustomers_GeneratesCorrectSql();
    Task CountActiveCustomers_GeneratesCorrectSql();
    Task SumAgesWithDb_GeneratesCorrectSql();
    Task CountCustomersWithDb_GeneratesCorrectSql();
    Task CountActiveCustomersWithDb_GeneratesCorrectSql();
    Task FromWhereAgeGreaterThanSum_GeneratesCorrectSql();
    Task FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql();
    Task FromWhereAgeIn_GeneratesCorrectSql();
    Task FromWhereAgeInSubquery_GeneratesCorrectSql();
    Task FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql();
    Task FromSubquery_GeneratesCorrectSql();
    Task FromWhereSelectWhereFromNested_GeneratesCorrectSql();
    Task FromWhereSelectWhereNested_GeneratesCorrectSql();
    Task FromGroupBySelect_GeneratesCorrectSql();
    Task FromGroupByMultipleSelect_GeneratesCorrectSql();
    Task FromGroupByHavingSelect_GeneratesCorrectSql();
    Task FromWhereGroupBySelect_GeneratesCorrectSql();
    Task FromWhereIsNullInt_GeneratesCorrectSql();
    Task FromWhereIsNotNullInt_GeneratesCorrectSql();
    Task InnerJoinBasic_GeneratesCorrectSql();
    Task InnerJoinWithSelect_GeneratesCorrectSql();
    Task InnerJoinWithWhere_GeneratesCorrectSql();
    Task InnerJoinWithOrderBy_GeneratesCorrectSql();
    Task LeftJoinBasic_GeneratesCorrectSql();
    Task LeftJoinWithSelect_GeneratesCorrectSql();
    Task LeftJoinWithWhere_GeneratesCorrectSql();
    Task LeftJoinWithOrderBy_GeneratesCorrectSql();
    Task InnerJoinWithGroupBy_GeneratesCorrectSql();
    Task LeftJoinWithAggregates_GeneratesCorrectSql();
    Task MultipleInnerJoinsFusion_GeneratesCorrectSql();
    Task MixedJoinTypesFusion_GeneratesCorrectSql();
    Task JoinFusionWithWhere_GeneratesCorrectSql();
    Task FromGroupByOrderBySelect_GeneratesCorrectSql();
    Task FromGroupByOrderByMultipleSelect_GeneratesCorrectSql();
    Task FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql();
    Task FromGroupByMultipleOrderBySelect_GeneratesCorrectSql();
    Task FromGroupByHavingOrderBySelect_GeneratesCorrectSql();
    Task ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql();
    Task ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql();
    Task FromGroupByMinMaxSelect_GeneratesCorrectSql();
    Task FromGroupByAvgSelect_GeneratesCorrectSql();
    Task FromSelectSum_GeneratesCorrectSql();
    Task FromSelectAvg_GeneratesCorrectSql();
    Task FromSelectMin_GeneratesCorrectSql();
    Task FromSelectMax_GeneratesCorrectSql();
    Task ParameterAsIntParam_GeneratesCorrectSql();
    Task ParameterAsStringParam_GeneratesCorrectSql();
    Task ParameterAsBoolParam_GeneratesCorrectSql();
    Task BoolColumnDirectComparison_GeneratesCorrectSql();
    Task BoolColumnLiteralTrue_GeneratesCorrectSql();
    Task BoolColumnLiteralFalse_GeneratesCorrectSql();
    Task FromOrderByMultipleOrderBySelect_GeneratesCorrectSql();
    Task FromProductWhereSelect_GeneratesCorrectSql();
    Task FromSelectOrderBy_GeneratesCorrectSql();
    Task FromWhereAnd_GeneratesCorrectSql();
    Task FromWhereAndSelect_GeneratesCorrectSql();
    Task FromWhereSelectParameterized_GeneratesCorrectSql();
    Task CaseStringExpression_GeneratesCorrectSql();
    Task CaseIntExpression_GeneratesCorrectSql();
    Task CaseBoolExpression_GeneratesCorrectSql();
    Task CaseInWhere_GeneratesCorrectSql();
    Task LikeWildcard_GeneratesCorrectSql();
    Task LikeSingleChar_GeneratesCorrectSql();
    Task LikeBothWildcards_GeneratesCorrectSql();
    Task LikeExact_GeneratesCorrectSql();
    Task AbsColumn_GeneratesCorrectSql();
    Task AbsInWhere_GeneratesCorrectSql();
    Task AbsExpression_GeneratesCorrectSql();
    Task AbsParameter_GeneratesCorrectSql();
    
    // New column types tests - Decimal
    Task FromWhereDecimalComparison_GeneratesCorrectSql();
    Task FromSelectDecimalArithmetic_GeneratesCorrectSql();
    Task FromWhereDecimalIsNull_GeneratesCorrectSql();
    Task FromWhereDecimalIsNotNull_GeneratesCorrectSql();
    Task CaseDecimalExpression_GeneratesCorrectSql();
    Task ParameterAsDecimalParam_GeneratesCorrectSql();
    
    // New column types tests - DateTime
    Task FromWhereCreatedDateComparison_GeneratesCorrectSql();
    Task FromWhereCreatedDateIsNull_GeneratesCorrectSql();
    Task FromWhereCreatedDateIsNotNull_GeneratesCorrectSql();
    Task FromSelectCreatedDateMinMax_GeneratesCorrectSql();
    Task CaseDateTimeExpression_GeneratesCorrectSql();
    Task ParameterAsDateTimeParam_GeneratesCorrectSql();
    
    // New column types tests - Guid
    Task FromWhereUniqueIdEquals_GeneratesCorrectSql();
    Task FromWhereUniqueIdNotEquals_GeneratesCorrectSql();
    Task FromWhereUniqueIdIsNull_GeneratesCorrectSql();
    Task FromWhereUniqueIdIsNotNull_GeneratesCorrectSql();
    Task CaseGuidExpression_GeneratesCorrectSql();
    Task ParameterAsGuidParam_GeneratesCorrectSql();
}

/// <summary>
/// Complete contract interface ensuring consistent statement test coverage across all database dialects
/// This represents all 14 statement test methods that must be implemented by all dialect-specific test classes
/// Uses Task return type to support both sync (unit tests) and async (integration tests) implementations
/// </summary>
public interface IStatementTestContract
{
    // All 14 statement test methods from SQL Server (baseline)
    Task InsertBasic_GeneratesCorrectSql();
    Task UpdateBasic_GeneratesCorrectSql();
    Task DeleteBasic_GeneratesCorrectSql();
    Task DeleteAll_GeneratesCorrectSql();
    Task UpdateConditional_GeneratesCorrectSql();
    Task InsertPartial_GeneratesCorrectSql();
    Task UpdateMultiple_GeneratesCorrectSql();
    Task DeleteConditional_GeneratesCorrectSql();
    Task UpdateSetNull_GeneratesCorrectSql();
    Task UpdateSetNullMixed_GeneratesCorrectSql();
    Task UpdateSetNullInt_GeneratesCorrectSql();
    Task UpdateSetNullWhere_GeneratesCorrectSql();
    Task InsertWithNull_GeneratesCorrectSql();
    Task InsertWithNullInt_GeneratesCorrectSql();
    
    // New column types statement tests
    Task InsertWithNewColumns_GeneratesCorrectSql();
    Task UpdateWithNewColumns_GeneratesCorrectSql();
    Task InsertWithNewColumnsNull_GeneratesCorrectSql();
    Task UpdateSetNewColumnsNull_GeneratesCorrectSql();
}

/// <summary>
/// Contract interface for SQL Server-specific dialect features
/// Uses Task return type to support both sync (unit tests) and async (integration tests) implementations
/// </summary>
public interface ISqlServerDialectTestContract
{
    // SQL Server-specific tests
    Task SqlServer_UsesAtSymbolPrefix();
}

/// <summary>
/// Contract interface for SQLite-specific dialect features
/// Uses Task return type to support both sync (unit tests) and async (integration tests) implementations
/// </summary>
public interface ISqliteDialectTestContract
{
    // SQLite-specific tests
    Task Sqlite_UsesColonPrefix();
}

/// <summary>
/// Contract interface for PostgreSQL-specific dialect features
/// </summary>
public interface IPostgreSqlDialectTestContract
{
    // PostgreSQL-specific tests can be added here as needed
}
