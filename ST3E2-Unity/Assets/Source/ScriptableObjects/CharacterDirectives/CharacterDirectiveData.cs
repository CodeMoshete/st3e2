using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDirectiveData", menuName = "ScriptableObjects/CharacterDirectiveData")]
public class CharacterDirectiveData : ScriptableObject
{
    public NavWorldID NavWorld;
    public List<CharacterDirective> BaseDirectives;
    public List<DirectiveContextArea> DirectiveContextAreas;
    public List<CharacterDirectiveData> SharedDirectives;
}
