﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suzumiya.Main.Core.Manager.Dialog
{
    public interface IDialogParameters
    {
        /// <summary>
        /// Adds the key and value to the collection.
        /// </summary>
        /// <param name="key">The key to reference this parameter value in the collection.</param>
        /// <param name="value">The parameter value to store.</param>
        void Add(string key, object value);

        /// <summary>
        /// Checks the collection for the presence of a key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns><c>true</c> if key exists; <c>false</c> otherwise.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// The number of parameters in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The keys in the collection.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Gets the parameter value referenced by a key.
        /// </summary>
        /// <typeparam name="T">The type of object to be returned.</typeparam>
        /// <param name="key">The key of the parameter value to be returned.</param>
        /// <returns>The matching parameter of type <typeparamref name="T"/>.</returns>
        T GetValue<T>(string key);

        /// <summary>
        /// Gets the parameter value if the referenced key exists.
        /// </summary>
        /// <typeparam name="T">The type of object to be returned.</typeparam>
        /// <param name="key">The key of the parameter value to be returned.</param>
        /// <param name="value">The matching parameter of type <typeparamref name="T"/> if the key exists.</param>
        /// <returns><c>true</c> if the parameter exists; <c>false</c> otherwise.</returns>
        bool TryGetValue<T>(string key, out T value);
    }
}
