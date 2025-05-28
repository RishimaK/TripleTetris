using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTool : MonoBehaviour
{
    // bool enabledTouch = false;

    public Image tool;
    public GameObject Service;
    public List<Sprite> sprites;
    public TextMeshProUGUI price;
    public TextMeshProUGUI goldTxt;
    public Transform ListTool;
    public GameManager gameManager;
    public Shop shop;

    SaveDataJson saveDataJson;
    AdsManager adsManager;
    int ToolPrice;
    string ToolType;

    void Initialize ()
    {
        saveDataJson = Service.GetComponent<SaveDataJson>();
        adsManager = Service.GetComponent<AdsManager>();

        goldTxt.text = $"{(int)saveDataJson.GetData("Gold")}";

        tool.sprite = sprites.Find( t => t.name == ToolType);
        tool.SetNativeSize();

        switch (ToolType)
        {
            case "Boom":
                ToolPrice = 200;
                break;
            case "TNT":
            case "Hammer":
                ToolPrice = 250;
                break;
            case "Rainbow":
                ToolPrice = 300;
                break;
        }

        price.text = $"{ToolPrice}";
    }

    public void OpenDialog (string toolType)
    {
        ToolType = toolType;
        Initialize();
        Transform board = gameObject.transform.GetChild(1);
        // audioManager.PlaySFX("click");
        gameObject.SetActive(true);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();

        board.DOScale(new Vector3(1f,1f,1f), 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
            // enabledTouch = true;

        });
    }

    public void CloseDialog()
    {
        Transform board = gameObject.transform.GetChild(1);
        // audioManager.PlaySFX("click");
        board.DOPause();
        // enabledTouch = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
            gameManager.ContinueGame();
        });
    }

    public void Buy ()
    {
        int gold = (int)saveDataJson.GetData("Gold");
        if(gold < ToolPrice)
        {
            
        }
        else
        {
            gold -= ToolPrice;
            goldTxt.text = $"{gold}";
            saveDataJson.SaveData("Gold", gold);
            shop.SetGold();

            GetTool();
        }
    }

    public void GetTool()
    {
        int toolVaule = (int)saveDataJson.GetData(ToolType) + 1;
        saveDataJson.SaveData(ToolType, toolVaule);
        ListTool.Find(ToolType).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{toolVaule}";
        CloseDialog();
    }

    public void WatchAds ()
    {
        adsManager.ShowRewardedAd("ShopTool");
    }
}
