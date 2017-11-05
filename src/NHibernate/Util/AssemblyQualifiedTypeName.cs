using System;

namespace NHibernate.Util
{
	[Serializable]
	public class AssemblyQualifiedTypeName
	{
		private readonly string _type;
		private readonly string _assembly;
		private readonly int _hashCode;

		public AssemblyQualifiedTypeName(string type, string assembly)
		{
			_type = type ?? throw new ArgumentNullException(nameof(type));
			_assembly = assembly;
			unchecked
			{
				_hashCode = (type.GetHashCode() * 397) ^ (assembly?.GetHashCode() ?? 0);
			}
		}

		public string Type => _type;

		public string Assembly => _assembly;

		public override bool Equals(object obj)
		{
			AssemblyQualifiedTypeName other = obj as AssemblyQualifiedTypeName;
			return Equals(other);
		}

		public override string ToString()
		{
			if (_assembly == null)
			{
				return _type;
			}

			return string.Concat(_type, ", ", _assembly);
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
			return Equals(obj._type, _type) && Equals(obj._assembly, _assembly);
		}

		public override int GetHashCode()
		{
			return _hashCode;
		}
	}
}
