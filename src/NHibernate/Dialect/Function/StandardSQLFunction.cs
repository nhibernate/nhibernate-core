using System.Collections;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;

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
	public class StandardSQLFunction : ISQLFunction
	{
		private IType returnType = null;
		protected readonly string name;

		/// <summary>
		/// Initializes a new instance of the StandardSQLFunction class.
		/// </summary>
		/// <param name="name">SQL function name.</param>
		public StandardSQLFunction(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Initializes a new instance of the StandardSQLFunction class.
		/// </summary>
		/// <param name="name">SQL function name.</param>
		/// <param name="typeValue">Return type for the fuction.</param>
		public StandardSQLFunction(string name, IType typeValue)
			: this(name)
		{
			returnType = typeValue;
		}

		#region ISQLFunction Members

		public virtual IType ReturnType(IType columnType, IMapping mapping)
		{
			return returnType ?? columnType;
		}

		public bool HasArguments
		{
			get { return true; }
		}

		public bool HasParenthesesIfNoArguments
		{
			get { return true; }
		}

		public virtual SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			SqlStringBuilder buf = new SqlStringBuilder();
			buf.Add(name)
				.Add("(");
			for (int i = 0; i < args.Count; i++)
			{
				object arg = args[i];
				if (arg is Parameter || arg is SqlString)
				{
					buf.AddObject(arg);
				}
				else
				{
					buf.Add(arg.ToString());
				}
				if (i < (args.Count - 1)) buf.Add(", ");
			}
			return buf.Add(")").ToSqlString();
		}

		#endregion

		public override string ToString()
		{
			return name;
		}
	}
}
