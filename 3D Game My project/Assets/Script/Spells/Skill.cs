using Cinemachine;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class Skill : MonoBehaviour
{
    [SerializeField] public ScrObj_skill scrObj_Skill;
    [SerializeField] public CharacterStatus live_charStats;
    [SerializeField] public CharacterBonusStats currentCharacterBonusStats;


    [Header("Testing/Debug")]
    [SerializeField] public bool skill_input;
    [SerializeField] public bool skill_otherInput;
    [SerializeField] public float skill_currentCastingProgress;
    [SerializeField] public bool skill_currentCastingFinished;
    [SerializeField] public bool skill_currentCastingInstant;
    
    [Header("Targets")]
    [CanBeNull, SerializeField] private List<Collider>[] skill_targetColliders;
    [SerializeField, TagField] public string[] skill_EnemiesArray; //Pozwala na wybór Enemies przy pomocy Tag     
    [SerializeField] public GameObject[] skill_casterGameobject;

    [Header("Skill Cone Settings")]
    [CanBeNull, SerializeField] private bool[] skill_targetInRange;
    [CanBeNull, SerializeField] private bool[] skill_targetInAngle;
    [Space]
    [CanBeNull, SerializeField] private float[] skill_currentRadius;
    [CanBeNull, SerializeField] private float[] skill_currentAngle;
    [Space]
    [CanBeNull, SerializeField] private float[] skill_currentVectorRadius;
    [CanBeNull, SerializeField] private float[] skill_currentVectorAngle;

    [Header("SkillDamage/Cost - current")]
    [SerializeField] public float[] skill_currentResourceCost;
    [SerializeField] public float[] skill_currentResourceType; //HP,Mana,Stamina value (float)
    [SerializeField] public float[] skill_currentDamage;
    [SerializeField] public float[] skill_currentCooldown;

    [Header("Current Types")]
    [SerializeField] public int currentRangeType; //trzyma informacje o numerze currentRangeTtype
    [SerializeField] public int currentResourceType; //trzyma informacje o numerze currentRangeTtype
    [SerializeField] public int currentCastingType; //trzyma informacje o numerze currentRangeTtype
    [SerializeField] public int currentEffectType; //trzyma informacje o numerze currentRangeTtype

    [Header("Utils")]
    [Tooltip("AudioSource skilla -> Caster"), SerializeField] public AudioSource[] skill_AudioSource;
    [Tooltip("VFX skilla -> Caster"), SerializeField] public VisualEffect[] skill_CastingVisualEffect;

    private void OnValidate()
    {
        skill_EnemiesArray = Static_SkillForge.EnemyArraySelector(live_charStats.currentEnemiesArray);        
    }

    private void FixedUpdate()
    {
        if (live_charStats.inputPrimary || live_charStats.inputSecondary) { Skill_Prepare(); } //Jeśli używa jakiegokolwiek inputa -> Skill_Prepare (tak żeby nie preparował non-stop)       

        Skill_Casting(scrObj_Skill);

        Skill_Range(scrObj_Skill);

        Skill_Effect(scrObj_Skill);
    }

    public void Skill_Prepare()
    {
        Static_SkillForge.InputSelector(scrObj_Skill.skill_InputCastingType, skill_input, skill_otherInput, live_charStats.inputPrimary, live_charStats.inputSecondary);

        for (int i = 0; i < scrObj_Skill.skill_ResourceTypesCount; i++)
        {
            Static_SkillForge.ResourceTypeSelector(scrObj_Skill.skill_ResourceType[i], live_charStats.currentHP, live_charStats.currentMP, live_charStats.currentStam, skill_currentResourceType[i]);
        }
        for (int i = 0; i < scrObj_Skill.skill_EffectTypesCount; i++)
        {
            Static_SkillForge.Skill_ValuesUpdate(skill_currentDamage[i], skill_currentResourceCost[i], skill_currentCooldown[i], scrObj_Skill.skill_BaseDamage[i], scrObj_Skill.skill_BaseResourceCost[i],
                scrObj_Skill.skill_BaseCooldown[i], scrObj_Skill.skill_Multiplier[i], live_charStats.currentCharLevel, currentCharacterBonusStats.bonus_Skill_Damage);
        }
       

        
    }

    public void Skill_Casting(ScrObj_skill scrObj_Skill)
    { 
        live_charStats.skill_CanCast = !skill_otherInput && !live_charStats.isRunning && live_charStats.currentMoveSpeed != live_charStats.currentRunSpeed;
        
        if (live_charStats.skill_CanCast)
        {
            for (currentCastingType = 0; currentCastingType < scrObj_Skill.skill_CastingTypesCount; currentCastingType++)
            {
                switch (scrObj_Skill.skill_CastingType[currentCastingType])
                {
                    case ScrObj_skill.Skill_CastingType.castable:
                        Static_SkillForge.Skill_Castable_VFX_Audio(skill_input, live_charStats.isCasting, live_charStats.skill_CanCast, skill_currentResourceType[currentCastingType], skill_currentResourceCost[currentCastingType], skill_currentCastingProgress,
                            skill_currentCastingFinished, scrObj_Skill.skill_TimeCast[currentCastingType], skill_CastingVisualEffect[currentCastingType],
                            skill_AudioSource[currentCastingType], scrObj_Skill.skill_CastingAudioClip[currentCastingType], scrObj_Skill.skill_CastingAudioVolume[currentCastingType], live_charStats.currentAnimator, scrObj_Skill.skill_AnimatorBool);
                        break;

                    case ScrObj_skill.Skill_CastingType.instant:
                        Static_SkillForge.Skill_Instant_VFX_Audio(skill_input, live_charStats.isCasting, live_charStats.skill_CanCast, skill_currentResourceType[currentCastingType], skill_currentResourceCost[currentCastingType], skill_currentCastingInstant,
                            skill_CastingVisualEffect[currentCastingType], skill_AudioSource[currentCastingType], scrObj_Skill.skill_CastingAudioClip[currentCastingType], scrObj_Skill.skill_CastingAudioVolume[currentCastingType], live_charStats.currentAnimator, scrObj_Skill.skill_AnimatorTrigger);
                        break;

                    case ScrObj_skill.Skill_CastingType.hold:
                        Static_SkillForge.Skill_Hold_VFX_Audio(skill_input, live_charStats.isCasting, live_charStats.skill_CanCast, skill_currentResourceType[currentCastingType], skill_currentResourceCost[currentCastingType], skill_CastingVisualEffect[currentCastingType],
                            skill_AudioSource[currentCastingType], scrObj_Skill.skill_CastingAudioClip[currentCastingType], scrObj_Skill.skill_CastingAudioVolume[currentCastingType], live_charStats.currentAnimator, scrObj_Skill.skill_AnimatorBool);
                        break;
                }
            }           
        }
    }    

    public void Skill_Range(ScrObj_skill scrObj_Skill)
    {
        if (!live_charStats.isDead && skill_input && live_charStats.skill_CanCast) //tylko dla żywych :P
        {
            for (currentRangeType = 0; currentRangeType < scrObj_Skill.skill_RangeTypesCount; currentRangeType++)
            {
                switch (scrObj_Skill.skill_RangeType[currentRangeType])
                {
                    case ScrObj_skill.Skill_RangeType.melee:

                        break;

                    case ScrObj_skill.Skill_RangeType.cone:
                        Static_SkillForge.Skill_Cone_AttackConeCheck(live_charStats.isCasting, skill_currentRadius[currentRangeType], skill_currentAngle[currentRangeType], scrObj_Skill.skill_MinRadius[currentRangeType],
                            scrObj_Skill.skill_MaxRadius[currentRangeType], scrObj_Skill.skill_MinAngle[currentRangeType], scrObj_Skill.skill_MaxAngle[currentRangeType], skill_currentVectorRadius[currentRangeType],
                            skill_currentVectorAngle[currentRangeType], scrObj_Skill.skill_TimeMaxRadius[currentRangeType], scrObj_Skill.skill_TimeMaxAngle[currentRangeType], skill_casterGameobject[currentRangeType],
                            live_charStats.currentEnemiesArray, skill_targetInRange[currentRangeType], skill_targetInAngle[currentRangeType], scrObj_Skill.skill_ObstaclesMask, skill_targetColliders[currentRangeType]);
                        break;

                    case ScrObj_skill.Skill_RangeType.projectile:

                        break;

                    case ScrObj_skill.Skill_RangeType.aoeMouse:

                        break;

                    case ScrObj_skill.Skill_RangeType.self:

                        break;

                    case ScrObj_skill.Skill_RangeType.summon:

                        break;
                }
            }
        }
    }

    public void Skill_Effect(ScrObj_skill scrObj_Skill)
    {
        for (currentEffectType = 0; currentEffectType < scrObj_Skill.skill_EffectTypesCount; currentEffectType++)
        {
            switch (scrObj_Skill.skill_EffectType[currentEffectType])
            {
                case ScrObj_skill.Skill_EffectType.hit:                    
                    Static_SkillForge.Skill_Cone_Damage(live_charStats.isCasting, skill_targetColliders[currentRangeType], live_charStats.currentXP, live_charStats.currentMP, skill_currentDamage[currentEffectType],
                        live_charStats.currentCharLevel, skill_currentResourceCost[currentResourceType]);
                    break;

                case ScrObj_skill.Skill_EffectType.boom:
                    break;

                case ScrObj_skill.Skill_EffectType.pierce:
                    break;

                case ScrObj_skill.Skill_EffectType.chain:
                    break;

                case ScrObj_skill.Skill_EffectType.dot:
                    break;

                case ScrObj_skill.Skill_EffectType.hot:
                    break;
                    
                case ScrObj_skill.Skill_EffectType.heal:
                    break;
                
                case ScrObj_skill.Skill_EffectType.summon:
                    break;
            }
        }
    }






#if UNITY_EDITOR //zamiast skryptu w Editor

    private void OnDrawGizmos() //rusyje wszystkie
    {
        //GizmosDrawer();
    }
    /*private void OnDrawGizmosSelected() //rysuje tylko zaznaczone
    {
        GizmosDrawer();
    }*/

    private void GizmosDrawer()
    {
        for (int j = 0; j < scrObj_Skill.skill_RangeTypesCount; j++)
        {
            Handles.color = live_charStats.spell_breathAngleColor;
            Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.AngleAxis(-(skill_currentAngle[j] / 2), Vector3.up) * transform.forward, skill_currentAngle[j], skill_currentRadius[j]); //rysuje coneAngle view               


            if (skill_targetInRange[j] && skill_targetInAngle[j])
            {
                Handles.color = live_charStats.spell_breathRaycastColor;
                for (int i = 0; i < skill_targetColliders[j].Count; i++)
                {
                    Handles.DrawLine(transform.position, skill_targetColliders[j][i].transform.position, live_charStats.fov_editorLineThickness); //rysowanie lini w kierunku targetów breatha jeśli nie zasłania go obstacle Layer
                }

            }
        }       
    }

#endif

}


