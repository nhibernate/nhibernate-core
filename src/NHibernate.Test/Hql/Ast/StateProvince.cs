namespace NHibernate.Test.Hql.Ast
{
	public class StateProvince
	{
		private long id;
		private string name;
		private string isoCode;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string IsoCode
		{
			get { return isoCode; }
			set { isoCode = value; }
		}

		public override bool Equals(object obj)
		{
			if (!(obj is StateProvince))
				return false;

			var stateProvince = ((StateProvince)obj);
			if (Name == null ^ stateProvince.Name == null)
			{
				return false;
			} 

			if (Name != null && stateProvince.Name != null && !stateProvince.Name.Equals(Name))
			{
				return false;
			}

			if (IsoCode == null ^ stateProvince.IsoCode == null)
			{
				return false;
			} 

			if (IsoCode != null && stateProvince.IsoCode != null && !stateProvince.IsoCode.Equals(IsoCode))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int result = (name != null ? name.GetHashCode() : 0);
			result = 31 * result + (isoCode != null ? isoCode.GetHashCode() : 0);
			return result;
		}
	}
}