using System;

namespace NHibernate.Test.NHSpecificTest.NH1574
{
	public class Principal
	{
		private bool approved;
		private bool autoStartVacations;
		private int availableVacationTicks;
		private string confirmationCode;
		private int credits;
		private int eloRanking;
		private string email;
		private string ip;
		private bool isBot;
		private int isInBattle;
		private bool isOnline;
		private bool ladderActive;
		private int ladderPosition;
		private DateTime lastLogin = DateTime.Now;
		private string locale;
		private bool locked;
		private int myStatsId;
		private string name;
		private string password;
		private string rawRoles;
		private bool receiveMail;
		private DateTime registDate = DateTime.Now;
		private int restUntil;
		private int stoppedUntil;
		private int vacationEndtick;
		private int vacationStartTick;

		private int id;
		private SpecializedTeamStorage team;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		/// <summary>
		/// Gets the Principal's IsBot
		/// </summary>
		public virtual bool IsBot
		{
			set { isBot = value; }
			get { return isBot; }
		}

		/// <summary>
		/// Gets the Principal's MyStatsId
		/// </summary>
		public virtual int MyStatsId
		{
			set { myStatsId = value; }
			get { return myStatsId; }
		}

		/// <summary>
		/// Gets the Principal's EloRanking
		/// </summary>
		public virtual int EloRanking
		{
			set { eloRanking = value; }
			get { return eloRanking; }
		}

		/// <summary>
		/// Gets the Principal's ReceiveMail
		/// </summary>
		public virtual bool ReceiveMail
		{
			set { receiveMail = value; }
			get { return receiveMail; }
		}

		/// <summary>
		/// Gets the Principal's Credits
		/// </summary>
		public virtual int Credits
		{
			set { credits = value; }
			get { return credits; }
		}

		/// <summary>
		/// Gets the Principal's LadderActive
		/// </summary>
		public virtual bool LadderActive
		{
			set { ladderActive = value; }
			get { return ladderActive; }
		}

		/// <summary>
		/// Gets the Principal's LadderPosition
		/// </summary>
		public virtual int LadderPosition
		{
			set { ladderPosition = value; }
			get { return ladderPosition; }
		}

		/// <summary>
		/// Gets the Principal's IsInBattle
		/// </summary>
		public virtual int IsInBattle
		{
			set { isInBattle = value; }
			get { return isInBattle; }
		}

		/// <summary>
		/// Gets the Principal's RestUntil
		/// </summary>
		public virtual int RestUntil
		{
			set { restUntil = value; }
			get { return restUntil; }
		}

		/// <summary>
		/// Gets the Principal's StoppedUntil
		/// </summary>
		public virtual int StoppedUntil
		{
			set { stoppedUntil = value; }
			get { return stoppedUntil; }
		}

		/// <summary>
		/// Gets the Principal's AvailableVacationTicks
		/// </summary>
		public virtual int AvailableVacationTicks
		{
			set { availableVacationTicks = value; }
			get { return availableVacationTicks; }
		}

		/// <summary>
		/// Gets the Principal's VacationStartTick
		/// </summary>
		public virtual int VacationStartTick
		{
			set { vacationStartTick = value; }
			get { return vacationStartTick; }
		}

		/// <summary>
		/// Gets the Principal's VacationEndtick
		/// </summary>
		public virtual int VacationEndtick
		{
			set { vacationEndtick = value; }
			get { return vacationEndtick; }
		}

		/// <summary>
		/// Gets the Principal's AutoStartVacations
		/// </summary>
		public virtual bool AutoStartVacations
		{
			set { autoStartVacations = value; }
			get { return autoStartVacations; }
		}

		/// <summary>
		/// Gets the Principal's Name
		/// </summary>
		public virtual string Name
		{
			set { name = value; }
			get { return name; }
		}

		/// <summary>
		/// Gets the Principal's Password
		/// </summary>
		public virtual string Password
		{
			set { password = value; }
			get { return password; }
		}

		/// <summary>
		/// Gets the Principal's Email
		/// </summary>
		public virtual string Email
		{
			set { email = value; }
			get { return email; }
		}

		/// <summary>
		/// Gets the Principal's Ip
		/// </summary>
		public virtual string Ip
		{
			set { ip = value; }
			get { return ip; }
		}

		/// <summary>
		/// Gets the Principal's RegistDate
		/// </summary>
		public virtual DateTime RegistDate
		{
			set { registDate = value; }
			get { return registDate; }
		}

		/// <summary>
		/// Gets the Principal's LastLogin
		/// </summary>
		public virtual DateTime LastLogin
		{
			set { lastLogin = value; }
			get { return lastLogin; }
		}

		/// <summary>
		/// Gets the Principal's Approved
		/// </summary>
		public virtual bool Approved
		{
			set { approved = value; }
			get { return approved; }
		}

		/// <summary>
		/// Gets the Principal's IsOnline
		/// </summary>
		public virtual bool IsOnline
		{
			set { isOnline = value; }
			get { return isOnline; }
		}

		/// <summary>
		/// Gets the Principal's Locked
		/// </summary>
		public virtual bool Locked
		{
			set { locked = value; }
			get { return locked; }
		}

		/// <summary>
		/// Gets the Principal's Locale
		/// </summary>
		public virtual string Locale
		{
			set { locale = value; }
			get { return locale; }
		}

		/// <summary>
		/// Gets the Principal's ConfirmationCode
		/// </summary>
		public virtual string ConfirmationCode
		{
			set { confirmationCode = value; }
			get { return confirmationCode; }
		}

		/// <summary>
		/// Gets the Principal's RawRoles
		/// </summary>
		public virtual string RawRoles
		{
			set { rawRoles = value; }
			get { return rawRoles; }
		}

		public virtual SpecializedTeamStorage Team
		{
			get { return team; }
			set { team = value; }
		}
	} ;
}