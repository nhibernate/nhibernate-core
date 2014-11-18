using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	internal class ProcessAsReadOnly : IResultOperatorProcessor<AsReadOnlyResultOperator>
	{
		public void Process(AsReadOnlyResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			tree.AddAdditionalCriteria((q, p) => q.SetReadOnly(true));
		}
	}
}