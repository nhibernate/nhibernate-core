namespace NHibernate.Validator.Tests.Integration
{
	public class MarsAddress
	{
		private string canal;
		private string continent;

		[Length(Min = 5)]
		public string Canal
		{
			get { return canal; }
			set { canal = value; }
		}

		[NotNull]
		public string Continent
		{
			get { return continent; }
			set { continent = value; }
		}
	}
}