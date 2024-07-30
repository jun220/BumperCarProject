using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPanelUI : MonoBehaviour
{
    public void OnEntranced() => GameLauncher.Instance.Entrance();
}