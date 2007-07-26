using System.IO;

namespace NHibernate.Cfg.MappingSchema
{
	/// <summary>
	/// Responsible for converting a <see cref="Stream" /> of HBM XML into an instance of
	/// <see cref="hibernatemapping" />.
	/// </summary>
	public interface IMappingDocumentParser
	{
		hibernatemapping Parse(Stream stream);
	}
}