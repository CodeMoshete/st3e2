using System;
using System.Collections.Generic;

public class CharacterSystemManager
{
    public NodeNavigationSystem Navigation { get; private set; }
    public CharacterDirectiveSystem Directives { get; private set; }

    int numSystems = 0;
    private List<ICharacterSystem> systems = new List<ICharacterSystem>();

    public CharacterSystemManager()
    {
        Navigation = CreateSystem<NodeNavigationSystem>();
        Directives = CreateSystem<CharacterDirectiveSystem>();

        Service.UpdateManager.AddObserver(Update);
    }

    private T CreateSystem<T>() where T : ICharacterSystem
    {
        T newSystem = Activator.CreateInstance<T>();
        newSystem.Initialize();
        systems.Add(newSystem);
        ++numSystems;
        return newSystem;
    }

    private void Update(float dt)
    {
        for (int i = 0; i < numSystems; ++i)
        {
            systems[i].Update(dt);
        }
    }

    public void PurgeEntity(CharacterEntity entity)
    {
        Navigation.RemoveCharacter(entity);
        Directives.RemoveCharacter(entity);
    }
}
