using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Type
{
	public class EnumStringType<T> : EnumStringType
	{
		public EnumStringType():base(typeof(T))
		{
			
		}
	}
}
