using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2409
{
	public class Message
	{
		public virtual int Id { get; set; }

		public virtual Contest Contest { get; set; }

		public virtual IList<MessageReading> Readings { get; set; }
	}
}
