using System;

namespace NHibernate.Test.ConnectionTest
{
	[Serializable]
	public class YetAnother
	{
		public virtual long Id { get; set; }

		public virtual string Name { get; set; }
	}
}