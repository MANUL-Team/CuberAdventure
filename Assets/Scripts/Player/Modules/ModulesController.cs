using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulesController : MonoBehaviour
{
    public ArmorSettings _armor{
        get{
            return armor;
        }
    }
    public CoreSettings _core{
        get{
            return core;
        }
    }
    public LaserSettings _laser{
        get{
            return laser;
        }
    }
    public TracksSettings _tracks{
        get{
            return tracks;
        }
    }
    public UWSSettings _uws{
        get{
            return uws;
        }
    }
    public TurbineSettings _turbine{
        get{
            return turbine;
        }
    }
    [SerializeField] private ArmorSettings armor;
    [SerializeField] private CoreSettings core;
    [SerializeField] private LaserSettings laser;
    [SerializeField] private TracksSettings tracks;
    [SerializeField] private UWSSettings uws;
    [SerializeField] private TurbineSettings turbine;
    [SerializeField] private int modulesCount = 4;
    private bool distribution = false;
    private bool checkedE;
    public void EnergyDistribution() {
        distribution = false;
        checkedE = true;
        if(armor.isEnergy){
            modulesCount++;
        }
        core.countLastEnegry = core.countEnegry;
        armor.haveE = 0;
        laser.haveE = 0;
        tracks.haveE = 0;
        uws.haveE = 0;
        turbine.haveE = 0;
        for(int i = 1; i <= core.countEnegry; i++){
            if(armor.haveE < armor.needEnergy && armor.isEnergy && core.countLastEnegry > 0){
                armor.haveE++;
                core.countLastEnegry--;
            }
            if(laser.haveE < laser.needEnergy && core.countLastEnegry > 0){
                laser.haveE++;
                core.countLastEnegry--;
            }
            if(tracks.haveE < tracks.needEnergy && core.countLastEnegry > 0){
                tracks.haveE++;
                core.countLastEnegry--;
            }
            if(uws.haveE < uws.needEnergy && core.countLastEnegry > 0){
                uws.haveE++;
                core.countLastEnegry--;
            }
            if(turbine.haveE < turbine.needEnergy && core.countLastEnegry > 0){
                turbine.haveE++;
                core.countLastEnegry--;
            }
        }
    }
    public void CheckAllModules(){
        armor.CheckModule();
        laser.CheckModule();
        tracks.CheckModule();
        uws.CheckModule();
        turbine.CheckModule();
        core.CheckModule();
    }
    private void Update() {
        if(distribution){
            EnergyDistribution();
        }
        if(armor.ready && laser.ready && tracks.ready && uws.ready && turbine.ready && core.ready && !checkedE){
            distribution = true;
        }
    }
}
