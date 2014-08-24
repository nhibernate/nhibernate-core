namespace NHibernate.Test.Operations
{
	public class PersonalDetails
	{
		private Person person;
		public virtual long Id { get; set; }
		public virtual string SomePersonalDetail { get; set; }

		public virtual Person Person
		{
			get { return person; }
			set
			{
				person = value;
				person.Details = this;
			}
		}
	}
}