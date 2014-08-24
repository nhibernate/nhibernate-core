namespace NHibernate.Cfg
{
	public class Hbm2DDLKeyWords
	{
		private readonly string value;

		private Hbm2DDLKeyWords(string value)
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
			if (obj.GetType() != typeof(Hbm2DDLKeyWords))
			{
				return false;
			}
			return Equals((Hbm2DDLKeyWords)obj);
		}

		public bool Equals(string other)
		{
			return value.Equals(other);
		}

		public bool Equals(Hbm2DDLKeyWords other)
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

		public static bool operator ==(string a, Hbm2DDLKeyWords b)
		{
			if (ReferenceEquals(null, b))
			{
				return false;
			}
			return b.Equals(a);
		}

		public static bool operator ==(Hbm2DDLKeyWords a, string b)
		{
			return b == a;
		}

		public static bool operator !=(Hbm2DDLKeyWords a, string b)
		{
			return !(a == b);
		}

		public static bool operator !=(string a, Hbm2DDLKeyWords b)
		{
			return !(a == b);
		}

		public static Hbm2DDLKeyWords None = new Hbm2DDLKeyWords("none");
		public static Hbm2DDLKeyWords Keywords = new Hbm2DDLKeyWords("keywords");
		public static Hbm2DDLKeyWords AutoQuote = new Hbm2DDLKeyWords("auto-quote");
	}
}