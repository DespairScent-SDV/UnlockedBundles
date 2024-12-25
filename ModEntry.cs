using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Locations;

namespace DespairScent.UnlockedBundles
{
    internal sealed class ModEntry : Mod
    {

        private static IModHelper _Helper;

        public override void Entry(IModHelper helper)
        {
            _Helper = helper;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(AccessTools.Method(typeof(CommunityCenter), nameof(CommunityCenter.shouldNoteAppearInArea)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(PatchJunimoNoteVisible)));
        }

        private static bool PatchJunimoNoteVisible(CommunityCenter __instance, ref bool __result, int area)
        {
            var areaToBundleDictionary = _Helper.Reflection.GetField<object>(__instance, "areaToBundleDictionary").GetValue() as Dictionary<int, List<int>>;

            bool isAreaComplete = true;
            for (int i = 0; i < areaToBundleDictionary[area].Count; i++)
            {
                foreach (int bundleIndex in areaToBundleDictionary[area])
                {
                    if (__instance.bundles.TryGetValue(bundleIndex, out var bundleEntries))
                    {
                        int bundleLength = bundleEntries.Length / 3;
                        for (int j = 0; j < bundleLength; j++)
                        {
                            if (!bundleEntries[j])
                            {
                                isAreaComplete = false;
                                break;
                            }
                        }
                    }
                    if (!isAreaComplete)
                    {
                        break;
                    }
                }
            }

            if (isAreaComplete || area < 0 || area == CommunityCenter.AREA_AbandonedJojaMart)
            {
                return true;
            }

            __result = true;
            return false;
        }

    }
}
