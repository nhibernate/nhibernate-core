using System;

namespace NHibernate
{
	/// <summary>
	/// Instances represent a lock mode for a row of a relational database table.
	/// </summary>
	/// <remarks>
	/// It is not intended that users spend much time worrying about locking since Hibernate
	/// usually obtains exactly the right lock level automatically. Some "advanced" users may
	/// wish to explicitly specify lock levels.
	/// </remarks>
	[Serializable]
	public sealed class LockMode
	{
		private readonly int level;
		private readonly string name;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="level"></param>
		/// <param name="name"></param>
		private LockMode(int level, string name)
		{
			this.level = level;
			this.name = name;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return name;
		}

		/// <summary>
		/// Is this lock mode more restrictive than the given lock mode?
		/// </summary>
		/// <param name="mode"></param>
		public bool GreaterThan(LockMode mode)
		{
			return level > mode.level;
		}

		/// <summary>
		/// Is this lock mode less restrictive than the given lock mode?
		/// </summary>
		/// <param name="mode"></param>
		public bool LessThan(LockMode mode)
		{
			return level < mode.level;
		}

		/// <summary>
		/// No lock required. 
		/// </summary>
		/// <remarks>
		/// If an object is requested with this lock mode, a <c>Read</c> lock
		/// might be obtained if necessary.
		/// </remarks>
		public static LockMode None = new LockMode(0, "None");

		/// <summary>
		/// A shared lock. 
		/// </summary>
		/// <remarks>
		/// Objects are loaded in <c>Read</c> mode by default
		/// </remarks>
		public static LockMode Read = new LockMode(5, "Read");

		/// <summary>
		/// An upgrade lock. 
		/// </summary>
		/// <remarks>
		/// Objects loaded in this lock mode are materialized using an
		/// SQL <c>SELECT ... FOR UPDATE</c>
		/// </remarks>
		public static LockMode Upgrade = new LockMode(10, "Upgrade");

		/// <summary>
		/// Attempt to obtain an upgrade lock, using an Oracle-style
		/// <c>SELECT ... FOR UPGRADE NOWAIT</c>. 
		/// </summary>
		/// <remarks>
		/// The semantics of this lock mode, once obtained, are the same as <c>Upgrade</c>
		/// </remarks>
		public static LockMode UpgradeNoWait = new LockMode(10, "UpgradeNoWait");

		/// <summary>
		/// A <c>Write</c> lock is obtained when an object is updated or inserted.
		/// </summary>
		/// <remarks>
		/// This is not a valid mode for <c>Load()</c> or <c>Lock()</c>.
		/// </remarks>
		public static LockMode Write = new LockMode(10, "Write");

		// TODO H3.2: Implement Force where required
		/// <summary> 
		/// Similar to <see cref="Upgrade"/> except that, for versioned entities,
		/// it results in a forced version increment.
		/// </summary>
		public static readonly LockMode Force = new LockMode(15, "Force");


		//TODO: need to implement .NET equivalent of readResolve - believe it is
		// the IObjectReference interface...
	}
}
