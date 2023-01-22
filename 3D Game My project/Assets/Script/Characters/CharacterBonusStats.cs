using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBonusStats : MonoBehaviour
{    
    [Space]
    [Header("Character Speed")]
    public float bonus_currentWalkSpeed;
    public float bonus_currentRunSpeed;
    //public float bonus_currentJumpPower;
    //public float bonus_currentJumpStamCost; //wpisywa� warto�ci ujemne

    [Space]
    [Header("Character Melee Combat")]
    //public float bonus_currentAttackCooldown;
    public float bonus_currentDamageCombo;
    //public float bonus_currentAttackStamCost;   //wpisywa� warto�ci ujemne
    
    [Space]
    [Header("Character Magic Combat")]
    public float bonus_Spell_Damage; //zwi�ksza� o 0.1f
    //public float bonus_breath_MPCost;  //wpisywa� warto�ci ujemne
    [Space, Header("Regen From Orbs")]
    public float percentRegen;

    [Space]
    [Header("Character Stats")]
    public float bonus_currentMaxHP;
    public float bonus_currentMaxMP;
    public float bonus_currentMaxStam;
    
    
    [Space]
    [Header("Skill Points")]
    public float bonus_SkillPoints;    

}
