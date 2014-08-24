namespace NHibernate.Test.NHSpecificTest.NH2148
{
	public class Book: IBook
	{
		public virtual int Id { get; set; }
		public virtual string ALotOfText { get; set; }
		public virtual string SomeMethod(string arg)
		{
			return arg;
		}
	}

	public interface IBook
	{
		string SomeMethod(string arg);
	}
}