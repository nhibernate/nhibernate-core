using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Transform;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	internal class ProcessSetResultTransformer : IResultOperatorProcessor<SetResultTransformerOperator>
	{
		public void Process(SetResultTransformerOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			tree.AddAdditionalCriteria((q, p) => q.SetResultTransformer(resultOperator.ResultTransformer.Value as IResultTransformer));
		}
	}
}
