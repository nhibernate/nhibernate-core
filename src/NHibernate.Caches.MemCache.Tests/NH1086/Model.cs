using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Caches.MemCache.Tests.NH1086
{
	[Serializable]
	public class CompId
	{
		private string keyString;
		private int keyInt;

		public virtual string KeyString
		{
			get { return keyString; }
			private set { keyString = value; }
		}

		public virtual int KeyInt
		{
			get { return keyInt; }
			private set { keyInt = value; }
		}

		private CompId() {}

		public CompId(string keyString, int keyInt)
		{
			this.keyString = keyString;
			this.keyInt = keyInt;
		}

		protected bool Equals(CompId compId)
		{
			if (compId == null) return false;
			return Equals(keyString, compId.keyString) && keyInt == compId.keyInt;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as CompId);
		}

		public override int GetHashCode()
		{
			// Generated using ReSharper
			return (keyString != null ? keyString.GetHashCode() : 0) + 29*keyInt;
		}
	}

	[Serializable]
	public class ClassWithCompId
	{
		private CompId id;
		private string name;

		public virtual CompId ID
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
