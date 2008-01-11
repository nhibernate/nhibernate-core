namespace NHibernate.Validator.Cfg
{
	using System;
	using Event;
	using log4net;
	using Mapping;
	using NHibernate.Cfg;
	using NHibernate.Event;
	using Environment=NHibernate.Validator.Environment;

	public class ValidatorInitializer
	{
		private ValidatorInitializer()
		{
		}

		private static readonly ILog log = LogManager.GetLogger(typeof(ValidatorInitializer));

		public static void Initialize(Configuration cfg)
		{
			bool ApplyToDDL = true;
			bool AutoRegisterListeners = true;

			string string_ApplyToDDL = cfg.GetProperty(Environment.APPLY_TO_DDL);
			string string_AutoRegisterListeners = cfg.GetProperty(Environment.AUTOREGISTER_LISTENERS);

			if (string.IsNullOrEmpty(string_ApplyToDDL))
				ApplyToDDL = true;
			else 
				ApplyToDDL = string_ApplyToDDL.Equals("true");


			if (string.IsNullOrEmpty(string_AutoRegisterListeners))
				AutoRegisterListeners = true;
			else
				AutoRegisterListeners = string_AutoRegisterListeners.Equals("true");

			//Apply To DDL
			if (ApplyToDDL)
			{
				foreach(PersistentClass persistentClazz in cfg.ClassMappings)
				{
					try
					{
						ClassValidator classValidator = new ClassValidator(persistentClazz.MappedClass);
						classValidator.Apply(persistentClazz);
					}
					catch(Exception ex)
					{
						log.Warn("Unable to apply constraints on DDL for " + persistentClazz.ClassName, ex);
					}
				}
			}

			//Autoregister Listeners
			if(AutoRegisterListeners)
			{
				cfg.SetListener(ListenerType.PreInsert, new ValidatePreInsertEventListener());
				cfg.SetListener(ListenerType.PreUpdate, new ValidatePreUpdateEventListener());
			}
		}
	}
}