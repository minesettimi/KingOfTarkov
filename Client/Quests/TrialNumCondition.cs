using System;
using EFT.Quests;

namespace KoTClient.Quests;

public class ConditionTrialNumber : Condition
{
}

public class TrialNumberHandler<T> where T : IConditional
{
    public ConditionTrialNumber condition;
    public EQuestStatus status;
    public T conditional;
    public QuestControllerAbstractClass<T> controller;

    public double GetCurrentValue(ConditionProgressChecker _)
    {
        return Plugin.StateService.StateData?.trial.trialNum - 1 ?? 0;
    }

    public void OnValueChanged()
    {
        Action<T, EQuestStatus, Condition, bool> onConditionValueChanged = controller.OnConditionValueChanged;

        onConditionValueChanged?.Invoke(conditional, status, condition, true);
    }
    
    public void OnDisconnect(ConditionProgressChecker _)
    {
        Plugin.StateService.TrialUpdate -= OnValueChanged;
    }

    public void OnReset(ConditionProgressChecker _)
    {
        controller.method_20(conditional, status, condition);
    }
}