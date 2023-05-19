using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// ANSI-SQL style cast(foo as type) where the type is a NHibernate type
	/// </summary>
	[Serializable]
	public class CastFunction : ISQLFunction, IFunctionGrammar, ISQLFunctionExtended
	{
		#region ISQLFunction Members

		// Since v5.3
		[Obsolete("Use GetReturnType method instead.")]
		public IType ReturnType(IType columnType, IMapping mapping)
		{
			//note there is a weird implementation in the client side
			//TODO: cast that use only costant are not supported in SELECT. Ex: cast(5 as string)
			return columnType;
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
		public string Name => "cast";

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
			if (args.Count != 2)
			{
				throw new QueryException("cast() requires two arguments");
			}
			string typeName = args[1].ToString();

			IType hqlType =
				TypeFactory.HeuristicType(typeName)
				?? throw new QueryException(string.Format("invalid Hibernate type for cast(): type {0} not found", typeName));

			string sqlType = GetCastTypeName(factory, hqlType, typeName);

			// TODO 6.0: Remove pragma block with its content
#pragma warning disable 618
			if (!CastingIsRequired(sqlType))
				return new SqlString("(", args[0], ")");
#pragma warning restore 618

			return Render(args[0], sqlType, factory);
		}

		#endregion

		// Since v5.3
		[Obsolete("This method has no usages and will be removed in a future version")]
		protected virtual bool CastingIsRequired(string sqlType)
		{
			return true;
		}

		/// <summary>
		/// Renders the SQL fragment representing the SQL cast.
		/// </summary>
		/// <param name="expression">The cast argument.</param>
		/// <param name="sqlType">The SQL type to cast to.</param>
		/// <param name="factory">The session factory.</param>
		/// <returns>A SQL fragment.</returns>
		protected virtual SqlString Render(object expression, string sqlType, ISessionFactoryImplementor factory)
		{
			return new SqlString("cast(", expression, " as ", sqlType, ")");
		}

		internal SqlString Render(IList args, IType expectedType, ISessionFactoryImplementor factory)
		{
			 return Render(args[0], GetCastTypeName(factory, expectedType, expectedType.Name), factory);
		}

		private static string GetCastTypeName(ISessionFactoryImplementor factory, IType hqlType, string typeName)
		{
			SqlType[] sqlTypeCodes = hqlType.SqlTypes(factory);
			if (sqlTypeCodes.Length != 1)
				throw new QueryException("invalid NHibernate type for cast(), was:" + typeName);

			return factory.Dialect.GetCastTypeName(sqlTypeCodes[0]);
		}

		#region IFunctionGrammar Members

		bool IFunctionGrammar.IsSeparator(string token)
		{
			return "as".Equals(token, StringComparison.OrdinalIgnoreCase);
		}

		bool IFunctionGrammar.IsKnownArgument(string token)
		{
			return false;
		}

		#endregion
	}
}
