using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Linq.Visitors
{
	public class SelectJoinGatherer:NHibernateExpressionVisitor
	{
		public SelectJoinGatherer(ISessionFactoryImplementor sessionFactory)
		{
			this.SessionFactory = sessionFactory;
			this.JoinSequence=new JoinSequence(sessionFactory);
		}

		public ISessionFactoryImplementor SessionFactory { get; protected set; }
		public JoinSequence JoinSequence { get; protected set; }
	}
}
