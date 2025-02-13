using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.Library;

using TOR_Core.CampaignMechanics.TORCustomSettlement;
using TOR_Core.Utilities;
using TOR_Core.Models;
using TOR_Core.Extensions;
using TaleWorlds.CampaignSystem;

namespace YuefTORMechanism.General.CursedSite
{
    [Harmony]
    internal class TORCursedSitePatch
    {
        // HarmonyPatch 用于拦截 OnSettlementHourlyTick 方法
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TORCustomSettlementCampaignBehavior), "OnSettlementHourlyTick")]
        static bool Prefix_OnSettlementHourlyTick(Settlement settlement)
        {
            // 判断是否启用了 CursedSite 调整
            if (GlobalSettings<MCMSetting>.Instance.Yuef_CursedSite_adjustment)
            {
                return false;
            }
            else
            {
                // 如果没有启用调整，则返回 true，继续执行原始方法
                return true;
            }
        }

        // HarmonyPostfix 用于修改 CalculateFinalSpeed 方法的返回值
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TORPartySpeedCalculatingModel), "CalculateFinalSpeed")]
        static void Postfix_CalculateFinalSpeed(MobileParty mobileParty, ref ExplainedNumber __result)
        {
            if (!GlobalSettings<MCMSetting>.Instance.Yuef_CursedSite_adjustment) return;


            bool isValidLordParty = mobileParty.IsLordParty &&
                mobileParty.LeaderHero != null &&
                mobileParty.LeaderHero.Culture.StringId != "mousillon";

            // 确保文化是"mousillon"以外的
            if (isValidLordParty)
            {
                // 查找周围25单位范围内的定居点
                MBReadOnlyList<Settlement> settlements = TORCommon.FindSettlementsAroundPosition(mobileParty.Position2D, 25f);

                // 遍历查找到的定居点，检查是否包含 CursedSiteComponent
                foreach (Settlement settlement in settlements)
                {
                    // 如果定居点有 CursedSiteComponent 并且该组件是激活状态
                    if (settlement.SettlementComponent is CursedSiteComponent site && site.IsActive && mobileParty.LeaderHero.GetDominantReligion() != site.Religion)
                    {
                        // 给定定居点的诅咒惩罚：减少速度 50%
                        __result.AddFactor(-0.5f, new TextObject("诅咒之地惩罚！"));
                        break;  // 找到符合条件的定居点后就可以停止查找
                    }
                }
            }
        }

    }
}
