using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckySpin : MonoBehaviour
{
    private AdsManager adsManager;
    private AudioManager audioManager;
    private SaveDataJson saveDataJson;
    public SaveDataJson Service;

    public RectTransform Dial;
    private bool slowDown = false;

    public Image RewardIcon;
    public TextMeshProUGUI RewardTxt;

    public RectTransform IcAnimation;
    public Shop shop;

    public GameObject btnExit;
    public GameObject btnReward;

    bool isInitialize = false;


    [Header("SpriteAllItem")]
    public Sprite[] sprites = new Sprite[8];

    [System.Serializable]
    public class Item
    {
        public int name;
        public float dropRate; // Tỉ lệ rơi (0-100)
        public string rewardItem;
        public int valueItem;
    }

    void Start()
    {
        Initialize();
    }

    void Initialize ()
    {
        isInitialize = true;

        adsManager = Service.GetComponent<AdsManager>();
        audioManager = Service.GetComponent<AudioManager>();
        saveDataJson = Service.GetComponent<SaveDataJson>();
    }
    private List<Item> items = new List<Item>
    {
        new Item { name = 1, dropRate = 40, rewardItem = "Gold", valueItem = 100},
        new Item { name = 2, dropRate = 7f, rewardItem = "TNT", valueItem = 1}, //250
        new Item { name = 3, dropRate = 50f, rewardItem = "Gold", valueItem = 50},
        new Item { name = 4, dropRate = 1f, rewardItem = "Rainbow", valueItem = 1}, //300
        new Item { name = 5, dropRate = 30f, rewardItem = "Gold", valueItem = 10},
        new Item { name = 6, dropRate = 7f, rewardItem = "Hammer", valueItem = 1}, //250
        new Item { name = 7, dropRate = 50f, rewardItem = "Gold", valueItem = 50},
        new Item { name = 8, dropRate = 15, rewardItem = "Boom", valueItem = 1}, //200
    };

    public Item GetRandomItem()
    {
        double randomValue = System.Math.Round(Random.Range(0f, 200));

        float currentSum = 0;
        foreach (var item in items)
        {
            currentSum += item.dropRate;
            if (randomValue <= currentSum)
            {
                return item;
            }
        }

        return items[5];
    }

    public void Rotation()
    {
        // audioManager.PlaySFX("click");
        adsManager.ShowRewardedAd("LuckyWheel");
    }

    public void GetReward()
    {
        adsManager.LogEvent("LuckySpin");
        adsManager.ShowRewardedAd("LuckySpin");
    }

    public void StartSpineWheel()
    {
        btnExit.SetActive(false);
        btnReward.SetActive(false);
        StartCoroutine(RotateDial(GetRandomItem().name));
    }

    IEnumerator RotateDial(int reward, float speed = 1, float eulerAnglesZ = 22.5f)
    {
        int price = 0;
        while (speed > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            if(speed >= 50 && !slowDown) 
            {
                slowDown = true;
                Dial.eulerAngles = new Vector3(0 ,0 ,45 + 45 * (reward - 2));
            }

            if(slowDown) speed -= 0.2f;
            else speed += 2;

            Dial.eulerAngles = new Vector3(0 ,0 ,Dial.eulerAngles.z - speed);

            eulerAnglesZ += speed;
            if(eulerAnglesZ >= 45)
            {
                price = CheckReward();
                // audioManager.PlaySFX("tach");
                eulerAnglesZ -= 45;
            }
        }
        // CheckReward();

        CollectReward(price);
        slowDown = false;
    }

    int CheckReward()
    {
        float val = Dial.eulerAngles.z;
        int priceNum = 0;
        if(val > 337.5 || val <= 22.5) priceNum = 1;
        else if (val > 292.5) priceNum = 8;
        else if (val > 247.5) priceNum = 7;
        else if (val > 202.5) priceNum = 6;
        else if (val > 157.5) priceNum = 5;
        else if (val > 112.5) priceNum = 4;
        else if (val > 67.5) priceNum = 3;
        else if (val > 22.5) priceNum = 2;
        RewardIcon.sprite = sprites[priceNum - 1];

        RewardTxt.text = $"{(int)saveDataJson.GetData(items[priceNum - 1].rewardItem)}";
        return priceNum;
    }

    void CollectReward (int price)
    {
        IcAnimation.GetComponent<Image>().sprite = sprites[price - 1];
        Item item = items[price - 1];
        // Transform particle = RewardIcon.transform.GetChild(0);
        int val = (int)saveDataJson.GetData(item.rewardItem);
        val += item.valueItem;
        audioManager.PlaySFX("merge");

        IcAnimation.localScale = new Vector3(0, 0, 1);
        IcAnimation.position = Dial.position;
        IcAnimation.gameObject.SetActive(true);
        IcAnimation.DOScale(new Vector3(1.2f, 1.2f, 1), 0.5f).SetEase(Ease.OutBack);
        IcAnimation.DOMove(RewardIcon.transform.position, 0.5f).SetDelay(0.5f).SetEase(Ease.InBack).OnComplete(() => {
            audioManager.PlaySFX("collect");
            // particle.gameObject.SetActive(true);
            // particle.GetComponent<ParticleSystem>().Play();

            RewardTxt.text = $"{val}";
            saveDataJson.SaveData(item.rewardItem, val);
            if(item.rewardItem == "Gold") 
            {
                shop.SetGold();
                saveDataJson.SaveData("PiggyBank", (int)saveDataJson.GetData("PiggyBank") + items[price - 1].valueItem);
            }
        });

        IcAnimation.DOScale(0f, 0.3f).SetDelay(1).SetEase(Ease.OutBack).OnComplete(() => {
            IcAnimation.gameObject.SetActive(false);
            // particle.gameObject.SetActive(false);
            btnExit.SetActive(true);
            btnReward.SetActive(true);
        });
    }

    public void OpenDialog()
    {
        // btnExit.SetActive(true);
        // btnReward.SetActive(true);
        if (!isInitialize) Initialize();

        gameObject.SetActive(true);
        RewardTxt.text = $"{(int)saveDataJson.GetData("Gold")}";
        PlayBtnAnimation();
        // audioManager.PlaySFX("click");
    }

    void PlayBtnAnimation()
    {
        RectTransform btn = btnReward.GetComponent<RectTransform>();
        btn.DOPause();
        btn.localScale = Vector3.one;
        btn.DOScale(new Vector3(1.1f, 1.1f, 1), 0.5f).SetLoops(10, LoopType.Yoyo).OnComplete(() => {
            if(gameObject.activeSelf) PlayBtnAnimation();
        });
    }

    public void Exit()
    {
        gameObject.SetActive(false);
        RewardIcon.sprite = sprites[0];
        // audioManager.PlaySFX("click");
    }
}
