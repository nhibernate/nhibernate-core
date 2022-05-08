using System;

namespace NHibernate.Test.NHSpecificTest.GH2641
{
	[Serializable]
	public class Entity
	{
		public virtual int Id { get; set; }
		public virtual double Value { get; set; }
	}
}
