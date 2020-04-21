using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// Summary description for NoArgSQLFunction.
	/// </summary>
	[Serializable]
	public class NoArgSQLFunction : ISQLFunction, ISQLFunctionExtended
	{
		public NoArgSQLFunction(string name, IType returnType)
			: this(name, returnType, true)
		{
		}

		public NoArgSQLFunction(string name, IType returnType, bool hasParenthesesIfNoArguments)
		{
#pragma warning disable 618
			Name = name;
#pragma warning restore 618
			FunctionReturnType = returnType;
			HasParenthesesIfNoArguments = hasParenthesesIfNoArguments;
		}

		public IType FunctionReturnType { get; protected set; }

		// Since v5.3
		[Obsolete("Use FunctionName property instead.")]
		public string Name { get; protected set; }

		#region ISQLFunction Members

		// Since v5.3
		[Obsolete("Use GetReturnType method instead.")]
		public IType ReturnType(IType columnType, IMapping mapping)
		{
			return FunctionReturnType;
		}

		/// <inheritdoc />
		public IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
#pragma warning disable 618
			return ReturnType(argumentTypes.FirstOrDefault(), mapping);
#pragma warning restore 618
		}

		/// <inheritdoc />
		public virtual IType GetEffectiveReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			return GetReturnType(argumentTypes, mapping, throwOnError);
		}

		/// <inheritdoc />
#pragma warning disable 618
		public string FunctionName => Name;
#pragma warning restore 618

		public bool HasArguments
		{
			get { return false; }
		}

		public bool HasParenthesesIfNoArguments { get; protected set; }

		public virtual SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count > 0)
			{
				throw new QueryException("function takes no arguments: " + FunctionName);
			}

			if (HasParenthesesIfNoArguments)
			{
				return new SqlString(FunctionName + "()");
			}

			return new SqlString(FunctionName);
		}

		#endregion
	}
}
