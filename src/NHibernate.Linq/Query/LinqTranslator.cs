using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Loader;

namespace NHibernate.Linq.LinqQuery
{
	public class LinqTranslator:OuterJoinLoader
	{
		public LinqTranslator(Expression expression, 
			IDictionary<string, IFilter> enabledFilters,
			ISessionFactoryImplementor sessionFactory):base(sessionFactory,enabledFilters)
		{
			this.expression = expression;
			this.sessionFactory = sessionFactory;
		}

		private readonly Expression expression;
		private readonly ISessionFactoryImplementor sessionFactory;



		public void Translate()
		{
			
		}

		protected void RenderSql()
		{
			
		}

	}
}
