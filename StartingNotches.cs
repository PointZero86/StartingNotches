using System;
using System.Collections.Generic;
using Modding;
using Satchel.BetterMenus;
using UnityEngine;

namespace StartingNotches
{
    public class StartingNotches : Mod, ITogglableMod, ICustomMenuMod, IGlobalSettings<GlobalSettings> {

        public static GlobalSettings GS { get; set; } = new();
        new public string GetName() => "Starting Notches";
        public override string GetVersion() => "1.0.1";

        private static StartingNotches instance;
        public StartingNotches() : base("Starting Notches") {
            instance = this;
        }
        internal static StartingNotches Instance {
            get {
                if (instance == null) {
                    throw new InvalidOperationException($"{nameof(StartingNotches)} was never initialized");
                }
                return instance;
            }
        }

        // Global Settings Thingies
        void IGlobalSettings<GlobalSettings>.OnLoadGlobal(GlobalSettings s)
        {
            GS = s ?? new();
        }
        GlobalSettings IGlobalSettings<GlobalSettings>.OnSaveGlobal()
        {
            return GS ?? new();
        }

        // Menu Thingies
        private Menu MenuRef;
        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? modToggleDelegates)
        {
            MenuRef = new Menu (
                name: ($"{nameof(StartingNotches)}"), // Title, Appears At Top Center 
                elements: new Element[]
                    {
                    // Blueprint For Mod Toggle:
                    Blueprints.CreateToggle(
                        toggleDelegates: modToggleDelegates.Value,
                        name: "Toggle Starting Notches",
                        description: "Controls whether or not the mod is active."),
                    // Amount Of Notches To Add:
                    new CustomSlider (
                        name: "Charm Notches",
                        storeValue: val => { GS.extraNotches = (int) val; },  
                        loadValue: () => GS.extraNotches,
                        minValue: 0,
                        maxValue: 20,
                        wholeNumbers: true
                    ),
                }
            );
            return MenuRef.GetMenuScreen(modListMenu);
        }
        public bool ToggleButtonInsideMenu => true; // Toggle Option Appears Within Mod Menu

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            instance.Log("Initializing");
            // When the mod is toggled on, OverrideNotchCount is enabled (Adds [extraNotches] Charm Notches)
            ModHooks.GetPlayerIntHook += OverrideNotchCount;
        }

        private static int OverrideNotchCount(string fieldName, int value)
        {
            if (fieldName == nameof(PlayerData.charmSlots))
            {
                return value + GS.extraNotches;
            }
            return value;
        }

        public void Unload()
        {
            // When the mod is toggled off, OverrideNotchCount is disabled (Removes [extraNotches] Charm Notches)
            ModHooks.GetPlayerIntHook -= OverrideNotchCount;
        }
    }
}
