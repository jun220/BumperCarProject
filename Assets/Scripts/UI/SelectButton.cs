using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    public Image selector;
    
    public void OnHover()
    {   
        selector.enabled = true;
    }

    public void OnExit()
    {
        selector.enabled = false;
    }
}
