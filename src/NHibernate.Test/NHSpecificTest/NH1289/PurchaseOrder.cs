using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1289
{
	public class PurchaseOrder : WorkflowItem
	{
		public virtual ISet<PurchaseItem> PurchaseItems
		{
			get;
			set;
		}
	}
}