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

		public SqlString Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count != 2)
			{
				throw new QueryException("cast() requires two arguments");
			}
			string typeName = args[1].ToString();
			string sqlType;
			IType hqlType = TypeFactory.HeuristicType(typeName);

			if (hqlType != null)
			{
				SqlType[] sqlTypeCodes = hqlType.SqlTypes(factory);
				if (sqlTypeCodes.Length != 1)
				{
					throw new QueryException("invalid NHibernate type for cast(), was:" + typeName);
				}

				sqlType = factory.Dialect.GetCastTypeName(sqlTypeCodes[0]);
				//{
				//  //trim off the length/precision/scale
				//  int loc = sqlType.IndexOf('(');
				//  if (loc>-1) 
				//  {
				//    sqlType = sqlType.Substring(0, loc);
				//  }
				//}
			}
			else
			{
				throw new QueryException(string.Format("invalid Hibernate type for cast(): type {0} not found", typeName));
			}

#pragma warning disable 618
			return CastingIsRequired(sqlType)
#pragma warning restore 618
				? Render(args, sqlType, factory)
				: new SqlString("(", args[0], ")");
		}

		#endregion

		// Since v5.3
		[Obsolete("This method has no usages and will be removed in a future version")]
		protected virtual bool CastingIsRequired(string sqlType)
		{
			return true;
		}

		protected virtual SqlString Render(IList args, string sqlType, ISessionFactoryImplementor factory)
		{
			return new SqlString("cast(", args[0], " as ", sqlType, ")");
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
