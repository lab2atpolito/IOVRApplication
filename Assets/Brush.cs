using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour
{
    [SerializeField]
    private bool _isPainting = false;
    private Vector3 _brushingPosition;

    // Start is called before the first frame update
    void Start()
    {
        _brushingPosition = this.gameObject.transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "PaintableObject")
        {
            Debug.Log("Collision Start");
            Vector3 contactPoint = collision.GetContact(0).point;
            _isPainting = true;
            _brushingPosition = contactPoint;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "PaintableObject")
        {
            _brushingPosition = collision.GetContact(0).point;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if( collision.gameObject.tag == "PaintableObject")
        {
            Debug.Log("Collision End");
            _isPainting = false;
            _brushingPosition = this.gameObject.transform.position;
        }
    }

    public bool IsPainting()
    {
        return _isPainting;
    }

    public Vector3 GetBrushingPosition()
    {
        return _brushingPosition;
    }
}
