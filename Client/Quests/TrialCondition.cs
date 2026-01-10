using System;
using EFT.Quests;

namespace KoTClient.Quests;

public class ConditionTrialCompletion : Condition
{
}

public class TrialCompletionHandler<T> where T : IConditional
{
    public ConditionSellItemToTrader condition;
    public EQuestStatus status;
    public T conditional;
    public QuestControllerAbstractClass<T> controller;
    public TaskConditionCounterClass counter;

    public double GetCurrentValue => counter.Value;

    public void OnValueChanged(TaskConditionCounterClass taskConditionCounter)
    {
        Action<T, EQuestStatus, Condition, bool> onConditionValueChanged = controller.OnConditionValueChanged;

        onConditionValueChanged?.Invoke(conditional, status, condition, true);
    }
    
    public void OnDisconnect()
    {
        counter.OnValueChanged -= OnValueChanged;
    }

    public void OnReset()
    {
        controller.method_20(conditional, status, condition);
    }
}