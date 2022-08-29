using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests.ConformistMappingRegistrationTests
{
	[TestFixture]
	public class ModelMapperAddMappingByTypeTests
	{
		public class WithOutPublicParameterLessCtor
		{
			public WithOutPublicParameterLessCtor(string something)
			{
			}
		}

		[Test]
		public void WhenRegisterClassMappingThroughTypeThenCheckIConformistHoldersProvider()
		{
			var mapper = new ModelMapper();
			Assert.That(() => mapper.AddMapping(typeof(object)), Throws.TypeOf<ArgumentOutOfRangeException>().And.Message.Contains("IConformistHoldersProvider"));
		}

		[Test]
		public void WhenRegisterClassMappingThroughTypeThenCheckParameterLessCtor()
		{
			var mapper = new ModelMapper();
			Assert.That(() => mapper.AddMapping(typeof(WithOutPublicParameterLessCtor)), Throws.TypeOf<MappingException>());
		}
	}
}
