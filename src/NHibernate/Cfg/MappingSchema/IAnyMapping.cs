using System.Collections.Generic;

namespace NHibernate.Cfg.MappingSchema
{
	public interface IAnyMapping
	{
		string MetaType { get; }
		ICollection<HbmMetaValue> MetaValues { get; }
	}
}