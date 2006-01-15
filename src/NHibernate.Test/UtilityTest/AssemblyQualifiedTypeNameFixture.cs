using System;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class AssemblyQualifiedTypeNameFixture
	{
		[Test]
		public void Construct()
		{
			string typeName = "MyType";
			string assemblyName = "MyAssembly";
			AssemblyQualifiedTypeName tn = new AssemblyQualifiedTypeName( typeName, assemblyName );

			Assert.AreEqual( typeName, tn.Type );
			Assert.AreEqual( assemblyName, tn.Assembly );
		}

		private AssemblyQualifiedTypeName CreateDefaultName()
		{
			return new AssemblyQualifiedTypeName( "MyType", "MyAssembly" );
		}

		[Test]
		public void Equals()
		{
			Assert.AreEqual(
				CreateDefaultName(),
				CreateDefaultName() );

			Assert.IsFalse( new AssemblyQualifiedTypeName( "T1", "A1" ).Equals( new AssemblyQualifiedTypeName( "T2", "A2" ) ) );

			Assert.AreEqual(
				CreateDefaultName().GetHashCode(),
				CreateDefaultName().GetHashCode() );
		}

		[Test, ExpectedException( typeof( ArgumentNullException ) )]
		public void ConstructWithNullType()
		{
			new AssemblyQualifiedTypeName( null, "SomeAssembly" );
		}

		[Test]
		public void ConstructWithNullAssembly()
		{
			new AssemblyQualifiedTypeName( "SomeType", null );
		}

		private void CheckInput( string input, string expectedType, string expectedAssembly )
		{
			AssemblyQualifiedTypeName tn = AssemblyQualifiedTypeName.Parse( input );
			Assert.AreEqual( expectedType, tn.Type, "Type name should match" );
			Assert.AreEqual( expectedAssembly, tn.Assembly, "Assembly name should match" );
		}

		[Test]
		public void ParseSimple()
		{
			CheckInput( "SomeType", "SomeType", null );
		}

		[Test]
		public void ParseQualified()
		{
			CheckInput( "SomeType,SomeAssembly", "SomeType", "SomeAssembly" );
		}

		[Test]
		public void ParseWithEscapes()
		{
			CheckInput( "Some\\,Type, SomeAssembly\\,", "Some\\,Type", "SomeAssembly\\," );
		}

		[Test]
		public void ParseFullAssemblyName()
		{
			string assemblyName = "SomeAssembly, SomeCulture, SomethingElse";
			CheckInput( "SomeType, " + assemblyName, "SomeType", assemblyName );
		}

		[Test]
		public void ParseWithDefaultAssemblyUnused()
		{
			string defaultAssembly = "DefaultAssembly";
			AssemblyQualifiedTypeName tn = AssemblyQualifiedTypeName.Parse( "SomeType, AnotherAssembly", defaultAssembly );
			Assert.AreEqual( "SomeType", tn.Type );
			Assert.AreEqual( "AnotherAssembly", tn.Assembly );
		}

		[Test]
		public void ParseWithDefaultAssembly()
		{
			string defaultAssembly = "SomeAssembly";
			AssemblyQualifiedTypeName tn = AssemblyQualifiedTypeName.Parse( "SomeType", defaultAssembly );
			Assert.AreEqual( "SomeType", tn.Type );
			Assert.AreEqual( defaultAssembly, tn.Assembly );
		}

		[Test]
		public void ParseWithDefaultNamespaceAndAssembly()
		{
			string defaultAssembly = "DefaultAssembly";
			string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = AssemblyQualifiedTypeName.Parse( "SomeType", defaultNamespace, defaultAssembly );
			Assert.AreEqual( "SomeType.DefaultNamespace", tn.Type );
		}

		[Test]
		public void ParseWithDefaultNamespaceNoAssembly()
		{
			string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = AssemblyQualifiedTypeName.Parse( "SomeType", defaultNamespace, null );
			Assert.AreEqual( "SomeType.DefaultNamespace", tn.Type );
			Assert.IsNull( tn.Assembly );
		}

		[Test]
		public void ParseWithDefaultNamespaceUnused()
		{
			string defaultAssembly = "DefaultAssembly";
			string defaultNamespace = "DefaultNamespace";

			AssemblyQualifiedTypeName tn = AssemblyQualifiedTypeName.Parse( "SomeType.SomeNamespace", defaultNamespace, defaultAssembly );
			Assert.AreEqual( "SomeType.SomeNamespace", tn.Type );
		}

		[Test]
		public void ParseTrims()
		{
			CheckInput( "\t  \nSomeType, SomeAssembly\n   \r\t", "SomeType", "SomeAssembly" );
		}

		[Test, ExpectedException( typeof( ArgumentException ) )]
		public void ParseInvalidEscape()
		{
			AssemblyQualifiedTypeName.Parse( "\\" );
		}

		[Test]
		public void ToStringSimple()
		{
			AssemblyQualifiedTypeName tn = new AssemblyQualifiedTypeName( "MyType", null );
			Assert.AreEqual( "MyType", tn.ToString() );
		}

		[Test]
		public void ToStringComplex()
		{
			AssemblyQualifiedTypeName tn = new AssemblyQualifiedTypeName( "MyType", "MyAssembly" );
			Assert.AreEqual( "MyType, MyAssembly", tn.ToString() );
		}

		[Test]
		public void ToStringEscaped()
		{
			AssemblyQualifiedTypeName tn = new AssemblyQualifiedTypeName( "Escaped\\,Type", "Escaped\\,Assembly" );
			Assert.AreEqual( tn.Type + ", " + tn.Assembly, tn.ToString() );
		}
	}
}