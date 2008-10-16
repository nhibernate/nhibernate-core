using System;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class TypeNameParserFixture
	{
		private void CheckInput(string input, string expectedType, string expectedAssembly)
		{
			AssemblyQualifiedTypeName tn = TypeNameParser.Parse(input);
			Assert.AreEqual(expectedType, tn.Type, "Type name should match");
			Assert.AreEqual(expectedAssembly, tn.Assembly, "Assembly name should match");
		}

		[Test]
		public void ParseSimple()
		{
			CheckInput("SomeType", "SomeType", null);
		}

		[Test]
		public void ParseQualified()
		{
			CheckInput("SomeType,SomeAssembly", "SomeType", "SomeAssembly");
		}

		[Test]
		public void ParseWithEscapes()
		{
			CheckInput("Some\\,Type, SomeAssembly\\,", "Some\\,Type", "SomeAssembly\\,");
		}

		[Test]
		public void ParseFullAssemblyName()
		{
			string assemblyName = "SomeAssembly, SomeCulture, SomethingElse";
			CheckInput("SomeType, " + assemblyName, "SomeType", assemblyName);
		}

		[Test]
		public void ParseGenericTypeName()
		{
			string assemblyName = "SomeAssembly";
			string typeName = "SomeType`1[System.Int32]";

			CheckInput(typeName + ", " + assemblyName, typeName, assemblyName);
		}

		[Test]
		public void ParseComplexGenericTypeName()
		{
			string typeName = "SomeType`1[[System.Int32, mscorlib], System.Int32]";

			CheckInput(typeName, typeName, null);
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void ParseUnmatchedBracket()
		{
			TypeNameParser.Parse("SomeName[");
		}

		[Test]
		public void ParseUnmatchedEscapedBracket()
		{
			TypeNameParser.Parse("SomeName\\[");
		}

		[Test]
		public void ParseWithDefaultAssemblyUnused()
		{
			string defaultAssembly = "DefaultAssembly";
			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeType, AnotherAssembly", null, defaultAssembly);
			Assert.AreEqual("SomeType", tn.Type);
			Assert.AreEqual("AnotherAssembly", tn.Assembly);
		}

		[Test]
		public void ParseWithDefaultAssembly()
		{
			string defaultAssembly = "SomeAssembly";
			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeType", null, defaultAssembly);
			Assert.AreEqual("SomeType", tn.Type);
			Assert.AreEqual(defaultAssembly, tn.Assembly);
		}

		[Test]
		public void ParseWithDefaultNamespaceAndAssembly()
		{
			string defaultAssembly = "DefaultAssembly";
			string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeType", defaultNamespace, defaultAssembly);
			Assert.AreEqual("DefaultNamespace.SomeType", tn.Type);
		}

		[Test]
		public void ParseWithDefaultNamespaceNoAssembly()
		{
			string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeType", defaultNamespace, null);
			Assert.AreEqual("DefaultNamespace.SomeType", tn.Type);
			Assert.IsNull(tn.Assembly);
		}

		[Test]
		public void ParseWithDefaultNamespaceUnused()
		{
			string defaultAssembly = "DefaultAssembly";
			string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeNamespace.SomeType", defaultNamespace, defaultAssembly);
			Assert.AreEqual("SomeNamespace.SomeType", tn.Type);
		}

		[Test]
		public void ParseTrims()
		{
			CheckInput("\t  \nSomeType, SomeAssembly\n   \r\t", "SomeType", "SomeAssembly");
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void ParseInvalidEscape()
		{
			TypeNameParser.Parse("\\");
		}

		[Test]
		public void ParseGenericTypeNameWithDefaults()
		{
			string fullSpec = "TName`1[PartialName]";
			string defaultassembly = "SomeAssembly";
			string defaultNamespace = "SomeAssembly.MyNS";
			string expectedType = "SomeAssembly.MyNS.TName`1[SomeAssembly.MyNS.PartialName, SomeAssembly]";
			string expectedAssembly = "SomeAssembly";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse(fullSpec, defaultNamespace, defaultassembly);
			Assert.AreEqual(expectedType, tn.Type, "Type name should match");
			Assert.AreEqual(expectedAssembly, tn.Assembly, "Assembly name should match");

			fullSpec = "TName`1[[PartialName]]";
			defaultassembly = "SomeAssembly";
			defaultNamespace = "SomeAssembly.MyNS";
			expectedType = "SomeAssembly.MyNS.TName`1[[SomeAssembly.MyNS.PartialName, SomeAssembly]]";
			expectedAssembly = "SomeAssembly";

			tn = TypeNameParser.Parse(fullSpec, defaultNamespace, defaultassembly);
			Assert.AreEqual(expectedType, tn.Type, "Type name should match");
			Assert.AreEqual(expectedAssembly, tn.Assembly, "Assembly name should match");

			fullSpec = "TName`2[[PartialName],[OtherPartialName]]";
			defaultassembly = "SomeAssembly";
			defaultNamespace = "SomeAssembly.MyNS";
			expectedType = "SomeAssembly.MyNS.TName`2[[SomeAssembly.MyNS.PartialName, SomeAssembly],[SomeAssembly.MyNS.OtherPartialName, SomeAssembly]]";
			expectedAssembly = "SomeAssembly";
			tn = TypeNameParser.Parse(fullSpec, defaultNamespace, defaultassembly);
			Assert.AreEqual(expectedType, tn.Type, "Type name should match");
			Assert.AreEqual(expectedAssembly, tn.Assembly, "Assembly name should match");
		}

		[Test]
		public void ParseGenericTypeNameWithDefaultNamespaceUnused()
		{
			string fullSpec = "TName`1[SomeAssembly.MyOtherNS.PartialName]";
			string defaultassembly = "SomeAssembly";
			string defaultNamespace = "SomeAssembly.MyNS";
			string expectedType = "SomeAssembly.MyNS.TName`1[SomeAssembly.MyOtherNS.PartialName, SomeAssembly]";
			string expectedAssembly = "SomeAssembly";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse(fullSpec, defaultNamespace, defaultassembly);
			Assert.AreEqual(expectedType, tn.Type, "Type name should match");
			Assert.AreEqual(expectedAssembly, tn.Assembly, "Assembly name should match");

			fullSpec = "SomeType`1[System.Int32]";
			defaultassembly = "SomeAssembly";
			defaultNamespace = null;
			expectedType = "SomeType`1[System.Int32]";
			expectedAssembly = "SomeAssembly";

			tn = TypeNameParser.Parse(fullSpec, defaultNamespace, defaultassembly);
			Assert.AreEqual(expectedType, tn.Type, "Type name should match");
			Assert.AreEqual(expectedAssembly, tn.Assembly, "Assembly name should match");

			fullSpec = typeof(MyGClass<int>).AssemblyQualifiedName;
			defaultassembly = "SomeAssembly";
			defaultNamespace = "SomeAssembly.MyNS";
			expectedType = typeof(MyGClass<int>).AssemblyQualifiedName;
			tn = TypeNameParser.Parse(fullSpec, defaultNamespace, defaultassembly);
			Assert.AreEqual(expectedType, tn.Type + ", " + tn.Assembly, "Type name should match");
		}

		public class MyGClass<T>
		{
			
		}
	}
}