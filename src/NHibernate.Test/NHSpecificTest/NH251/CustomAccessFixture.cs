using System.Reflection;

using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH251
{
	// NH-251: Custom access strategy does not work for components
	// SetUp blows up...
	// A quick & dirty workaround is to change the code in Binder.PropertiesFromXML()
	// from:
	// 		else if ( "component".Equals(name) ) 
	//		{
	//			string subpath = path + StringHelper.Dot + propertyName;
	//			System.Type reflectedClass = ReflectHelper.GetGetter( model.PersistentClazz, propertyName ).ReturnType;
	//			value = new Component(model);
	//			BindComponent(subnode, (Component) value, reflectedClass, subpath, true, mappings);
	//		} 
	//
	// ... to:
	//		else if ( "component".Equals(name) ) 
	//		{
	//			string subpath = path + StringHelper.Dot + propertyName;
	//			System.Type reflectedClass = null;
	//			value = new Component(model);
	//			BindComponent(subnode, (Component) value, reflectedClass, subpath, true, mappings);
	//		} 

	[TestFixture]
	public class CustomAccessFixture
	{
		[Test]
		public void ConfigurationIsOK()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource( "NHibernate.Test.NHSpecificTest.NH251.CustomAccessDO.hbm.xml",
				Assembly.GetExecutingAssembly() );

			cfg.BuildSessionFactory();
			cfg.GenerateSchemaCreationScript( new Dialect.MsSql2000Dialect() );
		}
	}
}