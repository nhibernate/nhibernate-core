using System;
using System.Xml.XPath;

namespace NHibernate.Cfg.ConfigurationSchema
{
	/// <summary>
	/// Configuration parsed values for a mapping XML node
	/// </summary>
	/// <remarks>
	/// There are 3 possible combinations of mapping attributes
	/// 1 - resource and assembly:  NHibernate will read the mapping resource from the specified assembly
	/// 2 - file only: NHibernate will read the mapping from the file.
	/// 3 - assembly only: NHibernate will find all the resources ending in hbm.xml from the assembly.
	/// </remarks>
	public class MappingConfiguration: IEquatable<MappingConfiguration>
	{
		internal MappingConfiguration(XPathNavigator mappingElement)
		{
			Parse(mappingElement);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingConfiguration"/> class.
		/// </summary>
		/// <param name="file">Mapped file.</param>
		/// <exception cref="ArgumentException">When <paramref name="file"/> is null or empty.</exception>
		public MappingConfiguration(string file)
		{
			if (string.IsNullOrEmpty(file))
				throw new ArgumentException("file is null or empty.", "file");
			this.file = file;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MappingConfiguration"/> class.
		/// </summary>
		/// <param name="assembly">The assembly name.</param>
		/// <param name="resource">The mapped embedded resource.</param>
		/// <exception cref="ArgumentException">When <paramref name="assembly"/> is null or empty.</exception>
		public MappingConfiguration(string assembly, string resource)
		{
			if (string.IsNullOrEmpty(assembly))
				throw new ArgumentException("assembly is null or empty.", "assembly");
			this.assembly = assembly;
			this.resource = resource;
		}

		private bool IsValid()
		{
			// validate consistent (all empty ignore the element)
			return (!string.IsNullOrEmpty(assembly) && string.IsNullOrEmpty(file)) ||
							(!string.IsNullOrEmpty(file) && string.IsNullOrEmpty(assembly)) ||
							(!string.IsNullOrEmpty(resource) && !string.IsNullOrEmpty(assembly)) ||
							IsEmpty();
		}

		private void Parse(XPathNavigator mappingElement)
		{
			if (mappingElement.MoveToFirstAttribute())
			{
				do
				{
					switch (mappingElement.Name)
					{
						case "assembly":
							assembly = mappingElement.Value;
							break;
						case "resource":
							resource = mappingElement.Value;
							break;
						case "file":
							file = mappingElement.Value;
							break;
					}
				}
				while (mappingElement.MoveToNextAttribute());
			}
			if (!IsValid())
			{
				throw new HibernateConfigException(string.Format(
@"Ambiguous mapping tag in configuration assembly={0} resource={1} file={2};
There are 3 possible combinations of mapping attributes
	1 - resource & assembly:  NHibernate will read the mapping resource from the specified assembly
	2 - file only: NHibernate will read the mapping from the file.
	3 - assembly only: NHibernate will find all the resources ending in hbm.xml from the assembly.",
					assembly,resource,file));
			}
		}

		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(resource) && 
				string.IsNullOrEmpty(assembly) && 
				string.IsNullOrEmpty(file);
		}

		private string file;
		public string File
		{
			get { return file; }
		}

		private string assembly;
		public string Assembly
		{
			get { return assembly; }
		}

		private string resource;
		public string Resource
		{
			get { return resource; }
		}

		#region IEquatable<MappingConfiguration> Members
		public bool Equals(MappingConfiguration other)
		{
			if (other == null)
				return false;

			// file assigned and equals
			if (!string.IsNullOrEmpty(this.file) && !string.IsNullOrEmpty(other.file))
				return this.file.Equals(other.file);

			// this or other have only assembly assigned (include an assembly mean include all its resources)
			if ((!string.IsNullOrEmpty(this.assembly) && !string.IsNullOrEmpty(other.assembly)) &&
				(string.IsNullOrEmpty(this.resource) || string.IsNullOrEmpty(other.resource)))
				return this.assembly.Equals(other.assembly);

			// this and other have both assembly&resource assigned
			if (!string.IsNullOrEmpty(this.assembly) && !string.IsNullOrEmpty(this.resource) &&
				!string.IsNullOrEmpty(other.assembly) && !string.IsNullOrEmpty(other.resource))
				return this.assembly.Equals(other.assembly) && this.resource.Equals(other.resource);

			// invalid or empty
			return false;
		}
		#endregion

		public override string ToString()
		{
			return string.Format("file='{0}';assembly='{1}';resource='{2}'",file,assembly,resource);
		}
	}
}
