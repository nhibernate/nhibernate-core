using System;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	#region Classes used by the mappings defined in the Dynamic Assembly

	public class A
	{
		public int Id
		{
			get { return id_; }
			set { id_ = value; }
		}

		private int id_;
	}

	public class B : A
	{
		public int Value
		{
			get { return value_; }
			set { value_ = value; }
		}

		private int value_;
	}

	public class C : B
	{
		public int Description
		{
			get { return description_; }
			set { description_ = value; }
		}

		private int description_;
	}

	#endregion

	/// <summary>
	/// Test fixture for NH-178
	/// </summary>
	[TestFixture]
	public class HbmOrderingFixture 
	{ 
		// Working directory
		private static string dir_; 

		// Base class mapping
		private const string aJoinedHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"">
          <class name=""NHibernate.Test.CfgTest.A, NHibernate.Test"" table=""A"">
            <id name=""Id"">
              <generator class=""assigned"" />
            </id>
          </class>
        </hibernate-mapping>";

		// Derived class mapping
		private const string bJoinedHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"">
          <joined-subclass 
              name=""NHibernate.Test.CfgTest.B, NHibernate.Test""
              extends=""NHibernate.Test.CfgTest.A, NHibernate.Test""
              table=""B"">
            <key column=""Id"" />
            <property name=""Value"" type=""Int32"" />
          </joined-subclass>
        </hibernate-mapping>";

		// Add another level
		private const string cJoinedHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"">
          <joined-subclass 
              name=""NHibernate.Test.CfgTest.C, NHibernate.Test""
              extends=""NHibernate.Test.CfgTest.B, NHibernate.Test""
              table=""B"">
            <key column=""Id"" />
            <property name=""Description"" type=""String"" />
          </joined-subclass>
        </hibernate-mapping>";

		private const string aSubclassHbmXml = 
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"">
          <class name=""NHibernate.Test.CfgTest.A, NHibernate.Test"" table=""A"" discriminator-value=""0"">
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
        <hibernate-mapping xmlns=""urn:nhibernate-mapping-2.0"">
          <joined-subclass 
              name=""NHibernate.Test.CfgTest.C, NHibernate.Test""
              extends=""NHibernate.Test.CfgTest.B, NHibernate.Test""
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

			StreamWriter aw = new StreamWriter( "A.hbm.xml" );
			aw.Write( aJoinedHbmXml );
			aw.Close();

			StreamWriter bw = new StreamWriter( "B.hbm.xml" );
			bw.Write( bJoinedHbmXml );
			bw.Close();

			StreamWriter cw = new StreamWriter( "C.hbm.xml" );
			cw.Write( cJoinedHbmXml );
			cw.Close();

			StreamWriter asw = new StreamWriter( "A.subclass.hbm.xml" );
			asw.Write( aJoinedHbmXml );
			asw.Close();

			StreamWriter bsw = new StreamWriter( "B.subclass.hbm.xml" );
			bsw.Write( bJoinedHbmXml );
			bsw.Close();

			StreamWriter csw = new StreamWriter( "C.subclass.hbm.xml" );
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
			assemblyName.Name = "MyTestA.dll";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name, true);
			assemblyBuilder.AddResourceFile("A.hbm.xml", "A.hbm.xml");
			assemblyBuilder.AddResourceFile("B.hbm.xml", "B.hbm.xml");
			assemblyBuilder.AddResourceFile("C.hbm.xml", "C.hbm.xml");
			assemblyBuilder.Save(assemblyName.Name);

			Configuration cfg = new Configuration();
			cfg.AddAssembly( Assembly.LoadFile(dir_ + "/" + assemblyName.Name) );
			// if no exception, success
		}
				
		[Test]
		public void BottomUpJoined() 
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "MyTestB.dll";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name, true);
			assemblyBuilder.AddResourceFile("C.hbm.xml", "C.hbm.xml");
			assemblyBuilder.AddResourceFile("B.hbm.xml", "B.hbm.xml");
			assemblyBuilder.AddResourceFile("A.hbm.xml", "A.hbm.xml");
			assemblyBuilder.Save(assemblyName.Name);

			Configuration cfg = new Configuration();
			cfg.AddAssembly( Assembly.LoadFile(dir_ + "/" + assemblyName.Name) );
			// if no exception, success
		}

		[Test]
		public void MixedJoined() 
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "MyTestC.dll";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name, true);
			assemblyBuilder.AddResourceFile("B.hbm.xml", "B.hbm.xml");
			assemblyBuilder.AddResourceFile("A.hbm.xml", "A.hbm.xml");
			assemblyBuilder.AddResourceFile("C.hbm.xml", "C.hbm.xml");
			assemblyBuilder.Save(assemblyName.Name);

			Configuration cfg = new Configuration();
			cfg.AddAssembly( Assembly.LoadFile(dir_ + "/" + assemblyName.Name) );
			// if no exception, success
		}

		[Test]
		public void MixedSubclass() 
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "MyTestCSubclass.dll";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name, true);
			assemblyBuilder.AddResourceFile("B.subclass.hbm.xml", "B.hbm.xml");
			assemblyBuilder.AddResourceFile("A.subclass.hbm.xml", "A.hbm.xml");
			assemblyBuilder.AddResourceFile("C.subclass.hbm.xml", "C.hbm.xml");
			assemblyBuilder.Save(assemblyName.Name);

			Configuration cfg = new Configuration();
			cfg.AddAssembly( Assembly.LoadFile(dir_ + "/" + assemblyName.Name) );
			// if no exception, success
		}
	}
}

