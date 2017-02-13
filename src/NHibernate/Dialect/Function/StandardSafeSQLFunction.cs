using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Provides a standard implementation that supports the majority of the HQL
	/// functions that are translated to SQL.
	/// </summary>
	/// <remarks>
	/// The Dialect and its sub-classes use this class to provide details required
	/// for processing of the associated function.
	/// </remarks>	
	[Serializable]
	public class StandardSafeSQLFunction : StandardSQLFunction
	{
		private int allowedArgsCount = 1;
		/// <summary>
		/// Initializes a new instance of the StandardSafeSQLFunction class.
		/// </summary>
		/// <param name="name">SQL function name.</param>
		/// <param name="allowedArgsCount">Exact number of arguments expected.</param>
		public StandardSafeSQLFunction(string name, int allowedArgsCount)
			: base(name)
		{
			this.allowedArgsCount = allowedArgsCount;
		}

		/// <summary>
		/// Initializes a new instance of the StandardSafeSQLFunction class.
		/// </summary>
		/// <param name="name">SQL function name.</param>
		/// <param name="typeValue">Return type for the function.</param>
		/// <param name="allowedArgsCount">Exact number of arguments expected.</param>
		public StandardSafeSQLFunction(string name, IType typeValue, int allowedArgsCount)
			: base(name, typeValue)
		{
			this.allowedArgsCount = allowedArgsCount;
		}

		public override SqlString Render(System.Collections.IList args, NHibernate.Engine.ISessionFactoryImplementor factory)
		{
			if (args.Count!= allowedArgsCount)
			{
				throw new QueryException(string.Format("function '{0}' takes {1} arguments.", name, allowedArgsCount));
			}

			return base.Render(args, factory);
		}
	}
}
