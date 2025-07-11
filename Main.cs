using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using UnityEngine;

namespace MvL_Unlocked {
	[BepInPlugin(modID, modName, version)]
	public class Main : BaseUnityPlugin {
		private const string modID = "dev.parax342.mariovsluigi.fps_fix";
		private const string modName = "FPS Fix";
		private const string version = "1.0.0";

		public static Main instance { get; private set; }

		public static BepInEx.Configuration.ConfigFile ModConfig = new BepInEx.Configuration.ConfigFile("fps_fix", true);

		public static ConfigEntry<bool> LowerFPS;
		public static ConfigEntry<int> TargetLowerFPS;

		void Awake() {

			instance = this;

			Main.LowerFPS = base.Config.Bind<bool>("Stuff", "LowerFPSWhenWindowUnfocused", true, "Lowers the game\'s FPS while the game is out of focus");
			Main.TargetLowerFPS = base.Config.Bind<int>("Stuff", "TargetUnfocusedFPS", 15, "The framerate the game will be lowered to when not in focus (Number greater than 1)");

			var harmony = new Harmony(modID);
			harmony.PatchAll();

			Debug.Log("Loaded FPS Fix Succesfully!");
		}
	}
}
