using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CharacterStatus : MonoBehaviour
{    
    #region Character Info
    [Serializable]
    public class CharInfo
    {
        public string _charName;
        public int _charLevel;
        [Tooltip("Pozwala na wybór Enemies przy pomocy Tag \n !!!nie ustawiaæ Environment!!! \n !!!Musi byæ ustawione na ka¿dej postacji Box te¿!!!"),  TagField] public string[] _enemiesArray;
        public bool _playerInputEnable;
        public bool _isPlayer;
    }   
    [Tooltip("Character Info")]public CharInfo charInfo;
    #endregion

    #region Current Char Status
    [Serializable]
    public class CharStatus
    {
        public bool _isMoving = false;
        public bool _isGrounded = false;
        public bool _isJumping = false;
        public bool _isAttacking = false;
        public bool _isRunning = false;
        public bool _isWalking = false;
        public bool _isIdle = false;
        public bool _isDead = false;
        public bool _isCasting = false;        
        
    }
    
    [Tooltip("Current Character Status - Booleany Statusowe")] public CharStatus charStatus;
    #endregion

    #region CharacterInput
    [Serializable]
    public class CharInput
    {
        [Tooltip("Aktualny obrót myszk¹")] public float _rotateHorizontal;
        public bool _moving;
        public bool _running;
        public bool _jumping;
        //[Tooltip("LMB / LPM")] public bool _primary;
        //[Tooltip("RMB / PPM")] public bool _secondary;
        //[Tooltip("Sensitivity myszki")] public float _rotateSensivity;
        [Tooltip("Czy mo¿na obracaæ myszk¹? - Player (X)")] public bool _enableMouseRotate;
        [Tooltip("Poruszanie siê we wskazany _walkPoint - nadpisanie dzia³ania AIControllera - Player (MiddleMouse)")] public bool _mouseCurrentMoving;     

        [Space]
        public bool _saveGame;
        public bool _loadGame;
        public bool _resetPosition;
        public bool _setBackupPosition;
        public float _mouseScroll;
        public bool _testing_Z_Key;
    }
    [Tooltip("Booleany/Vectory z klasy Input")] public CharInput charInput;
    #endregion

    #region NavMeshAge
    [Serializable]
    public class NavMeshAge
    {
        [Tooltip("Zasiêg w jakim mo¿e ustawiæ walkPoint od origina (_spawnPoint)")] public float _wanderingRange;
        [Tooltip("Punkt origin który ustawia siê po spawnie i zawsze zostaje ten sam")] public Vector3 _spawnPoint;
        [Tooltip("Aktualnie ustawiony _walkPoint")] public Vector3 _walkPoint;
        [Tooltip("Aktualnie ustawiony (myszk¹)_walkPoint - tylko Player")] public Vector3 _mouseWalkPoint;
        [Tooltip("Czy jest ustawioy _walkPoint?")] public bool _walkPointSet;
        [Tooltip("Zasiêg w jakim wylicza nowy walkPoint (-/+)")] public float _walkPointRange;
        [Tooltip("LayerMask do Raycasta - Co traktuje jako pod³o¿e?")] public LayerMask _whatIsGround;
        [Tooltip("Mechanizm ochronny jeœli wyjdzie poza _wanderingRange, resetuje walkPointSet do spawnPoint (max.10 i reset)")] public int _failsafeCounter;
        [Tooltip("Nie ustawione (Mia³ byæ delay - jeœli nie dojdzie do _walkPoint w okreœlonym czasie to reset")] public float _patrollingDelay;
        //[Tooltip("OpóŸnienie _AIRoutine w Sekndach - im wiêcej tym ³atwiej dla kompa"), Range(0f,0.3f)] public float _AIRoutineDelay;
        [Tooltip("Aktualny status - Czy jest w trakcie _AIRoutine? Jeœli jest nie odpali nowej")] public bool _isCheckingAIRoutine;
    }
    [Tooltip("Zmienne z NavMeshAgenta")] public NavMeshAge navMeshAge;
    #endregion

    #region FieldOfView
    [Serializable]
    public class FoV
    {        
        [Header("Angle")]
        [Tooltip("Aktualny DynamicSightAngle"), Range(0, 360)] public float _currentDynamicSightAngle;        
        [Tooltip("Min DynamicSightAngle"), Range(0, 360)] public float _minSightAngle;
        [Tooltip("Max DynamicSightAngle"), Range(0, 360)] public float _maxSightAngle;
        [Tooltip("Aktualny wektor zmiany wartoœci +/- na DynamicSightAngle")] public float _currentVectorDynamicSightAngle;   
        [Tooltip("ile sekund na ca³y ruch (MAX -> MAX)"), Range(0, 1)] public float _timeDynamicSightAngle;
                                                           
        [Header("Radius")]
        [Tooltip("Aktualny DynamicSightRadius"), Range(0, 20)] public float _currentDynamicSightRadius;
        [Tooltip("Min DynamicSightRadius"), Range(0, 20)] public float _minSightRadius;
        [Tooltip("Max DynamicSightRadius"), Range(0, 20)] public float _maxSightRadius;                
        [Tooltip("Aktualny wektor zmiany wartoœci +/- na DynamicSightRadius")] public float _currentVectorDynamicSightRadius;
        [Tooltip("ile sekund na ca³y ruch (MAX -> MAX)"), Range(0, 5)] public float _timeDynamicSightRadius;

        [Header("Range Booleans / Floats / Skills / Taget")]
        [Tooltip("Podaje true jeœli target wogóle znajduje siê w dynamic SightRange")] public bool _targetInDynamicSightRange;        
        [Space]
        [Tooltip("Podaje true jeœli target jest Aquired i znajduje siê w zasiêgu (MaxRadius) spellRangeSkilla (zmodyfikowanieg przez multiplier _AISpellRangeFromMax)")] public bool _targetInSpellRange;
        [Tooltip("Multiplier do (MaxRadius) spellRangeSkilla"), Range(0.1f, 1f)] public float _AISpellRangeSkillRadiusFromMax;
        [Tooltip("Aktualny spellRange pobrany z closeRangeSkill")] public float _spellRangeSkillMaxRadius;
        [Space]
        [Tooltip("Podaje true jeœli target jest Aquired i znajduje siê w zasiêgu (MaxRadius) closeRangeSkilla")] public bool _targetInAttackRange;
        [Space]
        [Tooltip("Podaje true jeœli target znajduje siê w dynamic SightRange && Angle && !Raycast(obstaclesLayerMask)")] public bool _targetAquired;
        [CanBeNull, Tooltip("Aktualnie znaleziony (DynamicSightRange) && goniony (Chasing) aquiredTargetGameObject")] public GameObject _aquiredTargetGameObject;
        [Tooltip("Dystans do aktualnego _aquiredTargetGameObject")]public float _distanceToTarget;
        
        [Header("Utils")]
        [Tooltip("Wszystkie Collidery z DunamicSightRange (Physics.OverlapCapsuleNonAlloc) (Uwaga na MAX iloœæ elementów w Array)")] public Collider[] _allTargetsInDynamicSightRange = new Collider[30];
        [Space]
        [Tooltip("Obstacles LayerMask dla FieldOfView")] public LayerMask _obstaclesLayerMask;
        [Space]
        [Tooltip("OpóŸnienie _FoVRoutine w Sekndach - im wiêcej tym ³atwiej dla kompa (Wy³¹czone przy AIController)"), Range(0f, 0.3f)] public float _routineDelay;
        [Tooltip("Aktualny status - Czy jest w trakcie _FoVRoutine? Jeœli jest nie odpali nowej")] public bool _isSearchingForTarget;
      /*  [Space]
        [Header("GizmosColor")]
        //[Tooltip("Gruboœæ lini Gizmos"), Range(0, 10)] public float _editorLineThickness;
        //[Tooltip("Kolor Gizmos - AngleLineColor (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _editorAngleLineColor; 
        //[Tooltip("Kolor Gizmos - AngleColor (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _editorAngleColor;
        //[Tooltip("Kolor  Gizmos - MaxRadius"), ColorUsageAttribute(true, true)] public Color _editorMaxRadiusColor;
        //[Tooltip("Kolor  Gizmos - DynamicRadius"), ColorUsageAttribute(true, true)] public Color _editorDynamicRadiusColor;
        //[Tooltip("Kolor  Gizmos - Raycast"), ColorUsageAttribute(true, true)] public Color _editorRaycastColor;
*/
    }
    [Tooltip("Field of View")] public FoV fov;
    #endregion

    #region CurrentCharMovement
    [Serializable]
    public class CharMovement
    {
        [Header("Character Movement variables")]
        public float _moveSpeed;
        public float _walkSpeed;
        public float _runSpeed;
        public float _jumpPower;
        public float _jumpStamCost;
        public float _jumpDeltaTime;
        [Tooltip("Grawitacja ON/Off, mo¿na mechanikê gravity guna zrobiæ :D")]public bool _gravityEnabled = true;
        //public float _gravity;
        public Vector3 _backupPosition;
        public Quaternion _backupRotation;
        public Vector3 _moveVector;
        public Vector3 _moveInputDirection;

        [Header("Character GroundMask")]
        public LayerMask currentGroundMask;
        public float currentGroundDistance;
    }
    [Tooltip("Current Character Movement")] public CharMovement charMove;
    #endregion

    #region CharSkillCombat
    [Serializable]
    public class CharSkillCombat
    {
        [Tooltip("Tablica Skillów podpiêtych pod postaæ\n [0] -> CloseRange / Primary (Player)\n [1] -> SpellRange / Secondary (Player)"), CanBeNull] public Skill[] _skillArray;

        //[Tooltip("Kolor Gizmos - AISpellRadius (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _editorAISpellRadiusColor;
        //[Tooltip("Kolor Gizmos - SkillAngle (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _skillAngleColor;
        //[Tooltip("Kolor Gizmos - SkillRaycast (kolor HDR picker)"), ColorUsageAttribute(true, true)] public Color _skillRaycastColor;          
    }
    [Tooltip("Character Primary/Secondary Combat")] public CharSkillCombat charSkillCombat;
    #endregion

    #region CharStats
    [Serializable]
    public class CharStats
    {
        [Tooltip("Aktualne Health Points postaci")] public float _hp;
        [Tooltip("Aktualne Maxymalne Health Points postaci")] public float _maxHP;
        [Tooltip("Aktualny Regen Health Points postaci (% z MaxHP)")] public float _regenHP;
        [Space]
        [Tooltip("Aktualne Mana Points postaci")] public float _mp;
        [Tooltip("Aktualne Maxymalne Mana Points postaci")] public float _maxMP;
        [Tooltip("Aktualny Regen Mana Points postaci (% z MaxHP)")] public float _regenMP;
        [Space]
        [Tooltip("Aktualne Stamina Points postaci")] public float _stam;
        [Tooltip("Aktualne Maxymalne Stamin Points postaci")] public float _maxStam;
        [Tooltip("Aktualny Regen Stamina Points postaci (% z MaxHP)")] public float _regenStam;
        [Space]
        [Tooltip("Aktualne ExP postaci")] public float _xp;
        [Tooltip("Aktualne ExP potrzebne do nastêpnego levela")] public float _neededXP;
        [Tooltip("Ile ExP dostaje przeciwnik za zabicie tej postaci?")] public float _xp_GainedFromKill;

        [Space]
        [Tooltip("Czas jako corpse po œmierci")] public float _corpseTime;
        [Tooltip("CZas respawnu po œmierci")] public float _respawnTime;

    }
    [Tooltip("Aktualne statysytyki postaci")] public CharStats charStats;
    #endregion

    #region CharComponents
    [Serializable]
    public class CharComponents
    {
        [CanBeNull] public Animator _Animator;
        [CanBeNull] public CharacterController _characterController;
        [CanBeNull] public Player_Input _player_Input;
        [CanBeNull] public NavMeshAgent _navMeshAgent;
        [CanBeNull] public AudioSource _audioSource;
        public ScrObj_charStats _scrObj_CharStats;
        public CharacterBonusStats _characterBonusStats;
        public ScrObj_GameSettings _scrObj_GameSettings;
    }
    [Tooltip("Komponenty podpiête pod live_charStats")]public CharComponents charComponents;
    #endregion

    #region Save/Load Character
    public void LoadCharStats()
    {
        //Objects
        if (GetComponent<Animator>() != null) charComponents._Animator = GetComponent<Animator>();                     //ci¹gnie componenty z przypiêtych do chara
        if (GetComponent<CharacterController>() != null) charComponents._characterController = GetComponent<CharacterController>();
        if (GetComponent<Player_Input>() != null) charComponents._player_Input = GetComponent<Player_Input>();
        if (GetComponent<NavMeshAgent>() != null)
        {
            charComponents._navMeshAgent = GetComponent<NavMeshAgent>();
            charComponents._navMeshAgent.stoppingDistance = 0f;
           


        }  
        if (GetComponent<AudioSource>() != null) charComponents._audioSource = GetComponent<AudioSource>();
        if (GetComponent<CharacterBonusStats>() != null) charComponents._characterBonusStats = GetComponent<CharacterBonusStats>();

        if (GetComponentInChildren<Skill>() != null) { SkillsSetup(); }

        Utils_BoxSpellsReset();


        //Info
        charInfo._charName = charComponents._scrObj_CharStats.charName;
        charInfo._isPlayer = Camera.main.GetComponent<CameraController>().player == gameObject;

        //Movement
        charMove._moveSpeed = charComponents._scrObj_CharStats.moveSpeed;
        charMove._walkSpeed = charComponents._scrObj_CharStats.walkSpeed + (charComponents._characterBonusStats.bonus_currentWalkSpeed * 0.1f);     //+bonus
        charMove._runSpeed = charComponents._scrObj_CharStats.runSpeed + (charComponents._characterBonusStats.bonus_currentWalkSpeed * 2f * 0.1f); //+bonus
        charMove._jumpPower = charComponents._scrObj_CharStats.jumpPower;
        charMove._jumpStamCost = charComponents._scrObj_CharStats.jumpStamCost;
        charMove._jumpDeltaTime = charComponents._scrObj_CharStats.JumpDeltaTime;
        //charMove._gravity = charComponents._scrObj_CharStats.gravity;

        //Input
        //charInput._rotateSensivity = charComponents._scrObj_CharStats.inputRotateSensivity;

        //NavMesh        
        navMeshAge._wanderingRange = charComponents._scrObj_CharStats.navMeAge_wanderingRange;
        navMeshAge._walkPointRange = charComponents._scrObj_CharStats.navMeAge_walkPointRange;
        navMeshAge._whatIsGround = charComponents._scrObj_CharStats.navMeAge_whatIsGround;
        navMeshAge._patrollingDelay = charComponents._scrObj_CharStats.navMeAge_patrollingDelay;
        //navMeshAge._AIRoutineDelay = charComponents._scrObj_CharStats.navMeAge_AIRoutineDelay;

        //FoV NavMesh
        fov._maxSightAngle = charComponents._scrObj_CharStats.fov_MaxSightAngle;
        fov._minSightAngle = charComponents._scrObj_CharStats.fov_MinSightAngle;
        fov._timeDynamicSightAngle = charComponents._scrObj_CharStats.fov_TimeDynamicSightAngle;
        fov._minSightRadius = charComponents._scrObj_CharStats.fov_MinSightRadius;
        fov._maxSightRadius = charComponents._scrObj_CharStats.fov_MaxSightRadius;
        fov._timeDynamicSightRadius = charComponents._scrObj_CharStats.fov_TimeDynamicSightRadius;

        fov._routineDelay = charComponents._scrObj_CharStats.fov_coneRoutineDelay;
        fov._obstaclesLayerMask = charComponents._scrObj_CharStats.fov_obstaclesLayerMask;

       /* fov._editorLineThickness = charComponents._scrObj_CharStats.fov_editorLineThickness;
        fov._editorAngleLineColor = charComponents._scrObj_CharStats.fov_editorAngleLineColor;
        fov._editorAngleColor = charComponents._scrObj_CharStats.fov_editorAngleColor;
        fov._editorMaxRadiusColor = charComponents._scrObj_CharStats.fov_editorRadiusColor;
        fov._editorDynamicRadiusColor = charComponents._scrObj_CharStats.fov_editorDynamicRadiusColor;
        fov._editorRaycastColor = charComponents._scrObj_CharStats.fov_editorRaycastColor;*/

        //Mask
        charMove.currentGroundMask = charComponents._scrObj_CharStats.groundMask;
        charMove.currentGroundDistance = charComponents._scrObj_CharStats.groundDistance;

        //HP,MP,STAM,XP
        charStats._maxHP = charComponents._scrObj_CharStats.baseHP + (charComponents._scrObj_CharStats.baseHP * (charInfo._charLevel * charComponents._scrObj_CharStats.HP_Multiplier)) + (charComponents._scrObj_CharStats.baseHP * (charComponents._characterBonusStats.bonus_currentMaxHP * charComponents._scrObj_CharStats.HP_Multiplier));   //current staty po przeliczenu multipliera * CharLevel  +bonus        
        charStats._maxMP = charComponents._scrObj_CharStats.baseMP + (charComponents._scrObj_CharStats.baseMP * (charInfo._charLevel * charComponents._scrObj_CharStats.MP_Multiplier)) + (charComponents._scrObj_CharStats.baseMP * (charComponents._characterBonusStats.bonus_currentMaxMP * charComponents._scrObj_CharStats.MP_Multiplier));   //+bonus            
        charStats._maxStam = charComponents._scrObj_CharStats.baseStam + (charComponents._scrObj_CharStats.baseStam * (charInfo._charLevel * charComponents._scrObj_CharStats.Stam_Multiplier)) + (charComponents._scrObj_CharStats.baseStam * (charComponents._characterBonusStats.bonus_currentMaxStam * charComponents._scrObj_CharStats.Stam_Multiplier)); //+bonus  
        charStats._neededXP = charComponents._scrObj_CharStats.baseNeedXP + (charComponents._scrObj_CharStats.baseNeedXP * (charInfo._charLevel * charComponents._scrObj_CharStats.XP_Multiplier));
        charStats._xp_GainedFromKill = charComponents._scrObj_CharStats.XP_GainedFromKill + (charComponents._scrObj_CharStats.XP_GainedFromKill * (charInfo._charLevel * charComponents._scrObj_CharStats.XP_Multiplier));
        charStats._corpseTime = charComponents._scrObj_CharStats.corpseTime;
        charStats._respawnTime = charComponents._scrObj_CharStats.respawnTime;


        //Damage        
        /*unused Old_DamageVars
         * charSkillCombat.currentDamageCombo = scrObj_CharStats.baseDamageCombo + (scrObj_CharStats.baseDamageCombo * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierDamageCombo)) + (scrObj_CharStats.baseDamageCombo * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_CharStatsbo)); //+bonus
        charSkillCombat.currentAttackStamCost = scrObj_CharStats.attackStamCost + (scrObj_CharStats.attackStamCost * (charInfo.currentCharLevel /2f * scrObj_CharStats.MultiplierDamageCombo));
        charSkillCombat.currentSpell_Damage =seSpell_Damage + (scrObj_CharStats.baseSpell_Damage * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage)) + (scrObj_CharStats.baseSpell_Damage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_CharStats.MultiplierSpell_Damage)); //+bonus
        charSkillCombat.currentSpell_MPCost = scrObj_CharStats.baseSpell_MPCost + (scrObj_CharStats.baseSpell_Damage * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage));
        
        charSkillCombat._editorAISpellRadiusColor = charComponents._scrObj_CharStats.spell_editorAISpellRadiusColor;
        charSkillCombat._skillAngleColor = charComponents._scrObj_CharStats.spell_breathAngleColor;
        charSkillCombat._skillRaycastColor = charComponents._scrObj_CharStats.spell_breathRaycastColor;
        */
    }

    public void UpdateBonusStats()
    {
        //Movement
        charMove._walkSpeed = charComponents._scrObj_CharStats.walkSpeed + (charComponents._characterBonusStats.bonus_currentWalkSpeed * 0.1f);     //+bonus
        charMove._runSpeed = charComponents._scrObj_CharStats.runSpeed + (charComponents._characterBonusStats.bonus_currentWalkSpeed * 2f * 0.1f); //+bonus

        //HP,MP,STAM,XP
        charStats._maxHP = charComponents._scrObj_CharStats.baseHP + (charComponents._scrObj_CharStats.baseHP * (charInfo._charLevel * charComponents._scrObj_CharStats.HP_Multiplier)) + (charComponents._scrObj_CharStats.baseHP * (charComponents._characterBonusStats.bonus_currentMaxHP * charComponents._scrObj_CharStats.HP_Multiplier));   //current staty po przeliczenu multipliera * CharLevel  +bonus
        charStats._maxMP = charComponents._scrObj_CharStats.baseMP + (charComponents._scrObj_CharStats.baseMP * (charInfo._charLevel * charComponents._scrObj_CharStats.MP_Multiplier)) + (charComponents._scrObj_CharStats.baseMP * (charComponents._characterBonusStats.bonus_currentMaxMP * charComponents._scrObj_CharStats.MP_Multiplier));   //+bonus
        charStats._maxStam = charComponents._scrObj_CharStats.baseStam + (charComponents._scrObj_CharStats.baseStam * (charInfo._charLevel * charComponents._scrObj_CharStats.Stam_Multiplier)) + (charComponents._scrObj_CharStats.baseStam * (charComponents._characterBonusStats.bonus_currentMaxStam * charComponents._scrObj_CharStats.Stam_Multiplier)); //+bonus    

        /*//Damage Old unused
        //charSkillCombat.currentDamageCombo = scrObj_CharStats.baseDamageCombo + (scrObj_CharStats.baseDamageCombo * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierDamageCombo)) + (scrObj_CharStats.baseDamageCombo * (currentCharacterBonusStats.bonus_currentDamageCombo * scrObj_CharStats.MultiplierDamageCombo)); //+bonus
        //charSkillCombat.currentAttackStamCost = scrObj_CharStats.attackStamCost + (scrObj_CharStats.attackStamCost * (charInfo.currentCharLevel / 2f * scrObj_CharStats.MultiplierDamageCombo));
        //charSkillCombat.currentSpell_Damage = scrObj_CharStats.baseSpell_Damage + (scrObj_CharStats.baseSpell_Damage * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage)) + (scrObj_CharStats.baseSpell_Damage * (currentCharacterBonusStats.bonus_Skill_Damage * scrObj_CharStats.MultiplierSpell_Damage)); //+bonus
        //charSkillCombat.currentSpell_MPCost = scrObj_CharStats.baseSpell_MPCost + (scrObj_CharStats.baseSpell_Damage * (charInfo.currentCharLevel * scrObj_CharStats.MultiplierSpell_Damage));
        */

        SaveBonusStats();
    }

    public void LevelGain()
    {
        charStats._xp -= charStats._neededXP; //przerzuca nadwy¿ke xp na next level
        charInfo._charLevel++;
        LoadCharStats();
    }

    ///////////LOAD GAME///////////
    public void LoadGame()
    {
        if (charInfo._isPlayer)
        {
            LoadLevel();
            LoadCharStats();
            ResetCharacterPosition();  //przy load state resetuje do backup pozycji 
        }
    }

    public void LoadLevel()
    {
        //PlayerPrefs CharacterLevel
        charInfo._charLevel = PlayerPrefs.GetInt("CharacterLevel", 0);  //PlayerPrefs przechowuje dane w rejestrze Unity, taki prosty save stat 
        charStats._xp = PlayerPrefs.GetInt("CharacterCurrentXP", 0);
        LoadBonusStats();
        UpdateBonusStats();
    }


    ///////////SAVE GAME///////////
    public void SaveGame()
    {
        if (charInfo._isPlayer)
        {
            {
                SaveState();
                SetCharacterPosition();//przy save state ustawia backup pozycje
            }
        }
    }

    public void SaveState()
    {
        if (charInfo._isPlayer)
        {
            charMove._backupPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //y+0.5f ¿eby zrespi³ siê powy¿ej terrain
            charMove._backupRotation = transform.rotation;
            PlayerPrefs.SetInt("CharacterLevel", charInfo._charLevel);
            PlayerPrefs.SetInt("CharacterCurrentXP", (int)charStats._xp);
            SaveBonusStats();
        }

    }

    public void SaveBonusStats()
    {
        if (charInfo._isPlayer)
        {
            PlayerPrefs.SetInt("BonusWalkSpeed", (int)charComponents._characterBonusStats.bonus_currentWalkSpeed);
            PlayerPrefs.SetInt("BonusDamageCombo", (int)charComponents._characterBonusStats.bonus_CloseRangeDamage);
            PlayerPrefs.SetInt("BonusSpellDamage", (int)charComponents._characterBonusStats.bonus_SpellRangeDamage);
            PlayerPrefs.SetInt("BonusMaxHP", (int)charComponents._characterBonusStats.bonus_currentMaxHP);
            PlayerPrefs.SetInt("BonusMaxMP", (int)charComponents._characterBonusStats.bonus_currentMaxMP);
            PlayerPrefs.SetInt("BonusMaxStam", (int)charComponents._characterBonusStats.bonus_currentMaxStam);
            PlayerPrefs.SetInt("BonusSkillPoints", (int)charComponents._characterBonusStats.bonus_SkillPoints);
        }
    }

    public void LoadBonusStats()
    {
        if (charInfo._isPlayer)
        {
            charComponents._characterBonusStats.bonus_currentWalkSpeed = PlayerPrefs.GetInt("BonusWalkSpeed", 0);
            charComponents._characterBonusStats.bonus_CloseRangeDamage = PlayerPrefs.GetInt("BonusDamageCombo", 0);
            charComponents._characterBonusStats.bonus_SpellRangeDamage = PlayerPrefs.GetInt("BonusSpellDamage", 0);
            charComponents._characterBonusStats.bonus_currentMaxHP = PlayerPrefs.GetInt("BonusMaxHP", 0);
            charComponents._characterBonusStats.bonus_currentMaxMP = PlayerPrefs.GetInt("BonusMaxMP", 0);
            charComponents._characterBonusStats.bonus_currentMaxStam = PlayerPrefs.GetInt("BonusMaxStam", 0);
            charComponents._characterBonusStats.bonus_SkillPoints = PlayerPrefs.GetInt("BonusSkillPoints", 0);
        }
    }

    ///////////////////////////////
    public void ResetLevel()
    {
        if (charInfo._isPlayer)
        {
            charInfo._charLevel = 0;
            charStats._xp = 0;
            charComponents._characterBonusStats.bonus_SkillPoints = 0;
            //PlayerPrefs.SetInt("CharacterLevel", currentCharLevel);
            //PlayerPrefs.SetInt("CharacterCurrentXP", (int)currentXP);
            LoadCharStats();
        }

    }
    #endregion

    #region Set/Reset Position
    public void SetCharacterPosition()
    {
        charMove._backupPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z); //y+0.5f ¿eby zrespi³ siê powy¿ej terrain
        charMove._backupRotation = transform.rotation;
        navMeshAge._spawnPoint = transform.position;
    }

    public void ResetCharacterPosition()
    {
        //transform.SetPositionAndRotation(charMove._backupPosition, charMove._backupRotation);
        transform.position = charMove._backupPosition;
        transform.rotation = charMove._backupRotation;
    } 
    #endregion

    public void ResourcesRegen()
    {
        //Stam Regen
        if (charStats._stam < charStats._maxStam && !charStatus._isRunning) charStats._stam = Mathf.MoveTowards(charStats._stam, charStats._maxStam, ((charStats._regenStam + charComponents._characterBonusStats.bonus_currentRegenStam * 0.1f) * charStats._maxStam * 0.01f) * Time.deltaTime); //regeneruje % Max stamy / sekunde //+0.1f bonusRegen
        //HP Regen
        if (charStats._hp < charStats._maxHP) charStats._hp = Mathf.MoveTowards(charStats._hp, charStats._maxHP, ((charStats._regenHP + charComponents._characterBonusStats.bonus_currentRegenHP * 0.1f) * charStats._maxHP * 0.01f) * Time.deltaTime); //regeneruje % Max HP / sekunde  //+0.1f bonusRegen
        //MP Regen
        if (charStats._mp < charStats._maxMP) charStats._mp = Mathf.MoveTowards(charStats._mp, charStats._maxMP, ((charStats._regenMP + charComponents._characterBonusStats.bonus_currentRegenMP * 0.1f) * charStats._maxMP * 0.01f) * Time.deltaTime); //regeneruje % Max MP / sekunde //+0.1f bonusRegen

    }

    /////////////////////////////////

    #region TakeDamage/Heal
    public void TakeDamageInstant(float damageValue, CharacterStatus attacker)
    {
        charStats._hp -= damageValue;
        charComponents._audioSource.PlayOneShot(charComponents._scrObj_CharStats.damagedEnemy, charComponents._scrObj_GameSettings._audioSettings._onTargetHitAudioVolume);
        if (charStats._hp <= 0f && !charStatus._isDead) //zadzia³a tylko raz
        {
            attacker.charStats._xp += charStats._xp_GainedFromKill;
            if (gameObject.CompareTag("Monster")) { charInfo._charLevel = UnityEngine.Random.Range(attacker.charInfo._charLevel - 3, attacker.charInfo._charLevel + 3); }  //po œmierci ustawia level targetu na zbli¿ony do atakuj¹cego
                                                                                                                                                                           //podbija lvl tylko Monsterów, Playera i Environment nie
        }
    }

    public void TakeDamageOverTime(float damageValue, CharacterStatus attacker)
    {
        charStats._hp = Mathf.MoveTowards(charStats._hp, -damageValue, Time.deltaTime * damageValue);
        if (charStats._hp <= 0f && !charStatus._isDead) //zadzia³a tylko raz
        {
            attacker.charStats._xp += charStats._xp_GainedFromKill;
            if (gameObject.CompareTag("Monster")) { charInfo._charLevel = UnityEngine.Random.Range(attacker.charInfo._charLevel - 3, attacker.charInfo._charLevel + 3); }  //po œmierci ustawia level targetu na zbli¿ony do atakuj¹cego
                                                                                                                                                                           //podbija lvl tylko Monsterów, Playera i Environment nie
        }
    }

    public void TakeHealInstant(float healValue)
    {
        if (charStats._hp < charStats._maxHP) charStats._hp += healValue;
        else { charStats._hp = charStats._maxHP; }
    }

    public void TakeHealOverTime(float healValue)
    {
        if (charStats._hp < charStats._maxHP) { charStats._hp = Mathf.MoveTowards(charStats._hp, charStats._maxHP, Time.deltaTime * healValue); }
        else { charStats._hp = charStats._maxHP; }
    }
    #endregion

    /////////////////////////////////

    #region Utils
    /// <summary>
    /// Specjalna klasa s³u¿¹ca do resetu spelli na Destructibles poniewa¿ mimo ¿e nie widaæ czasami mo¿e próbowaæ przypisaæ spell (niby sk¹d?)
    /// Wina tego ¿e Destructiblesy i Charactery s¹ na tym samym skrypcie live_charStats 
    /// </summary>
    public void Utils_BoxSpellsReset()
    {
        if (charInfo._charName == "Box" || gameObject.tag == "Destructibles")
        {
            charSkillCombat._skillArray = null;
            /*charSkillCombat._primarySkill = null;
            charSkillCombat._secondarySkill = null;
            fov._closeRangeSkill = null;
            fov._spellRangeSkill = null;*/
        }
    }

    public void Utils_RestoreResourcesToAll()
    {
        charStats._hp = charStats._maxHP;
        charStats._mp = charStats._maxMP;
        charStats._stam = charStats._maxStam;
        fov._AISpellRangeSkillRadiusFromMax = 0.5f;
    }

    private void SkillsSetup()
    {
        charSkillCombat._skillArray = GetComponentsInChildren<Skill>();  //za³adowanie wszystkich Skill do arraya

        if (!charInfo._isPlayer)    //Skills Select By MaxRange
        {                                                                                                                            
            charSkillCombat._skillArray = charSkillCombat._skillArray.OrderBy(x => x.scrObj_Skill._skillMaxRadius).ToArray();   //ustawianie _skill Array w kolejnoœci od najwiêkszego bazowego SkillRange dla non-Playera
        }
        else
        {
            Skill[] temporaryskillArray = new Skill[charSkillCombat._skillArray.Length];    //ustawienie zale¿ne od tego czy skill jest primary czy secondary [0]=inputType.primary [last]=inputType.secondary (Player)
            if (charSkillCombat._skillArray.First().scrObj_Skill._inputType == ScrObj_skill.InputType.primary)
            {
                temporaryskillArray[0] = charSkillCombat._skillArray.First();
                temporaryskillArray[1] = charSkillCombat._skillArray.Last();
            }
            else
            {
                temporaryskillArray[0] = charSkillCombat._skillArray.Last();
                temporaryskillArray[1] = charSkillCombat._skillArray.First();
            }
            charSkillCombat._skillArray = temporaryskillArray;
        }
        fov._spellRangeSkillMaxRadius = charSkillCombat._skillArray[1].scrObj_Skill._skillMaxRadius;

        for (int s = 0; s < charSkillCombat._skillArray.Length; s++) //Usawiania live_charStats w skillach 
        {
            charSkillCombat._skillArray[s].live_charStats = this;
        }
        

    } 
    #endregion
}
