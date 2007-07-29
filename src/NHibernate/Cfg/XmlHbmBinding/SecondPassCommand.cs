using System.Collections.Generic;

using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	/// <summary>Used by <see cref="DelegatingSecondPass" /> to allow binders to delay their binding</summary>
	internal delegate void SecondPassCommand(IDictionary<System.Type, PersistentClass> persistentClasses);
}