using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class Skill : MonoBehaviour
{
    [Tooltip("Scriptable Object Skilla"), SerializeField] public ScrObj_skill scrObj_Skill;
    [Tooltip("Live_charStats Castera"), SerializeField] public CharacterStatus live_charStats;
    [Tooltip("Bonus_charStats Castera"), SerializeField] public CharacterBonusStats currentCharacterBonusStats;
    [Tooltip("Ten GameObject skill"), SerializeField] public Skill skill;

    [Header("Input")]
    [Tooltip("Skill input lokalny dla klasy skill, Czy INPUTUJE casta?"), SerializeField] public bool skill_input;
    [Tooltip("Skill input Other lokalny dla klasy skill"), SerializeField] public bool skill_otherInput;

    [Space]
    [Tooltip("Lokalny boolean CanCast dla tego skilla dla klasy skill"), SerializeField] public bool skill_CanCast;

    [Header("Current IsCasting Values")]
    [Tooltip("Aktualny index Castowania -> currentCastingIndex z arraya EffetType [i]")]private int currentCastingIndex;
    [Tooltip("Bool zwracany true jeśli jest skończy castableCast (CastingType -> Finished Castable)"), SerializeField] public bool skill_IsCastingFinishedCastable;
    [Tooltip("Aktualny progress Castowania (CastingType -> Castable)"), SerializeField] public float skill_currentCastingProgress;
    [Space]
    [Tooltip("Bool zwracany true jeśli jest instantCast (CastingType -> Instant)"), SerializeField] public bool skill_IsCastingInstant;
    [Tooltip("Aktualny progress Cooldown skilla "), Range(0f, 1f), SerializeField] public float skill_currentCooldownRemaining;
    [Tooltip("Aktualny progress ComboValue skilla"), Range(0f, 2f), SerializeField] public float skill_currentComboProgress;
    [Space]
    [Tooltip("Bool zwracany true jeśli jest holdCast (CastingType -> Hold)"), SerializeField] public bool skill_IsCastingHold;

    [Header("SkillDamage/Cost - current")]
    [Tooltip("Aktualny Resource cost skilla"), SerializeField] public float _resourceCost;
    [Tooltip("Resource type dla Skilla (HP,Mana,Stamina value (float)) -> z live_charStats"), SerializeField] public float skill_currentResource;
    [Tooltip("Aktualny Damage skilla (zwracany po przeliczeniu)"), SerializeField] public float skill_currentDamage;    
    
    [Header("Targets")]
    [Tooltip("Enemies Array z klasy scr_skill(do bazowego enemies array dopisane Destructibles, (Metoda EnemyArraySelector)"), SerializeField, TagField] public string[] _enemiesArray; //Pozwala na wybór Enemies przy pomocy Tag     
    [Tooltip("Zwracana lista wszystkich colliderów w zasięgu skilla"), CanBeNull, SerializeField] public Collider[] skill_allLocalColliders = new Collider[30];
    [Tooltip("Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)"), CanBeNull, SerializeField] public List<Collider> skill_targetColliders;

    [Tooltip("GameObject skilla (castera) -> potrzebny do transform"), SerializeField] public GameObject skill_casterGameobject;

    [Header("Skill Cone Settings")]
    [Tooltip("Bool zwracający czy w Range jest przeciwnik"), CanBeNull, SerializeField] public bool skill_targetInRange;
    [Tooltip("Bool zwracający czy w Angle(i Range) jest przeciwnik"), CanBeNull, SerializeField] public bool skill_targetInAngle;
    [Space]
    [Tooltip("Aktualny Radius skilla"), CanBeNull, SerializeField] public float skill_currentRadius;
    [Tooltip("Aktualny Kąt skilla"), CanBeNull, SerializeField] public float skill_currentAngle;
    [Space]
    [Tooltip("(ref/refrence) Aktualny wektor(kierunek) w którum porusza się currentRadius skilla"), CanBeNull, SerializeField] public float skill_currentVectorRadius;
    [Tooltip("(ref/refrence) Aktualny wektor(kierunek) w którum porusza się currentAngle skilla"), CanBeNull, SerializeField] public float skill_currentVectorAngle;
   
    [Header("Utils")]
    [Tooltip("AudioSource skilla Instant -> Caster"), SerializeField] public AudioSource skill_AudioSourceInstant;
    [Tooltip("AudioSource skilla Castable -> Caster"), SerializeField] public AudioSource skill_AudioSourceCastable;
    [Tooltip("AudioSource skilla Hold -> Caster"), SerializeField] public AudioSource skill_AudioSourceHold;

    
    
    
    [Tooltip("AudioSource skilla -> Caster"), SerializeField] public AudioSource skill_AudioSourceCaster;
    [Tooltip("VFX skilla -> Caster"), CanBeNull, SerializeField] public VisualEffect skill_CastingVisualEffect;


    [Serializable]
    public class EffectDynamicValues
    {
        [Tooltip("Aktualny Damage skilla (zwracany po przeliczeniu)"), SerializeField] public float _currentDamage;
    }    
    
    [Serializable]
    public class TargetDynamicValues
    {
        [Header("Skill Cone Settings")]
        [Tooltip("Bool zwracający czy w Range jest przeciwnik"), CanBeNull, SerializeField] public bool _targetInRange;
        [Tooltip("Bool zwracający czy w Angle(i Range) jest przeciwnik"), CanBeNull, SerializeField] public bool _targetInAngle;
        [Space]
        [Tooltip("Aktualny Radius skilla"), CanBeNull, SerializeField] public float _currentRadius;
        [Tooltip("Aktualny Kąt skilla"), CanBeNull, SerializeField] public float _currentAngle;
        [Space]
        [Tooltip("(ref/refrence) Aktualny wektor(kierunek) w którum porusza się currentRadius skilla"), CanBeNull, SerializeField] public float _currentVectorRadius;
        [Tooltip("(ref/refrence) Aktualny wektor(kierunek) w którum porusza się currentAngle skilla"), CanBeNull, SerializeField] public float _currentVectorAngle;


        [Space]
        [Tooltip("Zwracana lista wszystkich colliderów w zasięgu skilla"), CanBeNull, SerializeField] public Collider[] _allLocalColliders = new Collider[40];
        [Tooltip("Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)"), CanBeNull, SerializeField] public List<Collider> _targetColliders;

        public EffectDynamicValues[] effectDynamicValues = new EffectDynamicValues[10];
    }
    public TargetDynamicValues[] targetDynamicValues = new TargetDynamicValues[10];



    private void OnValidate()
    {
        skill = this;
        //QuickSetup(live_charStats, scrObj_Skill, skill);  

        /*if(live_charStats.charInfo._isPlayer)
        {
            Debug.Log("0 -" + scrObj_Skill.skill_EffectTypeArray[0]);
            Debug.Log("1 -" + scrObj_Skill.skill_EffectTypeArray[1]);
            Debug.Log("2 -" + scrObj_Skill.skill_EffectTypeArray[2]);
        }  */

    }

    private void FixedUpdate()
    {
        Static_SkillForge.Utils.Skill_EveryFrameValuesUpdate(scrObj_Skill, skill, live_charStats, currentCharacterBonusStats);        
             
        Skill_UniversalCasting(scrObj_Skill);

        Skill_TargetType(scrObj_Skill);

        if (live_charStats.charStatus._isCasting && skill_CanCast)
        {     
            Skill_EffectTypeArray(scrObj_Skill, skill);
        }
    }

    /// <summary>
    /// <br>Refresz SerializeFields -> inspector (trzeba włączyć player Skeletona (wszystkie disabled Chars) inaczej wali nullException)</br>
    /// <br>live_charStats</br>
    /// <br>currentCharacterBonusStats</br>
    /// <br>skill_casterGameobject</br>
    /// <br>skill_AudioSource</br>
    /// <br>skill_CastingVisualEffect</br>
    /// <br>ScrObj_skill.Skill_InputType</br>
    /// </summary>
    void QuickSetup(CharacterStatus live_charStats, ScrObj_skill scrObj_Skill, Skill skill)
    {
        skill = this;

        live_charStats = GetComponentInParent<CharacterStatus>();
        currentCharacterBonusStats= GetComponentInParent<CharacterBonusStats>();
        skill_casterGameobject = gameObject;
        //skill_AudioSourceInstant= GetComponentInParent<AudioSource>();
        skill_CastingVisualEffect= GetComponent<VisualEffect>();

        switch (scrObj_Skill.skill_InputType)
        {
            case ScrObj_skill.Skill_InputType.primary:
                live_charStats.charSkillCombat._primarySkill = skill;
                break;
            case ScrObj_skill.Skill_InputType.secondary:
                live_charStats.charSkillCombat._secondarySkill = skill;
                break;
        }

        Static_SkillForge.Utils.Skill_EnemyArraySelector(skill, live_charStats);

        //Skills Select By MaxRange
        if (live_charStats.charSkillCombat._primarySkill != null && live_charStats.charSkillCombat._secondarySkill != null)
        {
            if (live_charStats.charSkillCombat._primarySkill.scrObj_Skill.skill_MaxRadius < live_charStats.charSkillCombat._secondarySkill.scrObj_Skill.skill_MaxRadius)
            {
                live_charStats.fov._closeRangeSkill = live_charStats.charSkillCombat._primarySkill;
                live_charStats.fov._spellRangeSkill = live_charStats.charSkillCombat._secondarySkill;
                live_charStats.fov._closeRangeSkillMinRadius = live_charStats.charSkillCombat._primarySkill.scrObj_Skill.skill_MinRadius;
                live_charStats.fov._spellRangeSkillMaxRadius = live_charStats.charSkillCombat._secondarySkill.scrObj_Skill.skill_MaxRadius;
            }
            else
            {
                live_charStats.fov._closeRangeSkill = live_charStats.charSkillCombat._secondarySkill;
                live_charStats.fov._spellRangeSkill = live_charStats.charSkillCombat._primarySkill;
                live_charStats.fov._closeRangeSkillMinRadius = live_charStats.charSkillCombat._secondarySkill.scrObj_Skill.skill_MinRadius;
                live_charStats.fov._spellRangeSkillMaxRadius = live_charStats.charSkillCombat._primarySkill.scrObj_Skill.skill_MaxRadius;
            }
        }
    }

    /// <summary>
    /// Metoda odpowiedzialna za wystartowanie skilla
    /// <br><i>Sprawdza warunek CanCast oraz w zależności od tego czy biega wyłącza odpowiednie CastingType</i></br>
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    private void Skill_UniversalCasting(ScrObj_skill scrObj_Skill)
    {
        skill_CanCast = !skill.skill_otherInput && !live_charStats.charStatus._isDead;

        if (skill_CanCast)  //CanCast jest wykorzystane w inpucie więc warunkiem nie może być resource!!!
        {
            Static_SkillForge.CastingType.Skill_CastingUniversal_VFX_Audio(scrObj_Skill, skill, live_charStats);

           /* brzydko wygląda na NonPlayerach
            * if (live_charStats.charStatus._isRunning || live_charStats.charMove._moveSpeed > live_charStats.charMove._walkSpeed)  //Jeśli biegnie lub speed jest większy od walkSpeed to resetuje wszystkie casty oprócz Instant
            {
                Static_SkillForge.Utils.Skill_ResetAnyCastingExceptInstant(scrObj_Skill, skill, live_charStats);
            }*/

            if (live_charStats.charInfo._isPlayer) //W miare na nonPlayerach
            {
                if (live_charStats.charStatus._isRunning || live_charStats.charMove._moveSpeed > live_charStats.charMove._walkSpeed)  //Jeśli biegnie lub speed jest większy od walkSpeed to resetuje wszystkie casty oprócz Instant
                {
                    Static_SkillForge.Utils.Skill_ResetAnyCastingExceptInstant(scrObj_Skill, skill, live_charStats);
                }
            }
            else
            {
                if (live_charStats.charStatus._isRunning || live_charStats.charMove._moveSpeed > live_charStats.charMove._walkSpeed)
                {
                    Static_SkillForge.Utils.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats);
                }
            }

        }
        else { Static_SkillForge.Utils.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats); }




        if (live_charStats.charStatus._isDead) { Static_SkillForge.Utils.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats); }
    }    

    private void Skill_EffectTypeArray(ScrObj_skill scrObj_Skill, Skill skill)
    {
        for (currentCastingIndex = 0; currentCastingIndex < scrObj_Skill.skill_EffectTypeArray.Length; currentCastingIndex++)
        {   
            switch (scrObj_Skill.skill_EffectTypeArray[currentCastingIndex])
            {
                case ScrObj_skill.Skill_EffectTypeArray.none:
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.hit:
                    Static_SkillForge.EffectType.Skill_Hit(scrObj_Skill, skill, live_charStats, Static_SkillForge.Utils.Skill_CastingTypeCurrentFloatReadOnly(currentCastingIndex, skill));
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.boom:
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.pierce:
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.chain:
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.damageOverTime:
                    Static_SkillForge.EffectType.Skill_DamageOverTime(scrObj_Skill, skill, live_charStats, Static_SkillForge.Utils.Skill_CastingTypeCurrentFloatReadOnly(currentCastingIndex, skill));
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.healOverTime:
                    Static_SkillForge.EffectType.Skill_HealOverTime(scrObj_Skill, skill, live_charStats, Static_SkillForge.Utils.Skill_CastingTypeCurrentFloatReadOnly(currentCastingIndex, skill));
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.heal:
                    Static_SkillForge.EffectType.Skill_HealOverTime(scrObj_Skill, skill, live_charStats, Static_SkillForge.Utils.Skill_CastingTypeCurrentFloatReadOnly(currentCastingIndex, skill));
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.summon:
                    break;
            }
        }        
    }

    private void Skill_TargetType(ScrObj_skill scrObj_Skill)
    {
        if (!live_charStats.charStatus._isDead /*&& live_charStats.skill_CanCast*/) //tylko dla żywych :P
        {
            switch (scrObj_Skill.skill_TargetType)
            {
                case ScrObj_skill.Skill_TargetType.melee:
                    Static_SkillForge.TargetType.Skill_Melee_Target(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_TargetType.cone:
                    Static_SkillForge.TargetType.Skill_Cone_Target(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_TargetType.projectile:

                    break;

                case ScrObj_skill.Skill_TargetType.aoeMouse:

                    break;

                case ScrObj_skill.Skill_TargetType.self:
                    Static_SkillForge.TargetType.Skill_Self_Target(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_TargetType.summon:

                    break;
            }
        }
    }     

    private void Skill_NewSkillMechanic(ScrObj_skill scrObj_Skill)
    {
        skill_CanCast = !skill.skill_otherInput && !live_charStats.charStatus._isDead;


        if (skill.skill_input && skill.skill_CanCast) 
        {
            for (int i = 0; i < scrObj_Skill.new_TargetType.Length; i++)
            {
                for (int j = 0; j < scrObj_Skill.new_TargetType[i].new_EffectType.Length; j++)
                {
                    switch (scrObj_Skill.new_EnumCastingType)
                    {
                       
                        case ScrObj_skill.New_EnumCastingType.Instant:
                            {
                                // static casting dla Instant
                            }
                            break;

                        case ScrObj_skill.New_EnumCastingType.Hold:
                            {
                                if (!live_charStats.charStatus._isRunning || live_charStats.charMove._moveSpeed <= 0.2f)
                                {
                                    // static casting dla Hold
                                }
                            }
                            break;

                        case ScrObj_skill.New_EnumCastingType.Castable:
                            {
                                if (!live_charStats.charStatus._isRunning || live_charStats.charMove._moveSpeed <= 0.2f)
                                {
                                    // static casting dla Castable
                                }
                            }
                            break;
                    }
                }
            }
        }
        else
        {
            Static_SkillForge.Utils.Skill_ResetCasting(scrObj_Skill, skill, live_charStats);
            for (int i = 0; i < scrObj_Skill.new_TargetType.Length; i++) //Reset całej tablicy targetType 
            {                
                Static_SkillForge.Utils.Skill_Target_Reset(skill, i); //Reset TargetList w targetTypeIndexie
            }                
        }
    }

#if UNITY_EDITOR //zamiast skryptu w Editor

    /*private void OnDrawGizmos() //rusyje wszystkie
    {
        GizmosDrawer();
    }*/
    private void OnDrawGizmosSelected() //rysuje tylko zaznaczone
    {
        GizmosDrawer();
    }

    private void GizmosDrawer()
    {
        Handles.color = live_charStats.charSkillCombat._skillAngleColor;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.AngleAxis(-(skill_currentAngle / 2), Vector3.up) * transform.forward, skill_currentAngle, skill_currentRadius); //rysuje coneAngle view               


        if (skill_targetInRange && skill_targetInAngle)
        {
            Handles.color = live_charStats.charSkillCombat._skillRaycastColor;
            for (int i = 0; i < skill_targetColliders.Count; i++)
            {
                Handles.DrawLine(transform.position, skill_targetColliders[i].transform.position, live_charStats.fov._editorLineThickness); //rysowanie lini w kierunku targetów breatha jeśli nie zasłania go obstacle Layer
                /*Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(Camera.main.aspect, 1f, 1f));
                Gizmos.DrawFrustum(transform.position, skill_currentAngle,skill_currentRadius,0f,1);*/
            }

        }
    }

#endif

}


