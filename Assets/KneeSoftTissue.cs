using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KneeSoftTissue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.name == "Tool")
        {
            Debug.Log("Collission");
            int contactPoints = collision.contactCount;
            Debug.Log(contactPoints);
            for(int i=0; i < contactPoints; i++)
            {
                ContactPoint point = collision.contacts[i];
                Debug.Log(point.point.ToString());
                Debug.DrawRay(point.point, point.normal * 2f, Color.red);
            }

        }
    }
}
