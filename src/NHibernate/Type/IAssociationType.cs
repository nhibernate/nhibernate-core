using System;

// using NHibernate.Engine;

namespace NHibernate.Type {

	/// <summary>
	/// A type that represents some kind of association between entities.
	/// </summary>
	public interface IAssociationType {

		// TODO: there's some compilation problem to solve here

		/*
		public static abstract class ForeignKeyType {
			
			protected ForeignKeyType() {}

			/// <summary>
			/// Should we cascade at this cascade point?
			/// </summary>
			/// <param name="cascadePoint"></param>
			/// <returns></returns>
			public abstract bool CascadeNow(int cascadePoint);
		}
		*/

		/*
		 * TODO: Translate in correct C# this code! Do we need a static constructor?
		 * 
		/// <summary>
		/// A foreign key from child to parent
		/// </summary>
		public static readonly ForeignKeyType ForeignKeyToParent = new ForeignKeyType() {
			public bool CascadeNow(int cascadePoint) {													return cascadePoint != Cascades.CascadeBeforeInsertAfterDelete;
			}
		}
		*/

		/*
		 * TODO: idem
		 * 
		/// <summary>
		/// A foreign key from parent to child
		/// </summary>
		public static readonly ForeignKeyType ForeignKeyFromParent = new ForeignKeyType() {
			public bool CascadeNow(int cascadePoint) {													return cascadePoint != Cascades.CascadeAfterInsertBeforeDelete;
			}
		}
		*/
		
		/// <summary>
		/// Get the foreign key directionality of this association
		/// </summary>
		// ForeignKeyType ForeignKeyType { get; }
	}
}