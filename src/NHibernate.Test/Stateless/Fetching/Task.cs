using System;


namespace NHibernate.Test.Stateless.Fetching
{
	public class Task
	{
		private long? id;
		private string description;
		private User user;
		private Resource resource;
		private DateTime dueDate;
		private DateTime? startDate;
		private DateTime? completionDate;

		public Task()
		{
		}

		public Task(User user, string description, Resource resource, DateTime dueDate) 
            : this(user, description, resource, dueDate, null, null)
		{
		}

		public Task(User user, string description, Resource resource, DateTime dueDate, DateTime? startDate, DateTime? completionDate)
		{
			this.user = user;
			this.resource = resource;
			this.description = description;
			this.dueDate = dueDate;
			this.startDate = startDate;
			this.completionDate = completionDate;
		}

		public virtual long? Id
		{
			get
			{
				return id;
			}

			set
			{
				this.id = value;
			}
		}


		public virtual User User
		{
			get
			{
				return user;
			}

			set
			{
				this.user = value;
			}
		}


		public virtual Resource Resource
		{
			get
			{
				return resource;
			}

			set
			{
				this.resource = value;
			}
		}


		public virtual string Description
		{
			get
			{
				return description;
			}

			set
			{
				this.description = value;
			}
		}


		public virtual DateTime DueDate
		{
			get
			{
				return dueDate;
			}

			set
			{
				this.dueDate = value;
			}
		}


		public virtual DateTime? StartDate
		{
			get
			{
				return startDate;
			}

			set
			{
				this.startDate = value;
			}
		}


		public virtual DateTime? CompletionDate
		{
			get
			{
				return completionDate;
			}

			set
			{
				this.completionDate = value;
			}
		}

	}

}
