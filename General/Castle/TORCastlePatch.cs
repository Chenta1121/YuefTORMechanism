
using System.Collections.Generic;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TOR_Core.Models;
using TOR_Core.Utilities;

namespace YuefTORMechanism.General.Castle
{
    [Harmony]
    internal class TORCastlePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TORPartySpeedCalculatingModel), "CalculateFinalSpeed")]
        static void Postfix_Castle_CalculateFinalSpeed(MobileParty mobileParty, ref ExplainedNumber __result)
        {
            // 如果不需要调整则直接返回
            if (!GlobalSettings<MCMSetting>.Instance.Yuef_Castle_adjustment) return;

            if (!mobileParty.IsLordParty || mobileParty.LeaderHero == null) return;

            var leaderClan = mobileParty.LeaderHero.Clan;
            if (leaderClan == null) return;  // 领主没有氏族，退出

            // 查找周围15单位范围内的定居点

            // 增加距离判断，确保定居点在 15 单位范围内
            IEnumerable<Settlement> settlements = TORCommon.FindSettlementsAroundPosition(mobileParty.Position2D, 15f, settlement =>
            {
                // 只选取距离 15 单位以内的城堡
                return settlement.IsCastle;
            });


            foreach (Settlement settlement in settlements)
            {
                // 如果是空的定居点或非城堡定居点，跳过
                if (settlement == null || !settlement.IsCastle) continue;

                var ownerClan = settlement.OwnerClan;
                var settlementName = settlement.Name.ToString();  // 提前获取城堡的名字

                // 如果拥有城堡的氏族和领主氏族都存在
                if (ownerClan != null && leaderClan != null)
                {
                    // 判断是否为敌方阵地
                    if (FactionManager.IsAtWarAgainstFaction(settlement.MapFaction, mobileParty.MapFaction))//判断条件依次为判断领主
                    {
                        __result.AddFactor(-0.4f, new TextObject(settlementName));
                        break;  // 一旦处理一个敌方阵地就停止
                    }

                    // 判断是否为我方阵地（同一 Kingdom 或由 Hero 所有）
                    if (ownerClan.Kingdom != null && leaderClan.Kingdom != null && ownerClan.Kingdom == leaderClan.Kingdom)
                    {
                        __result.AddFactor(0.4f, new TextObject(settlementName));
                        break;  // 一旦处理一个我方阵地就停止
                    }
                    else if (settlement.Owner == mobileParty.LeaderHero)
                    {
                        // 如果定居点由 Hero 直接拥有
                        __result.AddFactor(0.4f, new TextObject(settlementName));
                        break;
                    }
                }
            }
        }

    }

}
