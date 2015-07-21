using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1574
{
	public class TeamStorage
	{
		private DateTime creationDate = DateTime.Now;
		private int eloRanking;
		private int isInBattle;
		private bool ladderActive;
		private int ladderPosition;
		private int maxElo;
		private int minElo;
		private string name;
		private int numberPlayedBattles;
		private int restUntil;
		private int stoppedUntil;

		private int id;
		private IList<Principal> principals;

		/// <summary>
		/// Gets the TeamStorage's Name
		/// </summary>
		public virtual string Name
		{
			set { name = value; }
			get { return name; }
		}

		/// <summary>
		/// Gets the TeamStorage's EloRanking
		/// </summary>
		public virtual int EloRanking
		{
			set { eloRanking = value; }
			get { return eloRanking; }
		}

		/// <summary>
		/// Gets the TeamStorage's MaxElo
		/// </summary>
		public virtual int MaxElo
		{
			set { maxElo = value; }
			get { return maxElo; }
		}

		/// <summary>
		/// Gets the TeamStorage's MinElo
		/// </summary>
		public virtual int MinElo
		{
			set { minElo = value; }
			get { return minElo; }
		}

		/// <summary>
		/// Gets the TeamStorage's CreationDate
		/// </summary>
		public virtual DateTime CreationDate
		{
			set { creationDate = value; }
			get { return creationDate; }
		}

		/// <summary>
		/// Gets the TeamStorage's NumberPlayedBattles
		/// </summary>
		public virtual int NumberPlayedBattles
		{
			set { numberPlayedBattles = value; }
			get { return numberPlayedBattles; }
		}

		/// <summary>
		/// Gets the TeamStorage's LadderActive
		/// </summary>
		public virtual bool LadderActive
		{
			set { ladderActive = value; }
			get { return ladderActive; }
		}

		/// <summary>
		/// Gets the TeamStorage's LadderPosition
		/// </summary>
		public virtual int LadderPosition
		{
			set { ladderPosition = value; }
			get { return ladderPosition; }
		}

		/// <summary>
		/// Gets the TeamStorage's IsInBattle
		/// </summary>
		public virtual int IsInBattle
		{
			set { isInBattle = value; }
			get { return isInBattle; }
		}

		/// <summary>
		/// Gets the TeamStorage's RestUntil
		/// </summary>
		public virtual int RestUntil
		{
			set { restUntil = value; }
			get { return restUntil; }
		}

		/// <summary>
		/// Gets the TeamStorage's StoppedUntil
		/// </summary>
		public virtual int StoppedUntil
		{
			set { stoppedUntil = value; }
			get { return stoppedUntil; }
		}

		public virtual IList<Principal> Principals
		{
			get { return principals; }
			set { principals = value; }
		}

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	} ;
}