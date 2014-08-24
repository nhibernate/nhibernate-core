using System;

namespace NHibernate.Test.Interceptor
{
	public class Log
	{
		private long id;
		private string entityName;
		private string entityId;
		private string action;
		private DateTime time;

		public Log() {}

		public Log(string action, string entityId, string entityName)
		{
			this.action = action;
			this.entityId = entityId;
			this.entityName = entityName;
			time = DateTime.Now;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string EntityName
		{
			get { return entityName; }
			set { entityName = value; }
		}

		public virtual string EntityId
		{
			get { return entityId; }
			set { entityId = value; }
		}

		public virtual string Action
		{
			get { return action; }
			set { action = value; }
		}

		public virtual DateTime Time
		{
			get { return time; }
			set { time = value; }
		}
	}
}