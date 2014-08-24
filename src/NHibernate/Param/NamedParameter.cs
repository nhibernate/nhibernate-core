using NHibernate.Type;

namespace NHibernate.Param
{
	public class NamedParameter
	{
		public NamedParameter(string name, object value, IType type)
		{
			Name = name;
			Value = value;
			Type = type;
		}

		public string Name { get; private set; }
		public object Value { get; internal set; }
		public IType Type { get; internal set; }

		public bool Equals(NamedParameter other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Name, Name);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as NamedParameter);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}
	}
}