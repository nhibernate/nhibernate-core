using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test 
{
	
	[TestFixture]
	public class BasicTypes : TestCase 
	{

		[SetUp]
		public void SetUp() 
		{
			//log4net.Config.DOMConfigurator.Configure();
			ExportSchema( new string[] { "BasicClass.hbm.xml"}, true );
		}

		[Test]
		public void BasicTypesTest() 
		{
			ISession s1 = sessions.OpenSession();
			ITransaction t1 = s1.BeginTransaction();

			BasicClass basicClass = new BasicClass();
			basicClass.Id = 1;

			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			bf.Serialize(stream, 5);
			basicClass.BinaryProperty = stream.ToArray();

			basicClass.BooleanProperty = true;
			basicClass.ByteProperty = Byte.MaxValue;
			basicClass.CharacterProperty = 'a';
			basicClass.ClassProperty = typeof(object);
			basicClass.CultureInfoProperty = System.Globalization.CultureInfo.CurrentCulture;
			basicClass.DateTimeProperty = DateTime.Parse("2003-12-1");
			basicClass.DecimalProperty = 5.6435M;
			basicClass.DoubleProperty = 456343;
			basicClass.Int16Property = Int16.MaxValue;
			basicClass.Int32Property = Int32.MaxValue;
			basicClass.Int64Property = Int64.MaxValue;
			
			basicClass.SerializableProperty = new SerializableClass();
			basicClass.SerializableProperty.classId = 2;
			basicClass.SerializableProperty.classString = "string";

			basicClass.SingleProperty = Single.MaxValue;
			basicClass.StringProperty = "string property";
			basicClass.TimestampProperty = DateTime.Now;
			basicClass.TrueFalseProperty = true;
			basicClass.YesNoProperty = true;

			basicClass.StringArray = new string[] {"1 string", "2 string", "3 string"};
			basicClass.Int32Array = new int[] {5,4,3,2,1};

			s1.Save(basicClass);

			t1.Commit();
			s1.Close();

			ISession s2 = sessions.OpenSession();
			ITransaction t2 = s2.BeginTransaction();

			BasicClass basicClass2 = (BasicClass)s2.Load(typeof(BasicClass), 1);

			Assertion.AssertNotNull(basicClass2);
			Assertion.AssertEquals(basicClass.SerializableProperty.classId, basicClass2.SerializableProperty.classId);
			Assertion.AssertEquals(basicClass.SerializableProperty.classString, basicClass2.SerializableProperty.classString);
			t2.Commit();
			s2.Close();

			

		}
	
	}

}
