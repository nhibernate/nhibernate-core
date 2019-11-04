﻿using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3332
{
	public class State
	{
		private ISet<MasterEntity> _masterEntities = new HashSet<MasterEntity>();
		private ISet<StateDescription> _stateDescriptions = new HashSet<StateDescription>();
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private Int32 _id;
		private Byte[] _rowVersionId;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
		private String _name;

		public override int GetHashCode()
		{
			var toReturn = base.GetHashCode();
			toReturn ^= Id.GetHashCode();
			return toReturn;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			var toCompareWith = obj as State;
			return toCompareWith != null && Id == toCompareWith.Id;
		}

		public virtual Int32 Id
		{
			get { return _id; }
		}

		public virtual String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual Byte[] RowVersionId
		{
			get { return _rowVersionId; }
		}

		public virtual ISet<MasterEntity> MasterEntities
		{
			get { return _masterEntities; }
			set { _masterEntities = value; }
		}

		public virtual ISet<StateDescription> StateDescriptions
		{
			get { return _stateDescriptions; }
			set { _stateDescriptions = value; }
		}
	}
}
