using System;

namespace NHibernate.Engine
{
	/// <summary> 
	/// Represents the status of an entity with respect to
	/// this session. These statuses are for internal
	/// book-keeping only and are not intended to represent
	/// any notion that is visible to the application.
	/// </summary>
	[Serializable]
	public class Status
	{
		private readonly string name;

		private Status(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			this.name = name;
		}

		public static readonly Status Managed = new Status("Managed");
		public static readonly Status ReadOnly = new Status("ReadOnly");
		public static readonly Status Deleted = new Status("Deleted");
		public static readonly Status Gone = new Status("Gone");
		public static readonly Status Loading = new Status("Loading");
		public static readonly Status Saving = new Status("Saving");

		public override string ToString()
		{
			return name;
		}
	}
}