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

		public virtual string FieldInterceptor { get; set; }

		private byte[] _NoSetterImage;

		public virtual byte[] NoSetterImage
		{
			get { return _NoSetterImage; }
			set { _NoSetterImage = value; }
		}

	}
}
