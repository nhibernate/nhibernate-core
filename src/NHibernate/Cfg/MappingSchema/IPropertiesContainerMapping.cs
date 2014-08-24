using System.Collections.Generic;
namespace NHibernate.Cfg.MappingSchema
{
	public interface IPropertiesContainerMapping
	{
		IEnumerable<IEntityPropertyMapping> Properties { get; }
	}
}