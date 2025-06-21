using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public GameObject Service;
    public RectTransform IconPick;

    public GameObject ListTheme;
    SaveDataJson saveDataJson;


    void Start()
    {
        saveDataJson = Service.GetComponent<SaveDataJson>();
        CheckTheme();
    }

    void CheckTheme()
    {
        IconPick.localPosition = new Vector3(IconPick.localPosition.x, ListTheme.transform.GetChild((int)saveDataJson.GetData("CurrentTheme") - 1).localPosition.y + 74, 0);
    }

    public void ChangeTheme(int theme)
    {
        if ((int)saveDataJson.GetData("CurrentTheme") == theme) return;

        IconPick.localPosition = new Vector3(IconPick.localPosition.x, ListTheme.transform.GetChild(theme - 1).localPosition.y + 74, 0);
        saveDataJson.SaveData("CurrentTheme", theme);
    }
}
