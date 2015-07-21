using System;
using System.Collections.Generic;

namespace NHibernate.Mapping
{
	/// <summary> Placeholder for typedef information</summary>
	[Serializable]
	public class TypeDef
	{
		private readonly string typeClass;
		private readonly IDictionary<string, string> parameters;

		public TypeDef(string typeClass, IDictionary<string, string> parameters)
		{
			this.typeClass = typeClass;
			this.parameters = parameters;
		}

		public string TypeClass
		{
			get { return typeClass; }
		}

		public IDictionary<string, string> Parameters
		{
			get { return parameters; }
		}
	}
}
