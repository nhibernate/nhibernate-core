using System.Collections;
using System.Text;
using NHibernate.Engine;
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

		public virtual string Render(IList args, ISessionFactoryImplementor factory)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(name)
				.Append('(');
			for (int i = 0; i < args.Count; i++)
			{
				buf.Append(args[i]);
				if (i < (args.Count - 1)) buf.Append(", ");
			}
			return buf.Append(')').ToString();
		}

		#endregion

		public override string ToString()
		{
			return name;
		}
	}
}