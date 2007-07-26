using System;
using System.IO;
using System.Xml.Serialization;

namespace NHibernate.Cfg.MappingSchema
{
	public class MappingDocumentParser : IMappingDocumentParser
	{
		private readonly XmlSerializer serializer = new XmlSerializer(typeof (hibernatemapping));

		public hibernatemapping Parse(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			return (hibernatemapping) serializer.Deserialize(stream);

			// TODO: What if Deserialize() throws an exception? Can we provide the user with any more useful
			// information? Can we validate the stream against the XSD, and show any validation errors?
		}
	}
}