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

        NavComponent = new NodeNavigationComponent();
        NavComponent.TurnRate = 70f;
        NavComponent.WalkRate = 1f;

        AnimComponent = new AnimationComponent();
    }

    public void LoadAndShowView()
    {
        ShowView = true;
        isViewShown = true;
        StartCoroutine(LoadViewAsync());
    }

    private IEnumerator LoadViewAsync()
    {
        ResourceRequest viewRequest = Resources.LoadAsync<GameObject>(ViewPrefabPath);
        while (!viewRequest.isDone)
        {
            yield return null;
        }

        // The view may be unloaded before the load completes.
        if (isViewShown)
        {
            View = GameObject.Instantiate((GameObject)viewRequest.asset, transform);
            AnimComponent.OnViewCreated(View);
        }
    }

    public void DestroyView()
    {
        ShowView = false;
        isViewShown = false;
        AnimComponent.OnViewDestroyed();

        if (View != null)
        {
            GameObject.Destroy(View);
        }
    }

    private void Update()
    {
        if (CalculateNewPath)
        {
            NavWorld navWorld = Service.NavWorldManager.CurrentNavWorld;
            CalculateNewPath = false;
            NavComponent.NavigationQueue = navWorld.Navigate(
                NavComponent.CurrentNavNetwork,
                NavComponent.CurrentNode.name,
                DestNetworkName,
                DestNodeName);

            NavNetwork targetNetwork = 
                Service.NavWorldManager.CurrentNavWorld.GetNetworkByName(DestNetworkName);

            NavComponent.FinalDestination = targetNetwork.GetNodeByName(DestNodeName);
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
