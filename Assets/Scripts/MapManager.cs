using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Current { get; private set; }

    private void Awake() {
        Current = this;
    }


    [Header("SpawnPoints")]
    [SerializeField] private GameObject[] SpawnPoints;
    public Vector3 GetSpawnPoint(int index) => SpawnPoints[index].transform.position;
}
