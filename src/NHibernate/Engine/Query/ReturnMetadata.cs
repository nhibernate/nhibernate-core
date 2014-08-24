using System;
using NHibernate.Type;

namespace NHibernate.Engine.Query
{
	[Serializable]
	public class ReturnMetadata
	{
		private readonly string[] returnAliases;
		private readonly IType[] returnTypes;

		public ReturnMetadata(string[] returnAliases, IType[] returnTypes)
		{
			this.returnAliases = returnAliases;
			this.returnTypes = returnTypes;
		}

		public string[] ReturnAliases
		{
			get { return returnAliases; }
		}

		public IType[] ReturnTypes
		{
			get { return returnTypes; }
		}
	}
}
