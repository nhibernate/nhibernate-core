using System;
using NHibernate.Collection;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader {

	public enum OuterJoinLoaderType {
		Eager = -1,
		Auto = 0,
		Lazy = -1
	}
	
	/*public class OuterJoinLoader : Loader {
		
		protected static readonly IType[] NoTypes = new IType[0];
		protected static readonly string[][] NoStringArrays = new string[0][];
		protected static readonly string[] NoStrings = new string[0];
		
	}*/
}
