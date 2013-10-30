
namespace NHibernate.Test.NHSpecificTest.NH1082
{
	public class C
	{
		private int id;
		private string value;

		public virtual string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public virtual int ID
		{
			get { return id; }
			set { id = value; }
		}
	}
}
