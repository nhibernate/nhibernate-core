using System;

namespace NHibernate.Test.Subclass.EnumDiscriminator
{
	public class Baz
	{
		private Int64 id;
		private Colors color;

		public virtual Int64 Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Colors Color
		{
			get { return color; }
			set { color = value; }
		}
	}
}