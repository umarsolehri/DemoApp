using System.Reflection;

namespace Domain.Common
{
    public static class EnumHelper
    {

        public static List<EnumModel<T>> GetList<T>(bool useDescription = false) where T : struct, Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new EnumModel<T>
                {
                    Value = e,
                    Text = useDescription ? GetEnumDescription(e) : e.ToString()
                })
                .ToList();
        }

        public static string GetEnumDescription<T>(T enumValue) where T : Enum
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            return field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? enumValue.ToString();
        }

        public static string GetDescription<T>(this T enumValue) where T : Enum
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            return field?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? enumValue.ToString();
        }

        //public static IEnumerable<DropdownVm> GetEnumList<T>() where T : Enum
        //{
        //    return Enum.GetValues(typeof(T))
        //        .Cast<T>()
        //        .Select(e => new DropdownVm
        //        {
        //            Code = Convert.ToInt32(e),
        //            Label = e.GetDescription()
        //        });
        //}

    }

    public class EnumModel<T> where T : Enum
    {
        public T Value { get; set; }
        public string Text { get; set; }
    }
}
