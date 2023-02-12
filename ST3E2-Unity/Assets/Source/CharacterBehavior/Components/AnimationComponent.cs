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
            if (animator == null)
            {
                return new AnimatorControllerParameter[0];
            }

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

    public void ResetToDefaultState()
    {
        for (int i = 0, count = parameters.Length; i < count; ++i)
        {
            AnimatorControllerParameter parameter = parameters[i];
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                SetBool(parameter.name, false);
            }
        }

        boolStates.Clear();
    }
}
