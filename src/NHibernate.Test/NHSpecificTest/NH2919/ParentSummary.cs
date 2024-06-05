using System;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2919
{
	public class ParentSummary
	{
		public virtual Guid ID { get; set; }

		public virtual Parent Parent { get; set; }

		public virtual Child FirstChild
		{
			get { return Parent?.Children.FirstOrDefault(); }
			set { }
		}
	}
}
