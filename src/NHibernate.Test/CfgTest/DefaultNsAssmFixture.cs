using System;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	/// <summary>
	/// Test fixture for NH-185
	/// </summary>
	[TestFixture]
	public class DefaultNsAssmFixture 
	{ 
		// Working directory
		private static string dir_; 

		// Base class mapping
		private const string aJoinedHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"" namespace=""NHibernate.Test.CfgTest"" assembly=""NHibernate.Test"">
          <class name=""A"" table=""A"">
            <id name=""Id"">
              <generator class=""assigned"" />
            </id>
          </class>
        </hibernate-mapping>";

		// Derived class mapping
		private const string bJoinedHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"" namespace=""NHibernate.Test.CfgTest"">
          <joined-subclass 
              name=""B, NHibernate.Test""
              extends=""NHibernate.Test.CfgTest.A, NHibernate.Test""
              table=""B"">
            <key column=""Id"" />
            <property name=""Value"" type=""Int32"" />
          </joined-subclass>
        </hibernate-mapping>";

		// Add another level
		private const string cJoinedHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"" assembly=""NHibernate.Test"">
          <joined-subclass 
              name=""NHibernate.Test.CfgTest.C""
              extends=""NHibernate.Test.CfgTest.B, NHibernate.Test""
              table=""B"">
            <key column=""Id"" />
            <property name=""Description"" type=""String"" />
          </joined-subclass>
        </hibernate-mapping>";

		private const string aSubclassHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"" namespace=""NHibernate.Test.CfgTest"" assembly=""NHibernate.Test"">
          <class name=""A"" table=""A"" discriminator-value=""0"">
            <id name=""Id"">
              <generator class=""assigned"" />
            </id>
			<discriminator column=""disc_col"" type=""Int32"" />
          </class>
        </hibernate-mapping>";

		// Derived class mapping
		private const string bSubclassHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"">
          <joined-subclass 
              name=""NHibernate.Test.CfgTest.B, NHibernate.Test""
              extends=""NHibernate.Test.CfgTest.A, NHibernate.Test""
              table=""B""
			  discriminator-value=""1"">
            <key column=""Id"" />
            <property name=""Value"" type=""Int32"" />
          </joined-subclass>
        </hibernate-mapping>";

		// Add another level
		private const string cSubclassHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"" namespace=""NHibernate.Test.CfgTest"" assembly=""NHibernate.Test"">
          <joined-subclass 
              name=""NHibernate.Test.CfgTest.C, NHibernate.Test""
              extends=""B""
              table=""B""
			  discriminator-value=""2"">
            <key column=""Id"" />
            <property name=""Description"" type=""String"" />
          </joined-subclass>
        </hibernate-mapping>";

		#region NUnit.Framework.TestFixture Members

		[TestFixtureSetUp]
		public void TestFixtureSetUp() 
		{
			dir_ = Directory.GetCurrentDirectory();

			// Create hbm files (ideally, we could just embed them directly into the
			// assembly - same as VS does when 'Build Action' = 'Embedded Resource' - but
			// I could not find a way to do this, so we use files instead)

			StreamWriter aw = new StreamWriter( "A1.hbm.xml" );
			aw.Write( aJoinedHbmXml );
			aw.Close();

			StreamWriter bw = new StreamWriter( "B1.hbm.xml" );
			bw.Write( bJoinedHbmXml );
			bw.Close();

			StreamWriter cw = new StreamWriter( "C1.hbm.xml" );
			cw.Write( cJoinedHbmXml );
			cw.Close();

			StreamWriter asw = new StreamWriter( "A1.subclass.hbm.xml" );
			asw.Write( aJoinedHbmXml );
			asw.Close();

			StreamWriter bsw = new StreamWriter( "B1.subclass.hbm.xml" );
			bsw.Write( bJoinedHbmXml );
			bsw.Close();

			StreamWriter csw = new StreamWriter( "C1.subclass.hbm.xml" );
			csw.Write( cJoinedHbmXml );
			csw.Close();
		}

		[SetUp]
		public virtual void SetUp() 
		{
		}

		[TearDown]
		public virtual void TearDown()
		{
		}

		[TestFixtureTearDown]
		public virtual void TestFixtureTearDown() 
		{
		}

		#endregion
		
		[Test]
		public void TopDownJoined() 
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "MyTestA1.dll";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name, true);
			assemblyBuilder.AddResourceFile("A.hbm.xml", "A1.hbm.xml");
			assemblyBuilder.AddResourceFile("B.hbm.xml", "B1.hbm.xml");
			assemblyBuilder.AddResourceFile("C.hbm.xml", "C1.hbm.xml");
			assemblyBuilder.Save(assemblyName.Name);

			Configuration cfg = new Configuration();
			cfg.AddAssembly( Assembly.LoadFile(dir_ + "/" + assemblyName.Name) );
			// if no exception, success
		}
				
		[Test]
		public void BottomUpJoined() 
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "MyTestB1.dll";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name, true);
			assemblyBuilder.AddResourceFile("C.hbm.xml", "C1.hbm.xml");
			assemblyBuilder.AddResourceFile("B.hbm.xml", "B1.hbm.xml");
			assemblyBuilder.AddResourceFile("A.hbm.xml", "A1.hbm.xml");
			assemblyBuilder.Save(assemblyName.Name);

			Configuration cfg = new Configuration();
			cfg.AddAssembly( Assembly.LoadFile(dir_ + "/" + assemblyName.Name) );
			// if no exception, success
		}

		[Test]
		public void MixedJoined() 
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "MyTestC1.dll";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name, true);
			assemblyBuilder.AddResourceFile("B.hbm.xml", "B1.hbm.xml");
			assemblyBuilder.AddResourceFile("A.hbm.xml", "A1.hbm.xml");
			assemblyBuilder.AddResourceFile("C.hbm.xml", "C1.hbm.xml");
			assemblyBuilder.Save(assemblyName.Name);

			Configuration cfg = new Configuration();
			cfg.AddAssembly( Assembly.LoadFile(dir_ + "/" + assemblyName.Name) );
			// if no exception, success
		}

		[Test]
		public void MixedSubclass() 
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "MyTestCSubclass1.dll";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name, true);
			assemblyBuilder.AddResourceFile("B.subclass.hbm.xml", "B1.hbm.xml");
			assemblyBuilder.AddResourceFile("A.subclass.hbm.xml", "A1.hbm.xml");
			assemblyBuilder.AddResourceFile("C.subclass.hbm.xml", "C1.hbm.xml");
			assemblyBuilder.Save(assemblyName.Name);

			Configuration cfg = new Configuration();
			cfg.AddAssembly( Assembly.LoadFile(dir_ + "/" + assemblyName.Name) );
			// if no exception, success
		}
	}
}

