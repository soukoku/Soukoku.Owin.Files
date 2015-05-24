using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin
{
    /// <summary>
    /// Provides very basic methods to work with http headers.
    /// </summary>
    public class Headers
    {
        IDictionary<string, string[]> _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="Headers"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <exception cref="System.ArgumentNullException">store</exception>
        public Headers(IDictionary<string, string[]> store)
        {
            if (store == null) { throw new ArgumentNullException("store"); }

            _store = store;
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified header.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        public string this[string header]
        {
            get
            {
                string[] test;
                if (_store.TryGetValue(header, out test) && test != null)
                {
                    return string.Join(",", test);
                }
                return null;
            }
        }


        /// <summary>
        /// Sets the specified header values. Existing values will be replaced.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="value">The value.</param>
        public void Replace(string header, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _store.Remove(header);
            }
            else
            {
                _store[header] = new[] { value };
            }
        }

        /// <summary>
        /// Sets the specified header values. Existing values will be replaced.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="collapse">if set to <c>true</c> then collapse values into a csv right away.</param>
        /// <param name="values">The values.</param>
        public void Replace(string header, bool collapse, params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                _store.Remove(header);
            }
            else
            {
                if (collapse)
                {
                    _store[header] = new[] { string.Join(",", values) };
                }
                else
                {
                    _store[header] = values;
                }
            }
        }

        /// <summary>
        /// Appends the specified header values. Existing values will not be replaced.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="values">The values.</param>
        public void Append(string header, params string[] values)
        {
            if (values == null || values.Length == 0) { return; }

            string[] old;
            if (_store.TryGetValue(header, out old))
            {
                string[] final = new string[old.Length + values.Length];
                Array.Copy(old, final, old.Length);
                Array.Copy(values, 0, final, old.Length, values.Length);
                _store[header] = final;
            }
            else
            {
                _store[header] = values;
            }
        }

        #region common headers


        /// <summary>
        /// Gets or sets the content mime type.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType
        {
            get { return this["Content-Type"]; }
            set { this.Replace("Content-Type", value); }
        }

        /// <summary>
        /// Gets or sets the content byte length.
        /// </summary>
        /// <value>
        /// The length of the content.
        /// </value>
        public long? ContentLength
        {
            get
            {
                var test = this["Content-Length"];
                long val;
                if (long.TryParse(test, out val))
                {
                    return val;
                }
                return null;
            }
            set { this.Replace("Content-Length", value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null); }
        }

        #endregion
    }
}
