using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CharacterStatus : MonoBehaviour
{
    [Space]
    [Header("Character Info")]
    public string currentCharName;
    public int currentCharLevel;
    [TagField] public string[] currentEnemiesArray; //Pozwala na wybór Enemies przy pomocy Tag !!!nie ustawiaæ Environment!!!
    public bool playerInputEnable;
    public bool isPlayer;


    [Space]    
    [Header("Current Char Status")]
    public bool isMoving = false;
    public bool isGrounded = false;
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isRunning = false;
    public bool isWalking = false;
    public bool isIdle = false;
    public bool isDead = false;
    public bool isCasting = false;

    [Space]
    [Header("Booleany/Vectory z klasy Input")]
    public Vector3 inputMovingVector;
    public float inputRotateHorizontal;
    public bool inputMoving;
    public bool inputRunning;
    public bool inputJumping;
    public bool inputPrimary;
    public float inputRotateSensivity;
    public bool inputEnableMouseRotate;
    public bool inputMouseCurrentMoving;
    public bool inputSecondary;

    [Space]
    public bool inputSaveGame;
    public bool inputLoadGame;
    public bool inputResetPosition;
    public bool inputSetBackupPosition;
    public float inputMouseScroll;
    public bool Testing_Z_Key;

    [Space]
    [Header("Zmienne z NavMeshAgenta")]
    public float navMeAge_wanderingRange;    
    public Vector3 navMeAge_spawnPoint;
    public Vector3 navMeAge_walkPoint;
    public Vector3 navMeAge_mouseWalkPoint;    
    public bool navMeAge_walkPointSet;
    public float navMeAge_walkPointRange;
    public LayerMask navMeAge_whatIsGround;    
    public int navMeAge_failsafeCounter;
    public float navMeAge_patrollingDelay;
    public float navMeAge_AIRoutineDelay;
    public bool navMeAge_isCheckingAIRoutine;

    [Space]
    [Header("Field of View NavMeshAgent")]
    [Space]
    [Header("Angle")]
    [Range(0, 360)] public float fov_CurrentDynamicSightAngle;
    [Range(0, 360)] public float fov_MaxSightAngle;
    [Range(0, 360)] public float fov_MinSightAngle;
    public float fov_CurrentVectorDynamicSightAngle;   //wektor aktualnej zmiany wartoœci +/-

    [Space]
    [Header("Radius")]
    [Range(0, 1)] public float fov_TimeDynamicSightAngle; //ile sekund na ca³y ruch (90->360)
    [Range(0, 100)] public float fov_MinSightRadius;
    [Range(0, 100)] public float fov_MaxSightRadius;
    [Range(0, 100)] public float fov_CurrentDynamicSightRadius;    
    public float fov_lerpedDistance;    
    public float fov_CurrentVectorDynamicSightRadius;   //wektor aktualnej zmiany wartoœci +/-
    [Range(0, 5)] public float fov_TimeDynamicSightRadius;

    [Space]
    [Header("GizmosColor")]    
    [Range(0, 10)] public float fov_editorLineThickness;
    [ColorUsageAttribute(true,true)] public Color fov_editorAngleLineColor; //kolor HDR picker
    [ColorUsageAttribute(true, true)] public Color fov_editorAngleColor; //kolor HDR picker
    [ColorUsageAttribute(true, true)] public Color fov_editorRadiusColor;
    [ColorUsageAttribute(true, true)] public Color fov_editorDynamicRadiusColor;
    [ColorUsageAttribute(true, true)] public Color fov_editorRaycastColor;

    [Space]
    [Header("Range")]
    public bool fov_targetInAttackRange;
    public float fov_attackRange;
    public bool fov_targetInDynamicSightRange;
    public bool fov_inSpellkRange;

    [Space]
    [Header("Utils")]
    public Collider[] fov_allTargetsInDynamicSightRange;
    [Space]
    public List<Collider> fov_enemyTargetsInDynamicSightRange;
    [Space]
    public float fov_RoutineDelay;
    public LayerMask fov_obstaclesLayerMask;    
    public GameObject fov_aquiredTargetGameObject;
    public bool fov_isSearchingForTarget;
    public bool fov_targetAquired;   

    [Space]
    [Header("Character Speed")]
    public float currentMoveSpeed;
    public float currentWalkSpeed;
    public float currentRunSpeed;
    public float currentJumpPower;
    public float currentJumpStamCost;
    public bool currentJumpMode_J_ = true;
    public float currentJumpDeltaTime;
    public bool currentGravityEnabled = true;
    public float currentGravity;
    public Vector3 currentBackupPosition;
    public Quaternion currentBackupRotation;
    public Vector3 currentMoveVector;
    public Vector3 currentMoveInputDirection;

    [Space]
    [Header("Character Melee Combat")]
    public float currentAttackCooldown;
    public float currentDamageCombo;
    public float currentAttackStamCost;
    public float currentComboMeele;
    [Space]
    [Header("Character Magic Combat")]
    [CanBeNull]public Spell_FireBreath spell;
    public bool skill_CanCast;
    public float spell_coroutineDelay;
    public bool spell_OnCoroutine;
    [Range(0.2f,1f)]public float spell_AISpellRangeFromMax;
    public bool spell_targetInSpellRange;
    [ColorUsageAttribute(true, true)] public Color spell_editorAISpellRadiusColor; //kolor HDR picker
    [ColorUsageAttribute(true, true)] public Color spell_breathAngleColor;
    [ColorUsageAttribute(true, true)] public Color spell_breathRaycastColor;
    public float spell_MaxRadius;
    public float currentSpell_Damage;
    public float currentSpell_MPCost;   

    [Space]
    [Header("Current character Stats")]
    public float currentHP;
    public float currentMaxHP;

    [Space]
    public float currentMP;
    public float currentMaxMP;

    [Space]
    public float currentStam;
    public float currentMaxStam;

    [Space]
    public float currentXP;
    public float currentNeededXP;
    public float currentXP_GainedFromKill;

    [Space]
    public float corpseTime;
    public float respawnTime;

    [Space]
    [Header("Character GroundMask")]
    public LayerMask currentGroundMask;
    public float currentGroundDistance;

    [Space]
    [Header("Character Components")]
    [CanBeNull] public Animator currentAnimator;
    [CanBeNull] public CharacterController currentCharacterController;
    [CanBeNull] public Player_Input currentPlayer_Input;
    [CanBeNull] public NavMeshAgent currentNavMeshAgent;
    [CanBeNull] public AudioSource currentAudioSource;
    public ScrObj_charStats scrObj_CharStats;
    [CanBeNull] public MeeleAttack[] currentMeeleColliders;
    public CharacterBonusStats currentCharacterBonusStats;

    public void LoadCharStats()
    {
        //Info
        currentCharName = scrObj_CharStats.charName;
        isPlayer = Camera.main.GetComponent<CameraController>().player == gameObject;

        //Movement
        currentMoveSpeed = scrObj_CharStats.moveSpeed;
        currentWalkSpeed = scrObj_CharStats.walkSpeed + (currentCharacterBonusStats.bonus_currentWalkSpeed * 0.1f);     //+bonus
        currentRunSpeed = scrObj_CharStats.runSpeed + (currentCharacterBonusStats.bonus_currentWalkSpeed * 2f * 0.1f); //+bonus
        currentJumpPower = scrObj_CharStats.jumpPower;
        currentJumpStamCost = scrObj_CharStats.jumpStamCost;
        currentJumpDeltaTime = scrObj_CharStats.JumpDeltaTime;
        currentGravity = scrObj_CharStats.gravity;

        //Input
        inputRotateSensivity = scrObj_CharStats.inputRotateSensivity;

        //NavMesh
        fov_attackRange = scrObj_CharStats.navMeAge_attackRange;
        navMeAge_wanderingRange = scrObj_CharStats.navMeAge_wanderingRange;
        navMeAge_walkPointRange = scrObj_CharStats.navMeAge_walkPointRange;
        navMeAge_whatIsGround = scrObj_CharStats.navMeAge_whatIsGround;
        navMeAge_patrollingDelay = scrObj_CharStats.navMeAge_patrollingDelay;

        //FoV NavMesh
        fov_MaxSightAngle = scrObj_CharStats.fov_MaxSightAngle;
        fov_MinSightAngle = scrObj_CharStats.fov_MinSightAngle;
        fov_TimeDynamicSightAngle = scrObj_CharStats.fov_TimeDynamicSightAngle;
        fov_MinSightRadius = scrObj_CharStats.fov_MinSightRadius;
        fov_MaxSightRadius = scrObj_CharStats.fov_MaxSightRadius;
        fov_TimeDynamicSightRadius = scrObj_CharStats.fov_TimeDynamicSightRadius;
        fov_editorLineThickness = scrObj_CharStats.fov_editorLineThickness;
        fov_editorAngleLineColor = scrObj_CharStats.fov_editorAngleLineColor;
        fov_editorAngleColor = scrObj_CharStats.fov_editorAngleColor;
        fov_editorRadiusColor = scrObj_CharStats.fov_editorRadiusColor;
        fov_editorDynamicRadiusColor = scrObj_CharStats.fov_editorDynamicRadiusColor;
        fov_editorRaycastColor = scrObj_CharStats.fov_editorRaycastColor;
        fov_RoutineDelay = scrObj_CharStats.fov_coneRoutineDelay;
        fov_obstaclesLayerMask = scrObj_CharStats.fov_obstaclesLayerMask;

        //Mask
        currentGroundMask = scrObj_CharStats.groundMask;
        currentGroundDistance = scrObj_CharStats.groundDistance;

        //HP,MP,STAM,XP
        currentMaxHP = scrObj_CharStats.baseHP + (scrObj_CharStats.baseHP * (currentCharLevel * scrObj_CharStats.HP_Multiplier)) + (scrObj_CharStats.baseHP * (currentCharacterBonusStats.bonus_currentMaxHP * scrObj_CharStats.HP_Multiplier));   //current staty po przeliczenu multipliera * CharLevel  +bonus
        currentMaxMP = scrObj_CharStats.baseMP + (scrObj_CharStats.baseMP * (currentCharLevel * scrObj_CharStats.MP_Multiplier)) + (scrObj_CharStats.baseMP * (currentCharacterBonusStats.bonus_currentMaxMP * scrObj_CharStats.MP_Multiplier));   //+bonus     
        currentMaxStam = scrObj_CharStats.baseStam + (scrObj_CharStats.baseStam * (currentCharLevel * scrObj_CharStats.Stam_Multiplier)) + (scrObj_CharStats.baseStam * (currentCharacterBonusStats.bonus_currentMaxStam * scrObj_CharStats.Stam_Multiplier)); //+bonus        
        currentNeededXP = scrObj_CharStats.baseNeedXP + (scrObj_CharStats.baseNeedXP * (currentCharLevel * scrObj_CharStats.XP_Multiplier));       
        currentXP_GainedFromKill = scrObj_CharStats.XP_GainedFromKill + (scrObj_CharStats.XP_GainedFromKill * (currentCharLevel * scrObj_CharStats.XP_Multiplier));
        corpseTime = scrObj_CharStats.corpseTime;
        respawnTime = scrObj_CharStats.respawnTime;


        //Damage
        currentAttackCooldown = scrObj_CharStats.attackCooldown;
        currentDamageCombo = scrObj_CharStats.baseDamageCombo + (scrObj_CharStats.baseDamageCombo * (currentCharLevel * scrObj_CharStats.MultiplierDamageCombo)) + (scrObj_CharStats.baseDamageCombo * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_CharStats.MultiplierDamageCombo)); //+bonus
        currentAttackStamCost = scrObj_CharStats.attackStamCost + (scrObj_CharStats.attackStamCost * (currentCharLevel/2f * scrObj_CharStats.MultiplierDamageCombo));        
        currentSpell_Damage = scrObj_CharStats.baseSpell_Damage + (scrObj_CharStats.baseSpell_Damage * (currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage)) + (scrObj_CharStats.baseSpell_Damage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_CharStats.MultiplierSpell_Damage)); //+bonus
        currentSpell_MPCost = scrObj_CharStats.baseSpell_MPCost + (scrObj_CharStats.baseSpell_Damage * (currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage));
        spell_editorAISpellRadiusColor = scrObj_CharStats.spell_editorAISpellRadiusColor;
        spell_breathAngleColor = scrObj_CharStats.spell_breathAngleColor;
        spell_breathRaycastColor = scrObj_CharStats.spell_breathRaycastColor;

        //Objects
        if (GetComponent<Animator>() != null) currentAnimator = GetComponent<Animator>();                     //ci¹gnie componenty z przypiêtych do chara
        if (GetComponent<CharacterController>() != null) currentCharacterController = GetComponent<CharacterController>();
        if (GetComponent<Player_Input>() != null) currentPlayer_Input = GetComponent<Player_Input>();
        if (GetComponent<NavMeshAgent>() != null) 
        {
            currentNavMeshAgent = GetComponent<NavMeshAgent>();
            currentNavMeshAgent.stoppingDistance = fov_attackRange;
        }
        if (GetComponent<AudioSource>() != null) currentAudioSource = GetComponent<AudioSource>();        
        if (GetComponentInChildren<MeeleAttack>() != null) currentMeeleColliders = GetComponentsInChildren<MeeleAttack>();
        if (GetComponent<CharacterBonusStats>() != null) currentCharacterBonusStats = GetComponent<CharacterBonusStats>();

    }

    public void UpdateBonusStats()
    {
        //Movement
        currentWalkSpeed = scrObj_CharStats.walkSpeed + (currentCharacterBonusStats.bonus_currentWalkSpeed * 0.1f);     //+bonus
        currentRunSpeed = scrObj_CharStats.runSpeed + (currentCharacterBonusStats.bonus_currentWalkSpeed * 2f * 0.1f); //+bonus

        //HP,MP,STAM,XP
        currentMaxHP = scrObj_CharStats.baseHP + (scrObj_CharStats.baseHP * (currentCharLevel * scrObj_CharStats.HP_Multiplier)) + (scrObj_CharStats.baseHP * (currentCharacterBonusStats.bonus_currentMaxHP * scrObj_CharStats.HP_Multiplier));   //current staty po przeliczenu multipliera * CharLevel  +bonus
        currentMaxMP = scrObj_CharStats.baseMP + (scrObj_CharStats.baseMP * (currentCharLevel * scrObj_CharStats.MP_Multiplier)) + (scrObj_CharStats.baseMP * (currentCharacterBonusStats.bonus_currentMaxMP * scrObj_CharStats.MP_Multiplier));   //+bonus     
        currentMaxStam = scrObj_CharStats.baseStam + (scrObj_CharStats.baseStam * (currentCharLevel * scrObj_CharStats.Stam_Multiplier)) + (scrObj_CharStats.baseStam * (currentCharacterBonusStats.bonus_currentMaxStam * scrObj_CharStats.Stam_Multiplier)); //+bonus        

        //Damage
        currentDamageCombo = scrObj_CharStats.baseDamageCombo + (scrObj_CharStats.baseDamageCombo * (currentCharLevel * scrObj_CharStats.MultiplierDamageCombo)) + (scrObj_CharStats.baseDamageCombo * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_CharStats.MultiplierDamageCombo)); //+bonus
        currentAttackStamCost = scrObj_CharStats.attackStamCost + (scrObj_CharStats.attackStamCost * (currentCharLevel / 2f * scrObj_CharStats.MultiplierDamageCombo));
        currentSpell_Damage = scrObj_CharStats.baseSpell_Damage + (scrObj_CharStats.baseSpell_Damage * (currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage)) + (scrObj_CharStats.baseSpell_Damage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_CharStats.MultiplierSpell_Damage)); //+bonus
        currentSpell_MPCost = scrObj_CharStats.baseSpell_MPCost + (scrObj_CharStats.baseSpell_Damage * (currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage));
        SaveBonusStats();
    }

    public void LevelGain()
    {
        currentXP -= currentNeededXP; //przerzuca nadwy¿ke xp na next level
        currentCharLevel ++;
        LoadCharStats();
    }

    ///////////LOAD GAME///////////
    public void LoadGame()
    {
        if (isPlayer)
        {
            LoadLevel();
            LoadCharStats();
            ResetCharacterPosition();  //przy load state resetuje do backup pozycji 
        }
    }

    public void LoadLevel()
    {
        //PlayerPrefs CharacterLevel
        currentCharLevel = PlayerPrefs.GetInt("CharacterLevel", 0);  //PlayerPrefs przechowuje dane w rejestrze Unity, taki prosty save stat 
        currentXP = PlayerPrefs.GetInt("CharacterCurrentXP", 0);
        LoadBonusStats();
        UpdateBonusStats();
    } 


    ///////////SAVE GAME///////////
    public void SaveGame()
    {
        if (isPlayer)
        {
            {
                SaveState();
                SetCharacterPosition();//przy save state ustawia backup pozycje
            }
        }
    }

    public void SaveState()
    {
        if (isPlayer)
        {
            currentBackupPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //y+0.5f ¿eby zrespi³ siê powy¿ej terrain
            currentBackupRotation = transform.rotation;
            PlayerPrefs.SetInt("CharacterLevel", currentCharLevel);
            PlayerPrefs.SetInt("CharacterCurrentXP", (int)currentXP);
            SaveBonusStats();
        }

    }

    public void SaveBonusStats()
    {
        if (isPlayer)
        {
            PlayerPrefs.SetInt("BonusWalkSpeed", (int)currentCharacterBonusStats.bonus_currentWalkSpeed);
            PlayerPrefs.SetInt("BonusDamageCombo", (int)currentCharacterBonusStats.bonus_currentDamageCombo);
            PlayerPrefs.SetInt("BonusSpellDamage", (int)currentCharacterBonusStats.bonus_Skill_Damage);
            PlayerPrefs.SetInt("BonusMaxHP", (int)currentCharacterBonusStats.bonus_currentMaxHP);
            PlayerPrefs.SetInt("BonusMaxMP", (int)currentCharacterBonusStats.bonus_currentMaxMP);
            PlayerPrefs.SetInt("BonusMaxStam", (int)currentCharacterBonusStats.bonus_currentMaxStam);
            PlayerPrefs.SetInt("BonusSkillPoints", (int)currentCharacterBonusStats.bonus_SkillPoints);
        }
    }

    public void LoadBonusStats()
    {
        if (isPlayer)
        {
            currentCharacterBonusStats.bonus_currentWalkSpeed = PlayerPrefs.GetInt("BonusWalkSpeed", 0);
            currentCharacterBonusStats.bonus_currentDamageCombo = PlayerPrefs.GetInt("BonusDamageCombo", 0);
            currentCharacterBonusStats.bonus_Skill_Damage = PlayerPrefs.GetInt("BonusSpellDamage", 0);
            currentCharacterBonusStats.bonus_currentMaxHP = PlayerPrefs.GetInt("BonusMaxHP", 0);
            currentCharacterBonusStats.bonus_currentMaxMP = PlayerPrefs.GetInt("BonusMaxMP", 0);
            currentCharacterBonusStats.bonus_currentMaxStam = PlayerPrefs.GetInt("BonusMaxStam", 0);
            currentCharacterBonusStats.bonus_SkillPoints = PlayerPrefs.GetInt("BonusSkillPoints", 0);
        }
    }


    public void SetCharacterPosition()
    {
        currentBackupPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //y+0.5f ¿eby zrespi³ siê powy¿ej terrain
        currentBackupRotation = transform.rotation;
        navMeAge_spawnPoint = transform.position;
    }

    ///////////////////////////////
    public void ResetLevel()
    {
        if (isPlayer)
        {
            currentCharLevel = 0;
            currentXP = 0;
            currentCharacterBonusStats.bonus_SkillPoints = 0;
            //PlayerPrefs.SetInt("CharacterLevel", currentCharLevel);
            //PlayerPrefs.SetInt("CharacterCurrentXP", (int)currentXP);
            LoadCharStats();
        }

    }

    public void ResetCharacterPosition()
    {
        transform.position = currentBackupPosition;
        transform.rotation = currentBackupRotation;
    }

    public void ResourcesRegen()
    {
        //Stam Regen
        if (currentStam < currentMaxStam && !isRunning && !isAttacking /*&& currentStam<currentMaxStam*/) currentStam = Mathf.MoveTowards(currentStam, currentMaxStam, (scrObj_CharStats.regenStam + (scrObj_CharStats.regenStam * scrObj_CharStats.Stam_Multiplier * currentCharLevel * 1f) + currentMaxStam * 0.01f) * Time.deltaTime); //regeneruje f stamy / sekunde
        //HP Regen
        if (currentHP < currentMaxHP) currentHP = Mathf.MoveTowards(currentHP, currentMaxHP, (scrObj_CharStats.regenHP + (scrObj_CharStats.regenHP * scrObj_CharStats.HP_Multiplier * currentCharLevel * 1f) + currentMaxHP * 0.01f) * Time.deltaTime); //regeneruje f HP / sekunde
        //MP Regen
        if (currentMP < currentMaxMP && !inputSecondary/*&& !isAttacking*/ /*&& currentMP < currentMaxMP*/) currentMP = Mathf.MoveTowards(currentMP, currentMaxMP, (scrObj_CharStats.regenMP + (scrObj_CharStats.regenMP * scrObj_CharStats.MP_Multiplier * currentCharLevel * 1f) + currentMaxMP * 0.01f) * Time.deltaTime); //regeneruje f HP / sekunde

    }
}
