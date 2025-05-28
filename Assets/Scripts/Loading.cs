// using System;
using System.Collections;
using DG.Tweening;

// using System.Threading.Tasks;
using GoogleMobileAds.Api;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public GameObject Service;
    SaveDataJson saveDataJson;
    AudioManager audioManager;
    AdsManager adsManager;

    public SkeletonGraphic AnimLoading;
    public GameObject home;
    public Slider slider;


    // public GameObject UpdateVersion;
    // public GameObject LegendarySUB;

    // private bool isStartGame = false;
    // private bool loadedOpenAd = true;
    // private bool checkUpdate = true;

    public GameObject Banner;

    void Start()
    {
        saveDataJson = Service.GetComponent<SaveDataJson>();
        audioManager = Service.GetComponent<AudioManager>();
        adsManager = Service.GetComponent<AdsManager>();

        Invoke("StartGame", 5.5f);
        PlayAnimLoading(5);
        // if(!Application.isEditor) StartCoroutine(GetVersion());
    }

    public void PlayAnimLoading(float time)
    {
        // gameObject.SetActive(true);
        // AnimLoading.AnimationState.ClearTracks();
        // AnimLoading.startingAnimation = "";
        // AnimLoading.AnimationState.SetAnimation(0, "animation", false);
        // AnimLoading.timeScale = 1f * 6f / time;
        slider.value = 0.1f;
        slider.DOValue(0.9f, 4).SetEase(Ease.InQuad);
        slider.DOValue(1, 0.5f).SetEase(Ease.InCubic).SetDelay(5);

        Invoke("ContinueGame", time + 0.5f);
    }

    void ContinueGame()
    {
        gameObject.SetActive(false);
    }

    // IEnumerator GetVersion()
    // {
    //     checkUpdate = false;
    //     UnityWebRequest www = UnityWebRequest.Get("https://play.google.com/store/apps/details?id=com.vnstart.hidden.objects");
    //     yield return www.SendWebRequest();
    //     if(www.result != UnityWebRequest.Result.Success)
    //     {
    //         Debug.Log("web request failed");
    //         checkUpdate = true;
    //     }
    //     else
    //     {
    //         string responseText = www.downloadHandler.text;
    //         // if(www.downloadHandler.text == Application.version)
    //         if (responseText.Contains(Application.version))
    //         {
    //             Debug.Log("right version");
    //             checkUpdate = true;
    //         }
    //         else
    //         {
    //             Debug.Log("wrong version");
    //             UpdateVersion.SetActive(true);
    //         }
    //     }
    // }

    // public void UpdateVersionOfGame()
    // {
    //     audioManager.PlaySFX("click");
    //     Application.OpenURL("https://play.google.com/store/apps/details?id=com.vnstart.hidden.objects");

    //     int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    //     SceneManager.LoadScene(currentSceneIndex);
    // }

    void StartGame()
    {
        // if(!checkUpdate){
        //     Invoke("StartGame", Time.deltaTime);
        //     return;
        // }


        Banner.SetActive(true);
        adsManager.ChangeStatusStartGame();
        audioManager.PlayMusic();
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            adsManager.LoadBannerAd();
        });
    }
}
