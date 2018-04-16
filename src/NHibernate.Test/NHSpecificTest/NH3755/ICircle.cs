using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3755
{
	public interface ICircle : IShape
	{
		string Property2 { get; set; }
		string Property3 { get; set; }
	}
}
