namespace CocodriloDog.Core {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class Vector3Extensions {


		#region Public Static Methods

		/// <summary>
		/// Creates a vector with inverted <c>x</c>, <c>y</c>, and <c>z</c>.
		/// </summary>
		/// <returns>The inverse.</returns>
		/// <param name="position">Position.</param>
		public static Vector3 Inverse(this Vector3 position) {
			return new Vector3(1f / position.x, 1f / position.y, 1f / position.z);
		}

		#endregion


	}

}