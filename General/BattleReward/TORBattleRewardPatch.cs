using TOR_Core.Models;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace YuefTORMechanism.General.BattleReward
{
    [Harmony]
    internal class TORBattleRewardPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TORBattleRewardModel), "GetPartySavePrisonerAsMemberShareProbability")]
        static bool Prefix_GetPartySavePrisonerAsMemberShareProbability(PartyBase winnerParty, float lootAmount, ref float __result)
        {
            // 获取自定义调整设置的值
            bool isAdjustmentEnabled = GlobalSettings<MCMSetting>.Instance.Yuef_BattleReward_adjustment;

            // 如果启用了自定义调整
            if (isAdjustmentEnabled)
            {
                // 创建 DefaultBattleRewardModel 实例并获取结果
                DefaultBattleRewardModel rewardModel = new DefaultBattleRewardModel();
                __result = rewardModel.GetPartySavePrisonerAsMemberShareProbability(winnerParty, lootAmount);
                return false;  // 阻止原方法执行
            }

            // 如果未启用自定义调整，则继续执行原方法
            return true;
        }
    }
}
