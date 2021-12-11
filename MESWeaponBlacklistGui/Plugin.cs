using System.Reflection;
using HarmonyLib;
using Blacklist.Logic;
using VRage.Plugins;

namespace Blacklist
{
    // ReSharper disable NotAccessedField.Local
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin
    {
        private static bool initialized;
        private static MainLogic mainlogic;

        public void Dispose()
        {
        }

        public void Init(object gameInstance)
        {
            var harmony = new Harmony("MESWeaponBlacklistGui");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void Update()
        {
            if (initialized)
                return;

            mainlogic = new MainLogic();

            initialized = true;
        }
    }
}