using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Provides a substring implementation of the form substring(expr, start, length)
	/// for SQL dialects where the length argument is mandatory. If this is called
	/// from HQL with only two arguments, this implementation will generate the length
	/// parameter as (len(expr) + 1 - start).
	/// </summary>
	[Serializable]
	public class EmulatedLengthSubstringFunction : StandardSQLFunction
	{
		/// <summary>
		/// Initializes a new instance of the EmulatedLengthSubstringFunction class.
		/// </summary>
		public EmulatedLengthSubstringFunction()
			: base("substring", NHibernateUtil.String)
		{
		}


		#region ISQLFunction Members

		public override SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count < 2 || args.Count > 3)
				throw new QueryException("substring(): Incorrect number of parameters (expected 2 or 3, got " + args.Count + ")");

			if (args.Count == 2)
			{
				// Have the DB calculate the length argument itself.
				var lengthArg = new SqlString("len(", args[0], ") + 1 - (", args[1], ")");
				args = new[] { args[0], args[1], lengthArg };

				// Future possibility: Some databases, e.g. MSSQL, allows the length
				// parameter to be larger than the number of characters in the string
				// from start position to the end of the string. For such dialects,
				// perhaps it is better performance to simply pass Int32.MaxValue as length?
			}

			return base.Render(args, factory);
		}

		#endregion

	}
}
