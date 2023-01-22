using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MousePointer : MonoBehaviour
{
    

    // Update is called once per frame    
    private void Update()
    {
        PointerFollowMouse();
    }

    void PointerFollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;
        }
        transform.LookAt(Camera.main.transform);

        
    }
}
