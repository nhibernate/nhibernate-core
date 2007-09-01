using System.IO;
using System.Reflection;

namespace NHibernate.Cfg
{
	public class EmbeddedResource
	{
		private readonly Assembly assembly;
		private readonly string name;

		public EmbeddedResource(Assembly assembly, string name)
		{
			this.assembly = assembly;
			this.name = name;
		}

		public Assembly Assembly
		{
			get { return assembly; }
		}

		public string Name
		{
			get { return name; }
		}

		public Stream OpenStream()
		{
			return assembly.GetManifestResourceStream(name);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}

			EmbeddedResource other = obj as EmbeddedResource;

			return other != null
				&& name.Equals(other.Name)
				&& assembly.Equals(other.Assembly);
		}

		public override int GetHashCode()
		{
			return name.GetHashCode() ^ assembly.GetHashCode();
		}
	}
}
