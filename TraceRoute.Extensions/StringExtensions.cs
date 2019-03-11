using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TraceRoute.Extensions
{
    /// <summary>
    /// Provids extensions to teh string type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Detects The Type of a strings contents
        /// </summary>
        /// <param name="stringValue">The string to test</param>
        /// <returns>The value converted to its apparent type.</returns>
        public static object DetectType(this string stringValue)
        {
            var expectedTypes = new List<Type>
                {
                    typeof (bool),
                    typeof (Int16),
                    typeof (Int32),
                    typeof (Int64),
                    typeof (decimal),
                    typeof (Single),
                    typeof (double),
                    typeof (DateTime),
                    typeof (Guid)
                };
            foreach (var converter in expectedTypes.Select(TypeDescriptor.GetConverter).Where(converter => converter.CanConvertFrom(typeof (string))))
            {
                try
                {
                    // You'll have to think about localization here
                    var newValue = converter.ConvertFromInvariantString(stringValue);
                    if (newValue != null)
                    {
                        return newValue;
                    }
                }
// ReSharper disable EmptyGeneralCatchClause
                catch
// ReSharper restore EmptyGeneralCatchClause
                {
                    // Can't convert given string to this type
                }
            }
            return stringValue;
        }
    }
}