using System.Collections.Generic;
using NHibernate.Mapping;

namespace NHibernate.Cfg.MappingSchema
{
	public interface IDecoratable
	{
		IDictionary<string, MetaAttribute> MappedMetaData { get; }
		IDictionary<string, MetaAttribute> InheritableMetaData { get; }
	}
}