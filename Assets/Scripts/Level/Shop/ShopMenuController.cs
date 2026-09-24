using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuController : MonoBehaviour
{
    [SerializeField] private List<BuyItem> items = new List<BuyItem>();
    public int price;
    public int id;
    [SerializeField] private Slider slider;
    [SerializeField] private Text textSlider;
    [SerializeField] private GameObject scale;
    [SerializeField] private Transform itemsFolder;
    [SerializeField] private GameObject successful, fail;
    public GameObject descriptionObj;
    public Text description, name;
    public bool sell;
    public BuyItem selected;
    public Slider Amount => slider;
    public Text Counter => textSlider;
    Text payoutLine;
    Text dealTitle;

    public void BindPayout(Text text)
    {
        payoutLine = text;
    }

    public void BindTitle(Text text)
    {
        dealTitle = text;
    }

    public void NudgeUp()
    {
        Nudge(1);
    }

    public void NudgeDown()
    {
        Nudge(-1);
    }

    void Nudge(int delta)
    {
        if (slider == null)
            return;
        slider.value = Mathf.Clamp(slider.value + delta, slider.minValue, slider.maxValue);
        RefreshDealText();
    }
    bool ready;

    void Start()
    {
        TradeLook.PaintShopMode(this);
    }

    void OnEnable()
    {
        if (fail != null)
            fail.SetActive(false);
        if (successful != null)
            successful.SetActive(false);
        if (scale != null)
            scale.SetActive(false);
        if (!ready)
        {
            items.Clear();
            if (itemsFolder != null)
            {
                for (int i = 0; i < itemsFolder.childCount; i++)
                {
                    BuyItem item = itemsFolder.GetChild(i).GetComponent<BuyItem>();
                    if (item != null)
                        items.Add(item);
                }
            }
            TradeLook.ApplyShop(this);
            ready = true;
        }
        RefreshItems();
        TradeLook.PaintShopMode(this);
        TradeLook.Fade(transform.parent);
    }

    public void SetSelling(bool value)
    {
        sell = value;
        RefreshItems();
        if (selected != null)
            selected.SelectItem();
        TradeLook.PaintShopMode(this);
    }

    public void ChangeCount()
    {
        sell = false;
        OpenDeal();
    }

    public void ChangeCountForSell()
    {
        sell = true;
        OpenDeal();
    }

    public void BuyItem()
    {
        ConfirmDeal();
    }

    public void SellItem()
    {
        ConfirmDeal();
    }

    void OpenDeal()
    {
        if (sell)
        {
            int owned = PlayerPrefs.GetInt("Item " + id);
            if (price <= 0 || owned <= 0)
            {
                ShowFail(false);
                return;
            }
            scale.SetActive(true);
            scale.transform.SetAsLastSibling();
            slider.minValue = 1f;
            slider.maxValue = owned;
            slider.value = 1f;
            TradeLook.PaintShopMode(this);
            RefreshDealText();
            return;
        }
        int unit = UnitPrice();
        if (price <= 0 || unit <= 0)
        {
            ShowFail(true);
            return;
        }
        int coins = PlayerPrefs.GetInt("Coins");
        if (coins < unit)
        {
            ShowFail(true);
            return;
        }
        scale.SetActive(true);
        scale.transform.SetAsLastSibling();
        slider.minValue = 1f;
        slider.maxValue = Mathf.Max(1, Mathf.FloorToInt(coins / (float)unit));
        slider.value = 1f;
        TradeLook.PaintShopMode(this);
        RefreshDealText();
    }

    void RefreshDealText()
    {
        if (slider == null)
            return;
        int count = Mathf.Max(0, Mathf.RoundToInt(slider.value));
        if (textSlider != null)
            textSlider.text = count + " / " + Mathf.RoundToInt(slider.maxValue);
        bool ru = PlayerPrefs.GetInt("Language") == 0;
        if (dealTitle != null)
            dealTitle.text = ru ? "Количество" : "Amount";
        PaintConfirm(ru);
        if (payoutLine == null)
            return;
        int total = count * UnitPrice();
        if (sell)
            payoutLine.text = ru ? "Получите: " + total : "You receive: " + total;
        else
            payoutLine.text = ru ? "Цена: " + total : "Price: " + total;
    }

    void PaintConfirm(bool ru)
    {
        if (scale == null)
            return;
        Transform button = scale.transform.Find("BuyButton");
        if (button == null)
            button = scale.transform.Find("SellButton");
        if (button == null)
            return;
        Text text = button.GetComponentInChildren<Text>(true);
        if (text == null || text == dealTitle)
            return;
        text.enabled = true;
        text.text = sell ? (ru ? "Продать" : "Sell") : (ru ? "Купить" : "Buy");
    }

    void ShowFail(bool noMoney)
    {
        if (fail == null)
            return;
        fail.SetActive(true);
        fail.transform.SetAsLastSibling();
        Text text = null;
        Text[] texts = fail.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].GetComponentInParent<Button>() != null)
                continue;
            text = texts[i];
            break;
        }
        if (text != null)
        {
            bool ru = PlayerPrefs.GetInt("Language") == 0;
            if (noMoney)
                text.text = ru ? "Недостаточно денег!" : "You don't have enough money!";
            else
                text.text = ru ? "Этого предмета нет." : "You don't have this item.";
        }
    }

    void ConfirmDeal()
    {
        int count = Mathf.RoundToInt(slider.value);
        if (count <= 0 || price <= 0)
            return;
        string key = "Item " + id;
        int unit = UnitPrice();
        if (!sell)
        {
            int cost = count * unit;
            if (PlayerPrefs.GetInt("Coins") < cost)
            {
                ShowFail(true);
                return;
            }
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") - cost);
            PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + count);
        }
        else
        {
            int owned = PlayerPrefs.GetInt(key);
            if (count > owned)
                count = owned;
            if (count <= 0)
            {
                ShowFail(false);
                return;
            }
            PlayerPrefs.SetInt(key, owned - count);
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + count * unit);
        }
        if (successful != null)
        {
            successful.SetActive(true);
            successful.transform.SetAsLastSibling();
        }
        if (scale != null)
            scale.SetActive(false);
        RefreshItems();
    }

    int UnitPrice()
    {
        if (price <= 0)
            return 0;
        if (!sell)
            return price;
        return Mathf.Max(1, Mathf.RoundToInt(price * 0.8f));
    }

    public void RefreshItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
                items[i].Refresh();
        }
    }

    void FixedUpdate()
    {
        if (scale != null && scale.activeSelf)
            RefreshDealText();
    }
}
