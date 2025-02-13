using System.Collections.Generic;
using TOR_Core.CampaignMechanics;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TOR_Core.Extensions;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.ObjectSystem;
using MCM.Abstractions.Base.Global;

namespace YuefTORMechanism.WoodElves
{
    [Harmony]
    internal class TORTreemanRecruitmentPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TORAIRecruitmentCampaignBehavior), "AddDryadsToPartyOnEnteringSettlement")]
        static void Postfix_AddDryadsToPartyOnEnteringSettlement(MobileParty party, Settlement settlement, Hero hero)
        {
            if (!GlobalSettings<MCMSetting>.Instance.Yuef_Treeman_adjustment) return;

            // 如果参数无效（队伍、定居点、英雄为空），或英雄不是施法者，或者英雄文化不是 Battania，或者英雄是玩家角色，或者定居点是藏匿处，则跳过
            if (party == null || settlement == null || hero == null || !hero.IsSpellCaster() || hero.Culture.StringId != "battania" || hero.CharacterObject.IsPlayerCharacter || settlement.IsHideout)
            {
                return;
            }
            // 如果定居点ID不包含 "AL"（不符合特定区域），则跳过
            if (!settlement.StringId.Contains("AL"))
            {
                return;
            }
            CharacterObject treeman = MBObjectManager.Instance.GetObject<CharacterObject>("tor_we_treeman");
            CharacterObject treemanPlus = treeman.UpgradeTargets[0];
            TroopRoster memberRoster = party.MemberRoster;

            if (MBRandom.RandomFloat < 0.3f && treeman != null)
            {
                // 如果队伍中已经有超过 5 个树人或远古树人，则跳过
                if (party.MemberRoster.GetTroopCount(treeman) + party.MemberRoster.GetTroopCount(treemanPlus) > 5)
                {
                    return;
                }

                bool unitRemoved = false;

                // 优先遍历当前队伍, 尝试移除一个非Battania文化的单位
                for (int i = 0; i < memberRoster.Count; i++)
                {
                    CharacterObject character = memberRoster.GetCharacterAtIndex(i);

                    // 如果该单位不是Battania文化，则移除
                    if (character.Culture.StringId != "battania" && character != treeman)
                    {
                        party.MemberRoster.AddToCounts(character, -1, false, 0, 0, true, -1);
                        unitRemoved = true;
                        break;
                    }
                }

                // 如果没有找到符合条件的单位，随机移除一个非树人且非英雄的单位
                if (!unitRemoved && memberRoster.Count > 0)
                {
                    // 获取所有符合条件的单位（非树人且非英雄）
                    List<int> removableIndexes = new List<int>();

                    for (int i = 0; i < memberRoster.Count; i++)
                    {
                        CharacterObject character = memberRoster.GetCharacterAtIndex(i);
                        // 该单位不是树人且不是英雄
                        if (character != treeman && !character.IsHero)
                        {
                            removableIndexes.Add(i);
                        }
                    }
                    // 如果有符合条件的单位，则随机选择一个移除
                    if (removableIndexes.Count > 0)
                    {
                        int randomIndex = removableIndexes[MBRandom.RandomInt(0, removableIndexes.Count)];
                        CharacterObject characterToRemove = memberRoster.GetCharacterAtIndex(randomIndex);
                        // 使用负数移除该单位
                        party.MemberRoster.AddToCounts(characterToRemove, -1, false, 0, 0, true, -1);
                    }
                }
                // 向队伍中添加一个树人
                party.MemberRoster.AddToCounts(treeman, 1, false, 0, 0, true, -1);
            }
        }
    }
}
