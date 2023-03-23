using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSettings : MonoBehaviour
{
    [SerializeField] private CraftedItem module;
    [SerializeField] private UsedModules controller;
    public CraftedItem _module{
        get{
            return module;
        }
    }
    [SerializeField] private Image icon;
    public Image background;
    private void Start() {
        icon.sprite = module.icon;
    }
    public void Select(){
        controller.SelectModule(module);
    }
}
