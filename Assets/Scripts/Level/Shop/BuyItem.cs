using UnityEngine;
using UnityEngine.UI;

public class BuyItem : MonoBehaviour
{
    public Drop item;
    Text textPrice;
    Image icon;
    [SerializeField] private ShopMenuController shop;
    int lang;
    bool bound;

    void Awake()
    {
        Bind();
        TradeLook.FitFrame(GetComponent<Image>());
        TradeLook.FitIcon(icon);
    }

    void Start()
    {
        Bind();
        TradeLook.FitIcon(icon);
        Refresh();
    }

    void Bind()
    {
        if (bound)
            return;
        bound = true;
        if (transform.childCount > 1)
            textPrice = transform.GetChild(1).GetComponent<Text>();
        if (transform.childCount > 0)
            icon = transform.GetChild(0).GetComponent<Image>();
        if (icon != null && item != null)
            icon.sprite = item.icon;
        lang = PlayerPrefs.GetInt("Language");
    }

    public int ShownPrice()
    {
        if (item == null)
            return 0;
        return item.price;
    }

    public void Refresh()
    {
        Bind();
        if (textPrice == null || item == null)
            return;
        textPrice.text = ShownPrice().ToString();
    }

    public void SelectItem()
    {
        Bind();
        if (shop == null || item == null)
            return;
        shop.selected = this;
        shop.descriptionObj.SetActive(true);
        if (lang == 0)
        {
            shop.description.text = item.descriptionRu;
            shop.name.text = item.nameRu;
        }
        else
        {
            shop.description.text = item.descriptionEng;
            shop.name.text = item.nameEng;
        }
        shop.id = item.id;
        shop.price = ShownPrice();
        TradeLook.MarkSlot(this);
    }
}
