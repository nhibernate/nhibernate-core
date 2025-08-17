using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
	/// </remarks>
	public static class SessionFactoryObjectFactory
	{
		private static readonly INHibernateLogger log;

		private static readonly IDictionary<string, ISessionFactory> Instances = new Dictionary<string, ISessionFactory>();
		private static readonly IDictionary<string, ISessionFactory> NamedInstances = new Dictionary<string, ISessionFactory>();

		/// <summary></summary>
		static SessionFactoryObjectFactory()
		{
			log = NHibernateLogger.For(typeof(SessionFactoryObjectFactory));
			log.Debug("initializing class SessionFactoryObjectFactory");
		}

		/// <summary>
		/// Adds an Instance of the SessionFactory to the local "cache".
		/// </summary>
		/// <param name="uid">The identifier of the ISessionFactory.</param>
		/// <param name="name">The name of the ISessionFactory.</param>
		/// <param name="instance">The ISessionFactory.</param>
		/// <param name="properties">The configured properties for the ISessionFactory.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void AddInstance(string uid, string name, ISessionFactory instance, IDictionary<string, string> properties)
		{
			if (log.IsDebugEnabled())
			{
				string nameMsg = ((!string.IsNullOrEmpty(name)) ? name : "unnamed");

				log.Debug("registered: {0}({1})", uid, nameMsg);
			}

			Instances[uid] = instance;
			if (!string.IsNullOrEmpty(name))
			{
				log.Info("Factory name:{0}", name);
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
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void RemoveInstance(string uid, string name, IDictionary<string, string> properties)
		{
			if (!string.IsNullOrEmpty(name))
			{
				log.Info("unbinding factory: {0}", name);
				NamedInstances.Remove(name);
			}
			Instances.Remove(uid);
		}

		/// <summary>
		/// Get an instance of the SessionFactory from the local "cache" identified by name if it
		/// exists, otherwise run the provided factory and return its result.
		/// </summary>
		/// <param name="name">The name of the ISessionFactory.</param>
		/// <param name="instanceBuilder">The ISessionFactory factory to use if the instance is not
		/// found.</param>
		/// <returns>An instantiated ISessionFactory.</returns>
		/// <remarks>
		/// <para>It is the caller responsibility to ensure <paramref name="instanceBuilder"/>
		/// will add and yield a session factory of the requested <paramref name="name"/>.</para>
		/// <para>If the session factory instantiation is performed concurrently outside of a
		/// <c>GetOrAddNamedInstance</c> call, this method may yield an instance of it still being
		/// built, which may lead to threading issues.</para>
		/// </remarks>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static ISessionFactory GetOrBuildNamedInstance(string name, Func<ISessionFactory> instanceBuilder)
		{
			if (instanceBuilder == null)
				throw new ArgumentNullException(nameof(instanceBuilder));

			if (NamedInstances.TryGetValue(name, out var factory))
				return factory;
			return instanceBuilder();
		}

		/// <summary>
		/// Returns a Named Instance of the SessionFactory from the local "cache" identified by name.
		/// </summary>
		/// <param name="name">The name of the ISessionFactory.</param>
		/// <returns>An ISessionFactory if found, <see langword="null" /> otherwise.</returns>
		/// <remarks>If the session factory instantiation is performed concurrently, this method
		/// may yield an instance of it still being built, which may lead to threading issues.
		/// Use <see cref="GetOrBuildNamedInstance(string, Func{ISessionFactory})" /> to get or
		/// built the session factory in such case.</remarks>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static ISessionFactory GetNamedInstance(string name)
		{
			log.Debug("lookup: name={0}", name);
			ISessionFactory factory;
			bool found = NamedInstances.TryGetValue(name, out factory);
			if (!found)
			{
				log.Warn("Not found: {0}", name);
			}
			return factory;
		}

		/// <summary>
		/// Returns an Instance of the SessionFactory from the local "cache" identified by UUID.
		/// </summary>
		/// <param name="uid">The identifier of the ISessionFactory.</param>
		/// <returns>An ISessionFactory if found, <see langword="null" /> otherwise.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static ISessionFactory GetInstance(string uid)
		{
			log.Debug("lookup: uid={0}", uid);
			ISessionFactory factory;
			bool found = Instances.TryGetValue(uid, out factory);
			if (!found)
			{
				log.Warn("Not found: {0}", uid);
			}
			return factory;
		}
	}
}
