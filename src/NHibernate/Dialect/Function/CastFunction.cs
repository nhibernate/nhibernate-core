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
		//private LazyType returnType;
		#region ISQLFunction Members

		public IType ReturnType(IType columnType, IMapping mapping)
		{
			//note there is a weird implementation in the client side
			//TODO: cast that use only costant are not supported in SELECT. Ex: cast(5 as string)
			//return SetLazyType(columnType); 
			return columnType;
		}
		/*
		private LazyType SetLazyType(IType columnType)
		{
			if(returnType == null)
			{
				returnType = new LazyType();
			}
			returnType.RealType = columnType;
			return returnType;
		}
		 */
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
			//SetLazyType(hqlType);
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
			return new SqlStringBuilder()
				.Add("cast(")
				.AddObject(args[0])
				.Add(" as ")
				.Add(sqlType)
				.Add(")")
				.ToSqlString();
		}

		#endregion

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

		/// <summary>
		/// Delegate the values to a real type
		/// </summary>
		/// <remarks>
		/// The real return type of Cast is know only after the Cast is parsed.
		/// This class was created in NH to remove the responsibility of the parser about know the
		/// real return type.
		/// </remarks>
		[Serializable]
		private class LazyType: IType
		{
			public IType RealType { get; set; }

			#region Implementation of ICacheAssembler

			public object Disassemble(object value, ISessionImplementor session, object owner)
			{
				return RealType.Disassemble(value, session, owner);
			}

			public object Assemble(object cached, ISessionImplementor session, object owner)
			{
				return RealType.Assemble(cached, session, owner);
			}

			public void BeforeAssemble(object cached, ISessionImplementor session)
			{
				RealType.BeforeAssemble(cached, session);
			}

			#endregion

			#region Implementation of IType

			public string Name
			{
				get { return RealType.Name; }
			}

			public System.Type ReturnedClass
			{
				get { return RealType.ReturnedClass; }
			}

			public bool IsMutable
			{
				get { return RealType.IsMutable; }
			}

			public bool IsAssociationType
			{
				get { return RealType.IsAssociationType; }
			}

			public bool IsXMLElement
			{
				get { return RealType.IsXMLElement; }
			}

			public bool IsCollectionType
			{
				get { return RealType.IsCollectionType; }
			}

			public bool IsComponentType
			{
				get { return RealType.IsComponentType; }
			}

			public bool IsEntityType
			{
				get { return RealType.IsEntityType; }
			}

			public bool IsAnyType
			{
				get { return RealType.IsAnyType; }
			}

			public SqlType[] SqlTypes(IMapping mapping)
			{
				return RealType.SqlTypes(mapping);
			}

			public int GetColumnSpan(IMapping mapping)
			{
				return RealType.GetColumnSpan(mapping);
			}

			public bool IsDirty(object old, object current, ISessionImplementor session)
			{
				return RealType.IsDirty(old, current, session);
			}

			public bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
			{
				return RealType.IsDirty(old, current, checkable, session);
			}

			public bool IsModified(object oldHydratedState, object currentState, bool[] checkable, ISessionImplementor session)
			{
				return RealType.IsModified(oldHydratedState, currentState, checkable, session);
			}

			public object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner)
			{
				return RealType.NullSafeGet(rs, names, session, owner);
			}

			public object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
			{
				return RealType.NullSafeGet(rs, name, session, owner);
			}

			public void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
			{
				RealType.NullSafeSet(st, value, index, settable, session);
			}

			public void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session)
			{
				RealType.NullSafeSet(st, value, index, session);
			}

			public string ToLoggableString(object value, ISessionFactoryImplementor factory)
			{
				return RealType.ToLoggableString(value, factory);
			}

			public object DeepCopy(object val, EntityMode entityMode, ISessionFactoryImplementor factory)
			{
				return RealType.DeepCopy(val, entityMode, factory);
			}

			public object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner)
			{
				return RealType.Hydrate(rs, names, session, owner);
			}

			public object ResolveIdentifier(object value, ISessionImplementor session, object owner)
			{
				return RealType.ResolveIdentifier(value, session, owner);
			}

			public object SemiResolve(object value, ISessionImplementor session, object owner)
			{
				return RealType.SemiResolve(value, session, owner);
			}

			public object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copiedAlready)
			{
				return RealType.Replace(original, target, session, owner, copiedAlready);
			}

			public object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
			{
				return RealType.Replace(original, target, session, owner, copyCache, foreignKeyDirection);
			}

			public bool IsSame(object x, object y, EntityMode entityMode)
			{
				return RealType.IsSame(x, y, entityMode);
			}

			public bool IsEqual(object x, object y, EntityMode entityMode)
			{
				return RealType.IsEqual(x, y, entityMode);
			}

			public bool IsEqual(object x, object y, EntityMode entityMode, ISessionFactoryImplementor factory)
			{
				return RealType.IsEqual(x, y, entityMode, factory);
			}

			public int GetHashCode(object x, EntityMode entityMode)
			{
				return RealType.GetHashCode(x, entityMode);
			}

			public int GetHashCode(object x, EntityMode entityMode, ISessionFactoryImplementor factory)
			{
				return RealType.GetHashCode(x, entityMode, factory);
			}

			public int Compare(object x, object y, EntityMode? entityMode)
			{
				return RealType.Compare(x, y, entityMode);
			}

			public IType GetSemiResolvedType(ISessionFactoryImplementor factory)
			{
				return RealType.GetSemiResolvedType(factory);
			}

			public void SetToXMLNode(XmlNode node, object value, ISessionFactoryImplementor factory)
			{
				RealType.SetToXMLNode(node, value, factory);
			}

			public object FromXMLNode(XmlNode xml, IMapping factory)
			{
				return RealType.FromXMLNode(xml, factory);
			}

			public bool[] ToColumnNullness(object value, IMapping mapping)
			{
				return RealType.ToColumnNullness(value, mapping);
			}

			#endregion
		}
	}
}
