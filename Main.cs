using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;

namespace COM3D2.DisableVRControllerModes
{
    [BepInPlugin("DisableVRControllerModes", "Disable VR Controller Modes", "0.2")]
    [BepInDependency("COM3D2.GUIAPI", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        // Config
        internal static ConfigEntry<bool> disableCamera;
        internal static ConfigEntry<bool> disableItem;
        internal static ConfigEntry<bool> debug;

        internal static ManualLogSource logger;

        private static bool isYotogi = false;
        private static bool isDance = false;

        private void Awake()
        {
            // Disable plugin if the game isn't in VR mode            
            if (!GameMain.Instance.VRMode)
            {
                Logger.LogWarning("VR Mode Off: Shutting DisableVRControllerModes down.");
                Destroy(this);
                return;
            }

            // Logger
            Main.logger = base.Logger;

            // Harmony
            Harmony.CreateAndPatchAll(typeof(Main));
            Logger.LogInfo("Plugin DisableVRControllerModes is loaded!");

            // BepinEx Config
            disableCamera = Config.Bind("Modes", "Camera", true, "Disable Camera Mode for VR Controllers");
            disableItem = Config.Bind("Modes", "Item", true, "Disable Item Mode (Stick/Toys) for VR Controllers");
            debug = Config.Bind("Debug", "Debug Mode", false, "Enable debug lines in the console");
            

            // Check if COM3D2 GUIAPI is loaded
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("COM3D2.GUIAPI"))
            {
                GUIAPIHandling gUIAPIHandling = new();
                gUIAPIHandling.GUIAPIAwake();
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            isYotogi = scene.buildIndex == 14;
            isDance = FindObjectOfType<DanceMain>() != null;
        }

        // Occulus Patching
        [HarmonyPatch(typeof(OvrControllerBehavior2), "ChangeMode")]
        [HarmonyPrefix]
        public static void DisableCameraOculus(ref OvrControllerBehavior2.OvrControllerMode f_eNewMode)
        {
            if (debug.Value) { Main.logger.LogMessage($"New mode: {f_eNewMode}"); }
            if (disableCamera.Value)
            {
                if (f_eNewMode == OvrControllerBehavior2.OvrControllerMode.CAMERA)
                {
                    if (isDance) { f_eNewMode = OvrControllerBehavior2.OvrControllerMode.DANCE; }
                    else { f_eNewMode = OvrControllerBehavior2.OvrControllerMode.ITEM; }
                }
            }
            if (disableItem.Value)
            {
                if (f_eNewMode == OvrControllerBehavior2.OvrControllerMode.ITEM)
                {
                    if (isYotogi) { f_eNewMode = OvrControllerBehavior2.OvrControllerMode.YOTOGI; }
                    else { f_eNewMode = OvrControllerBehavior2.OvrControllerMode.HAND; }
                }
            }
            if (debug.Value) { Main.logger.LogMessage($"New mode after change: {f_eNewMode}"); }
        }

        // Vive Patching
        [HarmonyPatch(typeof(ViveControllerBehavior2), "ChangeMode")]
        [HarmonyPrefix]
        public static void DisableCamera(ref ViveControllerBehavior2.OvrControllerMode f_eNewMode)
        {
            if (debug.Value) { Main.logger.LogMessage($"New mode: {f_eNewMode}"); }
            if (disableCamera.Value)
            {
                if (f_eNewMode == ViveControllerBehavior2.OvrControllerMode.CAMERA)
                {
                    if (isDance) { f_eNewMode = ViveControllerBehavior2.OvrControllerMode.DANCE; }
                    else { f_eNewMode = ViveControllerBehavior2.OvrControllerMode.ITEM; }
                }
            }
            if (disableItem.Value)
            {
                if (f_eNewMode == ViveControllerBehavior2.OvrControllerMode.ITEM)
                {
                    if (isYotogi) { f_eNewMode = ViveControllerBehavior2.OvrControllerMode.YOTOGI; }
                    else { f_eNewMode = ViveControllerBehavior2.OvrControllerMode.HAND; }
                }
            }
            if (debug.Value) { Main.logger.LogMessage($"New mode after change: {f_eNewMode}"); }
        }
    }
}
