using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1742
{
	public class Event
	{
		private readonly IList<Description> descriptions = new List<Description>();

		public virtual IList<Description> Descriptions
		{
			get { return descriptions; }
		}

		public virtual int ID { get; set; }
		public virtual Device SendedBy { get; set; }
		public virtual DateTime Date { get; set; }
	}

	public class Device
	{
		public virtual int ID { get; set; }
	}

	public class Description
	{
		public virtual Event Event { get; set; }
		public virtual int ID { get; set; }
		public virtual string LanguageID { get; set; }
		public virtual string Value { get; set; }
	}
}