using System;

using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Base.Global;

using TaleWorlds.Localization;

namespace YuefTORMechanism
{
    internal class MCMSetting : AttributeGlobalSettings<MCMSetting>
    {
        public override string Id
        {
            get
            {
                return "YuefTORMechanism";
            }
        }
        public override string DisplayName
        {
            get
            {
                // 使用本地化字符串为显示名称提供文本
                string text = new TextObject("{=YueF_Mechanism_Name}TOR:Mechanism", null).ToString();
                // 获取当前程序集的版本号
                Version version = typeof(MCMSetting).Assembly.GetName().Version;
                // 返回带版本号的模块名称
                return text + ((version != null) ? version.ToString(3) : null);
            }
        }

        public override string FolderName
        {
            get
            {
                return "Yuef_TOR_Mechanism";
            }
        }

        public override string FormatType
        {
            get
            {
                return "json2";
            }
        }

        [SettingProperty("{=Yuef_CursedSite_Title}Cursed Land Mechanism Adjustment", RequireRestart = false, HintText = "{=Yuef_CursedSite_Hint}Enabling this will replace the original TOR health deduction injury mechanism with a speed reduction within the Cursed Land area.", Order = 1)]
        [SettingPropertyGroup("{=Yuef_General}General Mechanisms", GroupOrder = 0)]
        public bool Yuef_CursedSite_adjustment { get; set; } = false; 

        [SettingProperty("{=Yuef_Castle_Title}Castle Mechanism Adjustment", RequireRestart = false, HintText = "{=Yuef_Castle_Hint}Enabling this will add a mechanism to the castle: when the team is hostile to a nearby castle, they will suffer a movement speed penalty; otherwise, they will receive a movement speed bonus.", Order = 2)]
        [SettingPropertyGroup("{=Yuef_General}General Mechanisms", GroupOrder = 0)]
        public bool Yuef_Castle_adjustment { get; set; } = true;  

        [SettingProperty("{=Yuef_BattleReward_Title}Post-battle Captive Mechanism Adjustment", RequireRestart = false, HintText = "{=Yuef_BattleReward_Hint}Enabling this will restore the mechanism for saving captives.", Order = 3)]
        [SettingPropertyGroup("{=Yuef_General}General Mechanisms", GroupOrder = 0)]
        public bool Yuef_BattleReward_adjustment { get; set; } = false;  

        [SettingProperty("{=Yuef_Treeman_Title}AI Treeman Recruitment Adjustment", RequireRestart = false, HintText = "{=Yuef_Treeman_Hint}Enabling this will strengthen the recruitment mechanism of the Wood Elf culture. When the AI is in the Wood Elf Forest, there will be a higher chance of receiving Treeman aid.", Order = 11)]
        [SettingPropertyGroup("{=Yuef_WoodElves}Wood Elf Mechanisms", GroupOrder = 1)]
        public bool Yuef_Treeman_adjustment { get; set; } = true;  


    }
}