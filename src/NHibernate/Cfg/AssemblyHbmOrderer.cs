using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;
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

		Assembly _assembly;
		
		/// <summary>
		/// An unordered <see cref="IList"/> of all the mapped classes contained
		/// in the assembly.
		/// </summary>
		ArrayList _classes = new ArrayList();
		
		/// <summary>
		/// An <see cref="IList"/> of all the <c>hbm.xml</c> resources found
		/// in the assembly.
		/// </summary>
		ArrayList _hbmResources = new ArrayList();

		/// <summary>
		/// Creates a new instance of AssemblyHbmOrderer with all embedded resources
		/// ending in <c>.hbm.xml</c> added.
		/// </summary>
		public static AssemblyHbmOrderer CreateWithAllResourcesIn(Assembly assembly)
		{
			AssemblyHbmOrderer result = new AssemblyHbmOrderer(assembly);

			foreach( string fileName in assembly.GetManifestResourceNames() )
			{
				if( fileName.EndsWith( ".hbm.xml" ) )
				{
					result.AddResource( fileName );
				}
			}

			return result;
		}

		/// <summary>
		/// Creates a new instance of <see cref="AssemblyHbmOrderer"/>
		/// </summary>
		/// <param name="assembly">The <see cref="Assembly"/> to get resources from.</param>
		public AssemblyHbmOrderer(Assembly assembly) 
		{
			_assembly = assembly;
		}

		public void AddResource(string fileName)
		{
			_hbmResources.Add(fileName);
		}

		/// <summary>
		/// Gets an <see cref="IList"/> of <c>hbm.xml</c> resources in the correct order.
		/// </summary>
		/// <returns>
		/// An <see cref="IList"/> of <c>hbm.xml</c> resources in the correct order.
		/// </returns>
		public IList GetHbmFiles()
		{
			// tracks if any hbm.xml files make use of the "extends" attribute
			bool containsExtends = false;
			// tracks any extra files, i.e. those that do not contain a class definition.
			StringCollection extraFiles = new StringCollection();

			foreach( string fileName in _hbmResources ) 
			{
				bool addedToClasses = false;

				using (Stream xmlInputStream = _assembly.GetManifestResourceStream(fileName))
				{
					// XmlReader does not implement IDisposable on .NET 1.1 so have to use
					// try/finally instead of using here.
					XmlTextReader xmlReader = new XmlTextReader(xmlInputStream);
					try
					{
						while (xmlReader.Read())
						{
							if (xmlReader.NodeType != XmlNodeType.Element)
							{
								continue;
							}

							if (xmlReader.Name == "class")
							{
								xmlReader.MoveToAttribute("name");
								string className = StringHelper.GetClassname(xmlReader.Value);
								ClassEntry ce = new ClassEntry(null, className, fileName);
								_classes.Add(ce);
								addedToClasses = true;
							}
							else if (xmlReader.Name == "joined-subclass" || xmlReader.Name == "subclass")
							{
								xmlReader.MoveToAttribute("name");
								string className = StringHelper.GetClassname(xmlReader.Value);
								if (xmlReader.MoveToAttribute("extends"))
								{
									containsExtends = true;
									string baseClassName = StringHelper.GetClassname(xmlReader.Value);
									ClassEntry ce = new ClassEntry(baseClassName, className, fileName);
									_classes.Add(ce);
									addedToClasses = true;
								}
							}
						}
					}
					finally
					{
						xmlReader.Close();
					}
				}

				if (!addedToClasses)
				{
					extraFiles.Add(fileName);
				}
			}

			// only bother to do the sorting if one of the hbm files uses 'extends' - 
			// the sorting does quite a bit of looping through collections so if we don't
			// need to spend the time doing that then don't bother.
			if( containsExtends )
			{
				string[] extraFilesArray = new string[extraFiles.Count];
				extraFiles.CopyTo(extraFilesArray, 0);
				StringCollection result = OrderedHbmFiles( _classes );
				result.AddRange(extraFilesArray);
				return result;
			}
			else
			{
				return _hbmResources;
			}
		}

		/// <summary>
		/// Returns an <see cref="IList"/> of <c>hbm.xml</c> files in the order that ensures
		/// base classes are loaded before their subclass/joined-subclass.
		/// </summary>
		/// <param name="unorderedClasses">An <see cref="IList"/> of <see cref="ClassEntry"/> objects.</param>
		/// <returns>
		/// An <see cref="IList"/> of <see cref="String"/> objects that contain the <c>hbm.xml</c> file names.
		/// </returns>
		private StringCollection OrderedHbmFiles(IList unorderedClasses)
		{
			// Make sure joined-subclass mappings are loaded after base class
			ArrayList sortedList = new ArrayList();

			foreach( ClassEntry ce in unorderedClasses )
			{
				// this class extends nothing - so put it at the front of
				// the list because it is safe to process at any time.
				if( ce.BaseClassName==null )
				{
					sortedList.Insert( 0, ce );
				}
				else
				{
					int insertIndex = -1;
					
					// try to find this classes base class in the list already
					for( int i=0; i<sortedList.Count; i++ )
					{
						ClassEntry sce = (ClassEntry)sortedList[i];
						
						// base class was found - insert at the index 
						// immediately following it
						if( sce.ClassName == ce.BaseClassName )
						{
							insertIndex = i + 1;
							break;
						}
					}
					
					// This Classes' baseClass was not found in the list so we still don't 
					// know where to insert it.  Check to see if any of the classes that
					// have already been sorted derive from this class. 
					if( insertIndex==-1 )
					{
						for( int j=0; j<sortedList.Count; j++ )
						{
							ClassEntry sce = (ClassEntry)sortedList[j];
							
							// A class already in the sorted list derives from this class so
							// insert this class before the class deriving from it.
							if( sce.BaseClassName == ce.ClassName )
							{
								insertIndex = j;
								break;
							}
						}
					}
					
					// could not find any classes that were subclasses of this one or
					// that this class was a subclass of so it should be inserted at 
					// then end.
					if( insertIndex==-1 )
					{
						// Insert at end
						insertIndex = sortedList.Count;
					}


					sortedList.Insert( insertIndex, ce );
				}
			}

			// now that we know the order the classes should be loaded - order the
			// hbm.xml files those classes are contained in.
			StringCollection loadedFiles = new StringCollection();
			foreach( ClassEntry ce in sortedList )
			{
				// Check if this file already loaded (this can happen if
				// we have mappings for multiple classes in one file).
				if (loadedFiles.Contains( ce.FileName )==false )
				{
					loadedFiles.Add( ce.FileName );
				}
			}

			return loadedFiles;
		}

		/// <summary>
		/// Holds information about mapped classes found in the <c>hbm.xml</c> files.
		/// </summary>
		internal class ClassEntry
		{
			private string _baseClassName;

			private string _className;
			private string _fileName;

			public ClassEntry(string baseClassName, string className, string fileName)   
			{
				_baseClassName = baseClassName;
				_className = className;
				_fileName = fileName;
			}

			/// <summary>
			/// Gets the name of the Class that this Class inherits from, or <c>null</c>
			/// if this does not inherit from any mapped Class.
			/// </summary>
			public string BaseClassName
			{
				get { return _baseClassName; }
			}

			/// <summary>
			/// Gets the name of this Class.
			/// </summary>
			public string ClassName
			{
				get { return _className; }
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
