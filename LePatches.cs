using AlmostEngine;
using HarmonyLib;
using System;
using System.Net.Sockets;
using UnityEngine;
namespace cloudEXP
{

    [HarmonyPatch(typeof(MusicManager))]
    class MusManPatch
    {
        [HarmonyPatch("PlayMidi", new Type[]
        {
                typeof(string),
                typeof(bool)
        })]
        [HarmonyPrefix]
        public static bool ignoreplaySCH()
        {
            bool canplay = true;
            if (Singleton<BaseGameManager>.Instance != null)
            {
                SchProblem schProblem = Singleton<BaseGameManager>.Instance.GetComponent<SchProblem>();
                canplay = (schProblem == null || schProblem.schstate <= 0);
            }
            return canplay;
        }
        [HarmonyPatch("StopMidi", new Type[]
        {
        })]
        [HarmonyPrefix]
        public static bool ignorestopSCH()
        {
            bool canplay = true;
            if (Singleton<BaseGameManager>.Instance != null)
            {
                SchProblem schProblem = Singleton<BaseGameManager>.Instance.GetComponent<SchProblem>();
                canplay = (schProblem == null || schProblem.schstate <= 0);
            }
            return canplay;
        }
    }
    [HarmonyPatch(typeof(Elevator),"ButtonPressed")]
    static class ElevatingPatch
    {
        [HarmonyPostfix]
        public static void SpawnManager(Elevator __instance)
        {
            BaseGameManager bgm = Singleton<BaseGameManager>.Instance;
			if (bgm != null)
            {
                SchProblem schProblem = bgm.GetComponent<SchProblem>();
                if (schProblem != null && schProblem.schstate != -1)
                {
                    schProblem.schstate = -1;
                    Singleton<MusicManager>.Instance.StopMidi();
                    schProblem.bfalpha = 0.5f;
                    bgm.Ec.standardDarkLevel = Color.black;
                    bgm.Ec.InitializeLighting();
                }
            }
        }
    }
	[HarmonyPatch(typeof(ElevatorScreen), "StartGame")]
    static class InitPatch
    {
        [HarmonyPostfix]
        public static void SpawnManager(ElevatorScreen __instance)
        {
            if (Singleton<BaseGameManager>.Instance != null)
            {
                SchProblem schProblem = Singleton<BaseGameManager>.Instance.GetComponent<SchProblem>();
                if (schProblem == null)
                {
                    schProblem = Singleton<BaseGameManager>.Instance.gameObject.AddComponent<SchProblem>();
                }
                schProblem.Initialize();
            }
        }
    }
}