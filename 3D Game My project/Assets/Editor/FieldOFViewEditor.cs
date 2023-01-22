using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//[CustomEditor(typeof(FieldOFView))] //dla skryptów field of view
public class FieldOFViewEditor : MonoBehaviour //zmieniæ na editor i dzia³a jak Editor script (w klasie FoV trzeba zmieniæ live_charStats na public!!!)
{    
    /*private void OnSceneGUI()
    {



        FieldOFView fov = (FieldOFView)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.live_charStats.fov_coneRadius,fov.live_charStats.fov_editorLineThickness); //rysowanie lini po okrêgu

        Vector3 viewAngleLeft = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.live_charStats.fov_coneAngle / 2); //tworzy view angle w lewo od vectora transform.forward do coneAngle/2
        Vector3 viewAngleRight = DirectionFromAngle(fov.transform.eulerAngles.y, fov.live_charStats.fov_coneAngle / 2); //tworzy view angle w prawo od vectora transform.forward do coneAngle/2
        
        Handles.color = Color.red;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleLeft * fov.live_charStats.fov_coneRadius, fov.live_charStats.fov_editorLineThickness);//rysowanie lini left
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleRight * fov.live_charStats.fov_coneRadius, fov.live_charStats.fov_editorLineThickness);//rysowanie lini right

        if (fov.live_charStats.navMeAge_targetAquired)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.live_charStats.fov_aquiredTargetGameObject.transform.position, fov.live_charStats.fov_editorLineThickness); //rysowanie lini w kierunku playera jeœli nie zas³ania go obstacle Layer
        }
    
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)); //zwraca vector3 (x,y,z)
    }*/
}
