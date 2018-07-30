using UnityEngine;

public class PlayerMove : MonoBehaviour {

    private InputHandler _inputHandler;

    private Rigidbody _rb;
    private Transform _camTransform;
    private CapsuleCollider _col;
    
    private float movementSpeed = 5;
    private Vector3 _inputVector;
    private Vector3 _movement;
    private float rotationLerp = 0.2f;

    private int _checkpointID;
    private Vector3 _checkPointPos;
    private Quaternion _checkPointRot;

    private void Awake()
    {
        #region SaveLoad

        SaveLoad.Delete();
        Debug.Log("Save file delete");

        if (SaveLoad.FindSaveFile())
            SaveLoad.Load();
        else
            SaveLoad.MakeSaveFile();

        if (SaveLoad.CheckpointInitialized)
            LoadCheckpoint(SaveLoad.Floats);
        else
            SetCheckpoint(transform, -1);

        #endregion
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        _camTransform = FindObjectOfType<Camera>().transform;
        _inputHandler = FindObjectOfType<InputHandler>();

        PlayerToLastCheckpoint();
    }

    private void Update()
    {
        // read and normalize input
        _inputVector.x = _inputHandler.GetAxisInput(InputHandler.Axis.MoveX);
        _inputVector.y = 0f;
        _inputVector.z = _inputHandler.GetAxisInput(InputHandler.Axis.MoveZ);
        _inputVector.Normalize();

        _movement += _camTransform.right * _inputVector.x * Time.deltaTime;
        _movement += _camTransform.forward * _inputVector.z * Time.deltaTime;
        _movement.y = 0f;

        transform.forward = Vector3.Slerp(transform.forward, _movement.normalized, rotationLerp);

        if (transform.position.y < -10)
            PlayerToLastCheckpoint();
    }

    private void FixedUpdate()
    {
        _movement *= movementSpeed;

        Vector3 velocity = _rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        _rb.velocity = velocity;

        if (_movement != Vector3.zero && CheckMovementCollisions())
            _rb.MovePosition(transform.position + _movement);

        _movement = Vector3.zero;
    }
    
    private bool CheckMovementCollisions()
    {
        float radius;
        Vector3[] centers = MathHelp.CapsuleEndPoints(_col, out radius);

        RaycastHit[] hits = Physics.CapsuleCastAll(centers[0], centers[1], radius * 0.99f, _movement.normalized, _movement.magnitude);

        foreach (RaycastHit hit in hits)
        {
            StaticObject staticObject = hit.collider.GetComponent<StaticObject>();
            if (staticObject != null && !staticObject.isWalkable)
                return false;
        }

        return true;
    }

    private void LoadCheckpoint(float[] SaveLoadFloats)
    {
        // eulers to quaternion
        _checkPointPos.x = SaveLoadFloats[SaveLoad.PLAYER_EULER_X];
        _checkPointPos.y = SaveLoadFloats[SaveLoad.PLAYER_EULER_Y];
        _checkPointPos.z = SaveLoadFloats[SaveLoad.PLAYER_EULER_Z];
        _checkPointRot = Quaternion.Euler(_checkPointPos);

        // position
        _checkPointPos.x = SaveLoadFloats[SaveLoad.PLAYER_POS_X];
        _checkPointPos.y = SaveLoadFloats[SaveLoad.PLAYER_POS_X];
        _checkPointPos.z = SaveLoadFloats[SaveLoad.PLAYER_POS_X];
    }

    public void SetCheckpoint(Transform checkPoint, int checkpointID)
    {
        if (checkpointID == _checkpointID)
            return;

        _checkpointID = checkpointID;
        _checkPointPos = checkPoint.position;
        _checkPointRot = checkPoint.rotation;

        SaveLoad.SaveCheckpoint(checkPoint);
    }

    public void PlayerToLastCheckpoint()
    {
        transform.position = _checkPointPos;
        transform.rotation = _checkPointRot;

        _rb.velocity = Vector3.zero;
    }
}
