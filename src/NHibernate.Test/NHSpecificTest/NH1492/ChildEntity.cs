namespace NHibernate.Test.NHSpecificTest.NH1492
{
	public class ChildEntity
	{
		private string _description;
		private int _id;
		private Entity _parent;
		public ChildEntity() {}

		public ChildEntity(Entity parent, string description)
		{
			_description = description;
			if (parent != null)
			{
				_parent = parent;
				_parent.Childs.Add(this);
			}
		}

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public virtual Entity Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}
	}
}