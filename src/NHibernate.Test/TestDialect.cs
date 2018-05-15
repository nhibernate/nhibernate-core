using System;
using NHibernate.Id;
using NHibernate.SqlTypes;

namespace NHibernate.Test
{
	/// <summary>
	/// Like NHibernate's Dialect class, but for differences only important during testing.
	/// Defaults to true for all support.  Users of different dialects can turn support
	/// off if the unit tests fail.
	/// </summary>
	public class TestDialect
	{
		public static TestDialect GetTestDialect(Dialect.Dialect dialect)
		{
			var testDialectTypeName = "NHibernate.Test.TestDialects." + dialect.GetType().Name.Replace("Dialect", "TestDialect");
			var testDialectType = System.Type.GetType(testDialectTypeName);
			if (testDialectType != null)
				return (TestDialect) Activator.CreateInstance(testDialectType, dialect);
			return new TestDialect(dialect);
		}

		readonly Dialect.Dialect _dialect;

		public TestDialect(Dialect.Dialect dialect)
		{
			_dialect = dialect;
		}

		/// <summary>
		/// Has a native generator strategy resolving to identity.
		/// </summary>
		public bool HasIdentityNativeGenerator
			=> _dialect.NativeIdentifierGeneratorClass == typeof(IdentityGenerator);

		public virtual bool SupportsOperatorAll => true;
		public virtual bool SupportsOperatorSome => true;
		public virtual bool SupportsLocate => true;

		public virtual bool SupportsFullJoin => true;

		/// <summary>
		/// Does the dialect lack a true handling of decimal?
		/// </summary>
		public virtual bool HasBrokenDecimalType => false;

		public virtual bool SupportsNullCharactersInUtfStrings => true;

		public virtual bool SupportsSelectForUpdateOnOuterJoin => true;

		public virtual bool SupportsHavingWithoutGroupBy => true;

		public virtual bool SupportsComplexExpressionInGroupBy => true;

		public virtual bool SupportsCountDistinct => true;

		public virtual bool SupportsOrderByAggregate => true;

		public virtual bool SupportsOrderByColumnNumber => true;

		public virtual bool SupportsDuplicatedColumnAliases => true;

		/// <summary>
		/// Supports inserting in a table without any column specified in the insert.
		/// </summary>
		public virtual bool SupportsEmptyInserts => true;

		/// <summary>
		/// Either supports inserting in a table without any column specified in the insert, or has a native
		/// generator strategy resolving to something else than identity.
		/// </summary>
		/// <remarks>This property is useful for cases where empty inserts happens only when the entities
		/// generator is <c>native</c> while the dialect uses <c>identity</c> for this generator.</remarks>
		public bool SupportsEmptyInsertsOrHasNonIdentityNativeGenerator
			=> SupportsEmptyInserts || !HasIdentityNativeGenerator;


		/// <summary>
		/// Supports condition not bound to any data, like "where @p1 = @p2".
		/// </summary>
		public virtual bool SupportsNonDataBoundCondition => true;

		public bool SupportsSqlType(SqlType sqlType)
		{
			try
			{
				_dialect.GetTypeName(sqlType);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Supports the modulo operator on decimal types
		/// </summary>
		public virtual bool SupportsModuloOnDecimal => true;

		/// <summary>
		/// Supports aggregating sub-selects in order by clause
		/// </summary>
		public virtual bool SupportsAggregatingScalarSubSelectsInOrderBy => _dialect.SupportsScalarSubSelects;

		/// <summary>
		/// Supports order by and limits/top in correlated sub-queries
		/// </summary>
		public virtual bool SupportsOrderByAndLimitInSubQueries => true;

		/// <summary>
		/// Supports selecting a double literal.
		/// </summary>
		public virtual bool SupportsSelectingDoubleLiteral => true;

		/// <summary>
		/// Supports foreign keys on composite keys including a boolean column.
		/// </summary>
		public virtual bool SupportsFKOnCompositeKeyWithBoolean => true;

		/// <summary>
		/// Supports tests involving concurrency.
		/// </summary>
		public virtual bool SupportsConcurrencyTests => true;

		/// <summary>
		/// Supports batching together inserts/updates/Delets among which some depends (auto foreign key) on others
		/// in the batch.
		/// </summary>
		public virtual bool SupportsBatchingDependentDML => true;
	}
}
