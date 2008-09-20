using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Query;
using NHibernate.Linq.Util;
using NHibernate.Linq.Visitors;
using NHibernate.SqlCommand;

namespace NHibernate.Linq
{
	public class NHibernateQueryProvider : QueryProvider
	{
		private readonly ISession session;
		private readonly ISessionFactoryImplementor sessionFactory;

		public NHibernateQueryProvider(ISession session)
		{
			Guard.AgainstNull(session, "session");
			this.session = session;
			sessionFactory = this.session.SessionFactory as ISessionFactoryImplementor;
		}

		public override object Execute(Expression expression)
		{
			LinqTranslator translator = new LinqTranslator(expression, sessionFactory);
			translator.Translate();
			return translator.List(session as ISessionImplementor);
		}
	}
}