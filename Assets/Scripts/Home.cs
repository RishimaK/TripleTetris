// using System;
using Spine.Unity;
using TMPro;
using UnityEngine;

public class IntArrayWrapper
{
    public int[] array;
    
    public IntArrayWrapper(int[] arr)
    {
        array = arr;
    }
}
public class Home : MonoBehaviour
{
    BlockCreator blockCreator;
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
        // saveDataJson.SaveData("OpenedMap", 10);
        blockCreator = gameManager.GetComponent<BlockCreator>();
        dailyReward.CheckDailylyReward();
        // CreateRandomMap();
        CheckStar();
    }

    void CreateRandomMap()
    {
        int[] A = new int[] {};

        for (int i = 0; i < 5000; i++)
        {
            int ALength = A.Length;
            System.Array.Resize(ref A, A.Length + 1);
            int ran = Random.Range(0, saveDataJson.TakeMapShapeList().MapList.Length);
            A[ALength] = ran;
        }

        IntArrayWrapper wrapper = new IntArrayWrapper(A);
        string json = JsonUtility.ToJson(wrapper);
        Debug.Log(json);
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
        // if (currentMap >= saveDataJson.TakeMapData().map.Length) currentMap--;
        // if(! blockCreator.CheckMapLimit(currentMap))currentMap--;
        LeverText.text = $"{currentMap}";

        star.text = $"{(int)saveDataJson.GetData("Star")}";
        chestStar.CheckStar();
    }
}
