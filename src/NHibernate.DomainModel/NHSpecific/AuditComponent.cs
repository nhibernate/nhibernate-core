using System;

namespace NHibernate.DomainModel.NHSpecific {

	/// <summary>
	/// Summary description for AuditComponent.
	/// </summary>
	public class AuditComponent{

		private string _createdUserId;
		private System.DateTime _createdDate;
		private string _updatedUserId;
		private System.DateTime _updatedDate;

		public AuditComponent()
		{
		}

		public string CreatedUserId 
		{
			get {return _createdUserId;}
			set {_createdUserId = value;}
		}

		public System.DateTime CreatedDate 
		{
			get {return _createdDate;}
			set {_createdDate = value;}
		}

		public string UpdatedUserId 
		{
			get {return _updatedUserId;}
			set {_updatedUserId = value;}
		}

		public System.DateTime UpdatedDate 
		{
			get {return _updatedDate;}
			set {_updatedDate = value;}
		}

	}
}
