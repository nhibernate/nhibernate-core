using System;

namespace NHibernate.Test.NHSpecificTest.NH3126
{
	public class PropertyValue
	{
		public virtual Guid Id { get; set; }

		public virtual string Value { get; set; }
	}
}