using System;
using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Graphics.GUI;
using VRage.Game;
using VRageMath;

// ReSharper disable UnusedMember.Local

// ReSharper disable UnusedType.Global

// ReSharper disable InconsistentNaming

namespace Blacklist.Patches
{
    [HarmonyPatch(typeof(MyGuiScreenToolbarConfigBase))]
    public static class MyGuiScreenToolbarConfigBasePatch
    {
        public static event Action OnBlacklist;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(MyGuiScreenToolbarConfigBase.RecreateControls))]
        private static void RecreateControlsPostfix(MyGuiScreenToolbarConfigBase __instance)
        {
            createBlacklistButton(__instance);
        }
        private static void createBlacklistButton(MyGuiScreenToolbarConfigBase screen)
        {
            var button = new MyGuiControlButton
            {
                Text = "Blacklist",
                Name = "OpenBlacklistWindow",
                VisualStyle = MyGuiControlButtonStyleEnum.Small,
                Position = new Vector2(0.31f, -0.34f)
            };
            button.ButtonClicked += _ => OnBlacklist?.Invoke();
            screen.Elements.Add(button);
        }
    }
}