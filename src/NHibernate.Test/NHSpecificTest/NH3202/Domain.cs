using System;

namespace NHibernate.Test.NHSpecificTest.NH3202
{
	public class SequencedItem
	{
		public SequencedItem() { }
		public SequencedItem(int i)
		{
			I = i;
		}

		public virtual Guid Id { get; set; }

		public virtual int I { get; set; }
	}
}
