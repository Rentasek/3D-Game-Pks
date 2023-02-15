using System.Collections;
using System.Collections.Generic;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ItemResourceSphere : MonoBehaviour
{
    [SerializeField] private enum ResourceType { SkillPoint, HP, MP, Stamina }
    
    [Header("Resource Type")]
    [SerializeField] private ResourceType resourceType = new ResourceType();
    [Space, Header("Roll the Dice")]
    [SerializeField] private int randomRoll;
    [SerializeField] private bool reroll = false;    
        
    [Space, Header("Colors/Materials")]
    [SerializeField] private Material material;

    [SerializeField, ColorUsageAttribute(true, true)] private Color skill_colorLighter;
    [SerializeField, ColorUsageAttribute(true, true)] private Color skill_colorDarker;

    [SerializeField, ColorUsageAttribute(true, true)] private Color hp_colorLighter;
    [SerializeField, ColorUsageAttribute(true, true)] private Color hp_colorDarker;

    [SerializeField, ColorUsageAttribute(true, true)] private Color mp_colorLighter;
    [SerializeField, ColorUsageAttribute(true, true)] private Color mp_colorDarker;

    [SerializeField, ColorUsageAttribute(true, true)] private Color stamina_colorLighter;
    [SerializeField, ColorUsageAttribute(true, true)] private Color stamina_colorDarker;

    /*private void OnValidate() //wy³¹czyæ, tylko do ustawiania koloru w edytorze
    {
        SetupResourceOrb();
    }*/


    private void OnEnable() //przy stworzeniu z instantiate
    {
        SetupResourceOrb();
        Destroy(gameObject, 20f); //Destroy OBJ po 20 sek
    }
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.GetComponent<CharacterStatus>().currentCharStatus.isDead) //Tylko dla ¿ywych
        {
            switch (resourceType)
            {
                case ResourceType.SkillPoint:
                    other.gameObject.GetComponent<CharacterBonusStats>().bonus_SkillPoints++;
                    break;
                case ResourceType.HP:
                    if(other.gameObject.GetComponent<CharacterStatus>().currentHP >= other.GetComponent<CharacterStatus>().currentMaxHP)
                    {
                        other.gameObject.GetComponent<CharacterStatus>().currentHP = other.GetComponent<CharacterStatus>().currentMaxHP;
                        break;
                    }
                    other.gameObject.GetComponent<CharacterStatus>().currentHP += other.GetComponent<CharacterStatus>().currentMaxHP * other.GetComponent<CharacterBonusStats>().percentRegen;
                    break;

                case ResourceType.MP:
                    if (other.gameObject.GetComponent<CharacterStatus>().currentMP >= other.GetComponent<CharacterStatus>().currentMaxMP)
                    {
                        other.gameObject.GetComponent<CharacterStatus>().currentMP = other.GetComponent<CharacterStatus>().currentMaxMP;
                        break;
                    }
                    other.gameObject.GetComponent<CharacterStatus>().currentMP += other.GetComponent<CharacterStatus>().currentMaxMP * other.GetComponent<CharacterBonusStats>().percentRegen;
                    break;

                case ResourceType.Stamina:
                    if (other.gameObject.GetComponent<CharacterStatus>().currentStam >= other.GetComponent<CharacterStatus>().currentMaxStam)
                    {
                        other.gameObject.GetComponent<CharacterStatus>().currentStam = other.GetComponent<CharacterStatus>().currentMaxStam;
                        break;
                    }
                    other.gameObject.GetComponent<CharacterStatus>().currentStam += other.GetComponent<CharacterStatus>().currentMaxStam * other.GetComponent<CharacterBonusStats>().percentRegen;
                    break;
            }
            Debug.Log("Collision!! "+"Resource type: "+resourceType);            
            Destroy(gameObject);
        }
    }  

    public void SetupResourceOrb()
    {       

        material = new Material(material);

        if (!reroll)
        {
            randomRoll = Random.Range(0, 100);
            reroll = true;
        }
        //randomRoll = Random.Range(0, 100);
        if (randomRoll <= 10) resourceType = ResourceType.SkillPoint;
        else if (10 < randomRoll && randomRoll <= 40) resourceType = ResourceType.HP;
        else if (40 < randomRoll && randomRoll <= 70) resourceType = ResourceType.MP;
        else resourceType = ResourceType.Stamina;

              

        switch (resourceType)
        {
            case ResourceType.SkillPoint:                
                material.SetColor("_Color_lighter", skill_colorLighter);
                material.SetColor("_Color_darker", skill_colorDarker);
                break;
            case ResourceType.HP:                
                material.SetColor("_Color_lighter", hp_colorLighter);
                material.SetColor("_Color_darker", hp_colorDarker);
                break;
            case ResourceType.MP:
                material.SetColor("_Color_lighter", mp_colorLighter);
                material.SetColor("_Color_darker", mp_colorDarker);
                break;
            case ResourceType.Stamina:
                material.SetColor("_Color_lighter", stamina_colorLighter);
                material.SetColor("_Color_darker", stamina_colorDarker);
                break;
        }

        GetComponent<MeshRenderer>().sharedMaterial = material; 
    }


}
