using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ElementsEnums
{
	[TestFixture]
	public class IntEnumsBagNoNameFixture : AbstractIntEnumsBagFixture
	{
		protected override string[] Mappings
		{
			get { return new[] { "NHSpecificTest.ElementsEnums.SimpleWithEnumsNoName.hbm.xml" }; }
		}
	}
}
