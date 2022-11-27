public interface ICharacterSystem
{
    void Initialize();
    void AddCharacter(CharacterEntity character);
    void RemoveCharacter(CharacterEntity character);
    void Update(float dt);
    void Destroy();
}
