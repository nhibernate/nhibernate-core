namespace NHibernate.Test.TypesTest
{
	public class CharClass
	{
		public int Id { get; set; }
		public virtual char NormalChar { get; set; }
		public virtual char? NullableChar { get; set; }
		public virtual string AnsiString { get; set; }
		public virtual char AnsiChar { get; set; }
	}
}
