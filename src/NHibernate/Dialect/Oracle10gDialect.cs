using System.Collections.Generic;
using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	/// <summary> 
	/// A dialect specifically for use with Oracle 10g.
	/// </summary>
	/// <remarks>
	/// The main difference between this dialect and <see cref="Oracle9iDialect"/>
	/// is the use of "ANSI join syntax" here...
	/// </remarks>
	public class Oracle10gDialect : Oracle9iDialect
	{
		private bool _useBinaryFloatingPointTypes;

		public override JoinFragment CreateOuterJoinFragment()
		{
			return new ANSIJoinFragment();
		}

		public override void Configure(IDictionary<string, string> settings)
		{
			_useBinaryFloatingPointTypes = PropertiesHelper.GetBoolean(
				Environment.OracleUseBinaryFloatingPointTypes,
				settings,
				false);

			if (_useBinaryFloatingPointTypes)
			{
				RegisterFunction("mod", new ModulusFunction(true, true));
			}

			base.Configure(settings);
		}

		// Avoid registering weighted double type when using binary floating point types
		protected override void RegisterFloatingPointTypeMappings()
		{
			if (_useBinaryFloatingPointTypes)
			{
				// Use binary_float (available since 10g) instead of float. With Oracle, float is a decimal but
				// with a precision expressed in number of bytes instead of digits.
				RegisterColumnType(DbType.Single, "binary_float");
				// Using binary_double (available since 10g) instead of double precision. With Oracle, double
				// precision is a float(126), which is a decimal with a 126 bytes precision.
				RegisterColumnType(DbType.Double, "binary_double");
			}
			else
			{
				base.RegisterFloatingPointTypeMappings();
			}
		}

		protected override void RegisterFunctions()
		{
			base.RegisterFunctions();

			// DBMS_RANDOM package was available in previous versions, but it was requiring initialization and
			// was not having the value function.
			// It yields a decimal between 0 included and 1 excluded, with 38 significant digits. It sometimes
			// causes an overflow when read by the Oracle provider as a .Net Decimal, so better explicitly cast
			// it to double.
			RegisterFunction("random", new SQLFunctionTemplate(NHibernateUtil.Double, "cast(DBMS_RANDOM.VALUE() as binary_double)"));
		}

		/// <inheritdoc />
		public override bool SupportsCrossJoin => true;

		/// <inheritdoc />
		public override bool SupportsIEEE754FloatingPointNumbers => _useBinaryFloatingPointTypes;
	}
}
