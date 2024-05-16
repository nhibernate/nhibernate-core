using System;

namespace NHibernate.Test.NHSpecificTest.GH3530;

public abstract class Entity
{
	public virtual Guid Id { get; set; }
}

public abstract class DataEntity<T>:Entity where T : struct
{
	public virtual T DataValue { get; set; }
}

public class IntegerEntity : DataEntity<int> { }
public class DateTimeEntity : DataEntity<DateTime> { }

public class DoubleEntity : DataEntity<double> { }
public class DecimalEntity : DataEntity<decimal> { }
public class  NHDateTimeEntity : DataEntity<DateTime> { }
