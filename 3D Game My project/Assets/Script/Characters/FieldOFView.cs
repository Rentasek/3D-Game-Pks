using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class FieldOFView : MonoBehaviour
{
    [SerializeField] private CharacterStatus live_charStats;
    [SerializeField] private string[] enemiesArray;

    /* private void OnValidate()
   {
       live_charStats = GetComponentInParent<CharacterStatus>();
       enemiesArray = live_charStats.currentEnemiesArray;
   }*/

    // Start is called before the first frame update
    void Start()
    {
       // live_charStats = GetComponentInParent<CharacterStatus>();
       // enemiesArray = live_charStats.currentEnemiesArray;
    }
  

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        live_charStats = GetComponentInParent<CharacterStatus>();
        enemiesArray = live_charStats.currentEnemiesArray;        
    }

    private void FieldOfViewCheck()
    {
        live_charStats.fov_allTargetsInDynamicSightRange = Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius);

        //sprawdzanie czy jest enemy przed wrzucaniem targetu, musi byæ brake po (targetInDynamicRange = true) i zwrócony boolean, bez tego prawdopodobnie siê nadpisuje i przeciwnik nie reaguje na gracza

        foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)) //dla ka¿dego collidera w zasiêgu wzroku
        {
            if (live_charStats.currentEnemiesArray.Contains(collider.tag))    //jeœli ma tag zawarty w arrayu enemiesArray
            {
                live_charStats.fov_targetInDynamicSightRange = true;    //target jest w sight range                 
                break;
            }
            else
            {
                live_charStats.fov_targetInDynamicSightRange = false;                     //target nie jest w sight range
                live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                live_charStats.fov_targetAquired = false;
            }
        }

        if (live_charStats.fov_targetInDynamicSightRange)
        {
            foreach (Collider collider in Physics.OverlapSphere(live_charStats.gameObject.transform.position, live_charStats.fov_CurrentDynamicSightRadius)) //dla ka¿dego collidera w zasiêgu wzroku
            {
                if (live_charStats.currentEnemiesArray.Contains(collider.tag))    //jeœli ma tag zawarty w arrayu enemiesArray
                {
                    //live_charStats.fov_targetInDynamicSightRange = true;    //target jest w sight range 

                    Vector3 directionToTarget = (collider.transform.position - live_charStats.gameObject.transform.position).normalized; //0-1(normalized) ró¿nica pomiêdzy targetem a characterem

                    if (Vector3.Angle(live_charStats.gameObject.transform.forward, directionToTarget) < live_charStats.fov_CurrentDynamicSightAngle / 2) //sprawdzanie angle wektora forward charactera i direction to target
                                                                                                                                                         //target mo¿e byæ na + albo - od charactera dlatego w ka¿d¹ stronê angle / 2
                    {
                        if (!Physics.Raycast(live_charStats.gameObject.transform.position, directionToTarget, live_charStats.fov_CurrentDynamicSightRadius, live_charStats.fov_obstaclesLayerMask))
                        {
                            //jeœli raycast do targetu nie jest zas³oniêty przez jakiekolwiek obstacles!!
                            live_charStats.fov_aquiredTargetGameObject = collider.gameObject;           //ustawia znaleziony colliderem game objecta jako target
                            live_charStats.fov_targetAquired = true;
                            break;          //najwa¿niejszy jest brake, nie da siê jednoczeœnie porównaæ listy colliderów z Overlapa z list¹ tagów z enemiesArray
                                            //foreach sprawdza wszystkie collidery jeœli trafi => target bool = true, ale ¿e sprawdza wszystkie collidery
                                            //to trafia i pud³uje ca³y czas, dlatego trzeba foreacha zatrzymaæ breakiem kiedy trafi !!!
                        }
                        else
                        {
                            //live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                            live_charStats.fov_targetAquired = false;
                        }
                    }
                    else
                    {
                        //live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                        live_charStats.fov_targetAquired = false;
                    }
                }
                /*else
                {
                    live_charStats.fov_targetInDynamicSightRange = false;                     //target nie jest w sight range
                    live_charStats.fov_aquiredTargetGameObject = null;           //ustawia nie znaleziony colliderem game objecta jako null
                    live_charStats.fov_targetAquired = false;
                }*/
            }
        }

    }

    public IEnumerator FOVRoutine()
    {
        if (live_charStats.fov_isSearchingForTarget)
        {            
            yield break; // ¿eby nie nadpisywa³ coroutine co klatke
        }
        else
        {
            live_charStats.fov_isSearchingForTarget = true;

            yield return new WaitForSeconds(live_charStats.fov_RoutineDelay);
            
            
            FieldOfViewCheck();
            live_charStats.fov_isSearchingForTarget = false;
        }

    }

/*#if UNITY_EDITOR //zamiast skryptu w Editor

    *//*private void OnDrawGizmos() //rusyje wszystkie
    {
        GizmosDrawer();
    }*//*
    private void OnDrawGizmosSelected() //rysuje tylko zaznaczone
    {
        GizmosDrawer();
    }

    private void GizmosDrawer()
    {
        Handles.color = live_charStats.fov_editorRadiusColor;    
        //Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov_coneRadius, live_charStats.fov_editorLineThickness); //rysowanie lini po okrêgu
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov_MaxSightRadius);  //rysowanie solid okrêgu

        Handles.color = live_charStats.fov_editorDynamicRadiusColor; //closeSightRadius      
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.fov_CurrentDynamicSightRadius);  //rysowanie solid okrêgu

        Handles.color = live_charStats.spell_editorAISpellRadiusColor; //SpellAIRange      
        Handles.DrawSolidArc(transform.position, Vector3.up, Vector3.forward, 360, live_charStats.spell_MaxRadius * live_charStats.spell_AISpellRangeFromMax);  //rysowanie solid okrêgu


        *//*  
          Vector3 viewAngleLeft = DirectionFromAngle(transform.eulerAngles.y, -live_charStats.fov_coneAngle / 2); //tworzy view angle w lewo od vectora transform.forward do coneAngle/2
          Vector3 viewAngleRight = DirectionFromAngle(transform.eulerAngles.y, live_charStats.fov_coneAngle / 2); //tworzy view angle w prawo od vectora transform.forward do coneAngle/2
  *//*

        Handles.color = live_charStats.fov_editorAngleLineColor;
        Handles.DrawLine(transform.position, transform.position + Quaternion.AngleAxis(-(live_charStats.fov_CurrentDynamicSightAngle / 2), Vector3.up) * transform.forward * live_charStats.fov_MaxSightRadius, live_charStats.fov_editorLineThickness);//rysowanie lini left
        Handles.DrawLine(transform.position, transform.position + Quaternion.AngleAxis((live_charStats.fov_CurrentDynamicSightAngle / 2), Vector3.up) * transform.forward * live_charStats.fov_MaxSightRadius, live_charStats.fov_editorLineThickness);//rysowanie lini right

        Handles.color = live_charStats.fov_editorAngleColor;
        Handles.DrawSolidArc(transform.position, Vector3.up, *//*viewAngleLeft*//*Quaternion.AngleAxis(-(live_charStats.fov_CurrentDynamicSightAngle/2),Vector3.up)*transform.forward, live_charStats.fov_CurrentDynamicSightAngle, live_charStats.*//*fov_MaxSightRadius*//*fov_CurrentDynamicSightRadius); //rysuje coneAngle view               
                                                                                                                                                                                                                                  //Quaternion.AngleAxis korzysta z lokalnego transforma zamiast skomplikowanego Mathf.sin/cos

        if (live_charStats.navMeAge_walkPointSet)
        {
            Handles.color = live_charStats.fov_editorRaycastColor;
            Handles.DrawLine(transform.position, live_charStats.navMeAge_walkPoint, live_charStats.fov_editorLineThickness);
        }

        if (live_charStats.navMeAge_targetAquired && live_charStats.fov_aquiredTargetGameObject!=null)
        {
            Handles.color = live_charStats.fov_editorRaycastColor;
            Handles.DrawLine(transform.position, live_charStats.fov_aquiredTargetGameObject.transform.position, live_charStats.fov_editorLineThickness); //rysowanie lini w kierunku playera jeœli nie zas³ania go obstacle Layer
        }
    }
   *//* private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)); //zwraca vector3 (x,y,z)
    }*//*//Zbêdne dziêki wykorzystaniu quaterniona z lokalnym transform.forward

#endif*/
}
