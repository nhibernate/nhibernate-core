using System;

namespace NHibernate.Test.NHSpecificTest.NH401
{

    public class Club
    {

        private int m_Id;
        private System.Boolean m_Active;
        private System.Decimal m_Checklpcmember;
        private System.Boolean m_Freelpcmember;
        private System.DateTime m_Lastupdated;


		public Club()
		{
			this.Active = true;
			this.CheckLPCMember = 0;
			this.FreeLPCMember = false;
			this.LastUpdated = DateTime.Now;
		}

        public int Id
        {
            get
            {
                return m_Id;
			} set 
			  {
				m_Id = value;
			  }
        }

        public System.Boolean Active
        {
            get
            {
                return m_Active;
            }
            set
            {
                m_Active = value;
            }
        }

        public System.Decimal CheckLPCMember
        {
            get
            {
                return m_Checklpcmember;
            }
            set
            {
                m_Checklpcmember = value;
            }
        }

        public System.Boolean FreeLPCMember
        {
            get
            {
                return m_Freelpcmember;
            }
            set
            {
                m_Freelpcmember = value;
            }
        }

        public System.DateTime LastUpdated
        {
            get
            {
                return m_Lastupdated;
            }
            set
            {
                m_Lastupdated = value;
            }
        }

    }
}
