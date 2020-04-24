using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	internal class ModulusFunctionTemplate : SQLFunctionTemplate
	{
		private readonly ModulusFunctionTypeDetector _modulusFunctionTypeDetector;

		public ModulusFunctionTemplate(bool supportDecimals) : this(new ModulusFunctionTypeDetector(supportDecimals))
		{
		}

		public ModulusFunctionTemplate(ModulusFunctionTypeDetector modulusFunction) : base(NHibernateUtil.Int32, "((?1) % (?2))")
		{
			_modulusFunctionTypeDetector = modulusFunction;
		}

		/// <inheritdoc />
		public override IType GetEffectiveReturnType(IEnumerable<IType> argumentTypes, IMapping mapping, bool throwOnError)
		{
			return _modulusFunctionTypeDetector.GetReturnType(argumentTypes, mapping, throwOnError);
		}
	}
}
