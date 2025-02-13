using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;


namespace YuefTORMechanism.General.HideOut
{
    internal class HideoutBehavior : CampaignBehaviorBase
    {
        // 获取当前游戏的 HideoutCampaignBehavior 实例
        private HideoutCampaignBehavior HideoutCampaignBehavior => Campaign.Current.GetCampaignBehavior<HideoutCampaignBehavior>();

        public override void RegisterEvents()
        {
            // 注册事件，当游戏会话启动时触发
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunchedEvent);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // 如果需要同步数据，在此处理
        }

        private void OnSessionLaunchedEvent(CampaignGameStarter campaignGameStarter)
        {
            AddGameMenus(campaignGameStarter);
        }

        private void AddGameMenus(CampaignGameStarter campaignGameStarter)
        {
            const string menuLocation = "hideout_place";
            const string optionIdentifier = "str_order_attack";
            const string optionText = "{=TtGJqRI5}Send Troops";

            // 使用委托简化回调方法
            campaignGameStarter.AddGameMenuOption(
                menuLocation,
                optionIdentifier,
                optionText,
                null,
                new GameMenuOption.OnConsequenceDelegate(OnSendTroopsConsequence),
                true,
                2,
                false,
                null
            );
        }

        private void OnSendTroopsConsequence(MenuCallbackArgs args)
        {
            // 如果当前没有战斗或战斗尚未初始化，执行初始化
            if (PlayerEncounter.Battle == null)
            {
                if (MobileParty.MainParty.MapEvent != null)
                {
                    PlayerEncounter.Init();  // 初始化战斗
                }
                else
                {
                    PlayerEncounter.StartBattle();  // 开始战斗
                }
            }

            PlayerEncounter.Update();  // 更新战斗
            ExecuteHideOutFightLogic(args);  // 执行攻占 Hideout 的战斗逻辑
        }

        // 执行进攻逻辑
        private static void ExecuteHideOutFightLogic(MenuCallbackArgs args)
        {
            EncounterHideOutOrderAttack();
        }

        // 处理 Hideout 攻击逻辑
        private static void EncounterHideOutOrderAttack()
        {
            // 如果当前已有战斗模拟
            if (PlayerEncounter.Current != null)
            {
                // 返回到上一个游戏菜单
                GameMenu.ExitToLast();

                // 初始化战斗模拟
                PlayerEncounter.InitSimulation(null, null);

                // 如果战斗模拟成功，开始模拟
                if (PlayerEncounter.Current?.BattleSimulation != null)
                {
                    ((MapState)Game.Current.GameStateManager.ActiveState).StartBattleSimulation();
                }
            }
        }
    }
}


