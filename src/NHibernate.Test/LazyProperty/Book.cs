namespace NHibernate.Test.LazyProperty
{
	public class Book
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		private string _aLotOfText;

		public virtual string ALotOfText
		{
			get { return _aLotOfText; }
			set { _aLotOfText = value; }
		}
	}
}