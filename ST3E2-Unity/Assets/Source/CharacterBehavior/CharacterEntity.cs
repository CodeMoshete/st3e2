using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Utils;

public class CharacterEntity : MonoBehaviour
{
    public string ViewPrefabPath;

    public bool CalculateNewPath = false;
    public string DestNetworkName;
    public string DestNodeName;

    public bool ShowView = false;
    private bool isViewShown = false;

    // Components are added and removed by systems.
    [HideInInspector]
    public GameObject View;
    [HideInInspector]
    public NodeNavigationComponent NavComponent;
    [HideInInspector]
    public AnimationComponent AnimComponent;
    [HideInInspector]
    public CharacterDirectiveComponent DirectiveComponent;
    public List<CharacterDirective> DefaultDirectives;

    private bool isInitialized = false;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized)
        {
            return;
        }
        isInitialized = true;

        Service.CharacterSystems.Navigation.AddCharacter(this);
        NavComponent.TurnRate = 70f;
        NavComponent.WalkRate = 1f;

        AnimComponent = new AnimationComponent();
    }

    public void LoadAndShowView()
    {
        StartCoroutine(LoadViewAsync());
    }

    private IEnumerator LoadViewAsync()
    {
        ResourceRequest viewRequest = Resources.LoadAsync<GameObject>(ViewPrefabPath);
        while (!viewRequest.isDone)
        {
            yield return null;
        }

        View = GameObject.Instantiate((GameObject)viewRequest.asset, transform);
        AnimComponent.OnViewCreated(View);
    }

    public void DestroyView()
    {
        AnimComponent.OnViewDestroyed();
        GameObject.Destroy(View);
    }

    private void Update()
    {
        if (CalculateNewPath)
        {
            NavNetwork navNetwork = Service.NavWorldManager.CurrentNavWorld.GetNetworkByName(DestNetworkName);
            CalculateNewPath = false;
            NavComponent.NavigationQueue = navNetwork.Navigate(NavComponent.CurrentNode.name, DestNodeName);
            NavComponent.FinalDestination = navNetwork.GetNodeByName(DestNodeName);
        }

        if (ShowView && !isViewShown)
        {
            isViewShown = true;
            LoadAndShowView();
        }
        
        if (!ShowView && isViewShown)
        {
            isViewShown = false;
            DestroyView();
        }
    }
}
