using System;
using System.Collections;

namespace Refly.Templates
{

	public class ImportCollection : ArrayList
	{	
		public void Add(string import)
		{
			this.Add(new Import(import));
		}

		public void Add(Import import)
		{
			base.Add(import);
		}

		public new Import this[int index]
		{
			get
			{
				return (Import)base[index];
			}
			set
			{
				base[index]=value;
			}
		}
	}
}
