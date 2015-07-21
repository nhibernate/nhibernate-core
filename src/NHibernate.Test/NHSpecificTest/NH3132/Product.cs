using System;

namespace NHibernate.Test.NHSpecificTest.NH3132
{
	public class Product
	{
		public virtual Guid Id { get; set; }

		private string _name;
		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual string Lazy { get; set; }
	}
}
