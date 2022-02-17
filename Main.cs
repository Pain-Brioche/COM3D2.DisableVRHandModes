using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;

namespace COM3D2.DisableVRControllerModes
{
    [BepInPlugin("DisableVRControllerModes", "Disable VR Controller Modes", "0.3")]
    [BepInDependency("COM3D2.GUIAPI", BepInDependency.DependencyFlags.SoftDependency)]
    public class Main : BaseUnityPlugin
    {
        // Config
        internal static ConfigEntry<bool> disableCamera;
        internal static ConfigEntry<bool> disableItem;
        internal static ConfigEntry<bool> disableDance;
        internal static ConfigEntry<bool> debug;

        internal static ManualLogSource logger;
        private static OvrControllerBehavior2 ovrControllerBehavior2;
        private static ViveControllerBehavior2 viveControllerBehavior2;

        //private static bool isYotogi = false;
        //private static bool isDance = false;

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

            // BepinEx Config
            disableCamera = Config.Bind("Modes", "Camera", true, "Disable Camera Mode for VR Controllers");
            disableItem = Config.Bind("Modes", "Item", true, "Disable Item Mode (Toys) for VR Controllers");
            disableDance = Config.Bind("Modes", "Dance", true, "Disable Dance Stick for VR Controllers");
            debug = Config.Bind("Debug", "Debug Mode", false, "Enable debug lines in the console");
            

            // Check if COM3D2 GUIAPI is loaded
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("COM3D2.GUIAPI"))
            {
                GUIAPIHandling gUIAPIHandling = new();
                gUIAPIHandling.GUIAPIAwake();
            }

            //SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /*
        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            isYotogi = scene.buildIndex == 14;
            isDance = FindObjectOfType<DanceMain>() != null;
        }
        */

        // Occulus 
        [HarmonyPatch(typeof(OvrControllerBehavior2), "Awake")]
        [HarmonyPostfix]
        public static void OculusAwakePostfix(OvrControllerBehavior2 __instance)
        {
            //__instance.m_bModeHide = GetHidenModesArray();
            ovrControllerBehavior2 = __instance;
            UpdateOculusHiddenModeArray();
        }


        // Vive
        [HarmonyPatch(typeof(ViveControllerBehavior2), "Awake")]
        [HarmonyPostfix]
        public static void ViveAwakePostfix(ViveControllerBehavior2 __instance)
        {
            //__instance.m_bModeHide = GetHidenModesArray();
            viveControllerBehavior2 = __instance;
            UpdateViveHiddenModeArray();
        }

        internal static void UpdateOculusHiddenModeArray()
        {
            ovrControllerBehavior2.m_bModeHide = GetHidenModesArray();
            if (debug.Value)
            {
                logger.LogMessage("Oculus Modes");
                logger.LogMessage($"HAND mode: {ovrControllerBehavior2.m_bModeHide[0]}");
                logger.LogMessage($"CAMERA mode: {ovrControllerBehavior2.m_bModeHide[1]}");
                logger.LogMessage($"PEN mode: {ovrControllerBehavior2.m_bModeHide[2]}");
                logger.LogMessage($"DANCE mode: {ovrControllerBehavior2.m_bModeHide[3]}");
                logger.LogMessage($"ITEM mode: {ovrControllerBehavior2.m_bModeHide[4]}");
                logger.LogMessage($"VRIK_FACE mode: {ovrControllerBehavior2.m_bModeHide[5]}");
                logger.LogMessage($"VRIK_HAND mode: {ovrControllerBehavior2.m_bModeHide[6]}");
                logger.LogMessage($"VRIK_OTHER mode: {ovrControllerBehavior2.m_bModeHide[7]}");
                logger.LogMessage($"YOTOGI mode: {ovrControllerBehavior2.m_bModeHide[8]}");
            }
        }

        internal static void UpdateViveHiddenModeArray()
        {
            viveControllerBehavior2.m_bModeHide = GetHidenModesArray();
            if (debug.Value)
            {
                logger.LogMessage("Vive Modes");
                logger.LogMessage($"HAND mode: {viveControllerBehavior2.m_bModeHide[0]}");
                logger.LogMessage($"CAMERA mode: {viveControllerBehavior2.m_bModeHide[1]}");
                logger.LogMessage($"PEN mode: {viveControllerBehavior2.m_bModeHide[2]}");
                logger.LogMessage($"DANCE mode: {viveControllerBehavior2.m_bModeHide[3]}");
                logger.LogMessage($"ITEM mode: {viveControllerBehavior2.m_bModeHide[4]}");
                logger.LogMessage($"VRIK_FACE mode: {viveControllerBehavior2.m_bModeHide[5]}");
                logger.LogMessage($"VRIK_HAND mode: {viveControllerBehavior2.m_bModeHide[6]}");
                logger.LogMessage($"VRIK_OTHER mode: {viveControllerBehavior2.m_bModeHide[7]}");
                logger.LogMessage($"YOTOGI mode: {viveControllerBehavior2.m_bModeHide[8]}");
            }
        }

        private static bool[] GetHidenModesArray()
        {
            bool[] hidenModesArray = new bool[]
            {
                false,               // HAND
                disableCamera.Value, // CAMERA
                true,                // PEN
                disableDance.Value,  // DANCE
                disableItem.Value,   // ITEM
                true,                // VRIK_FACE
                true,                // VRIK_HAND
                true,                // VRIK_OTHER
                true                 // YOTOGI
            };
            return hidenModesArray;
        }

        /*
        // Occulus Patching
        [HarmonyPatch(typeof(OvrControllerBehavior2), "ChangeMode")]
        [HarmonyPrefix]
        public static void DisableCameraOculus(ref OvrControllerBehavior2.OvrControllerMode f_eNewMode)
        {
            if (debug.Value) { Main.logger.LogMessage($"New mode: {f_eNewMode}"); }

            if (disableCamera.Value && f_eNewMode == OvrControllerBehavior2.OvrControllerMode.CAMERA)
            {
                if (isDance) { f_eNewMode = OvrControllerBehavior2.OvrControllerMode.DANCE; }
                else { f_eNewMode = OvrControllerBehavior2.OvrControllerMode.ITEM; }
            }

            if (disableItem.Value && f_eNewMode == OvrControllerBehavior2.OvrControllerMode.ITEM)
            {
                if (isYotogi) { f_eNewMode = OvrControllerBehavior2.OvrControllerMode.YOTOGI; }
                else { f_eNewMode = OvrControllerBehavior2.OvrControllerMode.HAND; }
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
         */
    }
}
