using System;
using System.Collections.Generic;

using NHibernate.Mapping;

namespace NHibernate.Cfg.XmlHbmBinding
{
	/// <summary>Allows binders to defer binding until later.</summary>
	/// <remarks>
	/// <see cref="ISecondPass" /> could probably have been a delegate itself. But since not, this class will
	/// adapt the supplied <see cref="DelegatingSecondPass" /> delegate to the interface.
	/// </remarks>
	internal class DelegatingSecondPass : ISecondPass
	{
		private readonly SecondPassCommand command;

		public DelegatingSecondPass(SecondPassCommand command)
		{
			if (command == null)
				throw new ArgumentNullException("command");

			this.command = command;
		}

		public void DoSecondPass(IDictionary<System.Type, PersistentClass> persistentClasses)
		{
			command(persistentClasses);
		}
	}
}