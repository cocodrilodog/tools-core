namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public abstract class KeyedResource<T_Key, T_Resource, T_KeyedResourceEntry> : ScriptableObject
		where T_KeyedResourceEntry : KeyedResourceEntry<T_Key, T_Resource> {


		#region Public Properties

		/// <summary>
		/// A key that represents the current state of the system. For example
		/// <see cref="Application.systemLanguage"/> or <see cref="Application.platform"/>.
		/// </summary>
		public abstract T_Key CurrentKey { get; }

		/// <summary>
		/// A key that will be used if no resource is found by a provided key.
		/// </summary>
		public abstract T_Key FallbackKey { get; }

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets the resource that corresponds to the <see cref="CurrentKey"/>.
		/// </summary>
		/// <returns>The resource.</returns>
		public virtual T_Resource GetResource() {
			return GetResource(CurrentKey);
		}

		/// <summary>
		/// Gets the resource that corresponds to the <paramref name="key"/>.
		/// </summary>
		/// <param name="key">A key</param>
		/// <returns>The resource.</returns>
		public virtual T_Resource GetResource(T_Key key) {
			T_KeyedResourceEntry entry = m_KeyedResourceList.FirstOrDefault(e => e.Key.Equals(key));
			if (entry == null) {
				entry = m_KeyedResourceList.FirstOrDefault(e => e.Key.Equals(FallbackKey));
			}
			if (entry != null) {
				return entry.Resource;
			}
			return default;
		}

		#endregion


		#region Private Fields

		[SerializeField]
		public List<T_KeyedResourceEntry> m_KeyedResourceList;

		#endregion


	}

	/// <summary>
	/// Pairs a resource with a key.
	/// </summary>
	/// <typeparam name="T_Key">A key.</typeparam>
	/// <typeparam name="T_Resource">A resource.</typeparam>
	public class KeyedResourceEntry<T_Key, T_Resource> {


		#region Public Fields

		[SerializeField]
		public T_Key Key;

		[SerializeField]
		public T_Resource Resource;

		#endregion


	}

}