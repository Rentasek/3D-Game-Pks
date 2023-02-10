using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "SpellInfo", menuName = "Chars/SpellInfo")]
public class ScrObj_skill : ScriptableObject
{      
    public enum Skill_SkillCastingType
    {
        castable, instant, hold
    }
    [SerializeField] Skill_SkillCastingType skill_skillCastingType;

    public enum Skill_SkillRangeType
    {
        melee, cone, projectile, aoeMouse, self, summon
    }
    [SerializeField] Skill_SkillRangeType skill_skillRangeType;

    public enum Skill_SkillEffectType
    {
        hit, boom, pierce, chain, dot, hot, heal, summon
    }
    [SerializeField] Skill_SkillEffectType[] skill_skillEffectType;

    ///
    [Space]
    [Header("SkillRange")]
    [SerializeField] public float skill_MinRadius;
    [SerializeField] public float skill_MinAngle;
    [SerializeField] public float skill_MaxRadius;
    [SerializeField] public float skill_MaxAngle;
    
    [Header("SkillTime")]
    [Tooltip("Ile czasu potrzeba do Max/Min Radius"), SerializeField] public float skill_TimeMaxRadius;
    [Tooltip("Ile czasu potrzeba do Max/Min Angle"), SerializeField] public float skill_TimeMaxAngle;
    
    [Header("SkillDamage/Cost")]
    [SerializeField] public float skill_BaseMPCost;
    [SerializeField] public float skill_BaseDamage;
    [SerializeField] public float skill_BaseCooldown;
    [SerializeField] public float skill_Multiplier;
    
    [Header("SkillObstacles")]
    [SerializeField] public LayerMask skill_ObstaclesMask; //Obstacles dla skilla

    [Header("SkillAudio")]
    [Tooltip("AudioSource skilla -> Caster"), SerializeField] public AudioSource skill_AudioSource;
    [Tooltip("AudioClip skilla -> Caster"), SerializeField] public AudioClip skill_CastingAudioClip;
    [Tooltip("VFX skilla -> Caster"), SerializeField] public VisualEffect skill_CastingVisualEffect;
    [Tooltip("AudioClip skilla -> Target"), SerializeField] public AudioClip skill_OnHitAudioClip;
    [Tooltip("VFX skilla -> Target"), SerializeField] public VisualEffect skill_OnHitVisualEffect;
    [Tooltip("AudioVolume skilla -> Caster"), SerializeField] public float skill_CastingAudioVolume;
    [Tooltip("AudioVolume skilla -> Target"), SerializeField] public float skill_OnHitAudioVolume;

    [Header("SkillAnimator")]
    [SerializeField] public Animator skill_Animator;
    [Header("Animator Values")]
    [Tooltip("Jaki Float Animatora odpowiada za animację? - Nazwa"), SerializeField] public string skill_AnimatorFloat;
    [Tooltip("Jaki Trigger Animatora odpowiada za animację?  - Nazwa"), SerializeField] public string skill_AnimatorTrigger;
    [Tooltip("Jaki Bool Animatora odpowiada za animację? - Nazwa "), SerializeField] public string skill_AnimatorBool;

    [Tooltip("Na którym combo colider działa?"), SerializeField] public float skill_onCombo;

    [Header("SkillVariable VFX")]
    [SerializeField] public float skill_ParticlesPerSec;
    [SerializeField] public float skill_LifetimeMin;
    [SerializeField] public float skill_LifetimeMax;
    [SerializeField] public float skill_MovingTowardsFactor;

}


