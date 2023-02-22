using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    [SerializeField] private GameObject consoleMenu;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text text;
    private string[] wordsInCommand;
    [SerializeField] private PlayerStats stats;
    [SerializeField] private TeleportScript tp;
    public void OpenCloseConsole(){
        consoleMenu.SetActive(!consoleMenu.activeSelf);
    }
    public void EnterCommand(){
        wordsInCommand = inputField.text.Split(" ");
        if(wordsInCommand[0] == "give"){
            if(wordsInCommand[1] == "Weapon" || wordsInCommand[1] == "Gusenici" || wordsInCommand[1] == "Turbine" || wordsInCommand[1] == "Laser" || wordsInCommand[1] == "Armor"){
                PlayerPrefs.SetInt(wordsInCommand[1] + wordsInCommand[2], 1);
                text.text = text.text + "\nModule gived!";
            }
            else{
                if(wordsInCommand.Length == 1){
                    PlayerPrefs.SetInt("Item" + " " + wordsInCommand[1], PlayerPrefs.GetInt("Item" + " " + wordsInCommand[1]) + 1);
                    text.text = text.text + "\nItem gived!";
                }
                else{
                    PlayerPrefs.SetInt("Item" + " " + wordsInCommand[1], PlayerPrefs.GetInt("Item" + " " + wordsInCommand[1]) + int.Parse(wordsInCommand[2]));
                    text.text = text.text + "\nItem gived!";
                }
            }
        }
        else if(wordsInCommand[0] == "heal"){
            stats.hp = stats.maxHp;
            text.text = text.text + "\nHealled!";
        }
        else if(wordsInCommand[0] == "tp"){
            tp.currentId = int.Parse(wordsInCommand[1]);
            tp.Teleport();
            text.text = text.text + "\nTeleported!";
        }
        else if(wordsInCommand[0] == "setlevel"){
            PlayerPrefs.SetInt("PlayerLevel", int.Parse(wordsInCommand[1]));
            stats.LoadStats();
            text.text = text.text + "\nLevel setted!";
        }
        else if(wordsInCommand[0] == "time"){
            PlayerPrefs.SetInt("Time", int.Parse(wordsInCommand[1]));
            text.text = text.text + "\nTime setted!";
        }




        else{
            text.text = text.text + "\nUnknown command!";
        }
    }
}
