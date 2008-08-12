using System;
using System.Linq.Expressions;
using NHibernate.Linq.Util;

namespace NHibernate.Linq
{
	public class NHibernateQueryProvider : QueryProvider
	{
		private readonly ISession _session;

		public NHibernateQueryProvider(ISession session)
		{
			if (session == null) throw new ArgumentNullException("session");
			_session = session;
		}

		public override object Execute(Expression expression)
		{
			throw new NotImplementedException();

			/* iteratively process expression tree here converting to NH tree */

			//expression = Evaluator.PartialEval(expression);
			//expression = new BinaryBooleanReducer().Visit(expression);
			//expression = AssociationVisitor.RewriteWithAssociations(_session.SessionFactory, expression);
			//expression = CollectionAliasVisitor.AssignCollectionAccessAliases(expression);
			//expression = new PropertyToMethodVisitor().Visit(expression);
			//expression = new BinaryExpressionOrderer().Visit(expression);

			//once tree is converted to NH tree, pass it to NHibernateQueryTranslator
			//which will convert the tree to an NHibernate.SqlCommand.SqlString

			//NHibernateQueryTranslator translator = new NHibernateQueryTranslator(_session);
			//return translator.Translate(expression,this.queryOptions);
		}
	}
}
