using System;
using System.Collections;
using System.Data;
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
	public class CastFunction : ISQLFunction, IFunctionGrammar
	{
		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			//note there is a weird implementation in the client side
			//TODO: cast that use only costant are not supported in SELECT. Ex: cast(5 as string)
			return columnType;
		}

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
				if (sqlType == null)
				{
					//TODO: never reached, since GetTypeName() actually throws an exception!
					sqlType = typeName;
				}
				//else 
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

			if (CastingIsRequired(sqlType))
			{
				return new SqlString("cast(", args[0], " as ", sqlType, ")");
			}
			else
			{
				return new SqlString("(", args[0], ")");
			}
		}

		#endregion

		protected virtual bool CastingIsRequired(string sqlType)
		{
			return true;
		}

		#region IFunctionGrammar Members

		bool IFunctionGrammar.IsSeparator(string token)
		{
			return "as".Equals(token, StringComparison.InvariantCultureIgnoreCase);
		}

		bool IFunctionGrammar.IsKnownArgument(string token)
		{
			return false;
		}

		#endregion
	}
}
