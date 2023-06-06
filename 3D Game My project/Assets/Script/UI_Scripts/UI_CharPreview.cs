using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharPreview : MonoBehaviour
{
   /* [SerializeField, Range(0, 5)] public float _charTurnTime;
    [SerializeField, Range(0, 360)] public float _rotationAngle;
    [SerializeField] float _currentAngle;*/
    [SerializeField] GameObject _charPreview;
    [SerializeField] Animator _animator;

    private void OnValidate()
    {
        _charPreview = this.gameObject;
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime; //Animacja dzia³a poza metod¹ Update
        _animator.SetFloat("yAnim", 0f);       
    }

    /*// Update is called once per frame  //Nie dzia³a w timescale 0f
    void Update()
    {
        *//*
        _currentAngle = Mathf.MoveTowards(0, _rotationAngle, Time.deltaTime / _charTurnTime);   //progress 0-> Angle w (turnTime * 1sek));
        *//*_charPreview.transform.Rotate(Vector3.up, _currentAngle,);
        if (_currentAngle >= _rotationAngle)
        {
            _rotationAngle *= -1f;
        }*//*

    }*/
}
