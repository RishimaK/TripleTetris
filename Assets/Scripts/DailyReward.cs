using System;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    private int dailyRewardStack;
    public AudioManager audioManager;
    public SaveDataJson saveDataJson;

    public GameObject ListReward;
    public Shop shop;

    public GameObject ExitBtn;
    public Button ClaimBtn;
    public TextMeshProUGUI dailyRewardStackText;

    private bool isOpenApp = true;

    string FormatDateTime(DateTime dateTime) => dateTime.ToString("dd/MM/yyyy");

    public void CheckDailylyReward ()
    {
        DateTime currentTime = DateTime.Now;

        string dailyTime = (string)saveDataJson.GetData("DailyReward");
        string format = "dd/MM/yyyy";
        dailyRewardStack = (int)saveDataJson.GetData("DailyRewardStack");

        ExitBtn.SetActive(false);
        ClaimBtn.interactable = true;

        if(dailyTime == null || dailyTime == "")
        {
            dailyRewardStack = 1;
            gameObject.SetActive(true);
        }
        else
        {
            TimeSpan difference = currentTime - DateTime.ParseExact(dailyTime, format, null);
            if(difference.Days == 1)
            {
                // dailyRewardStack = (int)saveDataJson.GetData("DailyRewardStack");
                dailyRewardStack = dailyRewardStack == 7 ? 1 : dailyRewardStack + 1;

                gameObject.SetActive(true);
            }
            else if (difference.Days > 1)
            {
                dailyRewardStack = 1;
                gameObject.SetActive(true);
            }
            else
            {
                dailyRewardStack++;
                ExitBtn.SetActive(true);
                ClaimBtn.interactable = false;
                isOpenApp = false;
                SetScaleBtn();
                ExitBtn.transform.localScale = Vector3.zero;
            }
        }

        SetDailyReward();
    }

    void SetDailyReward()
    {
        dailyRewardStackText.text = $"{dailyRewardStack - 1}";
        for(int i = 0; i < dailyRewardStack - 1; i++)
        {
            Transform child = ListReward.transform.GetChild(i);
            if (i == 6)
            {
                child.GetChild(0).GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation", false);
                child.GetChild(0).GetComponent<SkeletonGraphic>().AnimationState.TimeScale = 10f;
                child.GetChild(3).gameObject.SetActive(false);
            }
            else
            {
                child.GetChild(1).gameObject.SetActive(true);
                child.GetChild(0).gameObject.SetActive(false);
            }
            child.GetChild(2).gameObject.SetActive(true);
        }
        // SetOscillatingRotation();
        PlayChestRewardAnimation();
    }
    // Sequence rotationSequence;
    private bool stopChestAnimation = true;
    public void PlayChestRewardAnimation()
    {
        if(!gameObject.activeSelf || !stopChestAnimation) return;

        //  chest = chest == null ? ListReward.transform.GetChild(dailyRewardStack - 1).GetChild(0) : chest;
        Transform chest = ListReward.transform.GetChild(dailyRewardStack - 1).GetChild(0);
        DOTween.Kill(chest);

        Sequence rotationSequence = DOTween.Sequence();
        rotationSequence.Append(chest.DOScale(1.1f, 0.1f).SetEase(Ease.OutBounce));

        for (int i = 0; i < 7; i++)
        {
            rotationSequence.Append(chest.DORotate(new Vector3(0, 0, 10), 0.1f)
                .SetEase(Ease.InOutQuad));
            
            rotationSequence.Append(chest.DORotate(new Vector3(0, 0, -10), 0.1f)
                .SetEase(Ease.InOutQuad));
        }
        rotationSequence.Append(chest.DORotate(new Vector3(0, 0, 0), 0.05f)
            .SetEase(Ease.InOutQuad));
        rotationSequence.Append(chest.DOScale(1f, 0.1f).SetEase(Ease.OutBounce));
        rotationSequence.OnComplete(() => {
            Invoke("PlayChestRewardAnimation", 1f);
        });
        // rotationSequence.SetAutoKill(false);
        // rotationSequence.Pause();
    }

    // void PlayChestRewardAnimation()
    // {
    //     if(!gameObject.activeSelf) return;
    //     Debug.Log("??");
    //     // Debug.Log(rotationSequence.aut);
    //     Sequence a = rotationSequence.Clone();
    //     a.Play();
    // }

    public void ClaimReward()
    {
        audioManager.PlaySFX("click");
        saveDataJson.SaveData("DailyRewardStack", dailyRewardStack);
        saveDataJson.SaveData("DailyReward", FormatDateTime(DateTime.Now));
        dailyRewardStackText.text = $"{dailyRewardStack}";
        int gold = 0;
        int TNT = 0;
        int hammer = 0;
        int boom = 0;
        int rainbow = 0;
        switch (dailyRewardStack)
        {
            case 1: gold = 100; break;
            case 2: TNT = 1; break;
            case 3: gold = 150; break;
            case 4: hammer = 2; break;
            case 5: gold = 200; break;
            case 6: boom = 2; break;
            case 7:
                gold = 500;
                rainbow = 4;
                TNT = 2;
            break;
        }

        // shop.CountCoins(gold);
        shop.AddMoreStuff(gold, boom, TNT, hammer, rainbow);
        PlayChestAnimation();
    }

    void PlayChestAnimation ()
    {
        Transform chest = ListReward.transform.GetChild(dailyRewardStack - 1).GetChild(0);
        Transform reward = ListReward.transform.GetChild(dailyRewardStack - 1).GetChild(2);
        stopChestAnimation = false;
        DOTween.KillAll(chest);

        if(dailyRewardStack == 7) ListReward.transform.GetChild(dailyRewardStack - 1).GetChild(3).gameObject.SetActive(false);

        chest.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "animation",  false);
        reward.localScale = Vector3.zero;
        reward.gameObject.SetActive(true);
        ClaimBtn.interactable = false;

        reward.DOScale(Vector3.one, 0.1f).SetDelay(1.13f).SetEase(Ease.OutBounce).OnComplete(() => {
            ExitBtn.SetActive(true);
        });
    }

    public void Exit()
    {
        CloseAnimation();
    }

    public void OpenAnimation()
    {
        gameObject.SetActive(true);
        ClaimBtn.interactable = false;
        audioManager.PlaySFX("click");
        Transform board = gameObject.transform.GetChild(1);
        board.localScale = new Vector3(0.6f,0.6f,1f);
        board.DOPause();

        PlayBtnAnimation();
        board.DOScale(new Vector3(1f,1f,1f), 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
        });
    }

    public void CloseAnimation()
    {
        Transform board = gameObject.transform.GetChild(1);
        audioManager.PlaySFX("click");
        board.DOPause();
        board.DOScale(new Vector3(0f,0f,1f), 0.2f).OnComplete(() => {
            gameObject.SetActive(false);
            SetScaleBtn();

            if(isOpenApp)
            {
                isOpenApp = false;
            } 
        });
    }

    public void PlayBtnAnimation()
    {
        Transform ListRewardTranForm = ListReward.transform;
        float delay = 0;

        ListRewardTranForm.GetChild(4).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ListRewardTranForm.GetChild(3).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        ListRewardTranForm.GetChild(5).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ListRewardTranForm.GetChild(0).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        ListRewardTranForm.GetChild(2).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ListRewardTranForm.GetChild(1).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ListRewardTranForm.GetChild(6).DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
        delay += 0.1f;
        ExitBtn.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
    }

    public void SetScaleBtn()
    {
        int num = ListReward.transform.childCount;
        for(int i = 0; i < num; i++) { ListReward.transform.GetChild(i).localScale = Vector3.zero; }
        ExitBtn.transform.localScale = Vector3.zero;
    }
}
