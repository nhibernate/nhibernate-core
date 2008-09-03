using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NHibernate.Engine;
using System.Collections;
using NHibernate.Persister.Collection;
using NHibernate.Linq.Util;
using NHibernate.Metadata;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// A queryspace is a term that is used with NHibernate AutoFlush feature. 
	/// When an entity(table) is evolved in a query, this entity(table) is added to queryspace.
	/// Before nhibernate executes the SQL query, it checks if each item in queryspace has any dirty
	/// entity. If there is any, session flush occurs so that query can act on fresh data.
	/// This visitor visits nodes of the expression tree, and appends each node corresponding 
	/// to an entity/collection to the queryspace.
	/// </summary>
	public class QuerySpacesFinderVisitor:ExpressionVisitor
	{
		public QuerySpacesFinderVisitor(ISessionFactoryImplementor sessionFactory)
			:this(sessionFactory,new List<string>())
		{
			
		}
		public QuerySpacesFinderVisitor(ISessionFactoryImplementor sessionFactory,IList<string> querySpaces)
		{
			this.QuerySpaces = querySpaces;
			this.sessionFactory = sessionFactory;
		}
		private readonly ISessionFactoryImplementor sessionFactory;

		/// <summary>
		/// Query spaces evolved in the query.
		/// </summary>
		public  IList<string> QuerySpaces { get; protected set; }
		protected override System.Linq.Expressions.Expression VisitConstant(System.Linq.Expressions.ConstantExpression c)
		{
			if(c.Value is IQueryable)
			{
				QuerySpaces.Add(((IQueryable)c.Value).ElementType.ToString());
			}
			return base.VisitConstant(c);
		}
		protected override System.Linq.Expressions.Expression VisitMemberAccess(System.Linq.Expressions.MemberExpression m)
		{
			if(m.Member.MemberType==MemberTypes.Property)
			{

				//TODO: need to find better way to find if a property is a collection/list.
				if (TypeSystem.GetElementType(m.Type) == null || TypeSystem.GetElementType(m.Type)==m.Type)
				{
					if(sessionFactory.GetImplementors(m.Type.ToString()).Length>0)
					{
						QuerySpaces.Add(m.Type.FullName);
					}
				}
				else
				{
					ICollectionMetadata metadata =
					sessionFactory.GetCollectionMetadata(string.Format("{0}.{1}", m.Member.DeclaringType.FullName, m.Member.Name));
					QuerySpaces.Add(metadata.ElementType.ReturnedClass.FullName);
				}
			}
			return base.VisitMemberAccess(m);
		}
	}
}
