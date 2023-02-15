using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CharacterStatus : MonoBehaviour
{    
    #region Character Info
    [Serializable]
    public class CharInfo
    {
        public string currentCharName;
        public int currentCharLevel;
        [Tooltip("Pozwala na wybór Enemies przy pomocy Tag !!!nie ustawiaæ Environment!!!"), TagField] public string[] currentEnemiesArray;
        public bool playerInputEnable;
        public bool isPlayer;
    }   
    [Tooltip("Character Info")]public CharInfo charInfo;
    #endregion

    #region Current Char Status
    [Serializable]
    public class CurrentCharStatus
    {
        public bool isMoving = false;
        public bool isGrounded = false;
        public bool isJumping = false;
        public bool isAttacking = false;
        public bool isRunning = false;
        public bool isWalking = false;
        public bool isIdle = false;
        public bool isDead = false;
        public bool isCasting = false;
    }
    
    [Tooltip("Current Character Status - Booleany Statusowe")] public CurrentCharStatus currentCharStatus;
    #endregion

    #region CharacterInput
    [Serializable]
    public class CharacterInput
    {
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
    }
    [Tooltip("Booleany/Vectory z klasy Input")] public CharacterInput characterInput;
    #endregion

    #region NavMeshAge
    [Serializable]
    public class NavMeshAge
    {
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
    }
    [Tooltip("Zmienne z NavMeshAgenta")] public NavMeshAge navMeshAge;
    #endregion

    #region FieldOfView
    [Serializable]
    public class FoV
    {        
        [Header("Angle")]
        [Range(0, 360)] public float fov_CurrentDynamicSightAngle;
        [Range(0, 360)] public float fov_MaxSightAngle;
        [Range(0, 360)] public float fov_MinSightAngle;
        public float fov_CurrentVectorDynamicSightAngle;   //wektor aktualnej zmiany wartoœci +/-
                
        [Header("Radius")]
        [Range(0, 1)] public float fov_TimeDynamicSightAngle; //ile sekund na ca³y ruch (90->360)
        [Range(0, 100)] public float fov_MinSightRadius;
        [Range(0, 100)] public float fov_MaxSightRadius;
        [Range(0, 100)] public float fov_CurrentDynamicSightRadius;
        public float fov_lerpedDistance;
        public float fov_CurrentVectorDynamicSightRadius;   //wektor aktualnej zmiany wartoœci +/-
        [Range(0, 5)] public float fov_TimeDynamicSightRadius;

        
        [Header("GizmosColor")]
        [Range(0, 10)] public float fov_editorLineThickness;
        [ColorUsageAttribute(true, true)] public Color fov_editorAngleLineColor; //kolor HDR picker
        [ColorUsageAttribute(true, true)] public Color fov_editorAngleColor; //kolor HDR picker
        [ColorUsageAttribute(true, true)] public Color fov_editorRadiusColor;
        [ColorUsageAttribute(true, true)] public Color fov_editorDynamicRadiusColor;
        [ColorUsageAttribute(true, true)] public Color fov_editorRaycastColor;
                
        [Header("Range")]
        public bool fov_targetInAttackRange;
        public float fov_attackRange;
        public Skill closeRangeSkill;
        public bool fov_targetInDynamicSightRange;
        public bool fov_inSpellRange;
        public Skill spellRangeSkill;

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
    }
    [Tooltip("Field of View")] public FoV fov;
    #endregion

    #region CurrentCharMovement
    [Serializable]
    public class CurrentCharMovement
    {
        [Header("Character Movement variables")]
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

        [Header("Character GroundMask")]
        public LayerMask currentGroundMask;
        public float currentGroundDistance;
    }
    [Tooltip("Current Character Movement")] public CurrentCharMovement currentCharMove;
    #endregion


    #region CharPrimaryCombat
    [Serializable]
    public class CharSkillCombat
    {
        [Header("Character Primary/Melee Combat")]
        [CanBeNull] public Skill skill_primarySkill;
        public float currentAttackCooldown;
        public float currentDamageCombo;
        public float currentAttackStamCost;
        public float currentComboMeele;
        [CanBeNull] public Skill currentCloseRangeSkill;

        [Header("Character Secondary/Magic Combat")]
        [CanBeNull] public Skill skill_secondarySkill;
        public bool skill_CanCast;
        public float spell_coroutineDelay;
        public bool spell_OnCoroutine;
        [Range(0.2f, 1f)] public float spell_AISpellRangeFromMax;
        public bool spell_targetInSpellRange;
        [ColorUsageAttribute(true, true)] public Color spell_editorAISpellRadiusColor; //kolor HDR picker
        [ColorUsageAttribute(true, true)] public Color spell_breathAngleColor;
        [ColorUsageAttribute(true, true)] public Color spell_breathRaycastColor;
        public float spell_MaxRadius;
        public float currentSpell_Damage;
        public float currentSpell_MPCost;
        [CanBeNull] public Skill currentSpellRangeSkill;
    }
    [Tooltip("Character Primary/Melee Combat")] public CharSkillCombat charSkillCombat;
    #endregion        
    

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
        charInfo.currentCharName = scrObj_CharStats.charName;
        charInfo.isPlayer = Camera.main.GetComponent<CameraController>().player == gameObject;

        //Movement
        currentCharMove.currentMoveSpeed = scrObj_CharStats.moveSpeed;
        currentCharMove.currentWalkSpeed = scrObj_CharStats.walkSpeed + (currentCharacterBonusStats.bonus_currentWalkSpeed * 0.1f);     //+bonus
        currentCharMove.currentRunSpeed = scrObj_CharStats.runSpeed + (currentCharacterBonusStats.bonus_currentWalkSpeed * 2f * 0.1f); //+bonus
        currentCharMove.currentJumpPower = scrObj_CharStats.jumpPower;
        currentCharMove.currentJumpStamCost = scrObj_CharStats.jumpStamCost;
        currentCharMove.currentJumpDeltaTime = scrObj_CharStats.JumpDeltaTime;
        currentCharMove.currentGravity = scrObj_CharStats.gravity;

        //Input
        characterInput.inputRotateSensivity = scrObj_CharStats.inputRotateSensivity;

        //NavMesh
        fov.fov_attackRange = scrObj_CharStats.navMeAge_attackRange;
        navMeshAge.navMeAge_wanderingRange = scrObj_CharStats.navMeAge_wanderingRange;
        navMeshAge.navMeAge_walkPointRange = scrObj_CharStats.navMeAge_walkPointRange;
        navMeshAge.navMeAge_whatIsGround = scrObj_CharStats.navMeAge_whatIsGround;
        navMeshAge.navMeAge_patrollingDelay = scrObj_CharStats.navMeAge_patrollingDelay;

        //FoV NavMesh
        fov.fov_MaxSightAngle = scrObj_CharStats.fov_MaxSightAngle;
        fov.fov_MinSightAngle = scrObj_CharStats.fov_MinSightAngle;
        fov.fov_TimeDynamicSightAngle = scrObj_CharStats.fov_TimeDynamicSightAngle;
        fov.fov_MinSightRadius = scrObj_CharStats.fov_MinSightRadius;
        fov.fov_MaxSightRadius = scrObj_CharStats.fov_MaxSightRadius;
        fov.fov_TimeDynamicSightRadius = scrObj_CharStats.fov_TimeDynamicSightRadius;
        fov.fov_editorLineThickness = scrObj_CharStats.fov_editorLineThickness;
        fov.fov_editorAngleLineColor = scrObj_CharStats.fov_editorAngleLineColor;
        fov.fov_editorAngleColor = scrObj_CharStats.fov_editorAngleColor;
        fov.fov_editorRadiusColor = scrObj_CharStats.fov_editorRadiusColor;
        fov.fov_editorDynamicRadiusColor = scrObj_CharStats.fov_editorDynamicRadiusColor;
        fov.fov_editorRaycastColor = scrObj_CharStats.fov_editorRaycastColor;
        fov.fov_RoutineDelay = scrObj_CharStats.fov_coneRoutineDelay;
        fov.fov_obstaclesLayerMask = scrObj_CharStats.fov_obstaclesLayerMask;

        //Mask
        currentCharMove.currentGroundMask = scrObj_CharStats.groundMask;
        currentCharMove.currentGroundDistance = scrObj_CharStats.groundDistance;

        //HP,MP,STAM,XP
        currentMaxHP = scrObj_CharStats.baseHP + (scrObj_CharStats.baseHP * (charInfo.currentCharLevel * scrObj_CharStats.HP_Multiplier)) + (scrObj_CharStats.baseHP * (currentCharacterBonusStats.bonus_currentMaxHP * scrObj_CharStats.HP_Multiplier));   //current staty po przeliczenu multipliera * CharLevel  +bonus
        currentMaxMP = scrObj_CharStats.baseMP + (scrObj_CharStats.baseMP * (charInfo.currentCharLevel * scrObj_CharStats.MP_Multiplier)) + (scrObj_CharStats.baseMP * (currentCharacterBonusStats.bonus_currentMaxMP * scrObj_CharStats.MP_Multiplier));   //+bonus     
        currentMaxStam = scrObj_CharStats.baseStam + (scrObj_CharStats.baseStam * (charInfo.currentCharLevel * scrObj_CharStats.Stam_Multiplier)) + (scrObj_CharStats.baseStam * (currentCharacterBonusStats.bonus_currentMaxStam * scrObj_CharStats.Stam_Multiplier)); //+bonus        
        currentNeededXP = scrObj_CharStats.baseNeedXP + (scrObj_CharStats.baseNeedXP * (charInfo.currentCharLevel * scrObj_CharStats.XP_Multiplier));       
        currentXP_GainedFromKill = scrObj_CharStats.XP_GainedFromKill + (scrObj_CharStats.XP_GainedFromKill * (charInfo.currentCharLevel * scrObj_CharStats.XP_Multiplier));
        corpseTime = scrObj_CharStats.corpseTime;
        respawnTime = scrObj_CharStats.respawnTime;


        //Damage
        charSkillCombat.currentAttackCooldown = scrObj_CharStats.attackCooldown;
        charSkillCombat.currentDamageCombo = scrObj_CharStats.baseDamageCombo + (scrObj_CharStats.baseDamageCombo * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierDamageCombo)) + (scrObj_CharStats.baseDamageCombo * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_CharStats.MultiplierDamageCombo)); //+bonus
        charSkillCombat.currentAttackStamCost = scrObj_CharStats.attackStamCost + (scrObj_CharStats.attackStamCost * (charInfo.currentCharLevel /2f * scrObj_CharStats.MultiplierDamageCombo));
        charSkillCombat.currentSpell_Damage = scrObj_CharStats.baseSpell_Damage + (scrObj_CharStats.baseSpell_Damage * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage)) + (scrObj_CharStats.baseSpell_Damage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_CharStats.MultiplierSpell_Damage)); //+bonus
        charSkillCombat.currentSpell_MPCost = scrObj_CharStats.baseSpell_MPCost + (scrObj_CharStats.baseSpell_Damage * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage));
        charSkillCombat.spell_editorAISpellRadiusColor = scrObj_CharStats.spell_editorAISpellRadiusColor;
        charSkillCombat.spell_breathAngleColor = scrObj_CharStats.spell_breathAngleColor;
        charSkillCombat.spell_breathRaycastColor = scrObj_CharStats.spell_breathRaycastColor;

        //Skills Select By MaxRange
        if (charSkillCombat.skill_primarySkill != null && charSkillCombat.skill_secondarySkill != null)
        {
            if (charSkillCombat.skill_primarySkill.scrObj_Skill.skill_MaxRadius < charSkillCombat.skill_secondarySkill.scrObj_Skill.skill_MaxRadius)
            {
                charSkillCombat.currentCloseRangeSkill = charSkillCombat.skill_primarySkill;
                charSkillCombat.currentSpellRangeSkill = charSkillCombat.skill_secondarySkill;
            }
            else
            {
                charSkillCombat.currentCloseRangeSkill = charSkillCombat.skill_secondarySkill;
                charSkillCombat.currentSpellRangeSkill = charSkillCombat.skill_primarySkill;
            }
        }
        

        //Objects
        if (GetComponent<Animator>() != null) currentAnimator = GetComponent<Animator>();                     //ci¹gnie componenty z przypiêtych do chara
        if (GetComponent<CharacterController>() != null) currentCharacterController = GetComponent<CharacterController>();
        if (GetComponent<Player_Input>() != null) currentPlayer_Input = GetComponent<Player_Input>();
        if (GetComponent<NavMeshAgent>() != null) 
        {
            currentNavMeshAgent = GetComponent<NavMeshAgent>();
            currentNavMeshAgent.stoppingDistance = fov.fov_attackRange;
        }
        if (GetComponent<AudioSource>() != null) currentAudioSource = GetComponent<AudioSource>();        
        if (GetComponentInChildren<MeeleAttack>() != null) currentMeeleColliders = GetComponentsInChildren<MeeleAttack>();
        if (GetComponent<CharacterBonusStats>() != null) currentCharacterBonusStats = GetComponent<CharacterBonusStats>();

    }

    public void UpdateBonusStats()
    {
        //Movement
        currentCharMove.currentWalkSpeed = scrObj_CharStats.walkSpeed + (currentCharacterBonusStats.bonus_currentWalkSpeed * 0.1f);     //+bonus
        currentCharMove.currentRunSpeed = scrObj_CharStats.runSpeed + (currentCharacterBonusStats.bonus_currentWalkSpeed * 2f * 0.1f); //+bonus

        //HP,MP,STAM,XP
        currentMaxHP = scrObj_CharStats.baseHP + (scrObj_CharStats.baseHP * (charInfo.currentCharLevel * scrObj_CharStats.HP_Multiplier)) + (scrObj_CharStats.baseHP * (currentCharacterBonusStats.bonus_currentMaxHP * scrObj_CharStats.HP_Multiplier));   //current staty po przeliczenu multipliera * CharLevel  +bonus
        currentMaxMP = scrObj_CharStats.baseMP + (scrObj_CharStats.baseMP * (charInfo.currentCharLevel * scrObj_CharStats.MP_Multiplier)) + (scrObj_CharStats.baseMP * (currentCharacterBonusStats.bonus_currentMaxMP * scrObj_CharStats.MP_Multiplier));   //+bonus     
        currentMaxStam = scrObj_CharStats.baseStam + (scrObj_CharStats.baseStam * (charInfo.currentCharLevel * scrObj_CharStats.Stam_Multiplier)) + (scrObj_CharStats.baseStam * (currentCharacterBonusStats.bonus_currentMaxStam * scrObj_CharStats.Stam_Multiplier)); //+bonus        

        //Damage
        charSkillCombat.currentDamageCombo = scrObj_CharStats.baseDamageCombo + (scrObj_CharStats.baseDamageCombo * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierDamageCombo)) + (scrObj_CharStats.baseDamageCombo * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_CharStats.MultiplierDamageCombo)); //+bonus
        charSkillCombat.currentAttackStamCost = scrObj_CharStats.attackStamCost + (scrObj_CharStats.attackStamCost * (charInfo.currentCharLevel / 2f * scrObj_CharStats.MultiplierDamageCombo));
        charSkillCombat.currentSpell_Damage = scrObj_CharStats.baseSpell_Damage + (scrObj_CharStats.baseSpell_Damage * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage)) + (scrObj_CharStats.baseSpell_Damage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_CharStats.MultiplierSpell_Damage)); //+bonus
        charSkillCombat.currentSpell_MPCost = scrObj_CharStats.baseSpell_MPCost + (scrObj_CharStats.baseSpell_Damage * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage));
        SaveBonusStats();
    }

    public void LevelGain()
    {
        currentXP -= currentNeededXP; //przerzuca nadwy¿ke xp na next level
        charInfo.currentCharLevel ++;
        LoadCharStats();
    }

    ///////////LOAD GAME///////////
    public void LoadGame()
    {
        if (charInfo.isPlayer)
        {
            LoadLevel();
            LoadCharStats();
            ResetCharacterPosition();  //przy load state resetuje do backup pozycji 
        }
    }

    public void LoadLevel()
    {
        //PlayerPrefs CharacterLevel
        charInfo.currentCharLevel = PlayerPrefs.GetInt("CharacterLevel", 0);  //PlayerPrefs przechowuje dane w rejestrze Unity, taki prosty save stat 
        currentXP = PlayerPrefs.GetInt("CharacterCurrentXP", 0);
        LoadBonusStats();
        UpdateBonusStats();
    } 


    ///////////SAVE GAME///////////
    public void SaveGame()
    {
        if (charInfo.isPlayer)
        {
            {
                SaveState();
                SetCharacterPosition();//przy save state ustawia backup pozycje
            }
        }
    }

    public void SaveState()
    {
        if (charInfo.isPlayer)
        {
            currentCharMove.currentBackupPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //y+0.5f ¿eby zrespi³ siê powy¿ej terrain
            currentCharMove.currentBackupRotation = transform.rotation;
            PlayerPrefs.SetInt("CharacterLevel", charInfo.currentCharLevel);
            PlayerPrefs.SetInt("CharacterCurrentXP", (int)currentXP);
            SaveBonusStats();
        }

    }

    public void SaveBonusStats()
    {
        if (charInfo.isPlayer)
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
        if (charInfo.isPlayer)
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
        currentCharMove.currentBackupPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //y+0.5f ¿eby zrespi³ siê powy¿ej terrain
        currentCharMove.currentBackupRotation = transform.rotation;
        navMeshAge.navMeAge_spawnPoint = transform.position;
    }

    ///////////////////////////////
    public void ResetLevel()
    {
        if (charInfo.isPlayer)
        {
            charInfo.currentCharLevel = 0;
            currentXP = 0;
            currentCharacterBonusStats.bonus_SkillPoints = 0;
            //PlayerPrefs.SetInt("CharacterLevel", currentCharLevel);
            //PlayerPrefs.SetInt("CharacterCurrentXP", (int)currentXP);
            LoadCharStats();
        }

    }

    public void ResetCharacterPosition()
    {
        transform.SetPositionAndRotation(currentCharMove.currentBackupPosition, currentCharMove.currentBackupRotation);        
    }

    public void ResourcesRegen()
    {
        //Stam Regen
        if (currentStam < currentMaxStam && !currentCharStatus.isRunning /*&& !isAttacking *//*&& currentStam<currentMaxStam*/) currentStam = Mathf.MoveTowards(currentStam, currentMaxStam, (scrObj_CharStats.regenStam + (scrObj_CharStats.regenStam * scrObj_CharStats.Stam_Multiplier * charInfo.currentCharLevel * 1f) + (currentMaxStam * 0.1f * scrObj_CharStats.Stam_Multiplier)) * Time.deltaTime); //regeneruje f stamy / sekunde
        //HP Regen
        if (currentHP < currentMaxHP) currentHP = Mathf.MoveTowards(currentHP, currentMaxHP, (scrObj_CharStats.regenHP + (scrObj_CharStats.regenHP * scrObj_CharStats.HP_Multiplier * charInfo.currentCharLevel * 1f) + (currentMaxHP * 0.1f * scrObj_CharStats.HP_Multiplier)) * Time.deltaTime); //regeneruje f HP / sekunde
        //MP Regen
        if (currentMP < currentMaxMP /*&& !inputSecondary*//*&& !isAttacking*/ /*&& currentMP < currentMaxMP*/) currentMP = Mathf.MoveTowards(currentMP, currentMaxMP, (scrObj_CharStats.regenMP + (scrObj_CharStats.regenMP * scrObj_CharStats.MP_Multiplier * charInfo.currentCharLevel * 1f) + (currentMaxMP * 0.1f * scrObj_CharStats.MP_Multiplier)) * Time.deltaTime); //regeneruje f HP / sekunde

    }

    /////////////////////////////////
    
    public void TakeDamgeInstant(float damageValue, CharacterStatus attacker)
    {
        currentHP -= damageValue;
        if (currentHP <= 0f && !currentCharStatus.isDead) //zadzia³a tylko raz
        {
            attacker.currentXP += currentXP_GainedFromKill;
            if (gameObject.CompareTag("Monster")) { charInfo.currentCharLevel = UnityEngine.Random.Range(attacker.charInfo.currentCharLevel - 3, attacker.charInfo.currentCharLevel + 3); }  //po œmierci ustawia level targetu na zbli¿ony do atakuj¹cego
                                                                                                                                                                                                                                  //podbija lvl tylko Monsterów, Playera i Environment nie
        }
    }

    public void TakeDamageOverTime(float damageValue, CharacterStatus attacker)
    {
        currentHP = Mathf.MoveTowards(currentHP, -damageValue, Time.deltaTime * damageValue);
        if (currentHP <= 0f && !currentCharStatus.isDead) //zadzia³a tylko raz
        {
            attacker.currentXP += currentXP_GainedFromKill;
            if (gameObject.CompareTag("Monster")) { charInfo.currentCharLevel = UnityEngine.Random.Range(attacker.charInfo.currentCharLevel - 3, attacker.charInfo.currentCharLevel + 3); }  //po œmierci ustawia level targetu na zbli¿ony do atakuj¹cego
                                                                                                                                                                                                                                  //podbija lvl tylko Monsterów, Playera i Environment nie
        }
    }

    public void TakeHealInstant(float healValue)
    {
        if (currentHP < currentMaxHP) currentHP += healValue;
        else { currentHP = currentMaxHP; }
    }

    public void TakeHealOverTime(float healValue)
    {
        if (currentHP < currentMaxHP) { currentHP = Mathf.MoveTowards(currentHP, currentMaxHP, Time.deltaTime * healValue); }
        else { currentHP = currentMaxHP; }
    }


}
