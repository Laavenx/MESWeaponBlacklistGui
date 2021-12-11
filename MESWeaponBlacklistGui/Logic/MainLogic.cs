using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;
using Blacklist.Gui;
using Blacklist.Patches;

namespace Blacklist.Logic
{
    public class MainLogic
    {
        public MainLogic()
        {
            MyGuiScreenToolbarConfigBasePatch.OnBlacklist += OnBlacklist;
        }

        private void OnBlacklist()
        {
            var currentToolbar = MyToolbarComponent.CurrentToolbar;
            if (currentToolbar == null)
                return;

            MyGuiSandbox.AddScreen(new WeaponDialog("Weapon list"));

        }
       
    }
}