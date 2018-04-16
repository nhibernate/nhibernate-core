﻿using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3332
{
	public class DataType
	{
		private ISet<DataTypeDescription> _dataTypeDescriptions = new HashSet<DataTypeDescription>();
		private ISet<MasterEntity> _masterEntities = new HashSet<MasterEntity>();
		// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
		private Int32 _id;
		private Byte[] _rowVersionId;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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
			var toCompareWith = obj as DataType;
			return toCompareWith != null &&
				   Id == toCompareWith.Id;
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

		public virtual ISet<DataTypeDescription> DataTypeDescriptions
		{
			get { return _dataTypeDescriptions; }
			set { _dataTypeDescriptions = value; }
		}

		public virtual ISet<MasterEntity> MasterEntities
		{
			get { return _masterEntities; }
			set { _masterEntities = value; }
		}
	}
}
