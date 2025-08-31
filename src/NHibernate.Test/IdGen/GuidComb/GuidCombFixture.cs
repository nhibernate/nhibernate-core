using System;
using NHibernate.Id;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.GuidComb;

[TestFixture]
public class GuidCombFixture
{
	class GuidCombGeneratorEx : GuidCombGenerator
	{
		public static Guid Generate(string guid, DateTime utcNow) => GenerateComb(Guid.Parse(guid), utcNow);
	}

	[Test]
	public void CanGenerateSequentialGuid()
	{
		Assert.AreEqual(Guid.Parse("076a04fa-ef4e-4093-8479-b0e10103cdc5"),
			GuidCombGeneratorEx.Generate(
				"076a04fa-ef4e-4093-8479-8599e96f14cf",
				new DateTime(2023, 12, 23, 15, 45, 55, DateTimeKind.Utc)),
			"seed: 076a04fa");

		Assert.AreEqual(Guid.Parse("81162ee2-a4cb-4611-9327-d61f0137e5b6"),
		    GuidCombGeneratorEx.Generate(
			    "81162ee2-a4cb-4611-9327-23bbda36176c",
			    new DateTime(2050, 01, 29, 18, 55, 35, DateTimeKind.Utc)),
			"seed: 81162ee2");

	}
	
}
