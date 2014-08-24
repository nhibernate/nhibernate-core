using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Param
{
	public class AggregatedIndexCollectionSelectorParameterSpecifications : IParameterSpecification
	{
		private readonly IList<IParameterSpecification> _paramSpecs;
		public AggregatedIndexCollectionSelectorParameterSpecifications(IList<IParameterSpecification> paramSpecs) {
			_paramSpecs = paramSpecs;
		}

		//public int Bind(IDbCommand statement, QueryParameters qp, ISessionImplementor session, int position)
		//{
		//  int bindCount = 0;

		//  foreach (IParameterSpecification spec in _paramSpecs)
		//  {
		//    bindCount += spec.Bind(statement, qp, session, position + bindCount);
		//  }
		//  return bindCount;
		//}


		public void Bind(IDbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public void Bind(IDbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public IType ExpectedType
		{
			get { return null; }
			set { }
		}

		public string RenderDisplayInfo() 
		
		{
			return "index-selector [" + CollectDisplayInfo() + "]" ;
		}

		public IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory)
		{
			throw new NotImplementedException();
		}

		private string CollectDisplayInfo() 
		{
			StringBuilder buffer = new StringBuilder();

			foreach (IParameterSpecification spec in _paramSpecs)
			{
				buffer.Append(spec.RenderDisplayInfo());
			}

			return buffer.ToString();
		}
	}
}