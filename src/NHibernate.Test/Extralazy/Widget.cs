namespace NHibernate.Test.Extralazy
{
	public class Widget
	{
		private string title;
		private string description;
		private User owner;

		protected Widget() { }
		public Widget(string title, string description, User owner)
		{
			this.title = title;
			this.description = description;
			this.owner = owner;
		}

		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		public virtual User Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		public override bool Equals(object obj)
		{
			var other = obj as Widget;

			if (other == null)
			{
				return false;
			}

			return Title.Equals(other.Title);
		}

		public static bool operator ==(Widget lhs, Widget rhs)
		{
			return Equals(lhs, rhs);
		}

		public static bool operator !=(Widget lhs, Widget rhs)
		{
			return !Equals(lhs, rhs);
		}

		public override int GetHashCode()
		{
			return Title.GetHashCode();
		}
	}
}
