using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestStar : MonoBehaviour
{
    bool enabledTouch = true;
    bool isInitialize = false;
    SaveDataJson saveDataJson;
    public GameObject Service;
    public Slider slider;
    public TextMeshProUGUI textSlider;
    public Button btn;
    public SkeletonGraphic ChestAnim;
    public GameObject ClaimBtn;
    public Shop shop;

    public Sprite[] ListSprite = new Sprite[5];

    void Initialize()
    {
        isInitialize = true;
        saveDataJson = Service.GetComponent<SaveDataJson>();

    }

    void SetValue ()
    {
        int star = (int)saveDataJson.GetData("Star");
        star = star % 100;
        textSlider.text = $"{star}";
        slider.value = star / 100f;

        if(star == 100)
        {
            btn.interactable = true;
        }
        else btn.interactable = false;
    }
    
    public void OpenDialog ()
    {
        if(!isInitialize) Initialize();
        SetValue();
        Transform board = gameObject.transform.GetChild(1);
        // audioManager.PlaySFX("click");
        gameObject.SetActive(true);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();
        enabledTouch = false;
        board.DOScale(new Vector3(1f,1f,1f), 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
            enabledTouch = true;

        });
    }

    public void CloseDialog()
    {
        Transform board = gameObject.transform.GetChild(1);
        // audioManager.PlaySFX("click");
        board.DOPause();
        enabledTouch = false;
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
            enabledTouch = true;
        });
    }

    public void CheckStar ()
    {
        if(!isInitialize) Initialize();

        int star = (int)saveDataJson.GetData("Star");
        if(star / 100 <= (int)saveDataJson.GetData("ChestStar")) return;

        Transform board = gameObject.transform.GetChild(1);
        gameObject.SetActive(true);
        board.gameObject.SetActive(false);
        gameObject.transform.GetChild(0).GetComponent<Button>().interactable = false;
        
        StartCoroutine(PlayAnimChestStar());
    }

    List<string> listReWard = new List<string>();

    IEnumerator PlayAnimChestStar ()
    {
        ChestAnim.AnimationState.ClearTracks();
        ChestAnim.Skeleton.SetToSetupPose();
        ChestAnim.gameObject.SetActive(true);
        ChestAnim.AnimationState.SetAnimation(0, "animation", false);

        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;
        int ran = Random.Range(1,4);
        ran = 2;
        float posX = canvasWidth / (ran + 1);
        listReWard.Clear();
        string reward = "";
        yield return new WaitForSeconds(1f);

        for(int i = 0; i < ran; i++)
        {
            Transform child = ChestAnim.transform.GetChild(i);
            child.gameObject.SetActive(true);
            child.localPosition = Vector3.zero;
            child.localScale = Vector3.zero;

            child.GetComponent<RectTransform>().DOLocalMove(new Vector3(-canvasWidth / 2 + posX * (i + 1), 480, 0), 0.5f).SetDelay(0.2f * i);
            child.DOScale(Vector3.one, 0.5f).SetDelay(0.2f * i);

            Image childImage = child.GetComponent<Image>();
            if(ran == 1)
            {
                reward = "Rainbow";
                childImage.sprite = ListSprite[4];
                child.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x1";
            }
            else if(ran == 2)
            {
                if(i == 0)
                {
                    reward = "Gold";
                    childImage.sprite = ListSprite[0];
                    child.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x30";
                }
                else
                {
                    childImage.sprite = ListSprite[Random.Range(1,4)];
                    reward = childImage.sprite.name;
                    child.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x1";
                }
            }
            else
            {
                reward = "Gold";
                childImage.sprite = ListSprite[0];
                child.GetChild(0).GetComponent<TextMeshProUGUI>().text = "x75";
                
            }
            listReWard.Add(reward);

            childImage.SetNativeSize();
        }

        ClaimBtn.transform.localScale = Vector3.zero;
        ClaimBtn.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.2f * ran + 0.5f).SetEase(Ease.OutBack);
        ClaimBtn.SetActive(true);
    }

    public void ClaimReward ()
    {
        int num = listReWard.Count;
        if(num == 3)
        {
            saveDataJson.SaveData("Gold", (int)saveDataJson.GetData("Gold") + 225);
        }
        else if (num == 1)
        {
            saveDataJson.SaveData("Rainbow", (int)saveDataJson.GetData("Rainbow") + 1);
        }
        else
        {
            saveDataJson.SaveData("Gold", (int)saveDataJson.GetData("Gold") + 30);
            saveDataJson.SaveData(listReWard[1], (int)saveDataJson.GetData(listReWard[1]) + 1);
        }
        saveDataJson.SaveData("ChestStar",(int)saveDataJson.GetData("ChestStar") + 1);
        shop.SetGold();

        gameObject.SetActive(false);
        Transform board = gameObject.transform.GetChild(1);
        board.gameObject.SetActive(true);
        ChestAnim.gameObject.SetActive(false);
        ClaimBtn.SetActive(false);
        gameObject.transform.GetChild(0).GetComponent<Button>().interactable = true;
    }
}
