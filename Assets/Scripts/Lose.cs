using DG.Tweening;
using UnityEngine;

public class Lose : MonoBehaviour
{
    bool isInitialize = false;

    Transform canvas;
    RectTransform Name;
    RectTransform Fox;
    Transform BtnHome;
    Transform BtnRePlay;

    public GameObject Service;
    AdsManager adsManager;

    public GameManager gameManager;

    void Initialize ()
    {
        isInitialize = true;

        Name = transform.GetChild(1).GetComponent<RectTransform>();
        Fox = transform.GetChild(2).GetComponent<RectTransform>();
        BtnHome = transform.GetChild(3);
        BtnRePlay = transform.GetChild(4);

        canvas = transform.parent.parent;
        adsManager = Service.GetComponent<AdsManager>();
    }

    public void PlayLoseAnimation ()
    {
        if (!isInitialize) Initialize();
        gameObject.SetActive(true);

        float xx = canvas.GetComponent<RectTransform>().rect.x + BtnHome.GetComponent<RectTransform>().rect.x;
        float yy = canvas.GetComponent<RectTransform>().rect.y + Name.rect.y;
        float nameY = Name.localPosition.y;
        float homex = BtnHome.localPosition.x;
        
        Name.localPosition = new Vector3 (0, -yy, 0);
        Name.DOLocalMove(new Vector3 (0, nameY, 0), 0.5f).SetEase(Ease.OutCubic);

        Fox.localScale = Vector3.zero;
        Fox.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce).SetDelay(0.5f);

        BtnHome.localPosition = new Vector3 (xx, BtnHome.localPosition.y, 0);
        BtnRePlay.localPosition = new Vector3 (-xx, BtnHome.localPosition.y, 0);
        BtnHome.DOLocalMove(new Vector3 (homex, BtnHome.localPosition.y, 0), 0.5f).SetDelay(1).SetEase(Ease.OutCubic);
        BtnRePlay.DOLocalMove(new Vector3 (-homex, BtnHome.localPosition.y, 0), 0.5f).SetDelay(1).SetEase(Ease.OutCubic);
    }

    public void GoToHome ()
    {
        gameObject.SetActive(false);
        gameManager.GoToHome();
        adsManager.ShowInterstitialAd();
    }

    public void Replay ()
    {
        gameObject.SetActive(false);
        gameManager.ReplayGame();
        adsManager.ShowInterstitialAd();
    }
}
