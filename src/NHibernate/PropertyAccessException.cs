using System;

namespace NHibernate 
{
	/// <summary>
	/// A problem occurred accessing a property of an instance of a persistent class by reflection
	/// </summary>
	[Serializable]
	public class PropertyAccessException : HibernateException 
	{
		private System.Type persistentType;
		private string propertyName;
		private bool wasSetter;

		public PropertyAccessException(Exception root, string message, bool wasSetter, System.Type persistentType, string propertyName) : base(message, root) 
		{
			this.persistentType = persistentType;
			this.wasSetter = wasSetter;
			this.propertyName = propertyName;
		}

		public System.Type PersistentType 
		{
			get { return persistentType; }
		}

		public override string Message 
		{
			get
			{
				return base.Message + 
					( wasSetter ? " setter of " : " getter of ") +
					persistentType.FullName +
					"." +
					propertyName;
			}
		}
	}
}
