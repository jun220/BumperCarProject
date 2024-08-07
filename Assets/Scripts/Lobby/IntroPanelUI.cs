using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPanelUI : MonoBehaviour
{
    public async void OnEntranced() => await GameLauncher.Instance.Entrance();
}