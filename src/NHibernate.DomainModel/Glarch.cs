using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Classic;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Glarch : Super, GlarchProxy, ILifecycle, INamed
	{
		private int _version;
		private GlarchProxy _next;
		private short _order;
		private IList<string> _strings;
		private ISet<string> _stringSets;
		private IList<FooComponent> _fooComponents;
		private GlarchProxy[] _proxyArray;
		private ISet<GlarchProxy> _proxySet;

		[NonSerialized]
		private IDictionary _dynaBean;

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
			get { return _next; }
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
		public IList<string> Strings
		{
			get { return _strings; }
			set { _strings = value; }
		}


		/// <summary>
		/// Gets or sets the _stringSets
		/// </summary> 
		//TODO: figure out why this is not in the mapping???
		public ISet<string> StringSets
		{
			get { return _stringSets; }
			set { _stringSets = value; }
		}


		/// <summary>
		/// Gets or sets the _fooComponents
		/// </summary> 
		public IList<FooComponent> FooComponents
		{
			get { return _fooComponents; }
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
		public ISet<GlarchProxy> ProxySet
		{
			get { return _proxySet; }
			set { _proxySet = value; }
		}

		#region NHibernate.Classic.ILifecycle Members

		public LifecycleVeto OnDelete(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}

		public void OnLoad(ISession s, object id)
		{
			if (! (((String) id).Length == 32))
			{
				throw new ArgumentException("id problem");
			}
		}

		public LifecycleVeto OnSave(ISession s)
		{
			_dynaBean = new Dictionary<string, object>();
			_dynaBean["foo"] = "foo";
			_dynaBean["bar"] = 66;
			_immutable = "never changes!";
			return LifecycleVeto.NoVeto;
		}

		public LifecycleVeto OnUpdate(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}

		#endregion

		public IDictionary DynaBean
		{
			get { return _dynaBean; }
			set { _dynaBean = value; }
		}

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
			get { return _derivedVersion; }
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
			get { return _multiple; }
			set { _multiple = value; }
		}

		public new string Name
		{
			get { return base._name; }
			set { _name = value; }
		}
	}
}