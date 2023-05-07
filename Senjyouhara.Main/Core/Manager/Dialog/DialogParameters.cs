using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public class DialogParameters : IDialogParameters
    {
        private readonly Dictionary<string, object> _entries = new();

        public int Count => throw new NotImplementedException();

        public IEnumerable<string> Keys => _entries.Keys;

        public void Add(string key, object value)
        {
            _entries[key] = value;
        }

        public bool ContainsKey(string key)
        {
           return _entries.ContainsKey(key);
        }

        public T GetValue<T>(string key)
        {
            return (T) _entries[key];
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            var k = _entries.ContainsKey(key);
            if (k)
            {
               value = (T)_entries[key];
               return true;
            }

            value = default;
            return k;
        }
    }
}
