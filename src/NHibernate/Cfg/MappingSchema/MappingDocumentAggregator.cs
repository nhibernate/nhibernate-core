using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NHibernate.Cfg.MappingSchema
{
	/// <summary>
	/// Responsible for building a list of <see cref="hibernatemapping" /> objects from a range of acceptable
	/// sources.
	/// </summary>
	public class MappingDocumentAggregator
	{
		private readonly List<hibernatemapping> documents = new List<hibernatemapping>();
		private readonly IMappingDocumentParser parser;

		public MappingDocumentAggregator()
			: this(new MappingDocumentParser())
		{
		}

		public MappingDocumentAggregator(IMappingDocumentParser parser)
		{
			if (parser == null)
				throw new ArgumentNullException("parser");

			this.parser = parser;
		}

		public void Add(hibernatemapping document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			documents.Add(document);
		}

		public void Add(Stream stream)
		{
			hibernatemapping document = parser.Parse(stream);
			Add(document);
		}

		/// <summary>Adds any embedded resource streams whose name ends with ".hbm.xml".</summary>
		/// <param name="assembly">An assembly containing embedded mapping documents.</param>
		public void Add(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			foreach (string resourceName in assembly.GetManifestResourceNames())
				if (resourceName.EndsWith(".hbm.xml"))
					using (Stream stream = assembly.GetManifestResourceStream(resourceName))
						Add(stream);
		}

		public void Add(FileInfo file)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			// TODO: Exception handling...
			// OpenRead() throws: DirectoryNotFoundException, IOException, UnauthorizedAccessException

			using (FileStream stream = file.OpenRead())
				Add(stream);
		}

		public void Add(string fileName)
		{
			// TODO: Exception handling...
			FileInfo file = new FileInfo(fileName);
			Add(file);
		}

		public IList<hibernatemapping> List()
		{
			return documents.ToArray();
		}
	}
}