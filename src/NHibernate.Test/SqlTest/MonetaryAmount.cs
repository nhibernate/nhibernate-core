using System;

namespace NHibernate.Test.SqlTest
{
	[Serializable]
	public class MonetaryAmount : IComparable
	{
		private decimal value;
		private string currency;

		public MonetaryAmount(decimal value, string currency)
		{
			this.value = value;
			this.currency = currency;
		}

		public string Currency
		{
			get { return currency; }
		}

		public decimal Value
		{
			get { return value; }
		}

		// ********************** Common Methods ********************** //

		public override bool Equals(Object o)
		{
			if (this == o)
				return true;
			if (!(o is MonetaryAmount))
				return false;

			MonetaryAmount monetaryAmount = (MonetaryAmount) o;

			if (!currency.Equals(monetaryAmount.currency))
				return false;
			if (!value.Equals(monetaryAmount.value))
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			int result;
			result = value.GetHashCode();
			result = 29 * result + currency.GetHashCode();
			return result;
		}

		public override string ToString()
		{
			return "Value: '" + Value + "', " +
			       "Currency: '" + Currency + "'";
		}

		public int CompareTo(Object o)
		{
			if (o is MonetaryAmount)
			{
				// TODO: This would actually require some currency conversion magic
				return Value.CompareTo(((MonetaryAmount) o).Value);
			}
			return 0;
		}

		// ********************** Business Methods ********************** //

		public static MonetaryAmount FromString(string amount, string currencyCode)
		{
			return new MonetaryAmount(decimal.Parse(amount),
			                          currencyCode);
		}

		public static MonetaryAmount Convert(MonetaryAmount amount,
		                                     string toCurrency)
		{
			// TODO: This requires some conversion magic and is therefore broken
			return new MonetaryAmount(amount.Value, toCurrency);
		}
	}
}