using System;
using NHibernate.Type;

namespace NHibernate.Test.NHSpecificTest.NH1941
{
	public class SexEnumStringType : EnumStringType<Sex>
	{
		public override object GetValue(object enumValue)
		{
			if (enumValue == null)
			{
				return string.Empty;
			}

			return (Sex)enumValue == Sex.Male ? "M" : "F";
		}

		public override object GetInstance(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			return (value.ToString() == "M") ? Sex.Male : Sex.Female;
		}
	}
}