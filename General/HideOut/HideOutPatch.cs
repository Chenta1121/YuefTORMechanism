using HarmonyLib;

using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace YuefTORMechanism.General.HideOut
{
    [Harmony]
    internal class HideOutPatch
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HideoutCampaignBehavior), "IsHideoutAttackableNow")]
        static bool Prefix_IsHideoutAttackableNow(ref bool __result)
        {
            __result = true;
            return false;

        }

    }
}
