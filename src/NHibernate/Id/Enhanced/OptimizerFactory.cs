using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NHibernate.Util;

namespace NHibernate.Id.Enhanced
{
	public partial class OptimizerFactory
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(OptimizerFactory));

		public const string None = "none";
		public const string HiLo = "hilo";
		public const string Pool = "pooled";
		public const string PoolLo = "pooled-lo";

		private static readonly System.Type[] CtorSignature = new[] { typeof(System.Type), typeof(int) };

		/// <summary>
		/// Marker interface for an optimizer that wishes to know the user-specified initial value.
		/// <p/>
		/// Used instead of constructor injection since that is already a public understanding and
		/// because not all optimizers care.
		/// </summary>
		public interface IInitialValueAwareOptimizer
		{
			/// <summary>
			/// Reports the user-specified initial value to the optimizer.
			/// <p/>
			/// <tt>-1</tt> is used to indicate that the user did not specify.
			/// <param name="initialValue">The initial value specified by the user, or <tt>-1</tt> to indicate that the
			/// user did not specify.</param>
			/// </summary>
			void InjectInitialValue(long initialValue);
		}

		private static IOptimizer BuildOptimizer(string type, System.Type returnClass, int incrementSize)
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
					optimizerClassName = typeof(NoopOptimizer).FullName;
					break;
				case HiLo:
					optimizerClassName = typeof(HiLoOptimizer).FullName;
					break;
				case Pool:
					optimizerClassName = typeof(PooledOptimizer).FullName;
					break;
				case PoolLo:
					optimizerClassName = typeof(PooledLoOptimizer).FullName;
					break;
				default:
					optimizerClassName = type;
					break;
			}

			try
			{
				System.Type optimizerClass = ReflectHelper.ClassForName(optimizerClassName);
				ConstructorInfo ctor = optimizerClass.GetConstructor(CtorSignature);

				if (ctor == null)
					throw new HibernateException("Optimizer does not have expected contructor");

				return (IOptimizer)ctor.Invoke(new object[] { returnClass, incrementSize });
			}
			catch (Exception)
			{
				Log.Error("Unable to instantiate id generator optimizer.");  // FIXME: Review log message.
			}

			// the default...
			return new NoopOptimizer(returnClass, incrementSize);
		}

		public static IOptimizer BuildOptimizer(string type, System.Type returnClass, int incrementSize, long explicitInitialValue)
		{
			// FIXME: Disable this warning, or refactor without the deprecated version.
			IOptimizer optimizer = BuildOptimizer(type, returnClass, incrementSize);

			if (optimizer is IInitialValueAwareOptimizer)
				((IInitialValueAwareOptimizer)optimizer).InjectInitialValue(explicitInitialValue);

			return optimizer;
		}

		#region Nested type: HiLoOptimizer

		public partial class HiLoOptimizer : OptimizerSupport
		{
			private long _upperLimit;
			private long _lastSourceValue = -1;
			private long _value;

			public HiLoOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (Log.IsDebugEnabled)
				{
					Log.Debug("Creating hilo optimizer with [incrementSize=" + incrementSize + "; returnClass=" + returnClass.FullName + "]");
				}
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public override long LastSourceValue
			{
				get { return _lastSourceValue; }
			}

			public long LastValue
			{
				get { return _value - 1; }
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public long HiValue
			{
				get { return _upperLimit; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return false; }
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override object Generate(IAccessCallback callback)
			{
				if (_lastSourceValue < 0)
				{
					_lastSourceValue = callback.GetNextValue();
					while (_lastSourceValue <= 0)
					{
						_lastSourceValue = callback.GetNextValue();
					}

					// upperLimit defines the upper end of the bucket values
					_upperLimit = (_lastSourceValue * IncrementSize) + 1;

					// initialize value to the low end of the bucket
					_value = _upperLimit - IncrementSize;
				}
				else if (_upperLimit <= _value)
				{
					_lastSourceValue = callback.GetNextValue();
					_upperLimit = (_lastSourceValue * IncrementSize) + 1;
				}
				return Make(_value++);
			}
		}

		#endregion

		#region Nested type: NoopOptimizer

		public partial class NoopOptimizer : OptimizerSupport
		{
			private long _lastSourceValue = -1;

			public NoopOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize) { }

			public override long LastSourceValue
			{
				get { return _lastSourceValue; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return false; }
			}

			public override object Generate(IAccessCallback callback)
			{
				// We must use a local variable here to avoid concurrency issues.
				// With the local value we can avoid synchronizing the whole method.

				long val = -1;
				while (val <= 0)
					val = callback.GetNextValue();

				// This value is only stored for easy access in test. Should be no
				// threading concerns there.
				_lastSourceValue = val;

				return Make(val);
			}
		}

		#endregion

		#region Nested type: OptimizerSupport

		/// <summary>
		/// Common support for optimizer implementations.
		/// </summary>
		public abstract partial class OptimizerSupport : IOptimizer
		{
			/// <summary>
			/// Construct an optimizer
			/// </summary>
			/// <param name="returnClass">The expected id class.</param>
			/// <param name="incrementSize">The increment size.</param>
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

		/// <summary>
		/// Optimizer which uses a pool of values, storing the next low value of the range in the database.
		/// <para>
		/// Note that this optimizer works essentially the same as the HiLoOptimizer, except that here the
		/// bucket ranges are actually encoded into the database structures.
		/// </para>
		/// <para>
		/// Note that if you prefer that the database value be interpreted as the bottom end of our current
		/// range, then use the PooledLoOptimizer strategy.
		/// </para>
		/// </summary>
		public partial class PooledOptimizer : OptimizerSupport, IInitialValueAwareOptimizer
		{
			private long _hiValue = -1;
			private long _value;
			private long _initialValue;

			public PooledOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (Log.IsDebugEnabled)
				{
					Log.Debug("Creating pooled optimizer with [incrementSize=" + incrementSize + "; returnClass=" + returnClass.FullName + "]");
				}
			}

			public override long LastSourceValue
			{
				get { return _hiValue; }
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public long LastValue
			{
				get { return _value - 1; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return true; }
			}

			public void InjectInitialValue(long initialValue)
			{
				_initialValue = initialValue;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override object Generate(IAccessCallback callback)
			{
				if (_hiValue < 0)
				{
					_value = callback.GetNextValue();
					if (_value < 1)
					{
						// unfortunately not really safe to normalize this
						// to 1 as an initial value like we do the others
						// because we would not be able to control this if
						// we are using a sequence...
						Log.Info("pooled optimizer source reported [" + _value + "] as the initial value; use of 1 or greater highly recommended");
					}

					if ((_initialValue == -1 && _value < IncrementSize) || _value == _initialValue)
						_hiValue = callback.GetNextValue();
					else
					{
						_hiValue = _value;
						_value = _hiValue - IncrementSize;
					}
				}
				else if (_value >= _hiValue)
				{
					_hiValue = callback.GetNextValue();
					_value = _hiValue - IncrementSize;
				}
				return Make(_value++);
			}
		}

		#endregion

		#region Nested type: PooledLoOptimizer

		public partial class PooledLoOptimizer : OptimizerSupport
		{
			private long _lastSourceValue = -1; // last value read from db source
			private long _value; // the current generator value

			public PooledLoOptimizer(System.Type returnClass, int incrementSize) : base(returnClass, incrementSize)
			{
				if (incrementSize < 1)
				{
					throw new HibernateException("increment size cannot be less than 1");
				}
				if (Log.IsDebugEnabled)
				{
					Log.DebugFormat("Creating pooled optimizer (lo) with [incrementSize={0}; returnClass={1}]", incrementSize, returnClass.FullName);
				}
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override object Generate(IAccessCallback callback)
			{
				if (_lastSourceValue < 0 || _value >= (_lastSourceValue + IncrementSize))
				{
					_lastSourceValue = callback.GetNextValue();
					_value = _lastSourceValue;
					// handle cases where initial-value is less than one (hsqldb for instance).
					while (_value < 1)
						_value++;
				}
				return Make(_value++);
			}

			public override long LastSourceValue
			{
				get { return _lastSourceValue; }
			}

			public override bool ApplyIncrementSizeToSourceValues
			{
				get { return true; }
			}

			/// <summary>
			/// Exposure intended for testing purposes.
			/// </summary>
			public long LastValue
			{
				get { return _value - 1; }
			}
		}

		#endregion
	}
}