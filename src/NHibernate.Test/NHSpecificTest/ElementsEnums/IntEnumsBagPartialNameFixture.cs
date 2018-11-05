using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ElementsEnums
{
	[TestFixture]
	public class IntEnumsBagPartialNameFixture : AbstractIntEnumsBagFixture
	{
		protected override string[] Mappings
		{
			get { return new[] { "NHSpecificTest.ElementsEnums.SimpleWithEnumsPartialName.hbm.xml" }; }
		}
	}
}