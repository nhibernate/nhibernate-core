using System;
using System.Data.Common;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Driver
{
	public class ReflectionDriveConnectionCommandProvider : IDriveConnectionCommandProvider
	{
		private readonly System.Type commandType;
		private readonly System.Type connectionType;

		public ReflectionDriveConnectionCommandProvider(System.Type connectionType, System.Type commandType)
		{
			if (connectionType == null)
			{
				throw new ArgumentNullException("connectionType");
			}
			if (commandType == null)
			{
				throw new ArgumentNullException("commandType");
			}
			this.connectionType = connectionType;
			this.commandType = commandType;
		}

		#region IDriveConnectionCommandProvider Members

		public DbConnection CreateConnection()
		{
			return (DbConnection) Environment.BytecodeProvider.ObjectsFactory.CreateInstance(connectionType);
		}

		public DbCommand CreateCommand()
		{
			return (DbCommand) Environment.BytecodeProvider.ObjectsFactory.CreateInstance(commandType);
		}

		#endregion
	}
}