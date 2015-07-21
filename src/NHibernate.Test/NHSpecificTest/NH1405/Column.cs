using System;

namespace NHibernate.Test.NHSpecificTest.NH1405
{
	public class Column
	{
		/// <summary>
		/// The column name.  Part 3 of 3 of the primary key.
		/// </summary>
		private String _columnName;

		/// <summary>
		/// Another column in the same table.
		/// </summary>
		private Column _controlColumn;

		/// <summary>
		/// The system ID.  Part 1 of 3 of the primary key.
		/// </summary>
		private String _systemId;

		/// <summary>
		/// The table name.  Part 2 of 3 of the primary key.
		/// </summary>
		private String _tableName;

		/// <summary>
		/// The column name.  Part 3 of 3 of the primary key.
		/// </summary>
		public virtual String ColumnName
		{
			get { return _columnName; }
			set { _columnName = value; }
		}

		/// <summary>
		/// Another column in the same table.
		/// </summary>
		public virtual Column ControlColumn
		{
			get { return _controlColumn; }
			set { _controlColumn = value; }
		}

		/// <summary>
		/// The system ID.  Part 1 of 3 of the primary key.
		/// </summary>
		public virtual String SystemId
		{
			get { return _systemId; }
			set { _systemId = value; }
		}

		/// <summary>
		/// The table name.  Part 2 of 3 of the primary key.
		/// </summary>
		public virtual String TableName
		{
			get { return _tableName; }
			set { _tableName = value; }
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			Column other = (Column) obj;
			if (null == _systemId)
			{
				if (null != other._systemId)
				{
					return false;
				}
			}
			else if (!_systemId.Equals(other._systemId))
			{
				return false;
			}
			if (null == _tableName)
			{
				if (null != other._tableName)
				{
					return false;
				}
			}
			else if (!_tableName.Equals(other._tableName))
			{
				return false;
			}
			if (null == _columnName)
			{
				if (null != other._columnName)
				{
					return false;
				}
			}
			else if (!_columnName.Equals(other._columnName))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			const int PRIME = 31;
			int result = 1;
			result = PRIME * result + ((null == _systemId) ? 0 : _systemId.GetHashCode());
			result = PRIME * result + ((null == _tableName) ? 0 : _tableName.GetHashCode());
			result = PRIME * result + ((null == _columnName) ? 0 : _columnName.GetHashCode());
			return result;
		}
	}
}