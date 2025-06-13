namespace Infrastructure.Common
{

    public class DateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
            parameter.DbType = DbType.Date;
        }

        public override DateOnly Parse(object value)
        {
            return DateOnly.FromDateTime((DateTime)value);
        }
    }

    public class TimeOnlyHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.Value = value.ToTimeSpan();
            parameter.DbType = DbType.Time;
        }

        public override TimeOnly Parse(object value)
        {
            if (value is TimeSpan ts)
                return TimeOnly.FromTimeSpan(ts);
            else if (value is DateTime dt)
                return TimeOnly.FromDateTime(dt);
            else
                throw new DataException("Cannot convert value to TimeOnly");
        }
    }

}
