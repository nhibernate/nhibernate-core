using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ElementsEnums
{
	[TestFixture]
	public class IntEnumsBagFixture : AbstractIntEnumsBagFixture
	{
		protected override string[] Mappings
		{
			get { return new[] { "NHSpecificTest.ElementsEnums.SimpleWithEnums.hbm.xml" }; }
		}
	}
}
