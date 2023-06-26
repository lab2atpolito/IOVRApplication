using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _hitPointDebug;

    private GameObject _hitPointIstance;
    private RaycastHit _hit;

    void Update()
    {
        Ray _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(_ray.origin,_ray.direction);
        if(Physics.Raycast(_ray, out _hit, 100))
        {
            if (_hit.collider.gameObject.tag == "Patient")
            {
                if(_hitPointIstance == null)
                {
                    _hitPointIstance = GameObject.Instantiate(_hitPointDebug, _hit.point, Quaternion.identity);
                }
                else
                {
                    _hitPointIstance.transform.position = _hit.point;
                }
                Debug.DrawLine(_hit.normal + _hit.point, _hit.point, Color.green);
                Vector3 _hitPoint = _hit.point;
            }
        }
        else
        {
            Destroy(_hitPointIstance);
        }
    }
}
