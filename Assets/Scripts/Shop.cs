using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    SaveDataJson saveDataJson;
    AudioManager audioManager;
    public GameObject Service;

    public TextMeshProUGUI goldInHome;
    public TextMeshProUGUI goldInShop;

    void Start()
    {
        saveDataJson = Service.GetComponent<SaveDataJson>();
        audioManager = Service.GetComponent<AudioManager>();
        SetGold();
    }

    public void SetGold()
    {
        goldInHome.text = $"{(int)saveDataJson.GetData("Gold")}";
        goldInShop.text = goldInHome.text;
    }

    public void AddMoreStuff(int gold = 0, int boom = 0, int tnt = 0, int hammer = 0, int rainbow = 0)
    {
        int Stuff = 0;

        if(gold != 0)
        {
            Stuff = (int)saveDataJson.GetData("Gold") + gold;
            saveDataJson.SaveData("Gold", Stuff);
            SetGold();
        }

        if(boom != 0)
        {
            Stuff = (int)saveDataJson.GetData("Boom") + boom;
            saveDataJson.SaveData("Boom", Stuff);
            // BoomTxt.text = $"{Stuff}";
        }

        if(tnt != 0)
        {
            Stuff = (int)saveDataJson.GetData("TNT") + tnt;
            saveDataJson.SaveData("TNT", Stuff);
            // UndoTxt.text = $"{Stuff}";
        }

        if(hammer != 0)
        {
            Stuff = (int)saveDataJson.GetData("Hammer") + hammer;
            saveDataJson.SaveData("Hammer", Stuff);
            // CompassTxt.text = $"{Stuff}";
        }

        if(rainbow != 0)
        {
            Stuff = (int)saveDataJson.GetData("Rainbow") + rainbow;
            saveDataJson.SaveData("Rainbow", Stuff);
            // FreezeTimerTxt.text = $"{Stuff}";
        }
    }

    public void AddPackage(string txt, int num)
    {
        // audioManager.PlaySFX("collect");
        switch (txt)
        {
            case "plus_gold":
                // CountCoins(5000 * num);
                AddMoreStuff(550 * num);
                break;
            case "normal_gold":
                AddMoreStuff(1100 * num);

                break;
            case "special_gold":
                AddMoreStuff(2200 * num);

                break;
            case "super_gold":
                AddMoreStuff(4400 * num);

                break;
            case "legendary_gold":
                AddMoreStuff(8800 * num);

                break;
            case "mythic_gold":
                AddMoreStuff(20000 * num);

                break;
            case "best_combo":
                AddMoreStuff(2000 * num, 1*num, 1*num, 1*num, 1*num);

                break;
            case "perfect_combo":
                AddMoreStuff(2000 * num, 3*num, 3*num, 3*num, 3*num);

                break;
            case "ultimate_combo":
                AddMoreStuff(2000 * num, 5*num, 5*num, 5*num, 5*num);

                break;
            case "sale":
                AddMoreStuff(3000);

                break;
            case "pig_coin":
                AddMoreStuff(5000);

                break;
            default:
                break;
        }
    }
}
