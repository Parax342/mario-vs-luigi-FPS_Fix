using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;

namespace MvL_Unlocked {

	[HarmonyPatch(typeof(NSMB.GlobalController))]
	public class UnlockValueLimits {

		private static bool hasUnfocusedBefore = false;

		[HarmonyPatch("OnApplicationFocus")]
		[HarmonyPrefix]
		static bool OnApplicationFocusPatch(NSMB.GlobalController __instance, ref bool focus, ref Coroutine ___fadeMusicRoutine, ref Coroutine ___fadeSfxRoutine, ref int ___previousFrameRate, ref int ___previousVsyncCount) {

			// This prevents a bug where the fps maxes out if the window is being focused the first time OnApplicationFocus() is run
			if (!hasUnfocusedBefore) {
				___previousFrameRate = Application.targetFrameRate;
				___previousVsyncCount = QualitySettings.vSyncCount;
				hasUnfocusedBefore = true;
			}

			// Lower the fps when window is out of focus, if the config is enabled
			if (Main.LowerFPS.Value) {
				if (!focus) {
					// Store old fps and vsync values to restore later
					___previousFrameRate = Application.targetFrameRate;
					___previousVsyncCount = QualitySettings.vSyncCount;

					// Lower fps and turn off vsync
					Application.targetFrameRate = Math.Max(Main.TargetLowerFPS.Value, 1);
					QualitySettings.vSyncCount = 0;
				}
				if (focus) {
					// Restore previous framerate
					Application.targetFrameRate = ___previousFrameRate;
					QualitySettings.vSyncCount = ___previousVsyncCount;
				}
			}

			// Copy of vanilla code to un-fade the music when refocusing the window
			// Exists to seperate music un-fading code from framerate changing Bcode
			if (focus) {
				Singleton<NSMB.Settings>.Instance.ApplyVolumeSettings();

				if (___fadeMusicRoutine != null) {
					__instance.StopCoroutine(___fadeMusicRoutine);
					___fadeMusicRoutine = null;
				}

				if (___fadeSfxRoutine != null) {
					__instance.StopCoroutine(___fadeSfxRoutine);
					___fadeSfxRoutine = null;
				}

				return false;
			}

			return true;
		}
	}
}