namespace NHibernate.Cfg
{
	public class SchemaAutoAction
	{
		private readonly string value;

		private SchemaAutoAction(string value)
		{
			this.value = value;
		}

		public override string ToString()
		{
			return value;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(SchemaAutoAction))
			{
				return false;
			}
			return Equals((SchemaAutoAction)obj);
		}

		public bool Equals(string other)
		{
			return value.Equals(other);
		}

		public bool Equals(SchemaAutoAction other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.value, value);
		}

		public override int GetHashCode()
		{
			return (value != null ? value.GetHashCode() : 0);
		}

		public static bool operator ==(string a, SchemaAutoAction b)
		{
			if (ReferenceEquals(null, b))
			{
				return false;
			}
			return b.Equals(a);
		}

		public static bool operator ==(SchemaAutoAction a, string b)
		{
			return b == a;
		}

		public static bool operator !=(SchemaAutoAction a, string b)
		{
			return !(a == b);
		}

		public static bool operator !=(string a, SchemaAutoAction b)
		{
			return !(a == b);
		}

		public static SchemaAutoAction Recreate = new SchemaAutoAction("create-drop");
		public static SchemaAutoAction Create = new SchemaAutoAction("create");
		public static SchemaAutoAction Update = new SchemaAutoAction("update");
		public static SchemaAutoAction Validate = new SchemaAutoAction("validate");
	}
}