using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJH.Utility.Utils
{
    [Serializable]
    public class SerializableKeyValue<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
    }

    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SerializableKeyValue<TKey, TValue>> _keyValueList = new List<SerializableKeyValue<TKey, TValue>>();

        public void OnBeforeSerialize()
        {
            if (_keyValueList == null) return;
            if (this.Count < _keyValueList.Count)
            {
                return;
            }

            _keyValueList.Clear();

            foreach (var kv in this)
            {
                _keyValueList.Add(new SerializableKeyValue<TKey, TValue>()
                {
                    Key = kv.Key,
                    Value = kv.Value
                });
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            foreach (var kv in _keyValueList)
            {
                if (!this.TryAdd(kv.Key, kv.Value))
                {
                    Debug.LogError($"List has duplicate Key");
                }
            }
        }
    }
}