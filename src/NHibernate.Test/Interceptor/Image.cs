namespace NHibernate.Test.Interceptor
{
	public class Image
	{
		private Detail details;
		private long id;
		private string name;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual Detail Details
		{
			get { return details; }
			set { details = value; }
		}

		public override string ToString()
		{
			return "Image/" + (details == null ? "no details" : details.ToString());
		}

		#region Nested type: Detail

		public class Detail
		{
			private string comment;
			private long perm1 = -1; // all bits turned on.

			public virtual long Perm1
			{
				get { return perm1; }
				set { perm1 = value; }
			}

			public virtual string Comment
			{
				get { return comment; }
				set { comment = value; }
			}

			public override string ToString()
			{
				return "Details=" + perm1;
			}
		}

		#endregion
	}
}