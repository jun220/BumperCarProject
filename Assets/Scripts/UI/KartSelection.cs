using UnityEngine;

public class KartSelection : MonoBehaviour
{
    [SerializeField] private GameObject[] typeUIs = new GameObject[MaxCount];

    private const int MaxCount = 6;
    private int curIndex;

    private void Start() {
        curIndex = 0;
        ShowTypeUI();
    }

    public void MoveNext() {
        curIndex = (curIndex + 1) % MaxCount;
        ShowTypeUI();
    }

    public void MovePrevious() {
        curIndex = (curIndex + (MaxCount - 1)) % MaxCount;
        ShowTypeUI();
    }

    private void ShowTypeUI() {
        for(int i = 0; i < MaxCount; ++i){
            typeUIs[i].SetActive(i == curIndex);
        }
    }
}
