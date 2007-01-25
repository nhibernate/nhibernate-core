using System;

using NHibernate;
using NHibernate.Classic;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Vetoer.
	/// </summary>
	public class Vetoer : ILifecycle
	{
		bool _onSaveCalled;
		bool _onUpdateCalled;
		bool _onDeleteCalled;

		private string _name;
		private string[] _strings;
		
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string[] Strings
		{
			get { return _strings; }
			set { _strings = value; }
		}

		#region ILifecycle Members

		public LifecycleVeto OnUpdate(ISession s)
		{
			bool result = !_onUpdateCalled;
			_onUpdateCalled = true;
			return ( result ? LifecycleVeto.Veto : LifecycleVeto.NoVeto );
		}

		public void OnLoad(ISession s, object id)
		{
		}

		public LifecycleVeto OnSave(ISession s)
		{
			bool result = !_onSaveCalled;
			_onSaveCalled = true;
			return ( result ? LifecycleVeto.Veto : LifecycleVeto.NoVeto );
		}

		public LifecycleVeto OnDelete(ISession s)
		{
			bool result = !_onDeleteCalled;
			_onDeleteCalled = true;
			return ( result ? LifecycleVeto.Veto : LifecycleVeto.NoVeto );
		}

		#endregion
	}
}
