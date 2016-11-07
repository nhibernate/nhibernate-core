using System;
using NUnit.Framework;
using NHibernate.Mapping.ByCode;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests.ConformistMappingRegistrationTests
{
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
			Assert.That(() => mapper.AddMapping(typeof (object)), Throws.TypeOf<ArgumentOutOfRangeException>().And.Message.ContainsSubstring("IConformistHoldersProvider"));
		}

		[Test]
		public void WhenRegisterClassMappingThroughTypeThenCheckParameterLessCtor()
		{
			var mapper = new ModelMapper();
			Assert.That(() => mapper.AddMapping(typeof (WithOutPublicParameterLessCtor)), Throws.TypeOf<MappingException>());
		}
	}
}