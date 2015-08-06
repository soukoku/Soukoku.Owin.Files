using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    /// <summary>
    /// Provides very basic methods to work with http headers.
    /// </summary>
    public class OwinHeaders : IDictionary<string, string[]>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="OwinHeaders"/> class.
        /// </summary>
        /// <param name="rawHeaders">The store.</param>
        /// <exception cref="System.ArgumentNullException">store</exception>
        public OwinHeaders(IDictionary<string, string[]> rawHeaders)
        {
            if (rawHeaders == null) { throw new ArgumentNullException("rawHeaders"); }

            _raw = rawHeaders;
        }

        IDictionary<string, string[]> _raw;

        #region custom header tools

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
                if (_raw.TryGetValue(header, out test) && test != null)
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
                _raw.Remove(header);
            }
            else
            {
                _raw[header] = new[] { value };
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
                _raw.Remove(header);
            }
            else
            {
                if (collapse)
                {
                    _raw[header] = new[] { string.Join(",", values) };
                }
                else
                {
                    _raw[header] = values;
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
            if (_raw.TryGetValue(header, out old))
            {
                string[] final = new string[old.Length + values.Length];
                Array.Copy(old, final, old.Length);
                Array.Copy(values, 0, final, old.Length, values.Length);
                _raw[header] = final;
            }
            else
            {
                _raw[header] = values;
            }
        }

        #endregion

        #region idictionary

        public bool ContainsKey(string key)
        {
            return _raw.ContainsKey(key);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, string[] value)
        {
            _raw.Add(key, value);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return _raw.Remove(key);
        }

        public bool TryGetValue(string key, out string[] value)
        {
            return _raw.TryGetValue(key, out value);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(KeyValuePair<string, string[]> item)
        {
            _raw.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            _raw.Clear();
        }

        public bool Contains(KeyValuePair<string, string[]> item)
        {
            return _raw.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
        {
            _raw.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, string[]> item)
        {
            return _raw.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
        {
            return _raw.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _raw.GetEnumerator();
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return _raw.Keys;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        public ICollection<string[]> Values
        {
            get
            {
                return _raw.Values;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get
            {
                return _raw.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _raw.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.String[]"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String[]"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string[] IDictionary<string, string[]>.this[string key]
        {
            get
            {
                return _raw[key];
            }

            set
            {
                _raw[key] = value;
            }
        }

        #endregion

        #region common headers


        /// <summary>
        /// Gets or sets the content mime type.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType
        {
            get { return this[HttpHeaderNames.ContentType]; }
            set { this.Replace(HttpHeaderNames.ContentType, value); }
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
                var test = this[HttpHeaderNames.ContentLength];
                long val;
                if (long.TryParse(test, out val))
                {
                    return val;
                }
                return null;
            }
            set { this.Replace(HttpHeaderNames.ContentLength, value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : null); }
        }

        #endregion
    }
}
