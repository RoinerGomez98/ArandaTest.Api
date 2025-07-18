using System.Net;
using System.Reflection;

namespace ArandaTest.Domain.Utils
{
    public class GenericResponse<T>
    {
        public string? Message { get; set; }
        public HttpStatusCode Status { get; set; }
        public T? Result { get; set; } = default;
        public static void InjectNonNull<TSource, TTarget>(TSource source, TTarget target)
        {
            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var sourceProp in sourceProps)
            {
                var targetProp = targetProps.FirstOrDefault(p => p.Name == sourceProp.Name && p.PropertyType == sourceProp.PropertyType);
                if (targetProp != null && targetProp.CanWrite)
                {
                    var value = sourceProp.GetValue(source);

                    if (value != null)
                    {
                        if (value is DateTime dt && dt == DateTime.MinValue)
                            continue;

                        targetProp.SetValue(target, value);
                    }
                }
            }
        }
    }
}