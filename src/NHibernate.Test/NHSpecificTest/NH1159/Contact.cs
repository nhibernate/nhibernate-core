using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH1159
{
	[Serializable]
	public partial class Contact
	{

		private bool isChanged;
		private Int64 id;
		private string forename;
		private string surname;
		private string preferredname;

		public Contact()
		{
			this.id = 0;
			this.forename = String.Empty;
			this.surname = String.Empty;
			this.preferredname = String.Empty;
		}

		public Contact(
			string forename,
			string surname)
			: this()
		{
			this.forename = forename;
			this.surname = surname;
			this.preferredname = String.Empty;
		}


		#region Public Properties

		public virtual Int64 Id
		{
			get { return id; }
			set
			{
				isChanged |= (id != value);
				id = value;
			}

		}

		public virtual string Forename
		{
			get { return forename; }

			set
			{
				if (value == null)
					throw new ArgumentOutOfRangeException("Null value not allowed for Forename", value, "null");

				if (value.Length > 50)
					throw new ArgumentOutOfRangeException("Invalid value for Forename", value, value.ToString());

				isChanged |= (forename != value); forename = value;
			}
		}

		public virtual string Surname
		{
			get { return surname; }

			set
			{
				if (value == null)
					throw new ArgumentOutOfRangeException("Null value not allowed for Surname", value, "null");

				if (value.Length > 50)
					throw new ArgumentOutOfRangeException("Invalid value for Surname", value, value.ToString());

				isChanged |= (surname != value); surname = value;
			}
		}

		public virtual string PreferredName
		{
			get { return preferredname; }

			set
			{
				if (value != null && value.Length > 50)
					throw new ArgumentOutOfRangeException("Invalid value for PreferredName", value, value.ToString());

				isChanged |= (preferredname != value); preferredname = value;
			}
		}

		/// <summary>
		/// Returns whether or not the object has changed it's values.
		/// </summary>
		public virtual bool IsChanged
		{
			get { return isChanged; }
		}

		#endregion

		#region Equals And HashCode Overrides
		/// <summary>
		/// local implementation of Equals based on unique value members
		/// </summary>
		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			if ((obj == null) || (obj.GetType() != this.GetType())) return false;
			Contact castObj = (Contact)obj;
			return (castObj != null) &&
				(this.id == castObj.Id);
		}

		/// <summary>
		/// local implementation of GetHashCode based on unique value members
		/// </summary>
		public override int GetHashCode()
		{

			int hash = 57;
			hash = 27 * hash * id.GetHashCode();
			return hash;
		}
		#endregion
	}
}
