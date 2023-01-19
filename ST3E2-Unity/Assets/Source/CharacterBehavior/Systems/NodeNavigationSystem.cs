using System.Collections.Generic;
using UnityEngine;

public class NodeNavigationSystem : ICharacterSystem
{
    private const string WALK_ANIM_KEY = "walking";
    private List<CharacterEntity> managedCharacters;

    public void Initialize()
    {
        managedCharacters = new List<CharacterEntity>();
        Service.EventManager.AddListener(EventId.CurrentNavNetworkChanged, OnNavNetworkChanged);
    }

    private bool OnNavNetworkChanged(object cookie)
    {
        string newNavNetwork = (string)cookie;
        for (int i = 0, count = managedCharacters.Count; i < count; ++i)
        {
            CharacterEntity character = managedCharacters[i];
            if (character.NavComponent.CurrentNavNetwork == newNavNetwork)
            {
                character.LoadAndShowView();
            }
            else
            {
                character.DestroyView();
            }
        }
        return false;
    }

    public void AddCharacter(CharacterEntity character)
    {
        if (!managedCharacters.Contains(character))
        {
            managedCharacters.Add(character);
            EvaluateCharacterVisibility(character);
        }
        else
        {
            Debug.LogError(string.Format("Character {0} has already been added!", character.name));
        }
    }

    public void RemoveCharacter(CharacterEntity character)
    {
        if (managedCharacters.Contains(character))
        {
            managedCharacters.Remove(character);
        }
        else
        {
            Debug.LogError(string.Format("Character {0} is not managed!", character.name));
        }
    }

    private void EvaluateCharacterVisibility(CharacterEntity character)
    {
        string navNetworkName =
                Service.NavWorldManager.CurrentNavWorld.ActiveNetwork.NetworkName;

        string characterNetwork = character.NavComponent.CurrentNavNetwork;

        if (characterNetwork == navNetworkName)
        {
            character.LoadAndShowView();
        }
        else
        {
            character.DestroyView();
        }
    }

    private GLLineController _lineController;
    private GLLineController lineController
    {
        get
        {
            if (_lineController == null)
            {
                _lineController = GameObject.Find("LineController").GetComponent<GLLineController>();
            }
            return _lineController;
        }
    }

    public void Update(float dt)
    {
        for (int i = 0, count = managedCharacters.Count; i < count; ++i)
        {
            CharacterEntity character = managedCharacters[i];
            NodeNavigationComponent navComp = character.NavComponent;

            if (navComp.IsNavigating && navComp.FinalDestination != null)
            {
                NavNode nextNode = navComp.NavigationQueue.Peek();

                bool isExitNodeTransition = false;

                Vector3 vectorToNext = nextNode.transform.position - character.transform.position;
                float destRadius = navComp.NavigationQueue.Count > 1 ? 
                    nextNode.TriggerRadius * nextNode.TriggerRadius :
                    0.1f;

                // Arrived at next node in the list.
                if (Vector3.SqrMagnitude(vectorToNext) < destRadius)
                {
                    string arrivalMsg = "Reached waypoint " + nextNode.name;
                    navComp.NavigationQueue.Dequeue();

                    // Apply any action effects from the node just arrived at.
                    if (nextNode.ArrivalAction != null)
                    {
                        // Some actions require access to the character on that node.
                        if (nextNode.CaptureCharactersForAction)
                        {
                            nextNode.EnqueueCharacter(character);

                            if (nextNode.DisableNavigationOnArrival)
                            {
                                Debug.Log("Disable nav comp for " + character.name);
                                character.AnimComponent.SetBool(WALK_ANIM_KEY, false);
                                navComp.IsNavigating = false;
                            }
                        }

                        nextNode.ArrivalAction.Initiate();

                        // Some action chains require the character to cease navigation.
                        if (!navComp.IsNavigating)
                        {
                            continue;
                        }
                    }

                    isExitNodeTransition = 
                        !string.IsNullOrEmpty(nextNode.ExitNodeTag) && 
                        !string.IsNullOrEmpty(navComp.NavigationQueue.Peek().ExitNodeTag);

                    if (navComp.NavigationQueue.Count == 0)
                    {
                        // Arrived at final destination.
                        navComp.FinalDestination = null;
                        character.AnimComponent.SetBool(WALK_ANIM_KEY, false);
                        navComp.CurrentNode = nextNode;
                        continue;
                    }

                    nextNode = navComp.NavigationQueue.Peek();
                    navComp.CurrentNode = nextNode;
                    Debug.Log(arrivalMsg + ", next waypoint " + nextNode.name);
                }

                Vector3 currentPos = character.transform.position;
                if (isExitNodeTransition)
                {
                    currentPos = nextNode.transform.position;
                    navComp.CurrentNavNetwork = nextNode.ParentNetworkName;
                    EvaluateCharacterVisibility(character);
                }
                else
                {
                    if (character.IsViewVisible)
                    {
                        // Only perform the more expensive calculations when the player is actually visible.
                        Vector3 normalVectorToNext = vectorToNext;
                        Vector2 flatVecToNext = new Vector2(normalVectorToNext.x, normalVectorToNext.z).normalized;
                        Vector2 flatFwd = new Vector2(character.transform.forward.x, character.transform.forward.z).normalized;
                        Vector2 flatRt = new Vector2(character.transform.right.x, character.transform.right.z).normalized;
                        float angleToTarget = Vector2.Angle(flatFwd, flatVecToNext);
                        float rightModifier = Vector2.Dot(flatRt, flatVecToNext) > 0 ? 1f : -1f;
                        float amountToRotate = Mathf.Min(angleToTarget, navComp.TurnRate * dt) * rightModifier;
                        character.transform.Rotate(new Vector3(0f, amountToRotate, 0f));

                        float walkDirectionModifier = Mathf.Max(Vector2.Dot(flatFwd, flatVecToNext), 0);
                        if (walkDirectionModifier > 0)
                        {
                            walkDirectionModifier = -Mathf.Pow(walkDirectionModifier - 1f, 4f) + 1f;
                        }

                        if (!character.AnimComponent.GetBool(WALK_ANIM_KEY) && walkDirectionModifier > 0f)
                        {
                            character.AnimComponent.SetBool(WALK_ANIM_KEY, true);
                        }

                        Vector3 moveVector = character.transform.forward * navComp.WalkRate * dt * walkDirectionModifier;
                
                        currentPos += moveVector;

                        if (navComp.CurrentNavNetwork == Service.NavWorldManager.CurrentNavWorld.ActiveNetwork.NetworkName)
                        {
                            currentPos = GetRaycastForGround(currentPos);
                        }
                    }
                    else
                    {
                        // If the player isn't visible, perform much more basic navigation.
                        Vector3 normalVectorToNext = vectorToNext.normalized;
                        Vector2 flatVecToNext = new Vector2(normalVectorToNext.x, normalVectorToNext.z);
                        float angleToTarget = Vector2.Angle(new Vector2(character.transform.forward.x, character.transform.forward.z), flatVecToNext);
                        character.transform.Rotate(new Vector3(0f, angleToTarget, 0f));
                        currentPos += normalVectorToNext * navComp.WalkRate * dt;
                        
                    }
                }

                character.transform.position = currentPos;
            }
        }
    }

    private Vector3 GetRaycastForGround(Vector3 currentPos)
    {
        Vector3 testPoint = currentPos;
        testPoint.y += 0.5f;

        int layerMask = 1 << LayerMask.NameToLayer("Teleport");

        RaycastHit hit;
        if (Physics.Raycast(testPoint, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            currentPos.y = hit.point.y;
        }

        return currentPos;
    }

    public void Destroy()
    {
        managedCharacters = null;
    }
}
