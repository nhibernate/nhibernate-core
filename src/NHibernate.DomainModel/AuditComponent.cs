using System;

namespace NHibernate.DomainModel {

	/// <summary>
	/// Summary description for AuditComponent.
	/// </summary>
	public class AuditComponent{

		private string createdUserId;
		private System.DateTime createdDate;
		private string updatedUserId;
		private System.DateTime updatedDate;

		public AuditComponent(){
		}

		public string CreatedUserId {
			get {return createdUserId;}
			set {createdUserId = value;}
		}

		public System.DateTime CreatedDate {
			get {return createdDate;}
			set {createdDate = value;}
		}

		public string UpdatedUserId {
			get {return updatedUserId;}
			set {updatedUserId = value;}
		}

		public System.DateTime UpdatedDate {
			get {return updatedDate;}
			set {updatedDate = value;}
		}

	}
}
