namespace CocodriloDog.Core {

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Wrapper for lists that work correctly with <see cref="HelpAttribute"/>, <see cref="HideAttribute"/>, and
	/// <see cref="ReadOnlyAttribute"/>.
	/// </summary>
	/// 
	/// <typeparam name="T">Any type</typeparam>
	/// 
	/// <remarks>
	/// Unity does not support property drawers for lists, only for the elements. This is a workaround for lists
	/// to work properly with the attributes mentioned above.
	/// </remarks>
	[Serializable]
	public class ListWrapper<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable {


		#region Public Constructors

		public ListWrapper() {
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

		public void AddRange(IEnumerable<T> collection) => m_List.AddRange(collection);

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


		#region Utility & Linq

		public void ForEach(Action<T> action) => m_List.ForEach(action);

		public IEnumerable<T> Where(Func<T, bool> condition) => m_List.Where(condition);

		public IEnumerable<T> Where(Func<T, int, bool> condition) => m_List.Where(condition);

		public IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector) => m_List.Select(selector);

		public IEnumerable<TResult> Select<TResult>(Func<T, int, TResult> selector) => m_List.Select(selector);

		public T FirstOrDefault() => m_List.FirstOrDefault();

		public T FirstOrDefault(Func<T, bool> predicate) => m_List.FirstOrDefault(predicate);

		#endregion


		#region Private Fields

		[SerializeField]
		private List<T> m_List;

		#endregion


	}

}