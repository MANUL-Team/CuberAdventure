using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsedModules : MonoBehaviour
{
    [SerializeField] private ButtonSettings[] modules;
    [SerializeField] private ModulesController controller;
    [SerializeField] private Text name, description, stats, selectText;
    [SerializeField] private GameObject[] typesOfModules;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private Animator cuber;
    private string type, id;
    private CraftedItem currentModule;
    public string _type{
        get{
            return type;
        }
    }
    public string _id{
        get{
            return id;
        }
    }
    public void ModulesCheck(){
        for(int i = 0; i < modules.Length; i++){
            if(PlayerPrefs.GetInt(modules[i]._module.module.ToString() + modules[i]._module.id.ToString()) == 0 && modules[i]._module.id != 0){
                modules[i].background.color = new Color(255/255f, 75/255f, 75/255f);
            }
            else{
                if(PlayerPrefs.GetInt(modules[i]._module.module.ToString()) == modules[i]._module.id){
                    modules[i].background.color = new Color(255/255f, 255/255f, 75/255f);
                }
                else{
                    modules[i].background.color = new Color(75/255f, 255/255f, 75/255f);
                }
            }
        }
    }
    private void Start() {
        ModulesCheck();
    }
    public void SelectModule(CraftedItem module){
        currentModule = module;
        name.text = module.nameRu;
        description.text = module.descriptionRu;
        stats.text = module.statsRu;
        type = module.module;
        id = module.id.ToString();
        if(PlayerPrefs.GetInt(module.module.ToString() + module.id) != 0 || module.id == 0){
            selectButton.SetActive(true);
            if(PlayerPrefs.GetInt(module.module.ToString()) == module.id){
                if(PlayerPrefs.GetInt("Language") == 0){
                    selectButton.GetComponent<Image>().color = new Color(50f/255f, 50/255f, 50f/255f);
                    selectText.text = "Selected";
                }
                else if(PlayerPrefs.GetInt("Language") == 1){
                    selectButton.GetComponent<Image>().color = new Color(50f/255f, 50/255f, 50f/255f);
                    selectText.text = "Выбрано";
                }
            }
            else if(PlayerPrefs.GetInt(module.module.ToString()) != module.id){
                if(PlayerPrefs.GetInt("Language") == 0){
                    selectButton.GetComponent<Image>().color = new Color(75f/255f, 73f/255f, 71f/255f);
                    selectText.text = "Select";
                }
                else if(PlayerPrefs.GetInt("Language") == 1){
                    selectButton.GetComponent<Image>().color = new Color(75f/255f, 73f/255f, 71f/255f);
                    selectText.text = "Выбрать";
                }
            }
        }
        else{
            selectButton.SetActive(false);
        }
        ModulesCheck();
    }
    public void ChangeModule(){
        PlayerPrefs.SetInt(type, int.Parse(id));
        SelectModule(currentModule);
        ModulesCheck();
    }
    public void GiveModule(){
        PlayerPrefs.SetInt(type + id, 1);
        ModulesCheck();
    }
    public void SelectTypeOfModule(int id){
        for(int i = 0; i < typesOfModules.Length; i++){
            if(i != id){
                typesOfModules[i].SetActive(false);
            }
            else{
                typesOfModules[i].SetActive(true);
            }
        }
        if(id == 0 || id == 1){
            cuber.SetBool("Transperency", true);
        }
        else{
            cuber.SetBool("Transperency", false);
        }
    }
    public void TransperencyOff(){
        cuber.SetBool("Transperency", false);
    }
}