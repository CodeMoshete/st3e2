using System.Collections.Generic;

public class CharacterDirectiveComponent
{
    public List<CharacterDirectiveData> WorldDirectives;
    public CharacterDirectiveData CurrentDirectiveData;
    public DirectiveContextArea CurrentContextArea;
    public CharacterDirective CurrentDirective;
    public bool IsDirectiveExpired;
    public float CurrentDirectiveDuration;
}
