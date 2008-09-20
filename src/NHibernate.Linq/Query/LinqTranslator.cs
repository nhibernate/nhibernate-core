using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Linq.Visitors;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System.Linq;

namespace NHibernate.Linq.Query
{
	public class LinqTranslator : BasicLoader
	{
		private readonly Expression expression;
		private readonly ISessionFactoryImplementor sessionFactory;
		private readonly IList<object> parameterList;


		public LinqTranslator(Expression expression,ISessionFactoryImplementor sessionFactory)
			: base(sessionFactory)

		{
			this.expression = expression;
			this.sessionFactory = sessionFactory;
			this.parameterList = new List<object>();
		}

		public virtual IType[] ReturnTypes { get; protected set; }

		public void Translate()
		{
			/*Expression modified = LocalVariableExpressionReducer.Reduce(expression);
			modified = LogicalExpressionReducer.Reduce(modified);
			modified = AssociationRewriteVisitor.Rewrite(modified, sessionFactory);
			modified = NHExpressionToSqlExpressionTransformer.Transform(sessionFactory, modified);
			sqlString = SqlExpressionToSqlStringVisitor.Translate(modified, sessionFactory, parameterList);*/
			
		}
		public IList List(ISessionImplementor sessionImplementor)
		{
			throw new NotImplementedException();
		}
		protected override string[] Suffixes
		{
			get { return suffixes; }
		}
		private string[] suffixes;

		protected override string[] CollectionSuffixes
		{
			get { return collectionSuffixes; }

		}
		private string[] collectionSuffixes;

		protected override SqlString SqlString
		{
			get { return sqlString; }

		}
		private SqlString sqlString;

		protected override ILoadable[] EntityPersisters
		{
			get { return entityPersisters; }
		}
		private ILoadable[] entityPersisters;

		protected override LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes)
		{
			return new LockMode[] { LockMode.Read };
		}

		public virtual ISet<string> QuerySpaces
		{
			get;
			protected set;
		}
	}
}