using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CharInfo", menuName = "Chars/CharInfo")]
public class ScrObj_charStats : ScriptableObject
{
    [Header("Character Info")]
    public string charName;
    //public int charLevel;
    [Space]
    [Header("Character Speed")]
    public float moveSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float jumpPower;
    public float jumpStamCost;
    public float JumpDeltaTime;
    public float gravity;

    [Space]
    [Header("Booleany/Vectory z klasy Input")]
    public float inputRotateSensivity;

    [Space]
    [Header("Zmienne z NavMeshAgenta")]
    public float navMeAge_attackRange;
    public float navMeAge_wanderingRange;
    public float navMeAge_walkPointRange;
    public LayerMask navMeAge_whatIsGround;    
    public float navMeAge_patrollingDelay; 
    public float navMeAge_AIRoutineDelay;

    [Space]
    [Header("Field of View NavMeshAgent")]
    [Range(0, 360)] public float fov_MaxSightAngle;
    [Range(0, 360)] public float fov_MinSightAngle;
    [Range(0, 1)] public float fov_TimeDynamicSightAngle;
    [Range(0, 100)] public float fov_MinSightRadius;
    [Range(0, 100)] public float fov_MaxSightRadius;
    [Range(0, 5)] public float fov_TimeDynamicSightRadius;
    [Range(0, 10)] public float fov_editorLineThickness;
    [ColorUsageAttribute(true, true)] public Color fov_editorAngleLineColor; //kolor HDR picker
    [ColorUsageAttribute(true, true)] public Color fov_editorAngleColor; //kolor HDR picker
    [ColorUsageAttribute(true, true)] public Color fov_editorRadiusColor;
    [ColorUsageAttribute(true, true)] public Color fov_editorDynamicRadiusColor;
    [ColorUsageAttribute(true, true)] public Color fov_editorRaycastColor;
    public float fov_coneRoutineDelay;
    public LayerMask fov_obstaclesLayerMask;      

    [Space]
    [Header("Character GroundMask")]
    public LayerMask groundMask;
    public float groundDistance;

    [Space]
    [Header("Character Stats")]
    public float baseHP;    
    public float regenHP;
    public float HP_Multiplier;
    [Space]
    public float baseMP;    
    public float regenMP;
    public float MP_Multiplier;
    [Space]
    public float baseStam;
    public float regenStam;
    public float Stam_Multiplier;
    [Space]
    public float baseNeedXP;
    public float XP_Multiplier;
    public float XP_GainedFromKill;
    [Space]
    public float corpseTime;
    public float respawnTime;

    [Space]
    [Header("Character Combat")]
    public float attackCooldown;

    [Space]
    public float baseDamageCombo;
    public float MultiplierDamageCombo;
    public float attackStamCost;
    [Space]
    [ColorUsageAttribute(true, true)] public Color spell_editorAISpellRadiusColor; //kolor HDR picker
    [ColorUsageAttribute(true, true)] public Color spell_breathAngleColor;
    [ColorUsageAttribute(true, true)] public Color spell_breathRaycastColor;
    public float baseSpell_Damage;
    public float MultiplierSpell_Damage;
    public float baseSpell_MPCost;


    [Space]
    [Header("Target Audio")]    
    public AudioClip damagedEnemy;
    public AudioClip damagedDestructibles;

}
