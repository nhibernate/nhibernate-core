using System;

namespace NHibernate.Test.NHSpecificTest.NH401
{
	public class Clubmember
	{
		private int m_Id;
		private Club m_Club;
		private DateTime m_Expirydate;
		private DateTime m_Joindate;
		private Decimal m_ProfileId;
		private Decimal m_Withdrawlamount;

		public Clubmember()
		{
			m_Id = 0;
			m_Club = new Club();
		}

		public int Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		public Decimal ProfileId
		{
			get { return m_ProfileId; }
			set { m_ProfileId = value; }
		}


		/*public Accountdistribution Accountdistribution
        {
            get
            {
                return m_Accountdistribution;
            }
            set
            {
                m_Accountdistribution = value;
            }
        }*/

		public Club Club
		{
			get { return m_Club; }
			set { m_Club = value; }
		}

		/*public System.Collections.IList Clubdistributions
        {
            get
            {
                return m_Clubdistributions;
            }
            set
            {
                m_Clubdistributions = value;
            }
        }

        public Creditcard Creditcard
        {
            get
            {
                return m_Creditcard;
            }
            set
            {
                m_Creditcard = value;
            }
        }*/

		public DateTime Expirydate
		{
			get { return m_Expirydate; }
			set { m_Expirydate = value; }
		}

		public DateTime Joindate
		{
			get { return m_Joindate; }
			set { m_Joindate = value; }
		}

		/* public System.Collections.IList Payments
        {
            get
            {
                return m_Payments;
            }
            set
            {
                m_Payments = value;
            }
        }

        public Paymenttype Paymenttypecode
        {
            get
            {
                return m_Paymenttypecode;
            }
            set
            {
                m_Paymenttypecode = value;
            }
        }

        public Nmrprofile Profile
        {
            get
            {
                return m_Profile;
            }
            set
            {
                m_Profile = value;
            }
        }*/

		public Decimal Withdrawlamount
		{
			get { return m_Withdrawlamount; }
			set { m_Withdrawlamount = value; }
		}

		/*public Withdrawlperiod Withdrawlperiodcode
        {
            get
            {
                return m_Withdrawlperiodcode;
            }
            set
            {
                m_Withdrawlperiodcode = value;
            }
        }*/
	}
}