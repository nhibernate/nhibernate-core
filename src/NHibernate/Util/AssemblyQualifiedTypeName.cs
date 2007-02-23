using System;

namespace NHibernate.Util
{
	public class AssemblyQualifiedTypeName
	{
		private string type;
		private string assembly;

		public AssemblyQualifiedTypeName(string type, string assembly)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.type = type;
			this.assembly = assembly;
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

			if (other == null) return false;

			return string.Equals(type, other.type)
			       && string.Equals(assembly, other.assembly);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 0;
				if (type != null)
				{
					hashCode += type.GetHashCode();
				}

				if (assembly != null)
				{
					hashCode += assembly.GetHashCode();
				}

				return hashCode;
			}
		}

		public override string ToString()
		{
			if (assembly == null)
			{
				return type;
			}

			return string.Concat(type, ", ", assembly);
		}
	}
}