using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	internal class IifSQLFunction : SQLFunctionTemplate
	{
		public IifSQLFunction() : base(null, "case when ?1 then ?2 else ?3 end")
		{
		}

		/// <inheritdoc />
		public override IType GetReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			var args = argumentTypes.ToList();
			if (args.Count != 3)
			{
				if (throwOnError)
				{
					throw new QueryException($"Invalid number of arguments for iif()");
				}

				return null;
			}

			return args[1] ?? args[2];
		}
	}
}
