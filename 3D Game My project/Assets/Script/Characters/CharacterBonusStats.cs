using UnityEngine;

public class CharacterBonusStats : MonoBehaviour
{    
    [Space]
    [Header("Character Speed")]
    [Tooltip("Bonus do WalkSpeed")] public float bonus_currentWalkSpeed;
    [Tooltip("Bonus do RunSpeed - wylicznay z walkSpeed")] public float bonus_currentRunSpeed;

    [Space]
    [Header("Character Melee Combat")]
    [Tooltip("Bonus Damage CloseRangeSkilla")] public float bonus_CloseRangeDamage;
    
    [Space]
    [Header("Character Magic Combat")]
    [Tooltip("Bonus Damage SpellRangeSkilla")] public float bonus_SpellRangeDamage; //zwiêkszaæ o 0.1f
    [Space, Header("Regen From Orbs")]
    [Tooltip("Ile MaxResource zostanie zwrócone po podniesieniu ResourceOrba"), Range(0f, 1f)] public float percentRegen;

    [Space]
    [Header("Character Stats")]
    [Tooltip("Bonus do Max HP")] public float bonus_currentMaxHP;
    [Tooltip("Bonus do Regenu HP")] public float bonus_currentRegenHP;

    [Tooltip("Bonus do Max MP")] public float bonus_currentMaxMP;
    [Tooltip("Bonus do Regenu MP")] public float bonus_currentRegenMP;

    [Tooltip("Bonus do Max Staminy")] public float bonus_currentMaxStam;
    [Tooltip("Bonus do Regenu Staminy")] public float bonus_currentRegenStam;

    [Space]
    [Header("Skill Points")]
    [Tooltip("Ile jest dostêpnych SkillPoints?")] public float bonus_SkillPoints;    

}
