namespace NHibernate.Validator.Tests.Integration
{
	public class MarsAddress
	{
		private string canal;
		private string continent;

		public MarsAddress()
		{
		}

		public MarsAddress(string canal, string continent)
		{
			this.canal = canal;
			this.continent = continent;
		}

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