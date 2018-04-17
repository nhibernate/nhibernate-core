using System;

namespace NHibernate.Test.Futures
{
	public class PolymorphicA
	{
		public virtual Guid Id { get; set; }
	}
	
	public class PolymorphicB : PolymorphicA
	{
	}
}
