using Spine.Unity;
using TMPro;
using UnityEngine;

public class Home : MonoBehaviour
{
    public GameManager gameManager;
    public SaveDataJson saveDataJson;

    public DailyReward dailyReward;
    public ChestStar chestStar;
    public TextMeshProUGUI star;
    public GameObject ListBg;

    public TextMeshProUGUI LeverText;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        // saveDataJson.SaveData("OpenedMap", 1);

        dailyReward.CheckDailylyReward();
        CheckStar();
    }

    public void StartGameBtn(SkeletonGraphic btn)
    {
        Spine.TrackEntry trackEntry = btn.AnimationState.SetAnimation(0, "Start", false);
        Invoke("StartGame", trackEntry.Animation.Duration);
    }

    void StartGame()
    {
        gameManager.Initialize();
        gameObject.SetActive(false);
        ListBg.SetActive(false);
        ListBg.transform.parent.GetChild(1).gameObject.SetActive(true);
    }

    public void CheckStar()
    {
        int currentMap = (int)saveDataJson.GetData("OpenedMap");
        if (currentMap >= saveDataJson.TakeMapData().map.Length) currentMap--;
        LeverText.text = $"{currentMap}";

        star.text = $"{(int)saveDataJson.GetData("Star")}";
        chestStar.CheckStar();
    }
}
