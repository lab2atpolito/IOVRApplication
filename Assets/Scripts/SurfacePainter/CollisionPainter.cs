using UnityEngine;

public class CollisionPainter : MonoBehaviour{
    public Color paintColor;

    [SerializeField]
    private PaintManager _paintManager;
    
    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    private void OnCollisionStay(Collision other) {
        Paintable p = other.collider.GetComponent<Paintable>();
        if(p != null){
            Vector3 pos = other.contacts[0].point;
            _paintManager.paint(p, pos, radius, hardness, strength, paintColor);
            Debug.Log("COLLISSION!");
        }
    }
}
