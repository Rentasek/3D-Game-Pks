using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "SpellInfo", menuName = "Chars/SpellInfo")]
public class ScrObj_skill : ScriptableObject
{
    public enum Skill_InputCastingType
    {
        primary, secondary
    }
    [SerializeField] public Skill_InputCastingType skill_InputCastingType;

    public enum Skill_CastingType
    {
        castable, instant, hold
    }
    [SerializeField] public Skill_CastingType[] skill_CastingType;

    public enum Skill_RangeType
    {
        melee, cone, projectile, aoeMouse, self, summon
    }
    [SerializeField] public Skill_RangeType[] skill_RangeType;

    public enum Skill_EffectType
    {
        hit, boom, pierce, chain, dot, hot, heal, summon
    }
    [SerializeField] public Skill_EffectType[] skill_EffectType;

    public enum Skill_ResourceType
    {
        hp, mana, stamina
    }
    [SerializeField] public Skill_ResourceType[] skill_ResourceType;


    [Space]
    [Header("CastingTypes")]
    public int skill_CastingTypesCount; 
    
    [Header("SkillRange")]
    [SerializeField] public float[] skill_MinRadius;
    [SerializeField] public float[] skill_MinAngle;
    [SerializeField] public float[] skill_MaxRadius;
    [SerializeField] public float[] skill_MaxAngle;
    [SerializeField] public int skill_RangeTypesCount;    

    [Header("SkillTime")]
    [Tooltip("Ile czasu potrzeba do Max/Min Radius"), SerializeField] public float[] skill_TimeMaxRadius;
    [Tooltip("Ile czasu potrzeba do Max/Min Angle"), SerializeField] public float[] skill_TimeMaxAngle;
    [Tooltip("Ile czasu potrzeba do Deploy Cast"), SerializeField] public float[] skill_TimeCast;

    [Header("EffectTypes")]
    [SerializeField] public int skill_EffectTypesCount;

    [Header("SkillDamage/Cost")]
    [SerializeField] public float[] skill_BaseResourceCost;
    [SerializeField] public float[] skill_BaseDamage;
    [SerializeField] public float[] skill_BaseCooldown;
    [SerializeField] public float[] skill_Multiplier;
    [SerializeField] public int skill_ResourceTypesCount;

    [Header("SkillObstacles")]
    [SerializeField] public LayerMask skill_ObstaclesMask; //Obstacles dla skilla

    [Header("SkillAudio")]    
    [Tooltip("AudioClip skilla -> Caster"), SerializeField] public AudioClip[] skill_CastingAudioClip;    
    [Tooltip("AudioClip skilla -> Target"), SerializeField] public AudioClip[] skill_OnHitAudioClip;
    [Tooltip("VFX skilla -> Target"), SerializeField] public VisualEffect[] skill_OnHitVisualEffect;
    [Tooltip("AudioVolume skilla -> Caster"), SerializeField] public float[] skill_CastingAudioVolume;
    [Tooltip("AudioVolume skilla -> Target"), SerializeField] public float[] skill_OnHitAudioVolume;    

    [Header("Animator Values")]
    [Tooltip("Jaki Float Animatora odpowiada za animację? - Nazwa"), SerializeField] public string skill_AnimatorFloat;
    [Tooltip("Jaki Trigger Animatora odpowiada za animację?  - Nazwa"), SerializeField] public string skill_AnimatorTrigger;
    [Tooltip("Jaki Bool Animatora odpowiada za animację? - Nazwa "), SerializeField] public string skill_AnimatorBool;

    [Tooltip("Na którym combo colider działa?"), SerializeField] public float[] skill_onCombo;

    [Header("SkillVariable VFX")]
    [SerializeField] public float[] skill_ParticlesPerSec;
    [SerializeField] public float[] skill_LifetimeMin;
    [SerializeField] public float[] skill_LifetimeMax;
    [SerializeField] public float[] skill_MovingTowardsFactor;

}


