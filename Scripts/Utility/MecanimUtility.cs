namespace CocodriloDog.Utility {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class MecanimUtility {


		#region Public Static Methods

		public static AnimationClip GetClipByName(Animator animator, string name) {
			foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips) {
				if(clip.name == name) {
					return clip;
				}
			}
			return null;
		}

		public static AnimationEvent GetAnimationEventByName(AnimationClip clip, string name) {
			foreach (AnimationEvent animationEvent in clip.events) {
				if (animationEvent.functionName == name) {
					return animationEvent;
				}
			}
			return null;
		} 

		#endregion


	}

}