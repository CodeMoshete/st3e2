using UnityEngine;

public class ShowObjectiveIconAction : CustomAction
{
    private const string ICON_RESOURCE_NAME = "ObjectiveIcon";
    private const float PLAYER_HEIGHT = 1f;

    public string SpawnTargetName;
    public bool OffsetToPlayerHeight;
    public CustomAction NextAction;
    private GameObject ObjectiveIcon;

    public override void Initiate()
    {
        GameObject spawnTarget = GameObject.Find(SpawnTargetName);
        Vector3 spawnPos = transform.position;
        if (spawnTarget != null)
        {
            spawnPos = spawnTarget.transform.position;
        }

        if (OffsetToPlayerHeight)
        {
            spawnPos.y += PLAYER_HEIGHT;
        }

        ObjectiveIcon = Instantiate(Resources.Load<GameObject>(ICON_RESOURCE_NAME), spawnPos, Quaternion.identity);

        if (NextAction != null)
        {
            NextAction.Initiate();
        }
    }

    public void HideObjectiveIcon()
    {
        Destroy(ObjectiveIcon);
    }
}
