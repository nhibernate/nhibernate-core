using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using Iesi.Collections;

using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Analyzes the contents of the <c>hbm.xml</c> files embedded in the 
	/// <see cref="Assembly"/> for their dependency order.
	/// </summary>
	/// <remarks>
	/// This solves the problem caused when you have embedded <c>hbm.xml</c> files
	/// that contain subclasses/joined-subclasses that make use of the <c>extends</c>
	/// attribute.  This ensures that those subclasses/joined-subclasses will not be
	/// processed until after the class they extend is processed.
	/// </remarks>
	public class AssemblyHbmOrderer
	{
		/*
		 * It almost seems like a better way to handle this would be to have
		 * Binder.BindRoot throw a different exception when a subclass/joined-subclass
		 * couldn't find their base class mapping.  The AddAssembly method could
		 * "queue" up the problem hbm.xml files and run through them at the end.
		 * This method solves the problem WELL enough to get by with for now.
		 */

		private readonly Assembly _assembly;

		/// <summary>
		/// An <see cref="IList"/> of all the <c>hbm.xml</c> resources found
		/// in the assembly.
		/// </summary>
		private readonly List<string> _hbmResources = new List<string>();

		/// <summary>
		/// Creates a new instance of AssemblyHbmOrderer with the specified <paramref name="resources" />
		/// added.
		/// </summary>
		public static AssemblyHbmOrderer CreateWithResources(Assembly assembly, IEnumerable resources)
		{
			AssemblyHbmOrderer result = new AssemblyHbmOrderer(assembly);

			foreach (string resource in resources)
			{
				result.AddResource(resource);
			}

			return result;
		}

		/// <summary>
		/// Creates a new instance of <see cref="AssemblyHbmOrderer"/>
		/// </summary>
		/// <param name="assembly">The <see cref="Assembly"/> to get resource streams from.</param>
		public AssemblyHbmOrderer(Assembly assembly)
		{
			_assembly = assembly;
		}

		/// <summary>
		/// Adds the specified resource to the resources being ordered.
		/// </summary>
		/// <param name="fileName">Name of the file (embedded resource).</param>
		public void AddResource(string fileName)
		{
			_hbmResources.Add(fileName);
		}

		private static ClassEntry BuildClassEntry(XmlReader xmlReader, string fileName, string assembly, string @namespace)
		{
			xmlReader.MoveToAttribute("name");
			string className = xmlReader.Value;

			string extends = null;
			if (xmlReader.MoveToAttribute("extends"))
			{
				extends = xmlReader.Value;
			}

			return new ClassEntry(extends, className, assembly, @namespace, fileName);
		}

		/// <summary>
		/// Gets an <see cref="IList"/> of <c>hbm.xml</c> resources in the correct order.
		/// </summary>
		/// <returns>
		/// An <see cref="IList"/> of <c>hbm.xml</c> resources in the correct order.
		/// </returns>
		public IList<string> GetHbmFiles()
		{
			return GetHbmFiles(new HashedSet());
		}

		/// <summary>
		/// Gets an <see cref="IList"/> of <c>hbm.xml</c> resources in the correct order.
		/// </summary>
		/// <param name="loadedClasses">Classes loaded by some other process.</param>
		/// <returns>
		/// An <see cref="IList"/> of <c>hbm.xml</c> resources in the correct order.
		/// </returns>
		public IList<string> GetHbmFiles(ISet loadedClasses)
		{
			HashedSet classes = new HashedSet();

			// tracks if any hbm.xml files make use of the "extends" attribute
			bool containsExtends = false;
			// tracks any extra files, i.e. those that do not contain a class definition.
			List<string> extraFiles = new List<string>();

			foreach (string fileName in _hbmResources)
			{
				bool fileContainsClasses = false;

				using (Stream xmlInputStream = _assembly.GetManifestResourceStream(fileName))
				{
					// XmlReader does not implement IDisposable on .NET 1.1 so have to use
					// try/finally instead of using here.
					XmlTextReader xmlReader = new XmlTextReader(xmlInputStream);

					string assembly = null;
					string @namespace = null;

					try
					{
						while (xmlReader.Read())
						{
							if (xmlReader.NodeType != XmlNodeType.Element)
							{
								continue;
							}

							switch (xmlReader.Name)
							{
								case "hibernate-mapping":
									assembly = xmlReader.MoveToAttribute("assembly") ? xmlReader.Value : null;
									@namespace = xmlReader.MoveToAttribute("namespace") ? xmlReader.Value : null;
									break;
								case "class":
								case "joined-subclass":
								case "subclass":
								case "union-subclass":
									ClassEntry ce = BuildClassEntry(xmlReader, fileName, assembly, @namespace);
									classes.Add(ce);
									fileContainsClasses = true;
									containsExtends = containsExtends || ce.FullExtends != null;
									break;
							}
						}
					}
					finally
					{
						xmlReader.Close();
					}
				}

				if (!fileContainsClasses)
				{
					extraFiles.Add(fileName);
				}
			}

			// only bother to do the sorting if one of the hbm files uses 'extends' -
			// the sorting does quite a bit of looping through collections so if we don't
			// need to spend the time doing that then don't bother.
			if (containsExtends)
			{
				// Add ordered hbms *after* the extra files, so that the extra files are processed first.
				// This may be useful if the extra files define filters, etc. that are being used by
				// the entity mappings.
				extraFiles.AddRange(OrderedHbmFiles(classes, loadedClasses));

				return extraFiles;
			}
			else
			{
				return _hbmResources;
			}
		}

		private static string FormatExceptionMessage(ISet classEntries)
		{
			StringBuilder message = new StringBuilder("These classes extend unmapped classes:");

			foreach (ClassEntry entry in classEntries)
			{
				message.Append('\n')
					.Append(entry.FullClassName)
					.Append(" extends ")
					.Append(entry.FullExtends);
			}

			return message.ToString();
		}

		/// <summary>
		/// Returns an <see cref="IList"/> of <c>hbm.xml</c> files in the order that ensures
		/// base classes are loaded before their subclass/joined-subclass.
		/// </summary>
		/// <param name="unorderedClasses">An <see cref="ISet"/> of <see cref="ClassEntry"/> objects.</param>
		/// <returns>
		/// An <see cref="IList"/> of <see cref="string"/> objects that contain the <c>hbm.xml</c> file names.
		/// </returns>
		/// <param name="loadedClasses"></param>
		private static IList<string> OrderedHbmFiles(ISet unorderedClasses, ISet loadedClasses)
		{
			// Make sure joined-subclass mappings are loaded after base class
			ArrayList sortedList = new ArrayList();
			ISet processedClassNames = new HashedSet();
			ArrayList processedInThisIteration = new ArrayList();

			while (true)
			{
				foreach (ClassEntry ce in unorderedClasses)
				{
					if (ce.FullExtends == null || processedClassNames.Contains(ce.FullExtends) || loadedClasses.Contains(ce.FullExtends.Type))
					{
						// This class extends nothing, or is derived from one of the classes that were already processed.
						// Append it to the list since it's safe to process now.
						sortedList.Add(ce);
						processedClassNames.Add(ce.FullClassName);
						processedInThisIteration.Add(ce);
					}
				}

				unorderedClasses.RemoveAll(processedInThisIteration);

				if (processedInThisIteration.Count == 0)
				{
					if (!unorderedClasses.IsEmpty)
					{
						throw new MappingException(FormatExceptionMessage(unorderedClasses));
					}
					break;
				}

				processedInThisIteration.Clear();
			}

			// now that we know the order the classes should be loaded - order the
			// hbm.xml files those classes are contained in.
			List<string> loadedFiles = new List<string>();
			foreach (ClassEntry ce in sortedList)
			{
				// Check if this file already loaded (this can happen if
				// we have mappings for multiple classes in one file).
				if (loadedFiles.Contains(ce.FileName) == false)
				{
					loadedFiles.Add(ce.FileName);
				}
			}

			return loadedFiles;
		}

		/// <summary>
		/// Holds information about mapped classes found in the <c>hbm.xml</c> files.
		/// </summary>
		internal class ClassEntry
		{
			private readonly AssemblyQualifiedTypeName _fullExtends;
			private readonly AssemblyQualifiedTypeName _fullClassName;
			private readonly string _fileName;

			public ClassEntry(string extends, string className, string assembly, string @namespace, string fileName)
			{
				_fullExtends = extends == null ? null : TypeNameParser.Parse(extends, @namespace, assembly);
				_fullClassName = TypeNameParser.Parse(className, @namespace, assembly);
				_fileName = fileName;
			}

			public AssemblyQualifiedTypeName FullExtends
			{
				get { return _fullExtends; }
			}

			public AssemblyQualifiedTypeName FullClassName
			{
				get { return _fullClassName; }
			}

			/// <summary>
			/// Gets the name of the <c>hbm.xml</c> file this class was found in.
			/// </summary>
			public string FileName
			{
				get { return _fileName; }
			}
		}
	}
}