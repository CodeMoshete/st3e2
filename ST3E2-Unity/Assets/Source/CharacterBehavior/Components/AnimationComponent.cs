using System.Collections.Generic;
using UnityEngine;

public class AnimationComponent
{
    private Animator animator;
    private Dictionary<string, bool> boolStates = new Dictionary<string, bool>();

    public AnimatorControllerParameter[] parameters
    {
        get
        {
            return animator.parameters;
        }
    }

    public void OnViewCreated(GameObject View)
    {
        animator = View.GetComponent<Animator>();
        foreach (KeyValuePair<string, bool> pair in boolStates)
        {
            animator.SetBool(pair.Key, pair.Value);
        }
    }

    public void OnViewDestroyed()
    {
        animator = null;
    }

    public void SetBool(string name, bool value)
    {
        if (!boolStates.ContainsKey(name))
        {
            boolStates.Add(name, value);
        }
        else
        {
            boolStates[name] = value;
        }

        if (animator != null)
        {
            animator.SetBool(name, value);
        }
    }

    public bool GetBool(string name)
    {
        bool retVal = false;
        boolStates.TryGetValue(name, out retVal);
        return retVal;
    }

}
