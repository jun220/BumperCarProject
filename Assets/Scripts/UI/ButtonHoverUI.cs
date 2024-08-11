using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHoverUI : MonoBehaviour
{
    [SerializeField] private Image selector;
    private Button button;

    private void Start() {
        button = GetComponent<Button>();
    }

    public void SetSelectorActive(bool active) {
        if(button.interactable)
            selector.enabled = active;
    }
}
