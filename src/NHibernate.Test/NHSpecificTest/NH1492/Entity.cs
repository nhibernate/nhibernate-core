using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1492
{
	public class Entity
	{
		private IList<ChildEntity> _childs = new List<ChildEntity>();
		private int _code;
		private string _deleted;
		private string _description;
		private int _id;
		public Entity() {}

		public Entity(int code, string description)
		{
			_code = code;
			_description = description;
		}

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual int Code
		{
			get { return _code; }
			set { _code = value; }
		}

		public virtual string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public virtual string Deleted
		{
			get { return _deleted; }
			set { _deleted = value; }
		}

		public virtual IList<ChildEntity> Childs
		{
			get { return _childs; }
			set { _childs = value; }
		}
	}
}