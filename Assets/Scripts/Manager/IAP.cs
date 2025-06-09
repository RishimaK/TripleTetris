using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

[Serializable]
public class ConsumableItem{
    // public string Name;
    public string Id;
    // public string description;
    // public float price;
}

[Serializable]
public class NonConsumableItem{
    // public string Name;
    public string Id;
    // public string description;
    // public float price;
}

[Serializable]
public class SubscriptionItem{
    // public string Name;
    public string Id;
    // public string description;
    // public float price;
    // public int timeDuration; // in Day
} 


public class IAP : MonoBehaviour, IDetailedStoreListener
{
    AudioManager audioManager;
    AdsManager adsManager;
    SaveDataJson saveDataJson;

    IStoreController m_StoreContoller;

    public ConsumableItem PlusGold;
    public ConsumableItem NormalGold;
    public ConsumableItem SpecialGold;
    public ConsumableItem SuperGold;
    public ConsumableItem LegendaryGold;
    public ConsumableItem MythicGold;

    public ConsumableItem BestCombo;
    public ConsumableItem PerfectCombo;
    public ConsumableItem UltimateCombo;

    public ConsumableItem Sale;
    public ConsumableItem PigCoin;

    public NonConsumableItem removeAds;
    // public SubscriptionItem sItem;
    // public GameObject AdsPurchasedWindow;
    public Shop shop;

    public Data data;
    public Payload payload;
    public PayloadData payloadData;

    public TextMeshProUGUI PlusGoldTxt;
    public TextMeshProUGUI NormalGoldTxt;
    public TextMeshProUGUI SpecialGoldTxt;
    public TextMeshProUGUI SuperGoldTxt;
    public TextMeshProUGUI LegendaryGoldTxt;
    public TextMeshProUGUI MythicGoldTxt;
    public TextMeshProUGUI BestComboTxt;
    public TextMeshProUGUI PerfectComboTxt;
    public TextMeshProUGUI UltimateComboTxt;
    public TextMeshProUGUI SaleTxt;
    public TextMeshProUGUI PigCoinTxt;
    public TextMeshProUGUI RemoveAdsTxt;

    // private bool allowToShowShopBanner = true;

    // public GameObject BtrRemoveAdsInShop;
    // public GameObject BtrRemoveAdsInSetting;

    public GameObject RemoveAdsBtn;
    public GameObject SaleBtn;
    public Sale SaleDialog;
    public PiggyBank piggyBank;
    // public RemoveAds removeAds;
    // private string exchangeRateApiUrl = "https://api.exchangerate-api.com/v4/latest/USD";

    void Start()
    {
        audioManager = GetComponent<AudioManager>();
        adsManager = GetComponent<AdsManager>();
        saveDataJson = GetComponent<SaveDataJson>();
        SetupBuilder();
    }

    public void ConnumableBtn(string val)
    {
        // audioManager.PlaySFX("click");
        if(val == PlusGold.Id) m_StoreContoller.InitiatePurchase(PlusGold.Id);
        else if(val == NormalGold.Id) m_StoreContoller.InitiatePurchase(NormalGold.Id);
        else if(val == SpecialGold.Id) m_StoreContoller.InitiatePurchase(SpecialGold.Id);
        else if(val == SuperGold.Id) m_StoreContoller.InitiatePurchase(SuperGold.Id);
        else if(val == LegendaryGold.Id) m_StoreContoller.InitiatePurchase(LegendaryGold.Id);
        else if(val == MythicGold.Id) m_StoreContoller.InitiatePurchase(MythicGold.Id);

        else if(val == BestCombo.Id) m_StoreContoller.InitiatePurchase(BestCombo.Id);
        else if(val == PerfectCombo.Id) m_StoreContoller.InitiatePurchase(PerfectCombo.Id);
        else if(val == UltimateCombo.Id) m_StoreContoller.InitiatePurchase(UltimateCombo.Id);

        else if(val == Sale.Id) m_StoreContoller.InitiatePurchase(Sale.Id);
        else if(val == PigCoin.Id) m_StoreContoller.InitiatePurchase(PigCoin.Id);

    }

    public void NonConnumableBtn()
    {
        m_StoreContoller.InitiatePurchase(removeAds.Id);
    }

    public void Subscription()
    {
        // m_StoreContoller.InitiatePurchase(sItem.Id);
    }

    void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(PlusGold.Id, ProductType.Consumable);
        builder.AddProduct(NormalGold.Id, ProductType.Consumable);
        builder.AddProduct(SpecialGold.Id, ProductType.Consumable);
        builder.AddProduct(SuperGold.Id, ProductType.Consumable);
        builder.AddProduct(LegendaryGold.Id, ProductType.Consumable);
        builder.AddProduct(MythicGold.Id, ProductType.Consumable);

        builder.AddProduct(BestCombo.Id, ProductType.Consumable);
        builder.AddProduct(PerfectCombo.Id, ProductType.Consumable);
        builder.AddProduct(UltimateCombo.Id, ProductType.Consumable);

        builder.AddProduct(Sale.Id, ProductType.Consumable);
        builder.AddProduct(PigCoin.Id, ProductType.Consumable);
    
        builder.AddProduct(removeAds.Id, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);
    }

    void CheckNonConsumable(string id) {

        if (m_StoreContoller!=null)
        {
            var product = m_StoreContoller.products.WithID(id);
            if (product!=null)
            { 
                if (product.hasReceipt || (bool)saveDataJson.GetData("RemoveAds"))//purchased
                {
                    RemoveAds();
                }
                else {
                    ShowAds();
                }
            }
        }
    }

    public void RemoveAds()
    {
        DisplayAds(false);
    }

    void ShowAds()
    {
        DisplayAds(true);
    }

    void DisplayAds(bool x)
    {
        if (!x)
        {
            // StartCoroutine(CloseShopBanner(0));

            // AdLoadedNewArea.transform.parent.gameObject.SetActive(false);
            // AdLoadedSetting.transform.parent.gameObject.SetActive(false);
            // AdLoadedSmallShop.transform.parent.gameObject.SetActive(false);
            adsManager.DestroyBannerAd();

            RemoveAdsBtn.GetComponent<Button>().interactable = false;
            SaleBtn.SetActive(false);
            if(SaleDialog.gameObject.activeSelf) SaleDialog.CloseDialog();
            // BtrRemoveAdsInSetting.GetComponent<Image>().color = new Color(0.372f, 0.372f, 0.372f);
            // BtrRemoveAdsInSetting.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.568f, 0.568f, 0.568f);
            // BtrRemoveAdsInShop.GetComponent<Image>().color = new Color(0.372f, 0.372f, 0.372f);
            // BtrRemoveAdsInShop.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0.568f, 0.568f, 0.568f);
            // BtrRemoveAdsInSetting.GetComponent<Button>().interactable = false;
            // BtrRemoveAdsInShop.GetComponent<Button>().interactable = false;


            // if(removeAds.gameObject.activeSelf) removeAds.Exit();
        }
        else
        {
            saveDataJson.SaveData("RemoveAds", false);
        }
    }

    void ActivateElitePass()
    {
        setupElitePass(true);
    }

    void DeActivateElitePass()
    {
        setupElitePass(false);
    }

    string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
    }

    void setupElitePass(bool x)
    {
        if (x)// active
        {
            // shop.ExitLegendarySub();
            // home.CheckUnlockMap();
            // RemoveAds();
            
            // DateTime currentTime = DateTime.Now;
            // string todayLegendarySub = (string)saveDataJson.GetData("TodayLegendarySUB");
            // BtnSub.SetActive(false);
            // if(todayLegendarySub == "" || DateTime.ParseExact(todayLegendarySub, "dd/MM/yyyy HH:mm:ss", null).Date < currentTime.Date)
            // {
            //     string todayLegendarySubstring = FormatDateTime(currentTime);
            //     saveDataJson.SaveData("TodayLegendarySUB", todayLegendarySubstring);
            //     shop.AddHint(sItem.Id);
            // }

        }
        else
        {
            // saveDataJson.SaveData("LegendarySUB", false);
            // saveDataJson.SaveData("TodayLegendarySUB", "");
            // saveDataJson.SaveData("OpenAllMaps", false);
            // LoadAds();
        }
    }

    void CheckSubscription(string id) {
        var subProduct = m_StoreContoller.products.WithID(id);
        if (subProduct != null)
        {
            try
            {
                if (subProduct.hasReceipt)
                {
                    var subManager = new SubscriptionManager(subProduct, null);
                    var info = subManager.getSubscriptionInfo();

                    if (info.isSubscribed() == Result.True)
                    {
                        print("We are subscribed");
                        ActivateElitePass();
                    }
                    else {
                        print("Un subscribed");
                        DeActivateElitePass();
                    }

                }
                else{
                    print("receipt not found !!");
                    DeActivateElitePass();
                }
            }
            catch (Exception)
            {
                print("It only work for Google store, app store, amazon store, you are using fake store!!");
            }
        }
        else {
            print("product not found !!");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogWarning($"Purchase failed: {product.definition.id}. Reason: {failureDescription.message}");
        // throw new NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogWarning($"IAP Initialization failed: {error}");
        // throw new NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogWarning($"IAP Initialization failed: {error}. Message: {message}");
        // throw new NotImplementedException();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        // Debug.LogWarning("?????????????????????????");
        var product = purchaseEvent.purchasedProduct;
        // Debug.LogWarning(product);
        // Debug.LogWarning("//////////////////////");
        // Debug.LogWarning(product.definition.id);
        

        if (product.definition.id == removeAds.Id)//non consumable
        {
            saveDataJson.SaveData("RemoveAds", true);
            RemoveAds();
        }
        // else if (product.definition.id == sItem.Id)//subscribed
        // {
        //     saveDataJson.SaveData("LegendarySUB", true);
        //     saveDataJson.SaveData("OpenAllMaps", true);
        //     ActivateElitePass();
        // }
        else
        // if (product.definition.id == PlusGold.Id || product.definition.id == NormalGold.Id || product.definition.id == SpecialGold.Id ||
        //         product.definition.id == SuperGold.Id || product.definition.id == LegendaryGold.Id) 
        {
            string receipt = product.receipt;
            data = JsonUtility.FromJson<Data>(receipt);
            int quantity = 1;

            #if UNITY_ANDROID
            if (data.Payload != "ThisIsFakeReceiptData")
            {
                payload = JsonUtility.FromJson<Payload>(data.Payload);
                payloadData = JsonUtility.FromJson<PayloadData>(payload.json);
                quantity = payloadData.quantity;
            }
            #endif

            for (int i = 0; i < quantity; i++)
            {
                shop.AddPackage(product.definition.id, quantity);
                if (product.definition.id == "sale")
                {
                    saveDataJson.SaveData("RemoveAds", true);
                    RemoveAds();
                }
                else if (product.definition.id == "pig_coin")
                {
                    piggyBank.GetReward();
                }
            }
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogWarning($"Purchase failed: {product.definition.id}. Reason: {failureReason}");
        // throw new NotImplementedException();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreContoller = controller;
        CheckNonConsumable(removeAds.Id);
        // CheckSubscription(sItem.Id);

        ConvertPriceToLocalCurrency(PlusGold);
        ConvertPriceToLocalCurrency(NormalGold);
        ConvertPriceToLocalCurrency(SpecialGold);
        ConvertPriceToLocalCurrency(SuperGold);
        ConvertPriceToLocalCurrency(LegendaryGold);
        ConvertPriceToLocalCurrency(MythicGold);

        ConvertPriceToLocalCurrency(BestCombo);
        ConvertPriceToLocalCurrency(PerfectCombo);
        ConvertPriceToLocalCurrency(UltimateCombo);

        ConvertPriceToLocalCurrency(Sale);
        ConvertPriceToLocalCurrency(PigCoin);

        ConvertPriceToLocalCurrency(removeAds);
    }

    void ConvertPriceToLocalCurrency(ConsumableItem item)
    {
        Product product = m_StoreContoller.products.WithID(item.Id);
        if(product == null) return;
        ProductMetadata metadata = product.metadata;
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        RegionInfo region = new RegionInfo(currentCulture.Name);

        string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
        if(item == PlusGold) PlusGoldTxt.text = txt;
        else if(item == NormalGold) NormalGoldTxt.text = txt;
        else if(item == SpecialGold) SpecialGoldTxt.text = txt;
        else if(item == SuperGold) SuperGoldTxt.text = txt;
        else if(item == LegendaryGold) LegendaryGoldTxt.text = txt;
        else if(item == MythicGold) MythicGoldTxt.text = txt;

        else if(item == BestCombo) BestComboTxt.text = txt;
        else if(item == PerfectCombo) PerfectComboTxt.text = txt;
        else if(item == UltimateCombo) UltimateComboTxt.text = txt;

        else if(item == Sale) SaleTxt.text = txt;
        else if(item == PigCoin) PigCoinTxt.text = txt;
    }

    void ConvertPriceToLocalCurrency(NonConsumableItem item)
    {
        Product product = m_StoreContoller.products.WithID(item.Id);
        if(product == null) return;
        ProductMetadata metadata = product.metadata;
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        RegionInfo region = new RegionInfo(currentCulture.Name);

        string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
        RemoveAdsTxt.text = txt;
    }

    void ConvertPriceToLocalCurrency(SubscriptionItem item)
    {
        Product product = m_StoreContoller.products.WithID(item.Id);
        if(product == null) return;
        ProductMetadata metadata = product.metadata;
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        RegionInfo region = new RegionInfo(currentCulture.Name);

        string txt = $"{metadata.localizedPrice}{region.CurrencySymbol}";
        // if(item == sItem) txtsItem.text = txt;
    }

    [Serializable]
    public class SkuDetails
    {
        public string productId;
        public string type;
        public string title;
        public string name;
        public string iconUrl;
        public string description;
        public string price;
        public long price_amount_micros;
        public string price_currency_code;
        public string skuDetailsToken;
    }

    [Serializable]
    public class PayloadData
    {
        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string purchaseToken;
        public int quantity;
        public bool acknowledged;
    }

    [Serializable]
    public class Payload
    {
        public string json;
        public string signature;
        public List<SkuDetails> skuDetails;
        public PayloadData payloadData;
    }

    [Serializable]
    public class Data
    {
        public string Payload;
        public string Store;
        public string TransactionID;
    }
}