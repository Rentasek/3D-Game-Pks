using Cinemachine;
using JetBrains.Annotations;
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

    [Header("Testing/Debug")]
    [Tooltip("Skill input lokalny dla klasy skill, Czy INPUTUJE casta?"), SerializeField] public bool skill_input;
    [Tooltip("Skill input Other lokalny dla klasy skill"), SerializeField] public bool skill_otherInput;
    [Tooltip("Aktualny progress Castowania (CastingType -> Castable)"), SerializeField] public float skill_currentCastingProgress;
    [Tooltip("Bool zwracany true jeśli progress castowania dojdzie do 100% (CastingType -> Castable)"), SerializeField] public bool skill_currentCastingFinished;
    [Tooltip("Bool zwracany true jeśli jest instantCast (CastingType -> Instant)"), SerializeField] public bool skill_currentCastingInstant;
    
    [Header("Targets")]
    [Tooltip("Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)"), CanBeNull, SerializeField] public List<Collider> skill_targetColliders;
    [Tooltip("Enemies Array z klasy skill(do bazowego enemies array dopisane Destructibles, (Metoda EnemyArraySelector)"), SerializeField, TagField] public string[] skill_EnemiesArray; //Pozwala na wybór Enemies przy pomocy Tag     
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

    [Header("SkillDamage/Cost - current")]
    [Tooltip("Aktualny Resource cost skilla"), SerializeField] public float skill_currentResourceCost;
    [Tooltip("Resource type dla Skilla (HP,Mana,Stamina value (float)) -> z live_charStats"), SerializeField] public float skill_currentResource;
    [Tooltip("Aktualny Damage skilla (zwracany po przeliczeniu)"), SerializeField] public float skill_currentDamage;
    [Tooltip("Aktualny Cooldown skilla (zwracany po przeliczeniu)"), SerializeField] public float skill_currentCooldown;
   
    [Header("Utils")]
    [Tooltip("AudioSource skilla -> Caster"), SerializeField] public AudioSource skill_AudioSource;
    [Tooltip("VFX skilla -> Caster"), SerializeField] public VisualEffect skill_CastingVisualEffect;

    private void OnValidate()
    {
        skill = this;
        skill_EnemiesArray = Static_SkillForge.EnemyArraySelector(live_charStats.currentEnemiesArray);

        

        Skill_Prepare();
    }

    private void FixedUpdate()
    {
        Static_SkillForge.ResourceTypeSelector_In(scrObj_Skill, skill, live_charStats);

        
        //Skill_Prepare();
        Skill_Casting(scrObj_Skill);

        Skill_Range(scrObj_Skill);

        Skill_Effect(scrObj_Skill);

        //if (live_charStats.isCasting) { ResourceTypeSelectorRefrence(); }

        Static_SkillForge.ResourceTypeSelector_Out(scrObj_Skill, skill, live_charStats);



    }

    /*public void ResourceTypeSelector()
    {
        switch (scrObj_Skill.skill_ResourceType)
        {
            case ScrObj_skill.Skill_ResourceType.hp:
                ResourceTypeRefrence(ref live_charStats.currentHP);
                break;

            case ScrObj_skill.Skill_ResourceType.mana:
                ResourceTypeRefrence(ref live_charStats.currentMP);
                break;

            case ScrObj_skill.Skill_ResourceType.stamina:
                ResourceTypeRefrence(ref live_charStats.currentStam);
                break;
        }
    }


    public void ResourceTypeSelectorRefrence()
    {
        switch(scrObj_Skill.skill_ResourceType)
        {
            case ScrObj_skill.Skill_ResourceType.hp:
                skill.skill_currentResource = live_charStats.currentHP;
            break;

            case ScrObj_skill.Skill_ResourceType.mana:
                skill.skill_currentResource = live_charStats.currentMP;
            break;

            case ScrObj_skill.Skill_ResourceType.stamina:
                skill.skill_currentResource = live_charStats.currentStam;
            break;
        }
    }

    public void ResourceTypeRefrence(ref float skill_currentResource)
    {
        skill_currentResource = skill.skill_currentResource;
    }*/

    public void Skill_Prepare()
    {        
        

        Static_SkillForge.Skill_ValuesUpdate(scrObj_Skill, skill, live_charStats, currentCharacterBonusStats);

    }

    public void Skill_Casting(ScrObj_skill scrObj_Skill)
    { 
        live_charStats.skill_CanCast = !skill.skill_otherInput && !live_charStats.isRunning && live_charStats.currentMoveSpeed != live_charStats.currentRunSpeed;
        
        if (live_charStats.skill_CanCast)
        {
            switch (scrObj_Skill.skill_CastingType)
            {
                case ScrObj_skill.Skill_CastingType.castable:
                    Static_SkillForge.Skill_Castable_VFX_Audio(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_CastingType.instant:
                    Static_SkillForge.Skill_Instant_VFX_Audio(scrObj_Skill, skill, live_charStats);
                    break;

                case ScrObj_skill.Skill_CastingType.hold:
                    Static_SkillForge.Skill_Hold_VFX_Audio(scrObj_Skill, skill, live_charStats);
                    break;
            }
        }
    }    

    public void Skill_Range(ScrObj_skill scrObj_Skill)
    {
        if (!live_charStats.isDead && live_charStats.skill_CanCast) //tylko dla żywych :P
        {
            switch (scrObj_Skill.skill_RangeType)
            {
                case ScrObj_skill.Skill_RangeType.melee:

                    break;

                case ScrObj_skill.Skill_RangeType.cone:
                    Static_SkillForge.Skill_Cone_AttackConeCheck(scrObj_Skill, skill, live_charStats);
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

    public void Skill_Effect(ScrObj_skill scrObj_Skill)
    {
        switch (scrObj_Skill.skill_EffectType)
        {
            case ScrObj_skill.Skill_EffectType.hit:
                Static_SkillForge.Skill_TargetList_DamageOverTime(skill, live_charStats);
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






#if UNITY_EDITOR //zamiast skryptu w Editor

    private void OnDrawGizmos() //rusyje wszystkie
    {
        GizmosDrawer();
    }
    /*private void OnDrawGizmosSelected() //rysuje tylko zaznaczone
    {
        GizmosDrawer();
    }*/

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


