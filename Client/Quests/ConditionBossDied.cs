using System;
using EFT.Quests;

namespace KoTClient.Quests;

public class ConditionBossDied : Condition
{
    public string target;

    public static string LocaleLocalizationKey => "QuestCondition/Elimination/Boss";

    public override string FormattedDescription
    {
        get
        {
            if (!DynamicLocale)
                return base.FormattedDescription;
            
            return GenerateFormattedDescription();
        }
    }

    public string GenerateFormattedDescription()
    {
        return " " + $"QuestCondition/Elimination/Kill/BotRole/{target}".Localized();
    }
}

public class BossDiedChecker : ConditionProgressChecker
{
    public new ConditionBossDied Condition;
    
    public BossDiedChecker(ConditionBossDied condition) : base(condition)
    {
        Condition = condition;
    }


    public override bool Test(object testValue)
    {
        if (testValue is not string role)
        {
            return base.Test(testValue);
        }
        
        Plugin.PluginLogger.LogError("KoT Test 2");

        return Condition.target.Contains(role, StringComparison.InvariantCultureIgnoreCase);
    }
}