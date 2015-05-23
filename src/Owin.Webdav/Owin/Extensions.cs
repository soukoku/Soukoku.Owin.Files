using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin
{
    /// <summary>
    /// Contains extension methods for Owin.
    /// </summary>
    public static class OwinExtensions
    {
        /// <summary>
        /// Gets the value from the Owin environment.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="environment">The environment.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Get<T>(this IDictionary<string, object> environment, string key)
        {
            return Get(environment, key, default(T));
        }

        /// <summary>
        /// Gets the value from the Owin environment.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="environment">The environment.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T Get<T>(this IDictionary<string, object> environment, string key, T defaultValue)
        {
            object value;
            if(environment.TryGetValue(key, out value))
            {
                return (T)value;
            }
            return defaultValue;
        }
    }
}
