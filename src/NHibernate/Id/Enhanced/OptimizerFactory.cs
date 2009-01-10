using System;
using System.Reflection;
using log4net;
using NHibernate.Util;

namespace NHibernate.Id.Enhanced
{
	public class OptimizerFactory
	{
		public const string HiLo = "hilo";
		public const string None = "none";
		public const string Pool = "pooled";
		private static readonly System.Type[] CtorSignature = new[] {typeof (System.Type), typeof (int)};
		private static readonly ILog log = LogManager.GetLogger(typeof (OptimizerFactory));

		public static IOptimizer BuildOptimizer(string type, System.Type returnClass, int incrementSize)
		{
			if (string.IsNullOrEmpty(type))
			{
				throw new ArgumentNullException("type");
			}
			if (returnClass == null)
			{
				throw new ArgumentNullException("returnClass");
			}
			string optimizerClassName;
			switch (type)
			{
				case None:
					optimizerClassName = typeof (NoopOptimizer).FullName;
					break;
				case HiLo:
					optimizerClassName = typeof (HiLoOptimizer).FullName;
					break;
				case Pool:
					optimizerClassName = typeof (PooledOptimizer).FullName;
					break;
				default:
					optimizerClassName = type;
					break;
			}

			try
			{
				System.Type optimizerClass = ReflectHelper.ClassForName(optimizerClassName);
				ConstructorInfo ctor = optimizerClass.GetConstructor(CtorSignature);
				return (IOptimizer)ctor.Invoke(new object[] { returnClass, incrementSize });
			}
			catch (Exception)
			{
				// intentionally empty
			}

			// the default...
			return new NoopOptimizer(returnClass, incrementSize);
		}

		#region Nested type: HiLoOptimizer

		public class HiLoOptimizer : OptimizerSupport
		{
			private long hiValue;
			private long lastSourceValue = -1;
			private long value_Renamed;

			public HiLoOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (log.IsDebugEnabled)
				{
					log.Debug("creating hilo optimizer with [incrementSize=" + incrementSize + "; returnClass=" + returnClass.FullName
					          + "]");
				}
			}

			public override long LastSourceValue
			{
				get { return lastSourceValue; }
			}

			public long LastValue
			{
				get { return value_Renamed - 1; }
			}

			public long HiValue
			{
				get { return hiValue; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return false; }
			}

			public override object Generate(IAccessCallback callback)
			{
				if (lastSourceValue < 0)
				{
					lastSourceValue = callback.NextValue;
					while (lastSourceValue <= 0)
					{
						lastSourceValue = callback.NextValue;
					}
					hiValue = (lastSourceValue * IncrementSize) + 1;
					value_Renamed = hiValue - IncrementSize;
				}
				else if (value_Renamed >= hiValue)
				{
					lastSourceValue = callback.NextValue;
					hiValue = (lastSourceValue * IncrementSize) + 1;
				}
				return Make(value_Renamed++);
			}
		}

		#endregion

		#region Nested type: NoopOptimizer

		public class NoopOptimizer : OptimizerSupport
		{
			private long lastSourceValue = -1;

			public NoopOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize) {}

			public override long LastSourceValue
			{
				get { return lastSourceValue; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return false; }
			}

			public override object Generate(IAccessCallback callback)
			{
				if (lastSourceValue == -1)
				{
					while (lastSourceValue <= 0)
					{
						lastSourceValue = callback.NextValue;
					}
				}
				else
				{
					lastSourceValue = callback.NextValue;
				}
				return Make(lastSourceValue);
			}
		}

		#endregion

		#region Nested type: OptimizerSupport

		public abstract class OptimizerSupport : IOptimizer
		{
			protected OptimizerSupport(System.Type returnClass, int incrementSize)
			{
				if (returnClass == null)
				{
					throw new HibernateException("return class is required");
				}
				ReturnClass = returnClass;
				IncrementSize = incrementSize;
			}

			public System.Type ReturnClass { get; protected set; }

			#region IOptimizer Members

			public int IncrementSize { get; protected set; }

			public abstract long LastSourceValue { get; }

			public abstract bool ApplyIncrementSizeToSourceValues { get; }
			public abstract object Generate(IAccessCallback param);

			#endregion

			protected virtual object Make(long value)
			{
				return IdentifierGeneratorFactory.CreateNumber(value, ReturnClass);
			}
		}

		#endregion

		#region Nested type: PooledOptimizer

		public class PooledOptimizer : OptimizerSupport
		{
			private long hiValue = -1;
			private long value_Renamed;

			public PooledOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (log.IsDebugEnabled)
				{
					log.Debug("creating pooled optimizer with [incrementSize=" + incrementSize + "; returnClass="
					          + returnClass.FullName + "]");
				}
			}

			public override long LastSourceValue
			{
				get { return hiValue; }
			}

			public long LastValue
			{
				get { return value_Renamed - 1; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return true; }
			}

			public override object Generate(IAccessCallback callback)
			{
				if (hiValue < 0)
				{
					value_Renamed = callback.NextValue;
					if (value_Renamed < 1)
					{
						// unfortunately not really safe to normalize this
						// to 1 as an initial value like we do the others
						// because we would not be able to control this if
						// we are using a sequence...
						log.Info("pooled optimizer source reported [" + value_Renamed
						         + "] as the initial value; use of 1 or greater highly recommended");
					}
					hiValue = callback.NextValue;
				}
				else if (value_Renamed >= hiValue)
				{
					hiValue = callback.NextValue;
					value_Renamed = hiValue - IncrementSize;
				}
				return Make(value_Renamed++);
			}
		}

		#endregion
	}
}