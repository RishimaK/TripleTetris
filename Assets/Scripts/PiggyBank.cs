using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PiggyBank : MonoBehaviour
{
    private AudioManager audioManager;
    private SaveDataJson saveDataJson;
    private Transform Board;
    bool isInitialize = false;

    public GameObject Service;
    public Slider slider;
    public TextMeshProUGUI totalCoin;
    public Button BuyBtn;
    public GameObject text1;
    public GameObject text2;
    public SkeletonGraphic FullAnimation;

    void Initialize()
    {
        isInitialize = true;
        Board = transform.GetChild(1);
        audioManager = Service.GetComponent<AudioManager>();
        saveDataJson = Service.GetComponent<SaveDataJson>();
    }

    public void OpenDialog()
    {
        if (!isInitialize) Initialize();
        int coin = (int)saveDataJson.GetData("PiggyBank");
        if (coin >= 5000)
        {
            coin = 5000;
            BuyBtn.interactable = true;
            text1.SetActive(false);
            text2.SetActive(true);
            FullAnimation.gameObject.SetActive(true);
            FullAnimation.AnimationState.SetAnimation(0, "animation", true);
        }
        totalCoin.text = $"{coin}";
        slider.value = coin / 5000f;

        gameObject.SetActive(true);

        Board.localScale = new Vector3(0.6f, 0.6f, 1f);
        Board.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack);
    }

    public void CloseDialog()
    {
        Board.DOPause();

        Board.DOScale(new Vector3(0f, 0f, 1f), 0.2f).OnComplete(() =>
        {
            gameObject.SetActive(false);

            if (FullAnimation.gameObject.activeSelf)
            {
                FullAnimation.gameObject.SetActive(false);
                FullAnimation.AnimationState.ClearTracks();
                FullAnimation.Skeleton.SetToSetupPose();
            }
        });
    }

    public void GetReward()
    {
        int coin = (int)saveDataJson.GetData("PiggyBank");
        coin -= 5000;
        saveDataJson.SaveData("PiggyBank", coin);
        slider.value = coin / 5000f;
        BuyBtn.interactable = false;

        text1.SetActive(true);
        text2.SetActive(false);
        FullAnimation.gameObject.SetActive(false);
    }
}
