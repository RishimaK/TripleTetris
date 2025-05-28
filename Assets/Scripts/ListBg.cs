using UnityEngine;

public class ListBg : MonoBehaviour
{
    void Start()
    {
        float canvaswidth = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(canvaswidth, 0, 0);
    }
}
