using System;
using NHibernate.Util;
using NHibernate.Type;
using NHibernate.Mapping;
using NHibernate.Cache;
using NHibernate.Dialect;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Summary description for Configuration.
	/// </summary>
	public class Configuration
	{
		public Configuration()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public PersistentClass GetClassMapping(System.Type persistentClass) {
			return null;
		}
	}
}
