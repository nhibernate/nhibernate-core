using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class FumCompositeID 
	{
		String string_;
		DateTime date_;
		short short_;
		
		public override bool Equals(object obj)
		{
			FumCompositeID that = (FumCompositeID) obj;
			return this.string_.Equals(that.string_) && this.short_==that.short_;
		}

		public override int GetHashCode()
		{
			return string_.GetHashCode();
		}

		public string @string
		{
			get
			{
				return string_;
			}
			set
			{
				this.string_ = value;
			}
		}
		public DateTime date
		{
			get
			{
				return date_;
			}
			set
			{
				this.date_ = value;
			}
		}
		public short @short
		{
			get
			{
				return short_;
			}
			set
			{
				this.short_ = value;
			}
		}
	}
}