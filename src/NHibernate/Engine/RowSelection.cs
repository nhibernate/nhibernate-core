using System;

namespace NHibernate.Engine {

	public sealed class RowSelection {
		private int firstRow;
		private int maxRows;
		private int timeout;

		public int FirstRow {
			get { return firstRow; }
			set { firstRow = value; }
		}

		public int MaxRows {
			get { return maxRows; }
			set { maxRows = value; }
		}

		public int Timeout {
			get { return timeout; }
			set { timeout = value; }
		}
	}
}
