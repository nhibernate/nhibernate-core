﻿using System;

namespace NHibernate.Test.NHSpecificTest.NH3332
{
	public class MasterEntity
	{
		private DataType _dataType;
		private State _state;
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private Int32 _id;
		private Byte[] _rowVersionId;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
		private DateTime? _lastModifiedTimeStamp;
		private String _name;

		public override int GetHashCode()
		{
			int toReturn = base.GetHashCode();
			toReturn ^= Id.GetHashCode();
			return toReturn;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var toCompareWith = obj as MasterEntity;
			return toCompareWith != null && Id == toCompareWith.Id;
		}

		public virtual Int32 Id
		{
			get { return _id; }
		}

		public virtual DateTime? LastModifiedTimeStamp
		{
			get { return _lastModifiedTimeStamp; }
			set { _lastModifiedTimeStamp = value; }
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

		public virtual DataType DataType
		{
			get { return _dataType; }
			set { _dataType = value; }
		}

		public virtual State State
		{
			get { return _state; }
			set { _state = value; }
		}
	}
}
