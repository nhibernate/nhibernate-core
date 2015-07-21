using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.TypesTest
{
	public class GenericEnumStringClass
	{
		public virtual int Id
		{ 
			get;
			set;
		}

		public virtual SampleEnum EnumValue
		{ 
			get;
			set;
		}
	}
}
