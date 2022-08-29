using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using NHibernate;
using NHibernate.Classic;

namespace NHibernate.Test.NHSpecificTest.NH2057
{
	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
