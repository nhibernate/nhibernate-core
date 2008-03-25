using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	/// <summary>
	/// ANSI-SQL style cast(foo as type) where the type is a NHibernate type
	/// </summary>
	public class CastFunction : ISQLFunction, IFunctionGrammar
	{
		public CastFunction()
		{
		}

		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			//note there is a wierd implementation in the client side
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

		public string Render(IList args, ISessionFactoryImplementor factory)
		{
			if (args.Count != 2)
			{
				throw new QueryException("cast() requires two arguments");
			}
			string typeName = (string) args[1];
			string sqlType = string.Empty;
			IType hqlType = TypeFactory.HeuristicType(typeName);
			if (hqlType != null)
			{
				SqlType[] sqlTypeCodes = hqlType.SqlTypes(factory);
				if (sqlTypeCodes.Length != 1)
				{
					throw new QueryException("invalid Hibernate type for cast()");
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
			return String.Format("cast({0} as {1})", args[0], sqlType);
		}

		#endregion

		#region IFunctionGrammar Members

		bool IFunctionGrammar.IsSeparator(string token)
		{
			return "as".Equals(token);
		}

		bool IFunctionGrammar.IsKnownArgument(string token)
		{
			return false;
		}

		#endregion
	}
}