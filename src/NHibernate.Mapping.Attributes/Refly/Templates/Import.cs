using System;
using System.ComponentModel;

namespace Refly.Templates
{
	public class Import
	{
		private string name="";

		public Import()
		{}

		public Import(string name)
		{
			this.name=name;
		}

		[Category("Data")]
		public String Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name=value;
			}
		}

		public override string ToString()
		{
			return String.Format("{0}",Name);
		}
	}
}
