using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1192
{
	public enum Status
	{
		None,
		Bold=1,
		Italic=2,
		Underlined=4

	}
	public class ObjectA
	{
		public virtual int Id { get; set; }
		public virtual Status FontType { get; set; }
		public virtual string Name { get; set; }
	}
}
