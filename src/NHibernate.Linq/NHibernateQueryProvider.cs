using System;
using NHibernate.Linq.Util;
using NHibernate.SqlCommand;
using Expression=System.Linq.Expressions.Expression;
using NHibernate.Linq.Visitors;
using NHibernate.Engine;
using System.Collections.Generic;

namespace NHibernate.Linq
{
	public class NHibernateQueryProvider : QueryProvider
	{
		private readonly ISession session;
		private readonly ISessionFactoryImplementor sessionFactory;
		public NHibernateQueryProvider(ISession session)
		{
			Guard.AgainstNull(session,"session");
			this.session = session;
			this.sessionFactory = this.session.SessionFactory as ISessionFactoryImplementor;
		}

		public override object Execute(Expression expression)
		{
			IList<object> parameterList = new List<object>();
			expression = LocalVariableExpressionReducer.Reduce(expression);
			expression = LogicalExpressionReducer.Reduce(expression);
			expression = AssociationRewriteVisitor.Rewrite(expression, sessionFactory);
			expression = NHExpressionToSqlExpressionTransformer.Transform(sessionFactory, expression);
			SqlString sql=SqlExpressionToSqlStringVisitor.Translate(expression, sessionFactory,parameterList);
			Console.WriteLine(sql);
			throw new NotImplementedException();
			//expression = AssociationVisitor.RewriteWithAssociations(session.SessionFactory, expression);
			//expression = CollectionAliasVisitor.AssignCollectionAccessAliases(expression);
			//expression = new PropertyToMethodVisitor().Visit(expression);
			//expression = new BinaryExpressionOrderer().Visit(expression);

			//once tree is converted to NH tree, pass it to NHibernateQueryTranslator
			//which will convert the tree to an NHibernate.SqlCommand.SqlString

			
			//return translator.Transform(expression,this.queryOptions);
		}
	}
}
