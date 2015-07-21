namespace NHibernate.AdoNet.Util
{
	/// <summary> Represents the the understood types or styles of formatting.  </summary>
	public class FormatStyle
	{
		public static readonly FormatStyle Basic = new FormatStyle("basic", new BasicFormatter());
		public static readonly FormatStyle Ddl = new FormatStyle("ddl", new DdlFormatter());
		public static readonly FormatStyle None = new FormatStyle("none", new NoFormatImpl());

		private FormatStyle(string name, IFormatter formatter)
		{
			Name = name;
			Formatter = formatter;
		}

		public string Name { get; private set; }

		public IFormatter Formatter { get; private set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as FormatStyle);
		}

		public bool Equals(FormatStyle other)
		{
			if (other == null)
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Name, Name);
		}

		public override int GetHashCode()
		{
			return (Name != null ? Name.GetHashCode() : 0);
		}

		#region Nested type: NoFormatImpl

		private class NoFormatImpl : IFormatter
		{
			#region IFormatter Members

			public string Format(string source)
			{
				return source;
			}

			#endregion
		}

		#endregion
	}
}