using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3332
{
	public class Culture
	{
		private ISet<DataTypeDescription> _dataTypeDescriptions = new HashSet<DataTypeDescription>();
		private ISet<StateDescription> _stateDescriptions = new HashSet<StateDescription>();
		private String _countryCode;
		private String _languageCode;
		private Byte[] _rowVersionId;

		public override int GetHashCode()
		{
			int toReturn = base.GetHashCode();
			toReturn ^= CountryCode.GetHashCode();
			toReturn ^= LanguageCode.GetHashCode();
			return toReturn;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var toCompareWith = obj as Culture;
			return toCompareWith != null &&
				   CountryCode == toCompareWith.CountryCode &&
				   LanguageCode == toCompareWith.LanguageCode;
		}

		public virtual System.String CountryCode
		{
			get { return _countryCode; }
			set { _countryCode = value; }
		}

		public virtual System.String LanguageCode
		{
			get { return _languageCode; }
			set { _languageCode = value; }
		}

		public virtual System.Byte[] RowVersionId
		{
			get { return _rowVersionId; }
		}

		public virtual ISet<DataTypeDescription> DataTypeDescriptions
		{
			get { return _dataTypeDescriptions; }
			set { _dataTypeDescriptions = value; }
		}

		public virtual ISet<StateDescription> StateDescriptions
		{
			get { return _stateDescriptions; }
			set { _stateDescriptions = value; }
		}
	}
}
