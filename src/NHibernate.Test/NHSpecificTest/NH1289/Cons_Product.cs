using System;

namespace NHibernate.Test.NHSpecificTest.NH1289
{
	[Serializable]
	public class Cons_Product : Product
	{
		#region Fields


		public virtual string ImageName
		{
			get;
			set;
		}

		#endregion
	}
}