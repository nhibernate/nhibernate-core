using System;
using NHibernate.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class TypeNameParserFixture
	{
		private static void CheckInput(string input, string expectedType, string expectedAssembly)
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
			const string assemblyName = "SomeAssembly, SomeCulture, SomethingElse";
			CheckInput("SomeType, " + assemblyName, "SomeType", assemblyName);
		}

		[Test]
		public void ParseGenericTypeName()
		{
			const string assemblyName = "SomeAssembly";
			const string typeName = "SomeType`1[System.Int32]";
			const string expectedTypeName = "SomeType`1[[System.Int32]]";

			CheckInput(typeName + ", " + assemblyName, expectedTypeName, assemblyName);
		}

		[Test]
		public void ParseComplexGenericTypeName()
		{
			const string typeName = "SomeType`2[[System.Int32, mscorlib], System.Int32]";
			const string expectedTypeName = "SomeType`2[[System.Int32, mscorlib],[System.Int32]]";

			CheckInput(typeName, expectedTypeName, null);
		}

		[Test, Ignore("Not a big problem because the next type request will throw the exception"), ExpectedException(typeof(ParserException))]
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
			const string defaultAssembly = "DefaultAssembly";
			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeType, AnotherAssembly", null, defaultAssembly);
			Assert.AreEqual("SomeType", tn.Type);
			Assert.AreEqual("AnotherAssembly", tn.Assembly);
		}

		[Test]
		public void ParseWithDefaultAssembly()
		{
			const string defaultAssembly = "SomeAssembly";
			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeType", null, defaultAssembly);
			Assert.AreEqual("SomeType", tn.Type);
			Assert.AreEqual(defaultAssembly, tn.Assembly);
		}

		[Test]
		public void ParseWithDefaultNamespaceAndAssembly()
		{
			const string defaultAssembly = "DefaultAssembly";
			const string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeType", defaultNamespace, defaultAssembly);
			Assert.AreEqual("DefaultNamespace.SomeType", tn.Type);
		}

		[Test]
		public void ParseWithDefaultNamespaceNoAssembly()
		{
			const string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeType", defaultNamespace, null);
			Assert.AreEqual("DefaultNamespace.SomeType", tn.Type);
			Assert.IsNull(tn.Assembly);
		}

		[Test]
		public void ParseWithDefaultNamespaceUnused()
		{
			const string defaultAssembly = "DefaultAssembly";
			const string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse("SomeNamespace.SomeType", defaultNamespace, defaultAssembly);
			Assert.AreEqual("SomeNamespace.SomeType", tn.Type);
		}

		[Test]
		public void ParseTrims()
		{
			CheckInput("\t  \nSomeType, SomeAssembly\n   \r\t", "SomeType", "SomeAssembly");
		}

		[Test]
		public void ParseInvalidEscape()
		{
			Assert.Throws<ArgumentException>(() => TypeNameParser.Parse("\\"));
		}

		[Test]
		public void ParseGenericTypeNameWithDefaults()
		{
			string fullSpec = "TName`1[PartialName]";
			string defaultassembly = "SomeAssembly";
			string defaultNamespace = "SomeAssembly.MyNS";
			string expectedType = "SomeAssembly.MyNS.TName`1[[SomeAssembly.MyNS.PartialName, SomeAssembly]]";
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
			string expectedType = "SomeAssembly.MyNS.TName`1[[SomeAssembly.MyOtherNS.PartialName, SomeAssembly]]";
			string expectedAssembly = "SomeAssembly";

			AssemblyQualifiedTypeName tn = TypeNameParser.Parse(fullSpec, defaultNamespace, defaultassembly);
			Assert.AreEqual(expectedType, tn.Type, "Type name should match");
			Assert.AreEqual(expectedAssembly, tn.Assembly, "Assembly name should match");

			fullSpec = "SomeType`1[System.Int32]";
			defaultassembly = "SomeAssembly";
			defaultNamespace = null;
			expectedType = "SomeType`1[[System.Int32]]";
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

		public class MyComplexClass<T1, T2, T3>
		{

		}

		[Test]
		public void ParseComplexGenericType()
		{
			var expectedType = typeof(MyComplexClass<MyGClass<int>, IDictionary<string, MyGClass<string>>, string>).AssemblyQualifiedName;
			var a = TypeNameParser.Parse(expectedType);
			Assert.AreEqual(expectedType, a.ToString(), "Type name should match");
		}

		[Test]
		[Description("Should parse arrays of System types")]
		public void SystemArray()
		{
			var expectedType = typeof(string[]).AssemblyQualifiedName;
			var a = TypeNameParser.Parse(expectedType);
			Assert.AreEqual(expectedType, a.ToString());
		}

		[Test]
		[Description("Should parse arrays of custom types")]
		public void CustomArray()
		{
			var expectedType = "A[]";
			var a = TypeNameParser.Parse(expectedType);
			Assert.AreEqual(expectedType, a.ToString());

			expectedType = typeof(MyGClass<int>[]).FullName;
			a = TypeNameParser.Parse(expectedType);
			Assert.AreEqual(expectedType, a.ToString());

			expectedType = typeof(MyGClass<int[]>[]).FullName;
			a = TypeNameParser.Parse(expectedType);
			Assert.AreEqual(expectedType, a.ToString());

			expectedType = "MyGClass`1[[System.Int32[]]][]";
			a = TypeNameParser.Parse(expectedType);
			Assert.AreEqual(expectedType, a.ToString());
		}

		[Test]
		public void NH1736()
		{
			var typeName =
				"Test.NHMapping.CustomCollection`2[[Test.Common.InvoiceDetail, Test.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3a873a127e0d1872],[Test.ReadOnlyBusinessObjectList`1[[Test.Common.InvoiceDetail, Test.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3a873a127e0d1872]], Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3a873a127e0d1872]], Test.NHMapping, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3a873a127e0d1872";
			var a = TypeNameParser.Parse(typeName);
			Assert.AreEqual(typeName, a.ToString());
		}

		[Test]
		[Description("Parse with new lines")]
		public void NH1822()
		{
			var typeName =
				@"OldMutual.SalesGear.Data.ReferenceType`2[ 
                [OldMutual.SalesGear.Reference.Core.Channel, OldMutual.SalesGear.Reference.Core], 
                [OldMutual.SalesGear.Reference.Core.Channels, OldMutual.SalesGear.Reference.Core] 
              ], OldMutual.SalesGear.Data";
			var expected = "OldMutual.SalesGear.Data.ReferenceType`2[[OldMutual.SalesGear.Reference.Core.Channel, OldMutual.SalesGear.Reference.Core],[OldMutual.SalesGear.Reference.Core.Channels, OldMutual.SalesGear.Reference.Core]], OldMutual.SalesGear.Data";
			var a = TypeNameParser.Parse(typeName);
			Assert.That(a.ToString(), Is.EqualTo(expected));
		}

		private class A<T>
		{
			public class B { }
		}

		private class Aa<T>
		{
			public class Bb<TX, TJ, TZ>
			{
				public class C { }
			}
		}

		[Test]
		[Description("Parser multiple nested classes with a generic in the middle.")]
		public void ParseNestedWithinGeneric()
		{
			// NH-1867
			CheckInput(typeof(A<int>.B).FullName, typeof(A<int>.B).FullName, null);
		}

		[Test]
		[Description("Parser multiple nested classes with a generics.")]
		public void ComplexNestedWithGeneric()
		{
			CheckInput(typeof(Aa<int>.Bb<int, short, string>).FullName, typeof(Aa<int>.Bb<int, short, string>).FullName, null);
			CheckInput(typeof(Aa<int>.Bb<int, short, string>.C).FullName, typeof(Aa<int>.Bb<int, short, string>.C).FullName, null);
		} 
	}
}