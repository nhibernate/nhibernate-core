using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NHibernate.Cfg.MappingSchema
{
	/// <summary>
	/// Responsible for building a list of <see cref="HbmMapping" /> objects from a range of acceptable
	/// sources.
	/// </summary>
	public class MappingDocumentAggregator
	{
		private readonly IAssemblyResourceFilter defaultFilter;
		private readonly List<HbmMapping> documents = new List<HbmMapping>();
		private readonly IMappingDocumentParser parser;

		/// <summary>
		/// Calls the greedy constructor, passing it new instances of <see cref="MappingDocumentParser" /> and
		/// <see cref="EndsWithHbmXmlFilter" />.
		/// </summary>
		public MappingDocumentAggregator()
			: this(new MappingDocumentParser(), new EndsWithHbmXmlFilter())
		{
		}

		public MappingDocumentAggregator(IMappingDocumentParser parser, IAssemblyResourceFilter defaultFilter)
		{
			if (parser == null)
				throw new ArgumentNullException("parser");

			if (defaultFilter == null)
				throw new ArgumentNullException("defaultFilter");

			this.parser = parser;
			this.defaultFilter = defaultFilter;
		}

		public void Add(HbmMapping document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			documents.Add(document);
		}

		public void Add(Stream stream)
		{
			HbmMapping document = parser.Parse(stream);
			Add(document);
		}

		public void Add(Assembly assembly, string resourceName)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				Add(stream);
		}

		/// <summary>Adds any embedded resource streams which pass the <paramref name="filter"/>.</summary>
		/// <param name="assembly">An assembly containing embedded mapping documents.</param>
		/// <param name="filter">A custom filter.</param>
		public void Add(Assembly assembly, IAssemblyResourceFilter filter)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			if (filter == null)
				throw new ArgumentNullException("filter");

			foreach (string resourceName in assembly.GetManifestResourceNames())
				if (defaultFilter.ShouldParse(resourceName))
					Add(assembly, resourceName);
		}

		/// <summary>Adds any embedded resource streams which pass the default filter.</summary>
		/// <param name="assembly">An assembly containing embedded mapping documents.</param>
		public void Add(Assembly assembly)
		{
			Add(assembly, defaultFilter);
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

		public IList<HbmMapping> List()
		{
			return documents.ToArray();
		}
	}
}