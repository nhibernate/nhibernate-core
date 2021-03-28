namespace NHibernate.Test.NHSpecificTest.GH2552
{
	public abstract class Person
	{
		private Details _details;

		public virtual int Id { get; protected set; }
		public virtual string Name { get; set; }

		public virtual Details Details
		{
			get { return _details; }
			set
			{
				_details = value;

				if (_details != null)
				{
					_details.Person = this;
				}
			}
		}
	}

	public class PersonByFK : Person { }

	public class PersonByRef : Person { }
}
