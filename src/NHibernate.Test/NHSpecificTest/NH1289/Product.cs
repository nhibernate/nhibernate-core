using System;

namespace NHibernate.Test.NHSpecificTest.NH1289
{
	[Serializable]
	public class Product
	{
		#region Fields
		public virtual String Description
		{
			get;
			set;
		}

		public virtual Int32 Id
		{
			get;
			set;
		}

		public virtual String Price
		{
			get;
			set;
		}

		public virtual String ProductName
		{
			get;
			set;
		}


		public virtual Int32? Units
		{
			get;
			set;
		}

		#endregion
	}
}