namespace CocodriloDog.Core {

	using System;
	using System.Collections.Generic;
	using System.Threading;
	using UnityEngine;

	/// <summary>
	/// Utility to extend Unity's <see cref="Awaitable"/> functionality.
	/// </summary>
	public static class AwaitableUtility {

		/// <summary>
		/// Waits while <paramref name="condition"/> is <c>true</c>.
		/// </summary>
		/// <param name="condition">A function that returns <c>true</c> or <c>false</c>.</param>
		/// <param name="token">The cancellation token.</param>
		/// <returns>The awaitable</returns>
		public static async Awaitable WaitWhile(Func<bool> condition, CancellationToken token = default) {
			while (condition()) {
				await Awaitable.NextFrameAsync(token);
			}
		}

		/// <summary>
		/// Waits until <paramref name="condition"/> is <c>true</c>.
		/// </summary>
		/// <param name="condition">A function that returns <c>true</c> or <c>false</c>.</param>
		/// <param name="token">The cancellation token.</param>
		/// <returns>The awaitable</returns>
		public static async Awaitable WaitUntil(Func<bool> condition, CancellationToken token = default) {
			while (!condition()) {
				await Awaitable.NextFrameAsync(token);
			}
		}

		/// <summary>
		/// Waits until all awaitables are done.
		/// </summary>
		/// <param name="awaitables">The awaitables to wait for.</param>
		/// <returns>The grouped awaitble.</returns>
		public static async Awaitable WhenAll(params Awaitable[] awaitables) {
			foreach (var awaitable in awaitables) {
				await awaitable;
			}
		}

	}

}