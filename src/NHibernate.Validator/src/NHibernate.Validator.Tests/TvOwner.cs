namespace NHibernate.Validator.Tests
{
	public class TvOwner
	{
		public int id;

		[NotNull, Valid] public Tv tv;
	}
}