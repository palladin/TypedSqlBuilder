using System.Threading.Tasks;
using TypedSqlBuilder.Core;

namespace TypedSqlBuilder.TestModels;

/// <summary>
/// Complete contract interface ensuring consistent query test coverage across all database dialects
/// This represents all 87 query test methods that must be implemented by all dialect-specific test classes
/// Uses Task return type to support both sync (unit tests) and async (integration tests) implementations
/// </summary>
public interface IQueryTestContract
{ 
    Task From_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromStatic_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectSingle_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereInt_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereString_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereMultiple_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOrderByAsc_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOrderByDesc_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereSelectOrderBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereOr_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectExpression_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereOrderBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereSelectNamed_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereFusionTwo_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereFusionThree_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereFusionWithSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereFusionWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOrderByThenBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOrderByThenByDescending_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOrderByDescendingThenBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOrderByMultiple_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereOrderByThenBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOrderByThenBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereIsNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereIsNotNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereIsNullCombined_GeneratesCorrectSql(DatabaseType databaseType);
    Task SumAges_GeneratesCorrectSql(DatabaseType databaseType);
    Task CountCustomers_GeneratesCorrectSql(DatabaseType databaseType);
    Task CountActiveCustomers_GeneratesCorrectSql(DatabaseType databaseType);    
    Task FromWhereAgeGreaterThanSum_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereAgeGreaterThanAverageAge_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereAgeIn_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereAgeInSubquery_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereAgeInSubqueryWithClosure_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSubquery_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereSelectWhereFromNested_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereSelectWhereNested_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByMultipleSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByHavingSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereGroupBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereIsNullInt_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereIsNotNullInt_GeneratesCorrectSql(DatabaseType databaseType);
    Task InnerJoinBasic_GeneratesCorrectSql(DatabaseType databaseType);
    Task InnerJoinWithSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task InnerJoinWithWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task InnerJoinWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task LeftJoinBasic_GeneratesCorrectSql(DatabaseType databaseType);
    Task LeftJoinWithSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task LeftJoinWithWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task LeftJoinWithOrderBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task InnerJoinWithGroupBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task LeftJoinWithAggregates_GeneratesCorrectSql(DatabaseType databaseType);
    Task MultipleInnerJoinsFusion_GeneratesCorrectSql(DatabaseType databaseType);
    Task MixedJoinTypesFusion_GeneratesCorrectSql(DatabaseType databaseType);
    Task JoinFusionWithWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByOrderByMultipleSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByOrderByThreeKeysSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByMultipleOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByHavingOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task ComplexJoinWhereGroupByHavingOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task ComplexLeftJoinWhereGroupByOrderBySelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByMinMaxSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByAvgSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectSum_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectAvg_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectMin_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectMax_GeneratesCorrectSql(DatabaseType databaseType);
    
    // Decimal Aggregate Functions
    Task SumPrices_GeneratesCorrectSql(DatabaseType databaseType);
    Task AvgPrices_GeneratesCorrectSql(DatabaseType databaseType);
    Task MinPrice_GeneratesCorrectSql(DatabaseType databaseType);
    Task MaxPrice_GeneratesCorrectSql(DatabaseType databaseType);
    Task SumExpensivePrices_GeneratesCorrectSql(DatabaseType databaseType);
    Task AvgExpensivePrices_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByDecimalAggregatesSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByDecimalSumSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromGroupByDecimalAvgSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task ParameterAsIntParam_GeneratesCorrectSql(DatabaseType databaseType);
    Task ParameterAsStringParam_GeneratesCorrectSql(DatabaseType databaseType);
    Task ParameterAsBoolParam_GeneratesCorrectSql(DatabaseType databaseType);
    Task BoolColumnDirectComparison_GeneratesCorrectSql(DatabaseType databaseType);
    Task BoolColumnLiteralTrue_GeneratesCorrectSql(DatabaseType databaseType);
    Task BoolColumnLiteralFalse_GeneratesCorrectSql(DatabaseType databaseType);    
    Task FromProductWhereSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectOrderBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereAnd_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereAndSelect_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereSelectParameterized_GeneratesCorrectSql(DatabaseType databaseType);
    Task CaseStringExpression_GeneratesCorrectSql(DatabaseType databaseType);
    Task CaseIntExpression_GeneratesCorrectSql(DatabaseType databaseType);
    Task CaseBoolExpression_GeneratesCorrectSql(DatabaseType databaseType);
    Task CaseInWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task LikeWildcard_GeneratesCorrectSql(DatabaseType databaseType);
    Task LikeSingleChar_GeneratesCorrectSql(DatabaseType databaseType);
    Task LikeBothWildcards_GeneratesCorrectSql(DatabaseType databaseType);
    Task LikeExact_GeneratesCorrectSql(DatabaseType databaseType);
    Task AbsColumn_GeneratesCorrectSql(DatabaseType databaseType);
    Task AbsInWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task AbsExpression_GeneratesCorrectSql(DatabaseType databaseType);
    Task AbsParameter_GeneratesCorrectSql(DatabaseType databaseType);
    
    // New column types tests - Decimal
    Task FromWhereDecimalComparison_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectDecimalArithmetic_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereDecimalIsNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereDecimalIsNotNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task CaseDecimalExpression_GeneratesCorrectSql(DatabaseType databaseType);
    Task ParameterAsDecimalParam_GeneratesCorrectSql(DatabaseType databaseType);
    
    // New column types tests - DateTime
    Task FromWhereCreatedDateComparison_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereCreatedDateIsNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereCreatedDateIsNotNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectCreatedDateMinMax_GeneratesCorrectSql(DatabaseType databaseType);
    Task CaseDateTimeExpression_GeneratesCorrectSql(DatabaseType databaseType);
    Task ParameterAsDateTimeParam_GeneratesCorrectSql(DatabaseType databaseType);
    
    // New column types tests - Guid
    Task FromWhereUniqueIdEquals_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereUniqueIdNotEquals_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereUniqueIdIsNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereUniqueIdIsNotNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task CaseGuidExpression_GeneratesCorrectSql(DatabaseType databaseType);
    Task ParameterAsGuidParam_GeneratesCorrectSql(DatabaseType databaseType);

    // String Functions Tests
    Task StringSubstring_GeneratesCorrectSql(DatabaseType databaseType);
    Task StringUpper_GeneratesCorrectSql(DatabaseType databaseType);
    Task StringLower_GeneratesCorrectSql(DatabaseType databaseType);
    Task StringTrim_GeneratesCorrectSql(DatabaseType databaseType);
    Task StringLength_GeneratesCorrectSql(DatabaseType databaseType);
    Task StringFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task StringFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType);

    // Date/Time Functions Tests
    Task DateTimeNow_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeYear_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeMonth_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeDay_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeAddDays_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeAddMonths_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeAddYears_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeDiffDays_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeDiffMonths_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeDiffYears_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task DateTimeFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType);

    // Mathematical Functions Tests
    Task DecimalRound_GeneratesCorrectSql(DatabaseType databaseType);
    Task DecimalCeiling_GeneratesCorrectSql(DatabaseType databaseType);
    Task DecimalFloor_GeneratesCorrectSql(DatabaseType databaseType);
    Task MathFunctionsInWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task MathFunctionsInSelect_GeneratesCorrectSql(DatabaseType databaseType);

    // LimitOffset Tests
    Task FromLimitOffset_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereLimitOffset_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereSelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOrderByLimitOffset_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereOrderByLimitOffset_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromWhereOrderBySelectLimitOffset_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromLimitOffsetOnly_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromOffsetOnly_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromLimitOffsetWithoutOrderBy_GeneratesCorrectSql(DatabaseType databaseType);
    
    // DISTINCT Tests
    Task FromSelectDistinct_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectDistinctWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectDistinctOrderBy_GeneratesCorrectSql(DatabaseType databaseType);
    Task FromSelectDistinctMultipleColumns_GeneratesCorrectSql(DatabaseType databaseType);
}


/// <summary>
/// Complete contract interface ensuring consistent statement test coverage across all database dialects
/// This represents all statement test methods that must be implemented by all dialect-specific test classes
/// Uses Task return type to support both sync (unit tests) and async (integration tests) implementations
/// </summary>
public interface IStatementTestContract
{
    // All statement test methods from SQL Server (baseline)
    Task InsertBasic_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateBasic_GeneratesCorrectSql(DatabaseType databaseType);
    Task DeleteBasic_GeneratesCorrectSql(DatabaseType databaseType);
    Task DeleteAll_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateConditional_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateMultiple_GeneratesCorrectSql(DatabaseType databaseType);
    Task DeleteConditional_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateSetNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateSetNullMixed_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateSetNullInt_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateSetNullWhere_GeneratesCorrectSql(DatabaseType databaseType);
    Task InsertWithNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task InsertWithNullInt_GeneratesCorrectSql(DatabaseType databaseType);

    // New column types statement tests
    Task InsertWithNewColumns_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateWithNewColumns_GeneratesCorrectSql(DatabaseType databaseType);
    Task InsertWithNewColumnsNull_GeneratesCorrectSql(DatabaseType databaseType);
    Task UpdateSetNewColumnsNull_GeneratesCorrectSql(DatabaseType databaseType);
}



/// <summary>
/// Contract interface for SQL Server-specific dialect features
/// Uses Task return type to support both sync (unit tests) and async (integration tests) implementations
/// </summary>
public interface ISqlServerDialectTestContract
{
    // SQL Server-specific tests

}

/// <summary>
/// Contract interface for SQLite-specific dialect features
/// Uses Task return type to support both sync (unit tests) and async (integration tests) implementations
/// </summary>
public interface ISqliteDialectTestContract
{
    // SQLite-specific tests

}

/// <summary>
/// Contract interface for PostgreSQL-specific dialect features
/// </summary>
public interface IPostgreSqlDialectTestContract
{
    // PostgreSQL-specific tests can be added here as needed
}
