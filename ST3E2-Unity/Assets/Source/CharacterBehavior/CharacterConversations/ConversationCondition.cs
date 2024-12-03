using System;

public enum ConditionCategory
{
    PlayerStat,
    CurrentArea,
    TimeOfDay
}

public enum ComparisonType
{
    Equals,
    GreaterThan,
    LessThan
}

[Serializable]
public class ConversationCondition
{
    public bool AndWithPrevious;
    public bool OrWithPrevious;
    public int ConditionGroup;
    public ConditionCategory Category;
    public ComparisonType Comparison;
    public string Value;
}
