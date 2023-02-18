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
    [Tooltip("Zasiêg w jakim mo¿e ustawiæ walkPoint od origina (_spawnPoint)")] public float navMeAge_wanderingRange;
    [Tooltip("Zasiêg w jakim wylicza nowy walkPoint (-/+)")] public float navMeAge_walkPointRange;
    [Tooltip("LayerMask do Raycasta - Co traktuje jako pod³o¿e?")] public LayerMask navMeAge_whatIsGround;
    [Tooltip("Nie ustawione (Mia³ byæ delay - jeœli nie dojdzie do _walkPoint w okreœlonym czasie to reset")] public float navMeAge_patrollingDelay;
    [Tooltip("OpóŸnienie _AIRoutine w Sekndach - im wiêcej tym ³atwiej dla kompa"), Range(0f, 0.3f)] public float navMeAge_AIRoutineDelay;

    [Space]
    [Header("Field of View NavMeshAgent")]
    [Tooltip("Min DynamicSightAngle"), Range(0, 360)] public float fov_MinSightAngle;
    [Tooltip("Max DynamicSightAngle"), Range(0, 360)] public float fov_MaxSightAngle;
    [Tooltip("ile sekund na ca³y ruch (MAX -> MAX)"), Range(0, 1)] public float fov_TimeDynamicSightAngle;
    [Tooltip("Min DynamicSightRadius"), Range(0, 20)] public float fov_MinSightRadius;
    [Tooltip("Max DynamicSightRadius"), Range(0, 20)] public float fov_MaxSightRadius;
    [Tooltip("ile sekund na ca³y ruch (MAX -> MAX)"), Range(0, 5)] public float fov_TimeDynamicSightRadius;
    [Tooltip("Gruboœæ lini Gizmos"), Range(0, 10)] public float fov_editorLineThickness;
    [Tooltip("Kolor Gizmos - AngleLineColor (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color fov_editorAngleLineColor; //kolor HDR picker
    [Tooltip("Kolor Gizmos - AngleColor (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color fov_editorAngleColor; //kolor HDR picker
    [Tooltip("Kolor  Gizmos - MaxRadius"), ColorUsageAttribute(true, true)] public Color fov_editorRadiusColor;
    [Tooltip("Kolor  Gizmos - DynamicRadius"), ColorUsageAttribute(true, true)] public Color fov_editorDynamicRadiusColor;
    [Tooltip("Kolor  Gizmos - Raycast"), ColorUsageAttribute(true, true)] public Color fov_editorRaycastColor;
    [Tooltip("OpóŸnienie _FoVRoutine w Sekndach - im wiêcej tym ³atwiej dla kompa (Wy³¹czone przy AIController)"), Range(0f, 0.3f)] public float fov_coneRoutineDelay;
    [Tooltip("Obstacles LayerMask dla FieldOfView")] public LayerMask fov_obstaclesLayerMask;      

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
