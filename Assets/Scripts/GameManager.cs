using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public SaveDataJson saveDataJson;
    public GameObject GameUI;
    public Home Home;
    public Win win;
    public Lose lose;

    public GameObject ListBlockToFind;
    public GameObject blockToFindPrefab;
    public TextureResources textureResources;
    public Movement PlayerBlock;
    public Setting setting;
    public GameObject ListTool;


    private int currentMap = 0;
    private BlockCreator blockCreator;
    
    void Start()
    {
        blockCreator = GetComponent<BlockCreator>();
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        GameUI.SetActive(true);

        SetListBlockToFind();
        SetTool();
        GetComponent<BlockCreator>().CreateLever(currentMap);
    }

    void SetTool()
    {
        foreach(Transform child in ListTool.transform)
        {
            child.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"{(int)saveDataJson.GetData(child.name)}";
        }
    }

    void SetListBlockToFind()
    {
        currentMap = (int)saveDataJson.GetData("OpenedMap");
        if(currentMap >= saveDataJson.TakeMapData().map.Length) currentMap--;
        string[] listFind = saveDataJson.TakeMapData().map[currentMap].ListFind;
        int[] valueFind = saveDataJson.TakeMapData().map[currentMap].ValueFind;
        float listFindLength = listFind.Length;
        float blockSize = 125;
        float pos = 0;

        float ListBlockToFindX = ListBlockToFind.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        if(listFindLength * blockSize < ListBlockToFindX)
        {
            pos = (ListBlockToFindX  - (listFindLength * blockSize)) / (listFindLength + 1);
            ListBlockToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(ListBlockToFindX, 0);
        }
        else
        {
            ListBlockToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(blockSize * listFindLength, 0);
        }

        for (int i = 0 ; i < listFindLength; i++)
        {
            Transform newBlock = ObjectPoolManager.SpawnObject(blockToFindPrefab, Vector3.zero, Quaternion.identity).transform;
            newBlock.SetParent(ListBlockToFind.transform);
            newBlock.localScale = Vector3.one;
            // newBlock.localPosition = new Vector3 (65 + (blockSize + 65)* i, 0, 0);
            newBlock.localPosition = new Vector3 (blockSize / 2 + pos + (blockSize + pos) * i, 0, 0);
            newBlock.name = listFind[i];

            newBlock.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"0/{valueFind[i]}";

            Image img = newBlock.GetChild(0).GetComponent<Image>();
            img.sprite = textureResources.ListBlockUI.FirstOrDefault(x => x.name == $"{newBlock.name}_icon");
            FitItemSizeToParent(img.GetComponent<RectTransform>());
        }
    }

    void FitItemSizeToParent(RectTransform item)
    {
        item.GetComponent<Image>().SetNativeSize();

        Vector2 itemSize = item.sizeDelta;
        // Vector2 imageSizeInCanvas = GetImageSizeInCanvas(item.transform.parent.GetComponent<Image>());
        // Vector2 parentSize = ConvertCanvasSizeToUnityUnits(imageSizeInCanvas);
        Vector2 parentSize = item.transform.parent.GetComponent<RectTransform>().sizeDelta;

        float scaleRatio = 1;
        if(itemSize.x >= itemSize.y && itemSize.x != parentSize.x)
        {
            scaleRatio = parentSize.x / itemSize.x;
        }
        else if(itemSize.y > itemSize.x && itemSize.y != parentSize.y)
        {
            scaleRatio = parentSize.y / itemSize.y;
        }

        item.transform.localScale = new Vector3(Mathf.Abs(scaleRatio), scaleRatio, 1f);
    }

    Vector2 GetImageSizeInCanvas(Image targetImage)
    {
        RectTransform imageRectTransform = targetImage.rectTransform;
        return new Vector2(
            imageRectTransform.rect.width * imageRectTransform.localScale.x,
            imageRectTransform.rect.height * imageRectTransform.localScale.y
        );
    }

    Vector2 ConvertCanvasSizeToUnityUnits(Vector2 canvasSize)
    {
        RectTransform canvasRectTransform = Home.transform.parent.parent.GetComponent<RectTransform>();
        // Lấy tỷ lệ giữa kích thước màn hình và kích thước Canvas
        Vector2 screenToCanvasRatio = new Vector2(
            Screen.width / canvasRectTransform.rect.width,
            Screen.height / canvasRectTransform.rect.height
        );

        // Chuyển đổi kích thước Canvas sang pixel
        Vector2 sizeInPixels = new Vector2(
            canvasSize.x * screenToCanvasRatio.x,
            canvasSize.y * screenToCanvasRatio.y
        );

        // Chuyển đổi từ pixel sang Unity units
        return sizeInPixels / Screen.dpi * 2.54f; // 2.54 là số cm trong 1 inch
    }

    public void GameOver()
    {
        ResetListToFind();
        gameObject.SetActive(false);
        GameUI.SetActive(false);
        Home.gameObject.SetActive(true);
        Home.CheckStar();
        Home.ListBg.SetActive(true);
        Home.ListBg.transform.parent.GetChild(1).gameObject.SetActive(false);
    }

    void ResetListToFind()
    {
        int length = ListBlockToFind.transform.childCount;
        for(int i = 0; i < 1;)
        {
            length--;
            if(length < 0) break;
            Transform block = ListBlockToFind.transform.GetChild(0);
            block.name = "BlockUI";
            ObjectPoolManager.ReturnObjectToPool(block.gameObject);
            block.transform.SetParent(ListBlockToFind.transform.parent);
        }
    }

    public void CheckScore(string block, int num)
    {
        Transform item = ListBlockToFind.transform.Find(block);
        if(item == null) return;
        TextMeshProUGUI itemText = item.GetChild(1).GetComponent<TextMeshProUGUI>();
        int score = Convert.ToInt32(itemText.text.Split("/")[0]);
        int total = Convert.ToInt32(itemText.text.Split("/")[1]);
        score += num;
        score = score <= total ? score : total;
        itemText.text = $"{score}/{total}";
    }

    public void CheckWinGame()
    {
        foreach (Transform child in ListBlockToFind.transform)
        {
            TextMeshProUGUI itemText = child.GetChild(1).GetComponent<TextMeshProUGUI>();
            int score = Convert.ToInt32(itemText.text.Split("/")[0]);
            int total = Convert.ToInt32(itemText.text.Split("/")[1]);
            if(total > score) return;
        }

        // Debug.Log("WinGame");
        WinGame();
    }

    void WinGame ()
    {
        StopGame();
        saveDataJson.SaveData("OpenedMap", ++currentMap);
        saveDataJson.SaveData("Gold", (int)saveDataJson.GetData("Gold") + 10);
        saveDataJson.SaveData("Star", (int)saveDataJson.GetData("Star") + blockCreator.totalStar);
        saveDataJson.SaveData("PiggyBank", (int)saveDataJson.GetData("PiggyBank") + 10);
        // saveDataJson.SaveData("s", (int)saveDataJson.GetData("Gold") + 10);

        win.PlayWinAnimation();
    }

    public void LoseGame ()
    {
        StopGame();
        lose.PlayLoseAnimation();
    }

    bool SupportToolsEnabledTouch;
    public void StopGame()
    {
        SupportToolsEnabledTouch = GetComponent<SupportTools>().enabledTouch;
        GetComponent<SupportTools>().enabledTouch = false;
        PlayerBlock.enabledTouch = false;
        PlayerBlock.allowMoveDown = false;
    }

    public void ContinueGame()
    {
        GetComponent<SupportTools>().enabledTouch = SupportToolsEnabledTouch;
        PlayerBlock.enabledTouch = true;
        PlayerBlock.allowMoveDown = true;
    }

    public void ReplayGame()
    {
        GetComponent<BlockCreator>().GameOver();
        ResetListToFind();

        Invoke("Initialize", 0.1f);
    }

    public void GoToHome ()
    {
        GetComponent<BlockCreator>().GameOver();
        GameOver();
    }

    public void OpenSettingDialog ()
    {
        setting.PlayShowDialog();
        StopGame();
    }
}
