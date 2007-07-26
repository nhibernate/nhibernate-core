using System.IO;

namespace NHibernate.Cfg.MappingSchema
{
	public interface IMappingDocumentParser
	{
		hibernatemapping Parse(Stream stream);
	}
}