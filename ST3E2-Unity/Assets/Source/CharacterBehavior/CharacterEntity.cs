using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Utils;

public class CharacterEntity : MonoBehaviour
{
    public string ViewPrefabPath;

    // For editor debug usage.
    public bool EvaluateDirectives;

    // For editor debug usage.
    public bool CalculateNewPath = false;
    public string DestNetworkName;
    public string DestNodeName;

    // For editor debug usage.
    public bool ShowView = false;
    private bool isViewShown = false;

    public bool IsViewVisible { get; private set; }

    // Components are added and removed by systems.
    [HideInInspector]
    public GameObject View;
    [HideInInspector]
    public NodeNavigationComponent NavComponent;
    [HideInInspector]
    public AnimationComponent AnimComponent;
    [HideInInspector]
    public CharacterDirectiveComponent DirectiveComponent;
    public List<CharacterDirectiveData> WorldDirectives;

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

        DirectiveComponent = new CharacterDirectiveComponent();
        DirectiveComponent.WorldDirectives = WorldDirectives;

        DontDestroyOnLoad(gameObject);
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
            IsViewVisible = true;
        }
    }

    public void DestroyView()
    {
        ShowView = false;
        isViewShown = false;
        IsViewVisible = false;
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

        if (EvaluateDirectives)
        {
            EvaluateDirectives = false;
            DirectiveComponent.CurrentDirectiveDuration = 0f;
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
