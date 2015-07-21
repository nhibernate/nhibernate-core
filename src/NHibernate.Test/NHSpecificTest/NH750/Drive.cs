using System;

namespace NHibernate.Test.NHSpecificTest.NH750
{
	public class Drive
	{
		public Drive() : base()
		{
		}

		public Drive(string classFullName)
			: this()
		{
			_classFullName = classFullName;
		}

		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _classFullName;

		public string ClassFullName
		{
			get { return _classFullName; }
			set { _classFullName = value; }
		}

		public override bool Equals(object obj)
		{
			Drive that = obj as Drive;
			if (that != null)
				return that.ClassFullName.Equals(_classFullName);
			else
				return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return _classFullName.GetHashCode();
		}
	}
}