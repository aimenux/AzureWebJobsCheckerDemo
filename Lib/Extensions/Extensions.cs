using System;
using System.Text;

namespace Lib.Extensions
{
    public static class Extensions
    {
        public static void WriteLine(this ConsoleColor color, object value)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        public static StringBuilder AppendLine(this StringBuilder builder, string propertyName, string propertyValue)
        {
            if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
            {
                return builder.AppendLine($"{propertyName.Trim()}: {propertyValue.Trim()}");
            }

            return builder;
        }

        public static string Dump(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var stringBuilder = new StringBuilder();
            var properties = obj.GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                var propertyName = propertyInfo.Name;
                var propertyValue = propertyInfo.GetValue(obj, null);
                stringBuilder.AppendLine(propertyName, propertyValue?.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}
