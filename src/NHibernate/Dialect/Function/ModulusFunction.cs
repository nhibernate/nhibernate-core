using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Dialect.Function
{
	[Serializable]
	internal class ModulusFunction : StandardSafeSQLFunction
	{
		private readonly ModulusFunctionTypeDetector _modulusFunctionTypeDetector;

		public ModulusFunction(bool supportDecimals, bool supportFloatingNumbers)
			: this(new ModulusFunctionTypeDetector(supportDecimals, supportFloatingNumbers))
		{
		}

		public ModulusFunction(ModulusFunctionTypeDetector modulusFunction) : base("mod", NHibernateUtil.Int32, 2)
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
