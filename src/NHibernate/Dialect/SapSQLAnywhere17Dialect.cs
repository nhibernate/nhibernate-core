using System;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.SqlCommand;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <inheritdoc />
	/// <remarks>
	/// The SapSQLAnywhere17Dialect uses the SybaseSQLAnywhere12Dialect as its
	/// base class.
	/// The dialect defaults the following configuration properties:
	/// <list type="table">
	/// 	<listheader>
	/// 		<term>Property</term>
	/// 		<description>Default Value</description>
	/// 	</listheader>
	/// 	<item>
	/// 		<term>connection.driver_class</term>
	/// 		<description><see cref="T:NHibernate.Driver.SapSQLAnywhere17Driver" /></description>
	/// 	</item>
	/// 	<item>
	/// 		<term>prepare_sql</term>
	/// 		<description><see langword="false" /></description>
	/// 	</item>
	/// </list>
	/// </remarks>
	public class SapSQLAnywhere17Dialect : SybaseSQLAnywhere12Dialect
	{
		public SapSQLAnywhere17Dialect()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SapSQLAnywhere17Driver";
		}

		#region SQL Anywhere 17 additional keywords

		private static readonly string[] DialectKeywords =
		{
			"datetimex"
			"executing",
			"executing_user",
			"extract",
			"invoking",
			"invoking_user",
			"pivot",
			"procedure_owner",
			"session_user",
			"unpivot"
		};

		#endregion

		protected override void RegisterKeywords()
		{
			base.RegisterKeywords();

			RegisterKeywords(DialectKeywords);
		}

		protected override void RegisterStringFunctions()
		{
			base.RegisterStringFunctions();

			// SQL Anywhere locate arguments are inverted compared to other databases. As fixing this is likely
			// a breaking change for users of older versions, changing it only in the new dialect.
			RegisterFunction("locate", new SQLFunctionTemplateWithRequiredParameters(NHibernateUtil.Int32, "locate(?2, ?1, ?3)", new object[] { null, null, "1" }));

			RegisterFunction("strguid", new SQLFunctionTemplate(NHibernateUtil.String, "cast(?1 as char(36))"));
		}

		protected override void RegisterMathFunctions()
		{
			base.RegisterMathFunctions();

			// While SQL AnyWhere always returns double for these functions, Linq requires them to return
			// the argument type. This is done by not hard-coding their return type in the function. In
			// such case, NHibernate takes care of converting the result back to the argument type, if
			// needed.
			// Hard-coding the return type should be done for functions where returning the argument type
			// does not make sense, like on string length, where returning a string instead of an int would
			// not make sense.
			// Unfortunately, removing this hard coding from SybaseSQLAnywhere10Dialect would be a breaking
			// change for those having coded HQL queries on projections of these functions with a hard-cast
			// to double. So changing it only for this new dialect.
			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));
		}

		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);

			RegisterConfigurationDependentFunctions();
		}

		protected virtual void RegisterConfigurationDependentFunctions()
		{
			// The SQL function str is in the SQL standard and is not meant for casting anything to a string. It is
			// a float formatting function. Unfortunately, HQL's str is used as a general purpose string cast. Previous
			// SQL Anywhere dialects are mapping it as the SQL standard's str function. For avoiding a breaking change
			// for them, it is fixed only in this new dialect.
			RegisterFunction("str", new SQLFunctionTemplate(NHibernateUtil.String, $"cast(?1 as nvarchar({DefaultCastLength}))"));
		}

		/// <inheritdoc />
		/// <remarks>
		/// SQL Anywhere does not supports <c>null</c> in unique constraints. As this disable generation of unique
		/// constraints when a column is nullable, even if the application never put <c>null</c> in it, it could
		/// be a breaking change. So this property is overriden to <c>false</c> only in this new 17 dialect.
		/// </remarks>
		public override bool SupportsNullInUnique => false;

		public override SqlString GetLimitString(SqlString sql, SqlString offset, SqlString limit)
		{
			// SQL Anywhere uses SELECT TOP n START AT m [ select list items ]
			// for LIMIT/OFFSET support.  Does not support a limit of zero.

			var insertionPoint = GetAfterSelectInsertPoint(sql);

			if (insertionPoint > 0)
			{
				if (limit == null && offset == null)
					throw new ArgumentException("Cannot limit with neither a limit nor an offset");

				var limitBuilder = new SqlStringBuilder();
				limitBuilder.Add("select");
				if (insertionPoint > 6)
				{
					limitBuilder.Add(" distinct ");
				}
				limitBuilder.Add(" top ");
				if (limit != null)
				{
					limitBuilder.Add(limit);
				}
				else
				{
					// This seems supported since SQL Anywhere 12.0.1 only. No reference found for previous version,
					// included 12.0.0.
					limitBuilder.Add("all ");
				}
				if (offset != null)
				{
					limitBuilder.Add(" start at ");
					limitBuilder.Add(offset);
				}
				limitBuilder.Add(sql.Substring(insertionPoint));
				return limitBuilder.ToSqlString();
			}
			return sql; // unchanged
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new SapSqlAnywhere17DataBaseMetaData(connection);
		}
	}
}
