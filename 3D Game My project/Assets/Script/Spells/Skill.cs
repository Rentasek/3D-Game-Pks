using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Current IsCasting Values")]
    [Tooltip("Bool zwracany true jeśli jest skończy castableCast (CastingType -> Finished Castable)"), SerializeField] public bool skill_IsCastingFinishedCastable;
    [Tooltip("Aktualny progress Castowania (CastingType -> Castable)"), SerializeField] public float skill_currentCastingProgress;
    [Space]
    [Tooltip("Bool zwracany true jeśli jest instantCast (CastingType -> Instant)"), SerializeField] public bool skill_IsCastingInstant;
    [Tooltip("Aktualny progress Cooldown skilla "), Range(0f, 1f), SerializeField] public float skill_currentCooldownRemaining;
    [Tooltip("Aktualny progress ComboValue skilla"), Range(0f, 1f), SerializeField] public float skill_currentComboProgress;
    [Space]
    [Tooltip("Bool zwracany true jeśli jest holdCast (CastingType -> Hold)"), SerializeField] public bool skill_IsCastingHold;

    [Header("SkillDamage/Cost - current")]
    [Tooltip("Aktualny Resource cost skilla"), SerializeField] public float skill_currentResourceCost;
    [Tooltip("Resource type dla Skilla (HP,Mana,Stamina value (float)) -> z live_charStats"), SerializeField] public float skill_currentResource;
    [Tooltip("Aktualny Damage skilla (zwracany po przeliczeniu)"), SerializeField] public float skill_currentDamage;    
    
    [Header("Targets")]
    [Tooltip("Enemies Array z klasy scr_skill(do bazowego enemies array dopisane Destructibles, (Metoda EnemyArraySelector)"), SerializeField, TagField] public string[] skill_EnemiesArray; //Pozwala na wybór Enemies przy pomocy Tag     
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
    [Tooltip("AudioSource skilla -> Caster"), SerializeField] public AudioSource skill_AudioSource;
    [Tooltip("VFX skilla -> Caster"), CanBeNull, SerializeField] public VisualEffect skill_CastingVisualEffect;

    private void OnValidate()
    {
        //QuickSetup(); //Refresz SerializeFields -> inspector (trzeba włączyć player Skeletona inaczej wali nullException)

        skill = this;
        skill_EnemiesArray = Static_SkillForge.EnemyArraySelector(live_charStats.currentEnemiesArray);
        
    }

    private void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Keypad0)) { skill.skill_currentCooldownRemaining = 1f; skill.skill_currentComboProgress = 1f; } ///Testing

        Static_SkillForge.Skill_EveryFrameValuesUpdate(scrObj_Skill, skill, live_charStats, currentCharacterBonusStats);


        //Skill_Casting(scrObj_Skill);

        Skill_UniversalCasting(scrObj_Skill);

        Skill_Range(scrObj_Skill);

        if (live_charStats.isCasting && live_charStats.skill_CanCast)
        {     
            //Skill_Effect(scrObj_Skill);
            Skill_EffectTypeArray(scrObj_Skill, skill);
        }
        
    }
    
    void QuickSetup()
    {
        live_charStats = GetComponentInParent<CharacterStatus>();
        currentCharacterBonusStats= GetComponentInParent<CharacterBonusStats>();
        skill_casterGameobject = gameObject;
        skill_AudioSource= GetComponentInParent<AudioSource>();
        skill_CastingVisualEffect= GetComponent<VisualEffect>();
    }


    private void Skill_UniversalCasting(ScrObj_skill scrObj_Skill)
    {
        live_charStats.skill_CanCast = /*!skill.skill_otherInput &&*/ !live_charStats.isRunning && live_charStats.currentMoveSpeed != live_charStats.currentRunSpeed && !live_charStats.isDead;

        if (live_charStats.skill_CanCast)
        {
            Static_SkillForge.Skill_CastingUniversal_VFX_Audio(scrObj_Skill, skill, live_charStats);
        }
        else Static_SkillForge.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats);
    }

    /*private void Skill_Casting(ScrObj_skill scrObj_Skill)
    {
        live_charStats.skill_CanCast = !skill.skill_otherInput && !live_charStats.isRunning && live_charStats.currentMoveSpeed != live_charStats.currentRunSpeed;

        if (live_charStats.skill_CanCast)
        {
            switch (scrObj_Skill.skill_CastingType)
            {
                case ScrObj_skill.Skill_CastingType.Castable:
                    Static_SkillForge.Skill_Castable_VFX_Audio(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_CastingType.Instant:
                    Static_SkillForge.Skill_Instant_VFX_Audio(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_CastingType.Hold:
                    Static_SkillForge.Skill_Hold_VFX_Audio(scrObj_Skill, skill, live_charStats);
                    break;
            }
        }
        else Static_SkillForge.Skill_ResetAnyCasting(scrObj_Skill, skill, live_charStats);
    }*/

    private void Skill_EffectTypeArray(ScrObj_skill scrObj_Skill, Skill skill)
    {
        for (int i = 0; i < scrObj_Skill.skill_EffectTypeArray.Length; i++)
        {   
            switch (scrObj_Skill.skill_EffectTypeArray[i])
            {
                case ScrObj_skill.Skill_EffectTypeArray.none:
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.hit:
                    Static_SkillForge.Skill_Hit(scrObj_Skill, skill, live_charStats, Static_SkillForge.CastingTypeCurrentFloatReadOnly(i, skill));
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.boom:
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.pierce:
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.chain:
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.damageOverTime:
                    Static_SkillForge.Skill_DamageOverTime(scrObj_Skill, skill, live_charStats, Static_SkillForge.CastingTypeCurrentFloatReadOnly(i, skill));
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.healOverTime:
                    Static_SkillForge.Skill_HealOverTime(scrObj_Skill, skill, live_charStats, Static_SkillForge.CastingTypeCurrentFloatReadOnly(i, skill));
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.heal:
                    Static_SkillForge.Skill_HealOverTime(scrObj_Skill, skill, live_charStats, Static_SkillForge.CastingTypeCurrentFloatReadOnly(i, skill));
                    break;

                case ScrObj_skill.Skill_EffectTypeArray.summon:
                    break;
            }
        }        
    }



    private void Skill_Range(ScrObj_skill scrObj_Skill)
    {
        if (!live_charStats.isDead && live_charStats.skill_CanCast) //tylko dla żywych :P
        {
            switch (scrObj_Skill.skill_RangeType)
            {
                case ScrObj_skill.Skill_RangeType.melee:
                    Static_SkillForge.Skill_Melee_Target(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_RangeType.cone:
                    Static_SkillForge.Skill_Cone_Target(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_RangeType.projectile:

                    break;

                case ScrObj_skill.Skill_RangeType.aoeMouse:

                    break;

                case ScrObj_skill.Skill_RangeType.self:
                    Static_SkillForge.Skill_Self_Target(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_RangeType.summon:

                    break;
            }
        }
    }

    /*private void Skill_Effect(ScrObj_skill scrObj_Skill)
    {
        switch (scrObj_Skill.skill_EffectType)
        {
            case ScrObj_skill.Skill_EffectType.hit:                
                break;

            case ScrObj_skill.Skill_EffectType.boom:
                break;

            case ScrObj_skill.Skill_EffectType.pierce:
                break;

            case ScrObj_skill.Skill_EffectType.chain:
                break;

            case ScrObj_skill.Skill_EffectType.damageOverTime:
                Static_SkillForge.Skill_DamageOverTime(scrObj_Skill, skill, live_charStats);
                break;

            case ScrObj_skill.Skill_EffectType.healOverTime:
                break;

            case ScrObj_skill.Skill_EffectType.heal:
                break;

            case ScrObj_skill.Skill_EffectType.summon:
                break;
        }
    }*/


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
        Handles.color = live_charStats.spell_breathAngleColor;
        Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.AngleAxis(-(skill_currentAngle / 2), Vector3.up) * transform.forward, skill_currentAngle, skill_currentRadius); //rysuje coneAngle view               


        if (skill_targetInRange && skill_targetInAngle)
        {
            Handles.color = live_charStats.spell_breathRaycastColor;
            for (int i = 0; i < skill_targetColliders.Count; i++)
            {
                Handles.DrawLine(transform.position, skill_targetColliders[i].transform.position, live_charStats.fov_editorLineThickness); //rysowanie lini w kierunku targetów breatha jeśli nie zasłania go obstacle Layer
            }

        }
    }

#endif

}


