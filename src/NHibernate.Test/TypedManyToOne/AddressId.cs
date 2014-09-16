using System;

namespace NHibernate.Test.TypedManyToOne
{
	[Serializable]
	public class AddressId
	{
		private string _type;
		private int? requestedHash;

		public virtual String Type 
		{
			get { return _type; }
			set 
			{
				if (!string.IsNullOrWhiteSpace(value))
					_type = value.Trim();
				else
					_type = value;
			}
		}
		public virtual String Id { get; set; }

		public AddressId(String type, String id)
		{
			Id = id;
			_type = type;
		}

		public AddressId() { }

		public override bool Equals(object obj)
		{
			return Equals(obj as AddressId);
		}

		public virtual bool Equals(AddressId other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return other.Id == Id && Equals(other.Type, Type);
		}

		public override int GetHashCode()
		{
			if (!requestedHash.HasValue)
			{
				unchecked
				{
					requestedHash = (Id.GetHashCode() * 397) ^ Type.GetHashCode();
				}
			}
			return requestedHash.Value;
		}

		public override string ToString()
		{
			return Type + '#' + Id;
		}
	}
}
