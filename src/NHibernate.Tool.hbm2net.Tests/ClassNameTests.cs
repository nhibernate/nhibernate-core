using System;

using NUnit.Framework;

namespace NHibernate.Tool.hbm2net.Tests
{
	[TestFixture]
	public class ClassNameTests
	{
		[Test]
		public void AssemblyNameWithDots()
		{
			ClassName name = new ClassName("Some.Qualified.Name, Some.Assembly.With.Dots");
			Assert.AreEqual("Some.Qualified.Name", name.FullyQualifiedName);
			Assert.AreEqual("Some.Qualified", name.PackageName);
			Assert.AreEqual("Name", name.Name);
		}
	}
}