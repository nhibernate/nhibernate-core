using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3057
{
	public abstract class BaseClass
	{
		public virtual int Id { get; set; }
		public virtual string InheritedProperty { get; set; }
	}

	public class AClass 
	{
		private ICollection<BClass> bs = new List<BClass>(); 
		
		public virtual int Id { get; set; }

		public virtual ICollection<BClass> Bs
		{
			get { return bs; }
			protected internal set { bs = value; }
		}
	}

	public class BClass : BaseClass
	{
		public virtual AClass A { get; set; }
	}
}