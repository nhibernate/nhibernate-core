using System;

namespace NHibernate.Test.NHSpecificTest.NH1289
{
	public class PurchaseItem
	{
		#region Fields

		public virtual Int32 Id
		{
			get;
			set;
		}

		#endregion


		public virtual PurchaseOrder PurchaseOrder
		{
			get;
			set;
		}


		public virtual Product Product
		{
			get;
			set;
		}
	}
}