using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_Collider : MonoBehaviour
{    
    [Tooltip("Live_charStats Castera"), SerializeField] public CharacterStatus live_charStats;    
    [Tooltip("Skill powi¹zany z Colliderem"), SerializeField] public Skill skill;

    private void OnEnable()
    {
        Invoke(nameof(DisableColliderObject), skill.scrObj_Skill._timeCast);
    }

    public void DisableColliderObject() 
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(skill._enemiesArray.Contains(other.tag))
        {
            for (int targetTypeIndex = 0; targetTypeIndex < skill.targetDynamicValues.Length; targetTypeIndex++)
            {
                if (skill.targetDynamicValues[targetTypeIndex]._targetColliders.IndexOf(skill._allLocalColliders[targetTypeIndex]) < 0) //sprawdza czy nie ma na liœcie. Je¿eli IndexOf < 0 czyli nie ma obiektów z tym indexem
                {
                    skill.targetDynamicValues[targetTypeIndex]._targetColliders.Add(skill._allLocalColliders[targetTypeIndex]); //przypisuje do listy colliders jeœli ma taga z listy enemies  
                }
            }
        }
    }    
}
