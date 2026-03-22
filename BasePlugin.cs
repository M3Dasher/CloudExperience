using BepInEx;
using System.IO;
using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Collections;


namespace cloudEXP
{
    [BepInPlugin("xcloud.bbp.schoolhouseproblem", "Schoolhouse Trouble In Escape", "1.0.0.0")]

    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

    public class BasePlugin : BaseUnityPlugin
    {
        private IEnumerator LoadThings()
        {
            yield return 2;
            yield return "Loading the FUCKING MIDIs...";
            string[] files = Directory.GetFiles(AssetLoader.GetModPath(this), "*.mid");
            for (int i = 0; i < files.Length; i++)
            {
                AssetLoader.MidiFromFile(files[i], Path.GetFileNameWithoutExtension(files[i]));
            }
            yield break;
        }
        public void Awake()
        {
            Harmony harmony = new Harmony("xcloud.bbp.schoolhouseproblem");
            LoadingEvents.RegisterOnAssetsLoaded(base.Info, this.LoadThings(), LoadingEventOrder.Start);
            harmony.PatchAllConditionals();
        }
    }
}