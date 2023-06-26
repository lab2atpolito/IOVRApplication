using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleansingInteraction : MonoBehaviour
{
    private bool _isCleasing = false;
    private GameObject _skinSurfaceHitDebug;

    [SerializeField] private GameObject _hitDebugPrefab;
    [SerializeField] private Material _hitPointMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.tag == "Patient")
        {
            _isCleasing = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.tag == "Patient")
        {
            Debug.Log("Collision!");
        } 
    }

    private void Update()
    {
        LayerMask layerMask = LayerMask.GetMask("Leg");

        Ray originRay = new Ray(this.transform.position, -this.transform.up);
        Debug.DrawRay(originRay.origin, originRay.direction);

        RaycastHit hit;
        Physics.Raycast(originRay, out hit, layerMask);

        // Debugging Skin Surface Hit
        if (hit.point != null)
        {
            if( hit.rigidbody.gameObject.tag == "Patient")
            {
                Debug.Log(hit.point.ToString());
                if (_skinSurfaceHitDebug == null)
                {
                    _skinSurfaceHitDebug = Instantiate(_hitDebugPrefab, hit.point, Quaternion.identity);
                    _skinSurfaceHitDebug.GetComponent<Renderer>().material = new Material(_hitPointMaterial);
                }
                else
                {
                    _skinSurfaceHitDebug.transform.position = hit.point;
                }
            }
        }
        else
        {
            if (_skinSurfaceHitDebug != null)
            {
                Destroy(_skinSurfaceHitDebug);
            }
        }

    }

    public bool IsCleansing()
    {
        return _isCleasing; 
    }
}
