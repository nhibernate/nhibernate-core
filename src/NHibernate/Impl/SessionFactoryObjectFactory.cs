using System.Collections;
using log4net;
using System.Collections.Generic;

namespace NHibernate.Impl
{
	/// <summary>
	/// Resolves <see cref="ISessionFactory"/> lookups and deserialization.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is used heavily be Deserialization.  Currently a SessionFactory is not really serialized. 
	/// All that is serialized is it's name and uid.  During Deserializaiton the serialized SessionFactory
	/// is converted to the one contained in this object.  So if you are serializing across AppDomains
	/// you should make sure that "name" is specified for the SessionFactory in the hbm.xml file and that the
	/// other AppDomain has a configured SessionFactory with the same name.  If
	/// you are serializing in the same AppDomain then there will be no problem because the uid will
	/// be in this object.
	/// </para>
	/// <para>
	/// TODO: verify that the AppDomain statements are correct.
	/// </para>
	/// </remarks>
	public sealed class SessionFactoryObjectFactory
	{
		// to stop this class from being unloaded - this is a comment
		// from h2.0.3 - is this applicable to .net also???
		private static readonly ILog log;

		// in h2.0.3 these use a class called "FastHashMap"
		private static readonly Hashtable Instances = new Hashtable();
		private static readonly Hashtable NamedInstances = new Hashtable();

		/// <summary></summary>
		static SessionFactoryObjectFactory()
		{
			log = LogManager.GetLogger(typeof(SessionFactoryObjectFactory));
			log.Debug("initializing class SessionFactoryObjectFactory");
		}

		private SessionFactoryObjectFactory()
		{
			// should not be created	
		}

		/// <summary>
		/// Adds an Instance of the SessionFactory to the local "cache".
		/// </summary>
		/// <param name="uid">The identifier of the ISessionFactory.</param>
		/// <param name="name">The name of the ISessionFactory.</param>
		/// <param name="instance">The ISessionFactory.</param>
		/// <param name="properties">The configured properties for the ISessionFactory.</param>
		public static void AddInstance(string uid, string name, ISessionFactory instance, IDictionary<string, string> properties)
		{
			if (log.IsDebugEnabled)
			{
				string nameMsg = ((name != null && name.Length > 0) ? name : "unnamed");

				log.Debug("registered: " + uid + "(" + nameMsg + ")");
			}

			Instances[uid] = instance;
			if (name != null && name.Length > 0)
			{
				log.Info("Factory name:" + name);
				NamedInstances[name] = instance;
			}
			else
			{
				log.Info("no name configured");
			}
		}

		/// <summary>
		/// Removes the Instance of the SessionFactory from the local "cache".
		/// </summary>
		/// <param name="uid">The identifier of the ISessionFactory.</param>
		/// <param name="name">The name of the ISessionFactory.</param>
		/// <param name="properties">The configured properties for the ISessionFactory.</param>
		public static void RemoveInstance(string uid, string name, IDictionary<string, string> properties)
		{
			if (name != null && name.Length > 0)
			{
				log.Info("unbinding factory: " + name);

				NamedInstances.Remove(name);
			}

			Instances.Remove(uid);
		}

		/// <summary>
		/// Returns a Named Instance of the SessionFactory from the local "cache" identified by name.
		/// </summary>
		/// <param name="name">The name of the ISessionFactory.</param>
		/// <returns>An instantiated ISessionFactory.</returns>
		public static ISessionFactory GetNamedInstance(string name)
		{
			log.Debug("lookup: name=" + name);
			ISessionFactory factory = NamedInstances[name] as ISessionFactory;
			if (factory == null)
			{
				log.Warn("Not found: " + name);
			}
			return factory;
		}

		/// <summary>
		/// Returns an Instance of the SessionFactory from the local "cache" identified by UUID.
		/// </summary>
		/// <param name="uid">The identifier of the ISessionFactory.</param>
		/// <returns>An instantiated ISessionFactory.</returns>
		public static ISessionFactory GetInstance(string uid)
		{
			log.Debug("lookup: uid=" + uid);
			ISessionFactory factory = Instances[uid] as ISessionFactory;
			if (factory == null)
			{
				log.Warn("Not found: " + uid);
			}
			return factory;
		}
	}
}