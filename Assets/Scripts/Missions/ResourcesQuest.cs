using UnityEngine;

public class ResourcesQuest : MonoBehaviour
{
    [SerializeField] private DialogChange main;
    [SerializeField] private int lvlArmor, lvlCore, lvlLaser, id, step;
    [SerializeField] private bool last;
    [SerializeField] private string missionName;

    private void FixedUpdate() {
        if(PlayerPrefs.GetInt("Armor") >= lvlArmor && PlayerPrefs.GetInt("Core") >= lvlCore && PlayerPrefs.GetInt("Laser") >= lvlLaser){
            main.StartNewMissionStep(missionName, step, last);
        }
    }
}
