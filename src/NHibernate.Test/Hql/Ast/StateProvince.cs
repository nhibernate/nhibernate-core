namespace NHibernate.Test.Hql.Ast
{
	public class StateProvince
	{
		private long id;
		private string name;
		private string isoCode;

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

		public virtual string IsoCode
		{
			get { return isoCode; }
			set { isoCode = value; }
		}
	}
}