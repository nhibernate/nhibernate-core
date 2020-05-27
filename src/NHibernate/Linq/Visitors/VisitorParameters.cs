using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Linq.ReWriters;
using NHibernate.Param;
using Remotion.Linq;

namespace NHibernate.Linq.Visitors
{
	public class VisitorParameters
	{
		public ISessionFactoryImplementor SessionFactory { get; private set; }

		public IDictionary<ConstantExpression, NamedParameter> ConstantToParameterMap { get; private set; }

		public List<NamedParameterDescriptor> RequiredHqlParameters { get; private set; }

		public QuerySourceNamer QuerySourceNamer { get; set; }

		/// <summary>
		/// Entity type to insert or update when the operation is a DML.
		/// </summary>
		public System.Type TargetEntityType { get; }

		public QueryMode RootQueryMode { get; }

		internal Dictionary<QueryModel, ResultOperatorRewriterResult> QueryModelRewriterResults { get; }
			= new Dictionary<QueryModel, ResultOperatorRewriterResult>();

		internal void AddQueryModelRewriterResult(QueryModel queryModel, ResultOperatorRewriterResult rewriterResult)
		{
			QueryModelRewriterResults.Add(queryModel, rewriterResult);
		}

		public VisitorParameters(
			ISessionFactoryImplementor sessionFactory, 
			IDictionary<ConstantExpression, NamedParameter> constantToParameterMap, 
			List<NamedParameterDescriptor> requiredHqlParameters, 
			QuerySourceNamer querySourceNamer,
			System.Type targetEntityType,
			QueryMode rootQueryMode)
		{
			SessionFactory = sessionFactory;
			ConstantToParameterMap = constantToParameterMap;
			RequiredHqlParameters = requiredHqlParameters;
			QuerySourceNamer = querySourceNamer;
			TargetEntityType = targetEntityType;
			RootQueryMode = rootQueryMode;
		}
	}
}
