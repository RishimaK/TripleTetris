using DG.Tweening;
using UnityEngine;

public class Setting : MonoBehaviour
{
    // bool isInitialize = false;
    public GameObject Service;
    public GameManager gameManager;
    AudioManager audioManager;
    SaveDataJson saveDataJson;
    AdsManager adsManager;
    public GameObject DialogSetting;

    // private Transform Board;
    public GameObject Sound;
    public GameObject Music;
    public GameObject Vibration;

    public GameObject SoundInHome;
    public GameObject MusicInHome;
    public GameObject VibrationInHome;

    void Start()
    {
        Initialize();
        CheckStatus();
    }

    void Initialize()
    {
        // isInitialize = true;
        // Board = DialogSetting.transform.GetChild(1);
        audioManager = Service.GetComponent<AudioManager>();
        saveDataJson = Service.GetComponent<SaveDataJson>();
        adsManager = Service.GetComponent<AdsManager>();
    }

    public void OpenDialog()
    {
        // if (!isInitialize) Initialize();
        // DialogSetting.SetActive(true);
        // Board.localScale = new Vector3(0.6f, 0.6f, 1f);
        // Board.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack);
    }

    public void CloseDialog()
    {
        // Board.DOPause();
        RectTransform board = DialogSetting.transform.GetChild(2).GetComponent<RectTransform>();
        board.DOScale(new Vector3(0f, 0f, 1f), 0.2f).OnComplete(() =>
        {
            DialogSetting.SetActive(false);
        });
    }

    void ContinueGame()
    {
        gameManager.ContinueGame();
    }

    void CheckStatus()
    {
        PlayerData data = saveDataJson.GetData();
        if (data.Sound)
        {
            ChangeState(Sound);
            ChangeState(SoundInHome);
        }
        if (data.Music)
        {
            ChangeState(Music);
            ChangeState(MusicInHome);
        }
        // if(data.Vibration) 
        // {
        //     ChangeState(Vibration);
        //     ChangeState(VibrationInHome);
        // }
    }

    public void Replay()
    {
        CloseDialog();
        gameManager.ReplayGame();
        adsManager.ShowInterstitialAd();
    }

    public void Exit()
    {
        CloseDialog();
        Invoke("ContinueGame", 0.2f);
    }

    public void GoToHome()
    {
        CloseDialog();
        gameManager.GoToHome();
        adsManager.ShowInterstitialAd();
    }

    public void ChangeSound()
    {
        // audioManager.PlaySFX("click");
        ChangeState(SoundInHome);
        bool status = ChangeState(Sound);
        saveDataJson.SaveData("Sound", status);
        // audioManager.ChangeStatusOfSound(status);
    }

    public void ChangeMusic()
    {
        // audioManager.PlaySFX("click");
        ChangeState(MusicInHome);
        bool status = ChangeState(Music);
        saveDataJson.SaveData("Music", status);
        // audioManager.ChangeStatusOfMusic(status);
    }

    public void ChangeVibration()
    {
        // audioManager.PlaySFX("click");
        ChangeState(VibrationInHome);
        bool status = ChangeState(Vibration);
        saveDataJson.SaveData("Vibration", status);
    }

    bool ChangeState(GameObject target)
    {
        GameObject on = target.transform.GetChild(0).gameObject;
        GameObject off = target.transform.GetChild(1).gameObject;

        off.SetActive(!off.activeSelf);
        on.SetActive(!on.activeSelf);

        return !target.transform.GetChild(1).gameObject.activeSelf;
    }

    public void PlayShowDialog()
    {
        DialogSetting.SetActive(true);
        RectTransform board = DialogSetting.transform.GetChild(2).GetComponent<RectTransform>();
        RectTransform body = board.GetChild(0).GetComponent<RectTransform>();
        RectTransform heard = board.GetChild(1).GetComponent<RectTransform>();

        board.localScale = Vector3.one;

        float height = body.rect.height;
        float YY = body.localPosition.y;
        heard.localScale = new Vector3(0.6f, 0.6f, 1f);
        body.sizeDelta = new Vector2(body.rect.width, 0);
        body.localPosition = new Vector3(0, height / 2, 0);

        heard.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.OutBack);

        body.DOSizeDelta(new Vector2(body.rect.width, height), 0.5f).SetDelay(0.3f);
        body.DOLocalMove(new Vector3(0, YY, 0), 0.5f).SetDelay(0.3f);
    }
}
