namespace CocodriloDog.Utility {

	using UnityEngine;
	using System.Collections;

	/// <summary>
	/// Mathf extensions.
	/// </summary>
	public static class Mathf2 {


		#region Public Static Methods

		/// <summary>
		/// Loops an integer between 0 and total - 1. Modifies aInt if is out of bounds either if is < 0 or >= total, 
		/// otherwise returns the same value.
		/// </summary>
		/// <returns>The int.</returns>
		/// <param name="aInt">A int.</param>
		/// <param name="total">Total.</param>
		public static int RepeatInt(int aInt, int total) {
			int modulo = aInt % total;
			return modulo < 0 ? modulo + total : modulo;
		}

		public static float RepeatFloat(float number, float total) {
			float modulo = number % total;
			return modulo < 0 ? modulo + total : modulo;
		}

		/// <summary>
		/// Calculates the average between a and b.
		/// </summary>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static float Average(float a, float b) {
			return (a + b) / 2;
		}

		/// <summary>
		/// Calculates de average between the provided numbers.
		/// </summary>
		/// <param name="numbers">Numbers.</param>
		public static float Average(params float[] numbers) {
			float sum = 0;
			foreach (float number in numbers)
				sum += number;
			return sum / numbers.Length;
		}

		/// <summary>
		/// Calculates de average between the provided numbers.
		/// </summary>
		/// <param name="numbers">Numbers.</param>
		public static float Average(params long[] numbers) {
			float sum = 0;
			foreach (long number in numbers)
				sum += number;
			return sum / numbers.Length;
		}

		/// <summary>
		/// Rotates a 2D vector 90 degrees, clockwise.
		/// </summary>
		/// <returns>The rotated vector.</returns>
		/// <param name="v">V.</param>
		public static Vector2 PerpendicularClockwise(Vector2 v) {
			return new Vector2(v.y, -v.x);
		}

		/// <summary>
		/// Rotates a 2D vector 90 degrees, counterclockwise.
		/// </summary>
		/// <returns>The rotated vector.</returns>
		/// <param name="v">V.</param>
		public static Vector2 PerpendicularCounterClockwise(Vector2 v) {
			return new Vector2(-v.y, v.x);
		}

		/// <summary>
		/// Calculates de signed angle between two 2D vectors, from v1 to v2.
		/// </summary>
		/// <returns>The signed angle.</returns>
		/// <param name="v1">V1.</param>
		/// <param name="v2">V2.</param>
		public static float SignedEulerAngle(Vector2 v1, Vector2 v2) {
			float perpDot = v1.x * v2.y - v1.y * v2.x;
			return (float)Mathf.Atan2(perpDot, Vector2.Dot(v1, v2)) * Mathf.Rad2Deg;
		}

		/// <summary>
		/// Rotates a 3D vector around the z axis and returns the result.
		/// </summary>
		/// <returns>The rotated vector.</returns>
		/// <param name="v">The initial vector.</param>
		/// <param name="degrees">Degrees.</param>
		public static Vector3 RotateAroundZ(Vector3 v, float degrees) {
			return Quaternion.Euler(0, 0, degrees) * v;
		}

		#endregion


	}

}