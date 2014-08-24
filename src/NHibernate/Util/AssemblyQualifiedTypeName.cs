using System;

namespace NHibernate.Util
{
	public class AssemblyQualifiedTypeName
	{
		private readonly string type;
		private readonly string assembly;
		private readonly int hashCode;

		public AssemblyQualifiedTypeName(string type, string assembly)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.type = type;
			this.assembly = assembly;
			unchecked
			{
				hashCode = (type.GetHashCode() * 397) ^ (assembly != null ? assembly.GetHashCode() : 0);
			}
		}

		public string Type
		{
			get { return type; }
		}

		public string Assembly
		{
			get { return assembly; }
		}

		public override bool Equals(object obj)
		{
			AssemblyQualifiedTypeName other = obj as AssemblyQualifiedTypeName;
			return Equals(other);
		}

		public override string ToString()
		{
			if (assembly == null)
			{
				return type;
			}

			return string.Concat(type, ", ", assembly);
		}

		public bool Equals(AssemblyQualifiedTypeName obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj.type, type) && Equals(obj.assembly, assembly);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}