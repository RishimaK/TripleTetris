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
    }

    public void Initialize()
    {
        blockCreator = GetComponent<BlockCreator>();

        gameObject.SetActive(true);
        GameUI.SetActive(true);

        SetListBlockToFind();
        SetTool();
        // currentMap = (int)saveDataJson.GetData("OpenedMap");
        blockCreator.CreateLever(currentMap);
    }

    void SetTool()
    {
        foreach (Transform child in ListTool.transform)
        {
            child.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"{(int)saveDataJson.GetData(child.name)}";
        }
    }

    void SetListBlockToFind()
    {
        currentMap = (int)saveDataJson.GetData("OpenedMap");
        // if(!blockCreator.CheckMapLimit(currentMap)) currentMap--;
        // int[] RandomRange = saveDataJson.TakeMapData().map[currentMap].RandomRange;

        string[] listFind = new string[] { };
        int[] valueFind = new int[] { };
        if (currentMap < saveDataJson.TakeMapData().map.Length)
        {
            listFind = saveDataJson.TakeMapData().map[currentMap].ListFind;
            valueFind = saveDataJson.TakeMapData().map[currentMap].ValueFind;
        }
        else (listFind, valueFind) = SetListItemToFind();

        float listFindLength = listFind == null ? 1 : listFind.Length;
        float blockSize = 125;
        float pos = 0;
        float distanceBetweenItem = blockSize / 3;

        float ListBlockToFindX = ListBlockToFind.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        if (listFindLength * blockSize < ListBlockToFindX)
        {
            pos = (ListBlockToFindX - (listFindLength * blockSize)) / (listFindLength + 1);
            ListBlockToFind.GetComponent<RectTransform>().sizeDelta = new Vector2(ListBlockToFindX, 0);
        }
        else
        {
            ListBlockToFind.GetComponent<RectTransform>().sizeDelta = new Vector2((blockSize + distanceBetweenItem) * listFindLength - distanceBetweenItem, 0);
        }

        int theme = (int)saveDataJson.GetData("CurrentTheme");

        for (int i = 0; i < listFindLength; i++)
        {
            Transform newBlock = ObjectPoolManager.SpawnObject(blockToFindPrefab, Vector3.zero, Quaternion.identity).transform;
            newBlock.SetParent(ListBlockToFind.transform);
            newBlock.localScale = Vector3.one;

            if (pos == 0) newBlock.localPosition = new Vector3(blockSize / 2 + (blockSize + distanceBetweenItem) * i, 0, 0);
            else newBlock.localPosition = new Vector3(blockSize / 2 + pos + (blockSize + pos) * i, 0, 0);

            newBlock.name = listFind == null ? "all" : listFind[i];

            newBlock.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"0/{valueFind[i]}";

            Image img = newBlock.GetChild(0).GetComponent<Image>();
            img.sprite =
                listFind == null ? textureResources.ListBlockUI.FirstOrDefault(x => x.name == $"all") :
                textureResources.ListBlockUI.FirstOrDefault(x => x.name == $"{theme}.{newBlock.name}");
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
        if (itemSize.x >= itemSize.y && itemSize.x != parentSize.x)
        {
            scaleRatio = parentSize.x / itemSize.x;
        }
        else if (itemSize.y > itemSize.x && itemSize.y != parentSize.y)
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
        for (int i = 0; i < 1;)
        {
            length--;
            if (length < 0) break;
            Transform block = ListBlockToFind.transform.GetChild(0);
            block.name = "BlockUI";
            ObjectPoolManager.ReturnObjectToPool(block.gameObject);
            block.transform.SetParent(ListBlockToFind.transform.parent);
        }
    }

    public void CheckScore(string block, int num)
    {
        Transform item = ListBlockToFind.transform.Find(block);
        TextMeshProUGUI itemText;
        if (ListBlockToFind.transform.GetChild(0).name == "all")
        {
            itemText = ListBlockToFind.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        else if (item == null) return;
        else itemText = item.GetChild(1).GetComponent<TextMeshProUGUI>();

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
            if (total > score) return;
        }

        // Debug.Log("WinGame");
        WinGame();
    }

    void WinGame()
    {
        StopGame();
        saveDataJson.SaveData("OpenedMap", ++currentMap);
        saveDataJson.SaveData("Gold", (int)saveDataJson.GetData("Gold") + 10);
        saveDataJson.SaveData("Star", (int)saveDataJson.GetData("Star") + blockCreator.totalStar);
        saveDataJson.SaveData("PiggyBank", (int)saveDataJson.GetData("PiggyBank") + 10);
        // saveDataJson.SaveData("s", (int)saveDataJson.GetData("Gold") + 10);

        win.PlayWinAnimation();
    }

    public void LoseGame()
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
        blockCreator.GameOver();
        ResetListToFind();

        Invoke("Initialize", 0.1f);
    }

    public void GoToHome()
    {
        blockCreator.GameOver();
        GameOver();
    }

    public void OpenSettingDialog()
    {
        setting.PlayShowDialog();
        StopGame();
    }

    (string[], int[]) SetListItemToFind()
    {
        float mapa = (float)currentMap % 20f;
        switch (mapa)
        {
            case 1: return (new string[] { "1", "4" }, new int[] { 8, 9 });
            case 2: return (new string[] { "5", "6", "7" }, new int[] { 9, 12, 9 });
            case 3: return (new string[] { "5", "3", "4" }, new int[] { 9, 15, 12 });
            case 4: return (new string[] { "7", "4" }, new int[] { 17, 15 });
            case 5: return (new string[] { "3", "4" }, new int[] { 16, 18 });
            case 6: return (new string[] { "6", "7" }, new int[] { 21, 3 });
            case 7: return (new string[] { "3", "4", "5" }, new int[] { 12, 8, 10 });
            case 8: return (new string[] { "4", "5", "6", "7" }, new int[] { 15, 5, 20, 8 });
            case 9: return (new string[] { "5" }, new int[] { 18 });
            case 10: return (null, new int[] { 35 });
            case 11: return (new string[] { "4", "7" }, new int[] { 12, 7 });
            case 12: return (new string[] { "1", "2", "3", "4" }, new int[] { 13, 6, 7, 14 });
            case 13: return (new string[] { "4" }, new int[] { 24 });
            case 14: return (new string[] { "5", "2", "3" }, new int[] { 14, 16 , 12 });
            case 15: return (new string[] { "3", "5", "7" }, new int[] { 15, 15, 15 });
            case 16: return (new string[] { "5", "8" }, new int[] { 17, 8 });
            case 17: return (new string[] { "6", "7" }, new int[] { 3, 20 });
            case 18: return (new string[] { "4", "5", "8", "7" }, new int[] { 21, 15, 18, 7 });
            case 19: return (new string[] { "1", "2" }, new int[] { 10, 10 });
            case 0: return (null, new int[] { 50 });
            default: return (null, new int[] { 30 });
        }
    }
}


// ,
//         {
//             "MapName": 10,
//             "BlockList": [
//                 [0, 0, -1, 0, 0, 0, -1, 0, 0],
//                 [0, 0, -1, 0, 0, 0, -1, 0, 0],
//                 [0, 0, -1, -1, 0, -1, -1, 0, 0],
//                 [0, 0, -1, -1, 0, -1, -1, 0, 0],
//                 [0, 0, -1, -1, 0, -1, -1, 0, 0],
//                 [0, 0, -1, -1, 0, -1, -1, 0, 0],
//                 [0, -1, -1, -1, 0, -1, -1, -1, 0],
//                 [0, -1, -1, -1, 0, -1, -1, -1, 0],
//                 [0, -1, -1, -1, 0, -1, -1, -1, 0],
//                 [0, -1, -1, -1, 0, -1, -1, -1, 0]
//             ],
//             "ListFind": null,
//             "ValueFind": [30],
//             "RandomRange": [3,7]
//         }