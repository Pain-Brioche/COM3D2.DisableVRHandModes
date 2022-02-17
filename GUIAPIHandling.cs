using System;

namespace COM3D2.DisableVRControllerModes
{
    public class GUIAPIHandling
    {
        public void GUIAPIAwake()
        {
            // COM3D2.GUIAPI
            GUIAPI.ConfigMenu ConfigMenu = GUIAPI.MenuHandler.CreateConfigMenu("Controller Modes");
            var enableModes = ConfigMenu.AddSection("Disable Modes:");

            var nDisableCamera = enableModes.AddSwitchControl("Camera", Main.disableCamera.Value);
            var nDisableItem = enableModes.AddSwitchControl("Item/Toys", Main.disableItem.Value);
            var nDisableDance = enableModes.AddSwitchControl("Dance Stick", Main.disableDance.Value);

            nDisableCamera.ValueChanged += UpdateConfig_Event;
            nDisableItem.ValueChanged += UpdateConfig_Event;
            nDisableDance.ValueChanged += UpdateConfig_Event;

            void UpdateConfig_Event(object sender, EventArgs e)
            {
                Main.disableCamera.Value = nDisableCamera.Value;
                Main.disableItem.Value = nDisableItem.Value;
                Main.disableDance.Value = nDisableDance.Value;
                Main.logger.LogDebug($"Setting changed: Disable Camera {Main.disableCamera.Value}, Disable Item {Main.disableItem.Value}, Disable Dance {Main.disableDance.Value}");
            }
        }
    }
}
