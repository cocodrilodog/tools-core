namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Wrapper for a <see cref="List{CompositeObject}"/> that is automatically drawn in the inspector
	/// by <c>CompositeListPropertyDrawer</c>, which handles Unity's prefab errors.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class CompositeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	where T : CompositeObject {


		#region Public Properties


		/// <summary>
		/// Used to enable/disable the ability to add or remove objects in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be called from <c>OnValidate</c>
		/// </remarks>
		public bool CanAddRemove {
			get => m_CanAddRemove;
			set => m_CanAddRemove = value;
		}

		/// <summary>
		/// Used to enable/disable the ability to reorder objects in the inspector.
		/// </summary>
		/// <remarks>
		/// This should be called from <c>OnValidate</c>
		/// </remarks>
		public bool CanReorder {
			get => m_CanReorder;
			set => m_CanReorder = value;
		}

		#endregion


		#region Public Constructors

		public CompositeList() {
			m_List = new List<T>();
		}

		#endregion


		#region IList<T>

		public int IndexOf(T item) => m_List.IndexOf(item);

		public void Insert(int index, T item) => m_List.Insert(index, item);

		public void RemoveAt(int index) => m_List.RemoveAt(index);

		public T this[int index] {
			get => m_List[index];
			set => m_List[index] = value;
		}

		#endregion


		#region ICollection<T>

		public int Count => m_List.Count;

		public bool IsReadOnly => ((ICollection<T>)m_List).IsReadOnly;

		public void Add(T item) => m_List.Add(item);

		public void Clear() => m_List.Clear();

		public bool Contains(T item) => m_List.Contains(item);

		public void CopyTo(T[] array, int arrayIndex) => m_List.CopyTo(array, arrayIndex);

		public bool Remove(T item) => m_List.Remove(item);

		#endregion


		#region IEnumerable<T>

		public IEnumerator<T> GetEnumerator() => m_List.GetEnumerator();

		#endregion


		#region IEnumerable

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_List).GetEnumerator();

		#endregion


		#region Utility

		public void ForEach(Action<T> action) => m_List.ForEach(action);

		#endregion


		#region Private Fields

		[SerializeReference]
		private List<T> m_List;

		[HideInInspector]
		[SerializeField]
		private bool m_CanAddRemove = true;

		[HideInInspector]
		[SerializeField]
		private bool m_CanReorder = true;

		#endregion


	}

}