using System;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NHibernate;
using NHibernate.Classic;

namespace NHibernate.Test.NHSpecificTest.NH1911
{

	public class LogEvent
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string Level { get; set; }
	}

}
