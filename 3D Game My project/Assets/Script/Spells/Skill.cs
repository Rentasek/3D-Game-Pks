using Cinemachine;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] public ScrObj_skill scrObj_Skill;
    [SerializeField] public CharacterStatus live_charStats;
    [SerializeField] public CharacterBonusStats currentCharacterBonusStats;


    [Header("Testing/Debug")]
    [SerializeField] public bool skill_input;
    
    [Header("Targets")]
    [CanBeNull, SerializeField] private Collider skill_targetCollider;
    [CanBeNull, SerializeField] private List<Collider> skill_targetColliders;
    [SerializeField, TagField] public string[] skill_EnemiesArray; //Pozwala na wybór Enemies przy pomocy Tag 

    [Header("Skill Cone Settings")]
    [CanBeNull, SerializeField] private bool skill_targetInRange;
    [CanBeNull, SerializeField] private bool skill_targetInAngle;
    [Space]
    [CanBeNull, SerializeField] private float skill_currentRadius;
    [CanBeNull, SerializeField] private float skill_currentAngle;
    [Space]
    [CanBeNull, SerializeField] private float skill_currentVectorRadius;
    [CanBeNull, SerializeField] private float skill_currentVectorAngle;

    [Header("SkillDamage/Cost - current")]
    [SerializeField] public float skill_currentMPCost;
    [SerializeField] public float skill_currentDamage;
    [SerializeField] public float skill_currentCooldown;

    private void OnValidate()
    {
        skill_EnemiesArray = Static_SkillForge.EnemyArraySelector(live_charStats.currentEnemiesArray);  
    }

    private void FixedUpdate()
    {
        

        skill_input = live_charStats.inputCasting; //testing inspector
        live_charStats.spell_CanCast = !live_charStats.inputAttacking && !live_charStats.isRunning && live_charStats.currentMoveSpeed != live_charStats.currentRunSpeed;

        if (!live_charStats.isDead && live_charStats.inputCasting && live_charStats.spell_CanCast && live_charStats.currentMP >= 1f) //tylko dla żywych :P
        {
            Static_SkillForge.Skill_ValuesUpdate(skill_currentDamage, skill_currentMPCost, skill_currentCooldown, scrObj_Skill.skill_BaseDamage, scrObj_Skill.skill_BaseMPCost,
                scrObj_Skill.skill_BaseCooldown, scrObj_Skill.skill_Multiplier, live_charStats.currentCharLevel, currentCharacterBonusStats.bonus_Skill_Damage);

            Static_SkillForge.Skill_VFX_Audio(live_charStats.inputCasting, live_charStats.isCasting, live_charStats.spell_CanCast, live_charStats.currentMP, scrObj_Skill.skill_CastingVisualEffect,
                scrObj_Skill.skill_AudioSource, scrObj_Skill.skill_CastingAudioClip, scrObj_Skill.skill_CastingAudioVolume, scrObj_Skill.skill_Animator, scrObj_Skill.skill_AnimatorBool); ;


            if (live_charStats.currentMP >= 1f)
            {
                Static_SkillForge.Skill_AttackConeCheck(live_charStats.isCasting, skill_currentRadius, skill_currentAngle, scrObj_Skill.skill_MinRadius, scrObj_Skill.skill_MaxRadius, scrObj_Skill.skill_MinAngle,
                    scrObj_Skill.skill_MaxAngle, skill_currentVectorRadius, skill_currentVectorAngle, scrObj_Skill.skill_TimeMaxRadius, scrObj_Skill.skill_TimeMaxAngle, gameObject, live_charStats.currentEnemiesArray,
                    skill_targetInRange, skill_targetInAngle, scrObj_Skill.skill_ObstaclesMask, skill_targetColliders);
            }

        }

        //IMPLEMENTACJA zadawania dmg//
        Static_SkillForge.Skill_Damage(live_charStats.isCasting, skill_targetColliders, live_charStats.currentXP, live_charStats.currentMP, skill_currentDamage, live_charStats.currentCharLevel, skill_currentMPCost);
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


