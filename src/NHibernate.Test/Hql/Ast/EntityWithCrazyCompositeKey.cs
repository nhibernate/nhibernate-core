namespace NHibernate.Test.Hql.Ast
{
	public class EntityWithCrazyCompositeKey
	{
		private CrazyCompositeKey id;
		private string name;

		public virtual CrazyCompositeKey Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}