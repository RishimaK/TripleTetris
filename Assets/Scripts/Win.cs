using System.Collections;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Win : MonoBehaviour
{
    public GameManager gameManager;
    BlockCreator blockCreator;
    AdsManager adsManager;
    public GameObject Service;

    bool isInitialize = false;
    Image BG;
    SkeletonGraphic anim1;
    SkeletonGraphic anim2;

    Transform FrameScore;

    Transform BtnHome;
    Transform BtnNext;

    TextMeshProUGUI GoldText;
    TextMeshProUGUI StarText;

    void Initialize ()
    {
        isInitialize = true;
        blockCreator = gameManager.GetComponent<BlockCreator>();
        BG = transform.GetChild(0).GetComponent<Image>();
        anim1 = transform.GetChild(2).GetComponent<SkeletonGraphic>();
        anim2 = transform.GetChild(1).GetComponent<SkeletonGraphic>();

        FrameScore = transform.GetChild(3);
        GoldText = FrameScore.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        StarText = FrameScore.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        BtnHome = transform.GetChild(4);
        BtnNext = transform.GetChild(5);

        adsManager = Service.GetComponent<AdsManager>();
    }

    void PlayAnim ()
    {
        anim1.gameObject.SetActive(true);
        anim2.gameObject.SetActive(true);
        anim1.AnimationState.SetAnimation(0, "animation", false);
        anim2.AnimationState.SetAnimation(0, "animation", true);
    }

    IEnumerator PlayScoreAnimation (float waitTime)
    {
        float xx = BG.GetComponent<RectTransform>().rect.x + BtnHome.GetComponent<RectTransform>().rect.x;

        float homeX = BtnHome.localPosition.x;
        BtnHome.localPosition = new Vector3 (xx,-779,0);
        BtnNext.localPosition = new Vector3 (-xx,-779,0);
        yield return new WaitForSeconds(waitTime);

        int i = 0;
        while (i < 10)
        {
            i++;
            yield return new WaitForSeconds(0.03f);
            GoldText.text = $"{i}";
        }

        FrameScore.GetChild(1).localScale = Vector3.zero;
        StarText.gameObject.SetActive(false);
        StarText.text = $"{blockCreator.totalStar}";
        FrameScore.GetChild(1).DOScale(1, 0.2f).SetDelay(0.2f).SetEase(Ease.OutBounce).OnComplete( () => StarText.gameObject.SetActive(true));

        BtnHome.transform.DOLocalMove(new Vector3(homeX, - 779, 0), 0.5f).SetDelay(0.7f).SetEase(Ease.OutCubic);
        BtnNext.transform.DOLocalMove(new Vector3(-homeX, - 779, 0), 0.5f).SetDelay(0.7f).SetEase(Ease.OutCubic);

    }

    void StopAllAnimation ()
    {
        anim1.AnimationState.ClearTracks();
        anim1.Skeleton.SetToSetupPose();
        // anim1.Update(0);
    }

    public void PlayWinAnimation ()
    {
        if (!isInitialize) Initialize();

        BG.color = new Color(0,0,0,0);
        BG.DOFade(0.59f, 0.5f);

        anim1.gameObject.SetActive(false);
        anim2.gameObject.SetActive(false);
        Invoke("PlayAnim", 0.5f);

        FrameScore.GetChild(0).localScale = Vector3.zero;
        FrameScore.GetChild(1).localScale = Vector3.zero;

        FrameScore.localScale = Vector3.zero;
        FrameScore.DOScale(1, 0.2f).SetDelay(1.5f).SetEase(Ease.OutBounce);

        FrameScore.GetChild(0).localScale = Vector3.zero;
        GoldText.transform.localScale = Vector3.zero;
        GoldText.text = "";
        FrameScore.GetChild(0).DOScale(1, 0.1f).SetDelay(1.7f).SetEase(Ease.OutBounce);
        GoldText.transform.DOScale(1, 0.1f).SetDelay(1.7f).SetEase(Ease.OutBounce);

        gameObject.SetActive(true);
        StartCoroutine(PlayScoreAnimation(1.7f));
    }

    public void GoToHome ()
    {
        StopAllAnimation();
        gameObject.SetActive(false);
        gameManager.GoToHome();
        adsManager.ShowInterstitialAd();
    }

    public void Replay ()
    {
        StopAllAnimation();
        gameObject.SetActive(false);
        gameManager.ReplayGame();
        adsManager.ShowInterstitialAd();
    }
}
