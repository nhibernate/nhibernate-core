namespace NHibernate.Test.NHSpecificTest.NH3932.Model
{
	public class Child
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public override bool Equals(object obj)
		{
			if (Id == 0)
				return base.Equals(obj);
			var that = obj as Child;
			return that?.Id == Id;
		}

		public override int GetHashCode()
		{
			return 0; //for simplicity - care only about equals impl
		}
	}
}