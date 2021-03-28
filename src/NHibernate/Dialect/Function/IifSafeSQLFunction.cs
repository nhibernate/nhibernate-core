using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	internal class IifSafeSQLFunction : StandardSafeSQLFunction
	{
		public IifSafeSQLFunction() : base("iif", 3)
		{
		}

		/// <inheritdoc />
		public override IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			var args = argumentTypes.ToList();
			if (args.Count != 3)
			{
				return null; // Not enough information
			}

			return args[1] ?? args[2];
		}
	}
}
