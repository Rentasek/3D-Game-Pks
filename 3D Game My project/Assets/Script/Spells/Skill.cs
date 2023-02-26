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
    [Tooltip("Skill input lokalny dla klasy skill, Czy INPUTUJE casta?"), SerializeField] public bool _skillInput;
    [Tooltip("Skill input Other lokalny dla klasy skill"), SerializeField] public bool _skillOtherInput;

    [Space]
    [Tooltip("Lokalny boolean CanCast dla tego skilla dla klasy skill"), SerializeField] public bool _canCast;

    [Header("Current IsCasting Values")]   
    [Tooltip("Aktualny progress Castowania"), SerializeField] public float _currentCastingProgress;    
    [Tooltip("Aktualny progress Cooldown skilla "), Range(0f, 1f), SerializeField] public float _currentCooldownRemaining;
    [Tooltip("Aktualny progress ComboValue skilla"), Range(0f, 2f), SerializeField] public float _currentComboProgress;   

    [Header("SkillDamage/Cost - current")]
    [Tooltip("Aktualny Resource cost skilla"), SerializeField] public float _resourceCost;

    [Header("Targets")]
    [Tooltip("Enemies Array z klasy scr_skill(do bazowego enemies array dopisane Destructibles, (Metoda EnemyArraySelector)"), SerializeField, TagField] public string[] _enemiesArray; //Pozwala na wybór Enemies przy pomocy Tag 
    #region TargetDynamicValues
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
        [Tooltip("Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)"), CanBeNull, SerializeField] public List<Collider> _targetColliders;

        [Space]
        [Tooltip("Zmienne do tablicy SkillEffect"), CanBeNull, SerializeField]
        public EffectDynamicValues[] effectDynamicValues = new EffectDynamicValues[3];
    }
    #endregion
    [Tooltip("Zmienne do tablicy SkillTarget"), CanBeNull, SerializeField] public TargetDynamicValues[] targetDynamicValues = new TargetDynamicValues[3];
    [Tooltip("Zwracana lista wszystkich colliderów w zasięgu skilla"), CanBeNull, SerializeField] public Collider[] _allLocalColliders = new Collider[30];
    [Tooltip("Zwracana lista colliderów zgodnych z parametrami(EnemyTag,InCurrentRadius,InCurrentAngle)"), CanBeNull, SerializeField] public List<Collider> skill_targetColliders;

    [Tooltip("GameObject skilla (castera) -> potrzebny do transform"), SerializeField] public GameObject _casterGameobject;   
    [Header("Utils")]

    [Tooltip("AudioSource skilla -> Caster"), SerializeField] public AudioSource _audioSourceCaster;
    [Tooltip("VFX skilla -> Caster"), CanBeNull, SerializeField] public VisualEffect _castingVisualEffect;
    [Tooltip("VFX skilla Chain -> Caster"), CanBeNull, SerializeField] public VisualEffect _chainVisualEffect;
    [Tooltip("VFX skilla Projectile -> Caster"), CanBeNull, SerializeField] public VisualEffect _projectileVisualEffect;

    private void OnValidate()
    {
        skill = this;

        //QuickSetup(scrObj_Skill, skill, live_charStats);

        //DynamicTargetArraySetup(scrObj_Skill, skill);  //rozwala refrencowanie - nie używać        
    }

    private void FixedUpdate()
    {
        Skill_SkillCastMechanic(scrObj_Skill, skill, live_charStats, currentCharacterBonusStats);   
    }

    #region QuickSetup
    /// <summary>
    /// <br>Refresz SerializeFields -> inspector (trzeba włączyć player Skeletona (wszystkie disabled Chars) inaczej wali nullException)</br>
    /// <br>live_charStats</br>
    /// <br>currentCharacterBonusStats</br>
    /// <br>skill_casterGameobject</br>
    /// <br>skill_AudioSource</br>
    /// <br>skill_CastingVisualEffect</br>
    /// <br>ScrObj_skill.Skill_InputType</br>
    /// </summary> 
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    /// <param name="live_charStats">Live_charStats Castera</param>
    private void QuickSetup(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats)
    {
        live_charStats = GetComponentInParent<CharacterStatus>();
        currentCharacterBonusStats = GetComponentInParent<CharacterBonusStats>();
        _casterGameobject = gameObject;
        _audioSourceCaster = GetComponent<AudioSource>(); 
        _castingVisualEffect = GetComponent<VisualEffect>();

        switch (scrObj_Skill._inputType)
        {
            case ScrObj_skill.InputType.primary:
                live_charStats.charSkillCombat._primarySkill = skill;
                break;
            case ScrObj_skill.InputType.secondary:
                live_charStats.charSkillCombat._secondarySkill = skill;
                break;
        }

        SkillForge.Utils.Skill_EnemyArraySelector(skill, live_charStats);

        //Skills Select By MaxRange
        if (live_charStats.charSkillCombat._primarySkill != null && live_charStats.charSkillCombat._secondarySkill != null)
        {
            if (live_charStats.charSkillCombat._primarySkill.scrObj_Skill._skillMaxRadius < live_charStats.charSkillCombat._secondarySkill.scrObj_Skill._skillMaxRadius)
            {
                live_charStats.fov._closeRangeSkill = live_charStats.charSkillCombat._primarySkill;
                live_charStats.fov._spellRangeSkill = live_charStats.charSkillCombat._secondarySkill;
                live_charStats.fov._spellRangeSkillMaxRadius = live_charStats.charSkillCombat._secondarySkill.scrObj_Skill._skillMaxRadius;
            }
            else
            {
                live_charStats.fov._closeRangeSkill = live_charStats.charSkillCombat._secondarySkill;
                live_charStats.fov._spellRangeSkill = live_charStats.charSkillCombat._primarySkill;
                live_charStats.fov._spellRangeSkillMaxRadius = live_charStats.charSkillCombat._primarySkill.scrObj_Skill._skillMaxRadius;
            }
        }
    }
    #endregion

    #region DynamicTargetArraySetup / Nie działa - nie używać, psuje refrence dynamicTargetValues
    /// <summary>
    /// Ustawianie długości tablic dla dynamicznych klas targetDynamicValues i effectDynamicValues  //rozwala refrencowanie - nie używać
    /// </summary>
    /// <param name="scrObj_Skill">Scriptable Object Skilla</param>
    /// <param name="skill">Ten GameObject skill</param>
    void DynamicTargetArraySetup(ScrObj_skill scrObj_Skill, Skill skill)
    {
        skill.targetDynamicValues = new TargetDynamicValues[scrObj_Skill._targetTypes.Length];

        /*for (int i = 0; i < skill.targetDynamicValues.Length; i++)
        {
            skill.targetDynamicValues[i].effectDynamicValues = new EffectDynamicValues[scrObj_Skill.new_TargetType[i].new_EffectType.Length];
        }*/
    } 
    #endregion

    private void Skill_SkillCastMechanic(ScrObj_skill scrObj_Skill, Skill skill, CharacterStatus live_charStats, CharacterBonusStats currentCharacterBonusStats)
    {
        SkillForge.Utils.Skill_EveryFrameValuesUpdate_new(scrObj_Skill, skill, live_charStats, currentCharacterBonusStats);

        _canCast = !skill._skillOtherInput && !live_charStats.charStatus._isDead;

        if (skill._skillInput && skill._canCast) 
        {
            
            switch (scrObj_Skill._castingType)
            {
                case ScrObj_skill.CastingType.Instant:
                    {
                        SkillForge.CastingType.CastingInstant(scrObj_Skill, skill, live_charStats);    // static casting dla Instant
                    }
                    break;
                case ScrObj_skill.CastingType.Hold:
                    {
                        if (!live_charStats.charStatus._isRunning || live_charStats.charMove._moveSpeed <= 0.2f)
                        {
                            SkillForge.CastingType.CastingHold(scrObj_Skill, skill, live_charStats);   // static casting dla Hold
                        }
                    }
                    break;
                case ScrObj_skill.CastingType.Castable:
                    {
                        if (!live_charStats.charStatus._isRunning || live_charStats.charMove._moveSpeed <= 0.2f)
                        {
                            SkillForge.CastingType.CastingCastable(scrObj_Skill, skill, live_charStats);    // static casting dla Castable
                        }
                    }
                    break;
            }
        }
        else
        {
            SkillForge.Utils.Skill_ResetCastingAndVFXAnimsAudio(scrObj_Skill, skill, live_charStats);
            for (int targetTypeIndex = 0; targetTypeIndex < scrObj_Skill._targetTypes.Length; targetTypeIndex++) //Dla wszystkich Elementów w TargetType[] //Reset całej tablicy targetType 
            {                
                //SkillForge.Utils.Skill_ResetTargetList(scrObj_Skill, skill, live_charStats, targetTypeIndex); //Reset TargetList w targetTypeIndexie
                SkillForge.Utils.Skill_ResetTargetList(scrObj_Skill, skill, live_charStats, targetTypeIndex);
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
        for (int i = 0; i < targetDynamicValues.Length; i++)
        {
            Handles.color = live_charStats.charSkillCombat._skillAngleColor;
            Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.AngleAxis(-(targetDynamicValues[i]._currentAngle / 2), Vector3.up) * transform.forward, targetDynamicValues[i]._currentAngle, targetDynamicValues[i]._currentRadius); //rysuje coneAngle view               


            if (targetDynamicValues[i]._targetInRange && targetDynamicValues[i]._targetInAngle)
            {
                Handles.color = live_charStats.charSkillCombat._skillRaycastColor;
                for (int j = 0; j < targetDynamicValues[i]._targetColliders.Count; j++)
                {
                    Handles.DrawLine(_casterGameobject.transform.position, targetDynamicValues[i]._targetColliders[i].transform.position, live_charStats.fov._editorLineThickness); //rysowanie lini w kierunku targetów breatha jeśli nie zasłania go obstacle Layer
                    /*Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(Camera.main.aspect, 1f, 1f));
                    Gizmos.DrawFrustum(transform.position, skill_currentAngle,skill_currentRadius,0f,1);*/
                }
            }
        }        
    }
#endif

}


