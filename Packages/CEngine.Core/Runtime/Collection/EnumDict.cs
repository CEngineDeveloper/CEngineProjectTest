using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CYM
{
    /// <summary>
    ///     Serializes a value for each entry in an enum
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <typeparam name="TData">The value type</typeparam>
    [Serializable]
    public class EnumDict<TEnum, TData>
        : ISerializationCallbackReceiver, IReadOnlyDictionary<TEnum, TData>
        where TEnum : Enum
    {
        [Serializable]
        internal struct DictEntry
        {

            [SerializeField] private TEnum key;
            [SerializeField] private TData value;


            public TEnum Key => key;

            public TData Value => value;


            public DictEntry(KeyValuePair<TEnum, TData> kv)
            {
                key = kv.Key;
                value = kv.Value;
            }

            public DictEntry(TEnum key, TData value)
            {
                this.key = key;
                this.value = value;
            }

        }


        [SerializeField] private DictEntry[] entries =
            Array.Empty<DictEntry>();

        private readonly Dictionary<TEnum, TData> dictionary =
            new Dictionary<TEnum, TData>();


        /// <summary>
        ///     Get the stored value for the given key
        /// </summary>
        /// <remarks>
        ///     This returns the default value for TData if no value was ever
        ///     specified
        /// </remarks>
        /// <param name="key">The key to get</param>
        public TData this[TEnum key] =>
            Get(key);

        public IEnumerable<TEnum> Keys => dictionary.Keys;

        public IEnumerable<TData> Values => dictionary.Values;

        public int Count => dictionary.Count;


        /// <summary>
        ///     Creates a new enum-dict with the default value for TData on each key
        /// </summary>
        public EnumDict()
        {
        }

        /// <summary>
        ///     Creates a new enum-dict with the specified key-value pairs. Missing keys
        ///     will get the default value for TData
        /// </summary>
        /// <param name="entries">The preset entries</param>
        public EnumDict(IDictionary<TEnum, TData> entries)
        {
            foreach (var keyValuePair in entries)
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);
        }


        public bool ContainsKey(TEnum key) =>
            dictionary.ContainsKey(key);

        public bool TryGetValue(TEnum key, out TData value) =>
            dictionary.TryGetValue(key, out value);

        public void OnBeforeSerialize() =>
            entries = EnumUtil<TEnum>.GetEnumValues()
                .Select(key =>
                {
                    var value = this[key];
                    return new DictEntry(key, value);
                }).ToArray();

        public void OnAfterDeserialize()
        {
            dictionary.Clear();
            foreach (var entry in entries)
                dictionary.Add(entry.Key, entry.Value);
        }

        /// <summary>
        ///     Get the stored value for the given key
        /// </summary>
        /// <remarks>
        ///     This returns the default value for TData if no value was ever
        ///     specified
        /// </remarks>
        /// <param name="key">The key to get</param>
        /// <returns>The value</returns>
        public TData Get(TEnum key) =>
            dictionary.TryGetValue(key, out var value)
                ? value
                : default;

        public IEnumerator<KeyValuePair<TEnum, TData>> GetEnumerator() =>
            dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}