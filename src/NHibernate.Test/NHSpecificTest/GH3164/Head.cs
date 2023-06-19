using System;

namespace NHibernate.Test.NHSpecificTest.GH3164
{
	[Serializable]
	public class Head : IHead
	{
		public virtual string Title { get; set; }

		public virtual bool DummyFieldToLoadEmptyComponent { get; }
	}
}
