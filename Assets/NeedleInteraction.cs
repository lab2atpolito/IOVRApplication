using System.Collections;
using UnityEngine;
using TMPro;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction;

public class NeedleInteraction : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private GameObject _hitDebugPrefab;
    [SerializeField] private Material _hitPointMaterial;

    [Space(10)]
    [SerializeField, Range(0, 1)] private float _maxDistance = 0.001f;
    [SerializeField] private float _angle = 0f;

    [SerializeField] private State _currentState;

    [SerializeField] private Transform _boneSurfaceTarget;
    [SerializeField] private Transform _skinSurfaceTarget;

    private float _positionPrecision;
    private float _anglePrecision;

    [SerializeField, Range(0, 20f)] private float _positionPrecisionThreshold;

    [SerializeField] TextMeshPro _positionPrecisionUI;
    [SerializeField] TextMeshPro _anglePrecisionUI;

    [Space(10)]
    [Header("Controls")]
    [SerializeField] private Transform _tipControl;
    [SerializeField] private Transform _5mmControl;
    [SerializeField] private Transform _baseControl;
    [SerializeField] private Transform _originControl;

    private GameObject _skinSurfaceHitDebug;
    private GameObject _boneSurfaceHitDebug;
    private GameObject _medullaryCavitySurfaceHitDebug;

    private const float SKIN_DYN_FRICTION = 0.6f;
    private const float BONE_DYN_FRICTION = 1f;
    private const float MEDULLARY_CAVITY_DYN_FRICTION = 0.2f;

    [SerializeField] private Grabbable _drill;

    [SerializeField] private bool _hasReachedMaxDepth = false;

    private bool _isHittingSkin = false;
    private bool _isOneLineVisible;

    //[SerializeField] private Material _skinMaterial;
    //[SerializeField] private float _brushSize;
    //[SerializeField] private Color _paintColor;

    private bool _isStyletAttached = true;

    //private bool _drawnBlood = false;

    void Start()
    {
        _currentState = State.IN_AIR;
    }

    private void Update()
    {
        LayerMask layerMask = LayerMask.GetMask("Leg");

        // Skin Surface Target
        var correctDirection = new Ray(_boneSurfaceTarget.position, _boneSurfaceTarget.forward);
        Debug.DrawRay(correctDirection.origin, correctDirection.direction, Color.cyan);

        RaycastHit skinTargetHit;
        Physics.Raycast(correctDirection, out skinTargetHit, 100, layerMask);

        if (skinTargetHit.point != null)
        {
            if (_skinSurfaceTarget == null)
            {
                _skinSurfaceTarget = Instantiate(_hitDebugPrefab, skinTargetHit.point, Quaternion.identity).transform;
            }
            else
            {
                _skinSurfaceTarget.position = skinTargetHit.point;
            }
        }

        Ray originRay = new Ray(_originControl.position, _originControl.forward);
        Debug.DrawRay(originRay.origin, originRay.direction);

        RaycastHit[] hitPoints = Physics.RaycastAll(originRay, 100, layerMask);
        //Debug.Log(hitPoints.Length);

        // Costruisco la lista di punti di intersezione (con tipologia e posizione)
        Hashtable layerHitPoints = new Hashtable();

        foreach (RaycastHit hit in hitPoints)
        {
            float dynFriction = hit.collider.material.dynamicFriction;
            switch (dynFriction)
            {
                case SKIN_DYN_FRICTION:
                    if(!layerHitPoints.Contains(LayerType.SKIN))
                        layerHitPoints.Add(LayerType.SKIN, hit);
                    break;
                case BONE_DYN_FRICTION:
                    if (!layerHitPoints.Contains(LayerType.BONE))
                        layerHitPoints.Add(LayerType.BONE, hit);
                    break;
                case MEDULLARY_CAVITY_DYN_FRICTION:
                    if (!layerHitPoints.Contains(LayerType.MEDULLARY_CAVITY))
                        layerHitPoints.Add(LayerType.MEDULLARY_CAVITY, hit);
                    break;
            }
        }

        // Debugging Skin Surface Hit
        if(layerHitPoints.Contains(LayerType.SKIN))
        {
            _isHittingSkin = true;
            if(_skinSurfaceHitDebug == null)
            {
                _skinSurfaceHitDebug = Instantiate(_hitDebugPrefab, ((RaycastHit) layerHitPoints[LayerType.SKIN]).point, Quaternion.identity);
                _skinSurfaceHitDebug.GetComponent<Renderer>().material = new Material(_hitPointMaterial);
            }
            else
            {
                _skinSurfaceHitDebug.transform.position = ((RaycastHit) layerHitPoints[LayerType.SKIN]).point;
            }
        }
        else
        {
            _isHittingSkin = false;
            if(_skinSurfaceHitDebug != null)
            {
                Destroy(_skinSurfaceHitDebug);
            }    
        }

        // Debugging Bone Surface Hit
        if (layerHitPoints.Contains(LayerType.BONE))
        {
            if (_boneSurfaceHitDebug == null)
            {
                _boneSurfaceHitDebug = Instantiate(_hitDebugPrefab, ((RaycastHit) layerHitPoints[LayerType.BONE]).point, Quaternion.identity);
                _boneSurfaceHitDebug.GetComponent<Renderer>().material = new Material(_hitPointMaterial);
            }
            else
            {
                _boneSurfaceHitDebug.transform.position = ((RaycastHit)layerHitPoints[LayerType.BONE]).point;
            }
        }
        else
        {
            if(_boneSurfaceHitDebug != null)
            {
                Destroy(_boneSurfaceHitDebug);
            }
        }

        // Debugging Medullary Cavity Surface Hit
        if (layerHitPoints.Contains(LayerType.MEDULLARY_CAVITY))
        {
            if (_medullaryCavitySurfaceHitDebug == null)
            {
                _medullaryCavitySurfaceHitDebug = Instantiate(_hitDebugPrefab, ((RaycastHit)layerHitPoints[LayerType.MEDULLARY_CAVITY]).point, Quaternion.identity);
                _medullaryCavitySurfaceHitDebug.GetComponent<Renderer>().material = new Material(_hitPointMaterial);
            }
            else
            {
                _medullaryCavitySurfaceHitDebug.transform.position = ((RaycastHit)layerHitPoints[LayerType.MEDULLARY_CAVITY]).point;
            }
        }
        else
        {
            if (_medullaryCavitySurfaceHitDebug != null)
            {
                Destroy(_medullaryCavitySurfaceHitDebug);
            }
        }

        // Skin Surface Hitting
        if (layerHitPoints.Contains(LayerType.SKIN))
        {
            float tipDistance = Vector3.Distance(_originControl.position, _tipControl.position);
            float hitDistance = ((RaycastHit)layerHitPoints[LayerType.SKIN]).distance;
            float baseDistance = Vector3.Distance(_originControl.position, _baseControl.position);
            float minDistance = Vector3.Distance(_originControl.position, _5mmControl.position);

            // Check 5mm distance
            if (minDistance < hitDistance)
            {
                _isOneLineVisible = true;
            }
            else
            {
                _isOneLineVisible = false;
            }

            if (baseDistance + 0.0025f < hitDistance)
                _hasReachedMaxDepth = false;
            else
                _hasReachedMaxDepth = true;

            if (tipDistance < hitDistance)
            {
                _currentState = State.IN_AIR;
                _skinSurfaceHitDebug.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
            }
            else
            {
                _skinSurfaceHitDebug.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
                if (layerHitPoints.Contains(LayerType.BONE))
                {
                    float boneHitDistance = ((RaycastHit)layerHitPoints[LayerType.BONE]).distance;
                    if (tipDistance < boneHitDistance)
                    {
                        _currentState = State.IN_SKIN;
                        _boneSurfaceHitDebug.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                    }
                    else
                    {
                        _currentState = State.IN_BONE;
                        _boneSurfaceHitDebug.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);
                        if (layerHitPoints.Contains(LayerType.MEDULLARY_CAVITY))
                        {
                            float medHitDistance = ((RaycastHit)layerHitPoints[LayerType.MEDULLARY_CAVITY]).distance;
                            if (tipDistance < medHitDistance)
                            {
                                _currentState = State.IN_BONE;
                                _medullaryCavitySurfaceHitDebug.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.3f);
                            }
                            else
                            {
                                _currentState = State.MEDULLARY_CAVITY;
                                _medullaryCavitySurfaceHitDebug.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.3f);

                            }
                        }
                        else
                        {
                            _currentState = State.IN_BONE;
                        }
                    }
                }
                else
                {
                    _currentState = State.IN_SKIN;
                }
            }       

            UpdateAnglePrecision();
            UpdatePositionPrecision(((RaycastHit)layerHitPoints[LayerType.SKIN]));

            // Draw Blood on skin after puncture
            /*if(_currentState == State.IN_SKIN && !_drawnBlood)
            {
                _drawnBlood = true;
                DrawPunctureBlood(((RaycastHit)layerHitPoints[LayerType.SKIN]));
                Debug.Log("Drawing blood on skin surface!");
            }
            else if(_currentState == State.IN_AIR)
            {
                _drawnBlood = false;
            }*/
        }
    }

    private void UpdatePositionPrecision(RaycastHit hit)
    {
        // Calculate the distance between ideal insertion point and actual insertion point
        float distance = Vector3.Distance(hit.point, _skinSurfaceTarget.transform.position);

        // Calculate precision value normalizing distance value using position precision threshold
        float precisionValue = (1f - Mathf.Clamp01(distance / _positionPrecisionThreshold)) * 100f;

        // Update position precision UI
        _positionPrecision = precisionValue;
    }

    private void UpdateAnglePrecision()
    {
        Vector3 idealDirection = _boneSurfaceTarget.forward;
        Vector3 actualDirection = this.transform.up;

        // Calculate angle precision value using the dot between ideal and actual directions
        float precisionValue = Mathf.Abs(Vector3.Dot(idealDirection, actualDirection)) * 100f;

        // Update angle precision UI
        _anglePrecision = precisionValue;
    }

    public State GetCurrentState()
    {
        return _currentState;
    }

    public bool HasReachedMaxDepth()
    {
        return _hasReachedMaxDepth; 
    }

    public bool IsStyletAttached()
    {
        return _isStyletAttached;
    }

    public void RemoveStylet()
    {
        _isStyletAttached = false;
    }

    public float GetPositionPrecision()
    {
        return _positionPrecision; 
    }

    public float GetAnglePrecision()
    {
        return _anglePrecision;
    }

    public bool IsHittingSkin()
    {
        return _isHittingSkin; 
    }

    public bool IsOneLineVisible()
    {
        return _isOneLineVisible;
    }

    /*private void DrawPunctureBlood(RaycastHit hitPoint)
    {
        // Ottieni le coordinate del punto di collisione nella texture
        Vector2 textureCoordinates = hitPoint.textureCoord;

        // Calcola i pixel da colorare
        Texture2D newTexture = Instantiate(_skinMaterial.GetTexture("_BaseMap") as Texture2D);
        int textureWidth = newTexture.width;
        int textureHeight = newTexture.height;
        int centerX = (int)(textureCoordinates.x * textureWidth);
        int centerY = (int)(textureCoordinates.y * textureHeight);

        // Applica il colore rosso ai pixel circostanti
        int halfBrushSize = (int)(_brushSize / 2f);
        int radiusSquared = halfBrushSize * halfBrushSize;
        for (int x = centerX - halfBrushSize; x < centerX + halfBrushSize; x++)
        {
            for (int y = centerY - halfBrushSize; y < centerY + halfBrushSize; y++)
            {
                if (x >= 0 && x < textureWidth && y >= 0 && y < textureHeight)
                {
                    int distanceSquared = (x - centerX) * (x - centerX) + (y - centerY) * (y - centerY);
                    if (distanceSquared <= radiusSquared)
                    {
                        newTexture.SetPixel(x, y, _paintColor);
                    }
                }
            }
        }

        // Applica i cambiamenti alla texture
        newTexture.Apply();
        _skinMaterial.mainTexture = newTexture;
    }*/
}

public enum LayerType
{
    SKIN,
    BONE,
    MEDULLARY_CAVITY
}

public enum State
{
    IN_AIR,
    IN_SKIN,
    IN_BONE,
    MEDULLARY_CAVITY,
}
