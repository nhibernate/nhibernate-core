using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Glarch : Super, GlarchProxy, ILifecycle, INamed
	{
	
		private int _version;
		private GlarchProxy _next;
		private short _order;
		private IList _strings;
		private Iesi.Collections.ISet _stringSets;
		private IList _fooComponents;
		private GlarchProxy[] _proxyArray;
		private Iesi.Collections.ISet _proxySet;
		private string _immutable;
		private int _derivedVersion;
		private object _any;
		private int _x;
		private Multiplicity _multiple;

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}
	
		public int Version
		{
			get { return _version; }
			set { this._version = value; }
		}

		

		/// <summary>
		/// Gets or sets the _next
		/// </summary> 
		public GlarchProxy Next
		{
			get { return _next;  }
			set { _next = value; }
		}

		
		/// <summary>
		/// Gets or sets the _order
		/// </summary> 
		public short Order
		{
			get { return _order; }
			set { _order = value; }
		}

		
		/// <summary>
		/// Gets or sets the _strings
		/// </summary> 
		public IList Strings
		{
			get { return _strings;  }
			set { _strings = value; }
		}

		
		/// <summary>
		/// Gets or sets the _stringSets
		/// </summary> 
		//TODO: figure out why this is not in the mapping???
		public Iesi.Collections.ISet StringSets
		{
			get { return _stringSets; }
			set { _stringSets = value; }
		}

		
		/// <summary>
		/// Gets or sets the _fooComponents
		/// </summary> 
		public IList FooComponents
		{
			get { return _fooComponents;  }
			set { _fooComponents = value; }
		}

		
		/// <summary>
		/// Gets or sets the _proxyArray
		/// </summary> 
		public GlarchProxy[] ProxyArray
		{
			get { return _proxyArray; }
			set { _proxyArray = value; }
		}

		
		/// <summary>
		/// Gets or sets the _proxySet
		/// </summary> 
		public Iesi.Collections.ISet ProxySet
		{
			get { return _proxySet; }
			set { _proxySet = value; }
		}
	
		#region NHibernate.ILifecycle Members

		public LifecycleVeto OnDelete(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}
	
		public void OnLoad(ISession s, object id) 
		{
			if ( ! ( ( (String) id ).Length==32 ) ) throw new ArgumentException("id problem");
		}
	
		public LifecycleVeto OnSave(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}
	
		public LifecycleVeto OnUpdate(ISession s) 
		{
			return LifecycleVeto.NoVeto;
		}
	
		#endregion

		
		/// <summary>
		/// Gets or sets the _immutable
		/// </summary> 
		public string Immutable
		{
			get { return _immutable; }
			set { _immutable = value; }
		}

		/// <summary>
		/// Gets or sets the _derivedVersion
		/// </summary> 
		public int DerivedVersion
		{
			get { return _derivedVersion;  }
			set { _derivedVersion = value; }
		}

		/// <summary>
		/// Gets or sets the _any
		/// </summary>
		public object Any
		{
			get { return _any; }
			set { _any = value; }
		}

		/// <summary>
		/// Gets or sets the _multiple
		/// </summary> 
		public Multiplicity Multiple
		{
			get  { return _multiple; }
			set  { _multiple = value; }
		}

		public new string Name
		{
			get { return base._name; }
			set { _name = value; }
		}

	}
}



