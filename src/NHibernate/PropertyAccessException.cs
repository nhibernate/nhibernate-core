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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="message"></param>
		/// <param name="wasSetter"></param>
		/// <param name="persistentType"></param>
		/// <param name="propertyName"></param>
		public PropertyAccessException( Exception root, string message, bool wasSetter, System.Type persistentType, string propertyName ) : base( message, root )
		{
			this.persistentType = persistentType;
			this.wasSetter = wasSetter;
			this.propertyName = propertyName;
		}

		/// <summary></summary>
		public System.Type PersistentType
		{
			get { return persistentType; }
		}

		/// <summary></summary>
		public override string Message
		{
			get
			{
				return base.Message +
					( wasSetter ? " setter of " : " getter of " ) +
					persistentType.FullName +
					"." +
					propertyName;
			}
		}
	}
}