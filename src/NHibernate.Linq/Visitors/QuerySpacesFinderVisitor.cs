using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
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
	public class QuerySpacesFinderVisitor : ExpressionVisitor
	{
		private readonly ISessionFactoryImplementor sessionFactory;

		public QuerySpacesFinderVisitor(ISessionFactoryImplementor sessionFactory)
			: this(sessionFactory, new List<string>())
		{
		}

		public QuerySpacesFinderVisitor(ISessionFactoryImplementor sessionFactory, IList<string> querySpaces)
		{
			QuerySpaces = querySpaces;
			this.sessionFactory = sessionFactory;
		}

		/// <summary>
		/// Query spaces evolved in the query.
		/// </summary>
		public IList<string> QuerySpaces { get; protected set; }

		protected override Expression VisitConstant(ConstantExpression c)
		{
			if (c.Value is IQueryable)
			{
				QuerySpaces.Add(((IQueryable) c.Value).ElementType.ToString());
			}
			return base.VisitConstant(c);
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			if (m.Member.MemberType == MemberTypes.Property)
			{
				//TODO: need to find better way to find if a property is a collection/list.
				if (TypeSystem.GetElementType(m.Type) == null || TypeSystem.GetElementType(m.Type) == m.Type)
				{
					if (sessionFactory.GetImplementors(m.Type.ToString()).Length > 0)
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