namespace CocodriloDog.Core {

	using UnityEngine;

	/// <summary>
	/// Makes the field read-only.
	/// </summary>
	/// <remarks>
	/// For it to work properly with lists, use <see cref="ListWrapper{T}"/>.
	/// </remarks>
	public class ReadOnlyAttribute : PropertyAttribute { }

}