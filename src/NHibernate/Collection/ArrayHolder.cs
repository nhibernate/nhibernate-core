using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection {
	/// <summary>
	/// A persistent wrapper for an array. lazy initialization is NOT supported
	/// </summary>
	public class ArrayHolder {//: PersistentCollection{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PersistentCollection));

		private object array;
		private System.Type elementClass;
		private IList temp;

		//public ArrayHolder(ISessionImplementor session, object array) : base(session) {
		//	this.array = array;
		//	initialized = true;
		//}

		
			

	}
}
