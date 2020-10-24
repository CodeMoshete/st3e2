using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallSegment : MonoBehaviour
{
    [System.Serializable]
    public struct Connector
    {
        public GameObject Socket;
        public GameObject ConnectedPiece;
    }

    [SerializeField]
    public List<Connector> Sides;

    public bool IsAnchorSegment;
}

