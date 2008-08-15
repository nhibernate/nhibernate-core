using System;
using NHibernate.Criterion;
using NHibernate.Linq.Util;
using Expression=System.Linq.Expressions.Expression;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq
{
	public class NHibernateQueryProvider : QueryProvider
	{
		private readonly ISession session;

		public NHibernateQueryProvider(ISession session)
		{
			Guard.AgainstNull(session,"session");
			this.session = session;
		}

		public override object Execute(Expression expression)
		{
			throw new NotImplementedException();

			/* iteratively process expression tree here converting to NH tree */

			//expression = LocalVariableExpressionReducer.Reduce(expression);
			//expression =  LogicalExpressionReducer.Reduce(expression);
			//expression = AssociationVisitor.RewriteWithAssociations(session.SessionFactory, expression);
			//expression = CollectionAliasVisitor.AssignCollectionAccessAliases(expression);
			//expression = new PropertyToMethodVisitor().Visit(expression);
			//expression = new BinaryExpressionOrderer().Visit(expression);

			//once tree is converted to NH tree, pass it to NHibernateQueryTranslator
			//which will convert the tree to an NHibernate.SqlCommand.SqlString

			//NHibernateQueryTranslator translator = new NHibernateQueryTranslator(session);
			//return translator.Translate(expression,this.queryOptions);
		}
	}
}
