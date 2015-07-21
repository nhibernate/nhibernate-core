using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH2278
{
	public class CustomList<T> : List<T>, ICustomList<T>
	{
	}
}
