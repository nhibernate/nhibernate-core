using System;
using NUnit.Framework;
using NHibernate.Mapping.ByCode;
using SharpTestsEx;

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
			mapper.Executing(x => x.AddMapping(typeof(object))).Throws<ArgumentOutOfRangeException>().And.ValueOf.Message.Should().Contain("IConformistHoldersProvider");
		}

		[Test]
		public void WhenRegisterClassMappingThroughTypeThenCheckParameterLessCtor()
		{
			var mapper = new ModelMapper();
			mapper.Executing(x => x.AddMapping(typeof(WithOutPublicParameterLessCtor))).Throws<MappingException>();
		}
	}
}