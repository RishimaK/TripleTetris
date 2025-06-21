using System.Collections;
using System.Threading.Tasks;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SupportTools : MonoBehaviour
{
    public GameObject Service;
    private SaveDataJson saveDataJson;
    public Movement movement;
    public GameObject ListTool;
    public ShopTool shopTool;
    BlockCreator blockCreator;
    GameManager gameManager;
    public bool enabledTouch = false;
    Vector3 FirstBlockPosition;
    float BlockSize;
    string ToolType = "";

    public SkeletonGraphic TNTAnim;
    public SkeletonGraphic HammerAnim;
    public SkeletonGraphic BoomAnim;
    
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        blockCreator = GetComponent<BlockCreator>();
        saveDataJson = Service.GetComponent<SaveDataJson>();
    }

    public void Initialize(Vector3 firstBlockPosition, float blockSize)
    {
        FirstBlockPosition = firstBlockPosition;
        BlockSize = blockSize;
    }

    void StopPlayerBlock ()
    {
        movement.enabledTouch = false;
        movement.allowMoveDown = false;
    }

    void ContinuePlayerBlock ()
    {
        movement.enabledTouch = true;
        movement.allowMoveDown = true;
    }

    public void Boom ()
    {
        if (ToolType == "Boom")
        {
            blockCreator.RedoListBlockInfluence();
            DisableTouch();
            return;
        }

        ToolType = "Boom";
        if((int)saveDataJson.GetData(ToolType) == 0)
        {
            gameManager.StopGame();
            shopTool.OpenDialog(ToolType);
            ToolType = "";
            return;
        }
        ActiveTool();

        StopPlayerBlock();
        movement.DisableTouch();
        enabledTouch = true;
    }

    public void TNT ()
    {
        if (ToolType == "TNT")
        {
            blockCreator.RedoListBlockInfluence();
            DisableTouch();
            return;
        }
        
        ToolType = "TNT";
        if((int)saveDataJson.GetData(ToolType) == 0)
        {
            gameManager.StopGame();
            shopTool.OpenDialog(ToolType);
            ToolType = "";
            return;
        }
        ActiveTool();
        StopPlayerBlock();
        movement.DisableTouch();
        enabledTouch = true;
    }

    public void Hammer ()
    {
        if (ToolType == "Hammer")
        {
            blockCreator.RedoListBlockInfluence();
            DisableTouch();
            return;
        }

        ToolType = "Hammer";
        if((int)saveDataJson.GetData(ToolType) == 0)
        {
            gameManager.StopGame();
            shopTool.OpenDialog(ToolType);
            ToolType = "";
            return;
        }
        ActiveTool();
        StopPlayerBlock();
        movement.DisableTouch();
        enabledTouch = true;
    }

    public void RainbowBlock ()
    {
        // if (ToolType == "Rainbow") { DisableTouch(); return; }
        if (ToolType == "Rainbow") return;
        ToolType = "Rainbow";
        int toolValue = (int)saveDataJson.GetData(ToolType);
        if (toolValue == 0)
        {
            gameManager.StopGame();
            shopTool.OpenDialog(ToolType);
            ToolType = "";
            return;
        }

        saveDataJson.SaveData(ToolType, toolValue - 1);
        ActiveTool();
        // StopPlayerBlock();

        blockCreator.ChangePlayerBlockToRainbow();
    }

    void ActiveTool()
    {
        foreach (Transform child in ListTool.transform)
        {
            if (child.name != ToolType)
            {
                child.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                child.GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                child.GetChild(1).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                child.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }
    }

    void UnActiveTool()
    {
        foreach (Transform child in ListTool.transform)
        {
            if (child.name != ToolType)
            {
                child.GetComponent<Image>().color = Color.white;
                child.GetChild(0).GetComponent<Image>().color = Color.white;
                child.GetChild(1).GetComponent<Image>().color = Color.white;
                child.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
        }
    }

    Vector2 CurrentTouch = new Vector2(1000, 1000);

    async void Update()
    {
        if(!enabledTouch) return;
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                CurrentTouch = ConvertPixelToWorldUnit(touch.position);
                blockCreator.CheckListBlockToDestroy(ToolType, CurrentTouch);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // move down
                CurrentTouch = ConvertPixelToWorldUnit(touch.position);
                blockCreator.CheckListBlockToDestroy(ToolType, CurrentTouch);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                int x = Mathf.RoundToInt((CurrentTouch.y - FirstBlockPosition.y) / BlockSize);
                int y = Mathf.RoundToInt((CurrentTouch.x - FirstBlockPosition.x) / BlockSize);
                if (x < 0 || x >= 16 || y < 0 || y >= 9) return;

                bool isUseTool = false;
                switch (ToolType)
                {
                    case "Boom":
                        StartCoroutine(SetToolAnimation(BoomAnim, CurrentTouch));
                        isUseTool = await ActiveBoomAsync();
                        // isUseTool = blockCreator.ActiveBoom();
                        break;
                    case "TNT":
                        StartCoroutine(SetToolAnimation(TNTAnim, CurrentTouch));
                        isUseTool = await DeleteVerticalRowAsync();
                        // isUseTool = blockCreator.DeleteVerticalRow();
                        break;
                    case "Hammer":
                        StartCoroutine(SetToolAnimation(HammerAnim, CurrentTouch));
                        isUseTool = await DeleteHorizontalRowwAsync();
                        // isUseTool = blockCreator.DeleteHorizontalRow();
                        break;
                }

                if (isUseTool)
                {
                    int toolValue = (int)saveDataJson.GetData(ToolType) - 1;
                    saveDataJson.SaveData(ToolType, toolValue);
                    ListTool.transform.Find(ToolType).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{toolValue}";
                    enabledTouch = false;
                }
            }
        }
    }

    async Task<bool> DeleteVerticalRowAsync()
    {
        await Task.Delay(1500);
        return blockCreator.DeleteVerticalRow();
    }

    async Task<bool> DeleteHorizontalRowwAsync()
    {
        await Task.Delay(1500);
        return blockCreator.DeleteHorizontalRow();
    }

    async Task<bool> ActiveBoomAsync()
    {
        await Task.Delay(600);
        return blockCreator.ActiveBoom();
    }

    IEnumerator SetToolAnimation(SkeletonGraphic toolAnim, Vector2 CurrentTouch)
    {
        toolAnim.gameObject.SetActive(true);
        toolAnim.transform.position = new Vector3(CurrentTouch.x, CurrentTouch.y, toolAnim.transform.position.z);
        toolAnim.AnimationState.SetAnimation(0, "animation", false);
        yield return new WaitForSeconds(toolAnim.AnimationState.GetCurrent(0).Animation.Duration);
        toolAnim.gameObject.SetActive(false);
        toolAnim.AnimationState.ClearTracks();
        toolAnim.Skeleton.SetToSetupPose();
    }

    public void DisableTouch()
    {
        // enabledTouch = false;
        UnActiveTool();
        ContinuePlayerBlock();
        ToolType = "";
        CurrentTouch = new Vector2(1000, 1000);
        Invoke("EnabledTouchInMovement", 0.1f);
    }

    void EnabledTouchInMovement()
    {
        movement.EnabledTouch();
    }

    Vector3 ConvertPixelToWorldUnit(Vector2 pixelPosition)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(
            new Vector3(pixelPosition.x, pixelPosition.y, 10 + FirstBlockPosition.z)
        );

        return worldPoint;
    }
}
