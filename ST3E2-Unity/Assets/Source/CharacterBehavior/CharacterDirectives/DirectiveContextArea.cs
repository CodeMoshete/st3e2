using System;
using System.Collections.Generic;

[Serializable]
public class DirectiveContextArea
{
    public string Name;
    public int EntryWeight;
    public int ExitWeight;
    public bool TimeOfDayBound;
    public float StartTime;
    public float EndTime;
    public string NavNetworkName;
    public bool DirectivesInheritBaseSettings;
    public List<CharacterDirective> Directives;
}
