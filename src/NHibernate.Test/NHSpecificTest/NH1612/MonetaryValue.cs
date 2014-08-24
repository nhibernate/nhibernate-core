namespace NHibernate.Test.NHSpecificTest.NH1612
{
	public struct MonetaryValue
	{
		private string _currencySymbol;
		private double _amount;

		public MonetaryValue(string currencySymbol, double amount)
		{
			_currencySymbol = currencySymbol;
			_amount = amount;
		}

		public string CurrencySymbol
		{
			get { return _currencySymbol; }
			private set { _currencySymbol = value; }
		}

		public double Amount
		{
			get { return _amount; }
			private set { _amount = value; }
		}

		public static bool operator ==(MonetaryValue left, MonetaryValue right)
		{
			return left._currencySymbol == right._currencySymbol && left._amount == right._amount;
		}

		public static bool operator !=(MonetaryValue left, MonetaryValue right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return obj is MonetaryValue ? this == (MonetaryValue) obj : false;
		}

		public override int GetHashCode()
		{
			return (_currencySymbol ?? string.Empty).GetHashCode() ^ _amount.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", _currencySymbol, _amount);
		}
	}
}