using UnityEngine;

public class InputHandler : MonoBehaviour
{

    [HideInInspector]
    public float moveXInput, moveZInput;
    [HideInInspector]
    public float lookXInput, lookYInput;
    [HideInInspector]
    public float rotationInput;
    [HideInInspector]
    public float distanceInput;

    private float _newAimInput;
    private float _lastAimInput;

    private float _newActivationInput;
    private float _lastActivationInput;

    private float _newJumpInput;
    private float _lastJumpInput;

    private float _newPushInput;
    private float _lastPushInput;

    private float _newPullInput;
    private float _lastPullInput;

    private float _newChangeInput;
    private float _lastChangeInput;

    private float _newShootInput;
    private float _lastShootInput;

    private float _newFreezeInput;
    private float _lastFreezeInput;

    private float _newInput;
    private float _lastInput;

    private string[] _controllerNames;
    private string _firstController;

    private float _checkDelay = 5f;
    private float _lastCheck;

    private const string XBOX_CONTROLLER = "Xbox ";
    private const string PS_CONTROLLER = "PS ";
    private const string RP_CONTROLLER = "RumblePad ";

    public enum Key
    {
        Aim = 0,
        Activation = 1,
        Jump = 2,
        Push = 3,
        Pull = 4,
        Change = 5,
        Shoot = 6,
        Freeze = 7
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CheckController();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;

            if(Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        if((_checkDelay + _lastCheck) < Time.realtimeSinceStartup)
        {
            CheckController();
            _lastCheck = Time.realtimeSinceStartup;
        }

        if(_firstController.Length > 0)
        {
            if (_firstController.Contains("xbox"))
            {
                ReadInput(XBOX_CONTROLLER);
            }
            else if (_firstController.Contains("wireless controller"))
            {
                ReadInput(PS_CONTROLLER);
            }
            else if (_firstController.Contains("rumblepad"))
            {
                ReadInput(RP_CONTROLLER);
            }

        }
        else
        {
            ReadInput("");
        }

    }

    public void CheckController()
    {
        _firstController = "";
        _controllerNames = Input.GetJoystickNames();
        
        foreach(string str in _controllerNames)
        {
            if(str != "" && str != null)
            {
                _firstController = str.ToLower();
                break;
            }
        }
    }

    private void ReadInput(string controller)
    {
        
        moveXInput = Input.GetAxisRaw(controller + "MoveX");
        moveZInput = Input.GetAxisRaw(controller + "MoveZ");

        lookXInput = Input.GetAxisRaw(controller + "LookX");
        lookYInput = Input.GetAxisRaw(controller + "LookY");

        _lastAimInput = _newAimInput;
        _newAimInput = Input.GetAxisRaw(controller + "Aim");

        _lastActivationInput = _newActivationInput;
        _newActivationInput = Input.GetAxisRaw(controller + "Activation");

        _lastJumpInput = _newJumpInput;
        _newJumpInput = Input.GetAxisRaw(controller + "Jump");

        rotationInput = Input.GetAxisRaw(controller + "Rotation");

        distanceInput = Input.GetAxisRaw(controller + "Distance");

        if(controller == "")
        {
            _lastPushInput = _newPushInput;
            _newPushInput = Input.GetAxisRaw(controller + "Push");

            _lastPullInput = _newPullInput;
            _newPullInput = Input.GetAxisRaw(controller + "Pull");

            _lastChangeInput = _newChangeInput;
            _newChangeInput = Input.GetAxisRaw(controller + "Change Skill");

            _lastShootInput = _newShootInput;
            _newShootInput = Input.GetAxisRaw(controller + "Shoot");

            _lastFreezeInput = _newFreezeInput;
            _newFreezeInput = Input.GetAxisRaw(controller + "Freeze");
        }

    }

    private void GetKeyInputs(Key key, out float newInput, out float lastInput)
    {

        switch ((int)key)
        {
            case 0:
                newInput = _newAimInput;
                lastInput = _lastAimInput;
                break;

            case 1:
                newInput = _newActivationInput;
                lastInput = _lastActivationInput;
                break;

            case 2:
                newInput = _newJumpInput;
                lastInput = _lastJumpInput;
                break;

            case 3:
                newInput = _newPushInput;
                lastInput = _lastPushInput;
                break;

            case 4:
                newInput = _newPullInput;
                lastInput = _lastPullInput;
                break;

            case 5:
                newInput = _newChangeInput;
                lastInput = _lastChangeInput;
                break;

            case 6:
                newInput = _newShootInput;
                lastInput = _lastShootInput;
                break;

            case 7:
                newInput = _newFreezeInput;
                lastInput = _lastFreezeInput;
                break;

            default:
                newInput = 0;
                lastInput = 0;
                break;
        }
        
    }

    public bool KeyUp(Key key)
    {
        GetKeyInputs(key, out _newInput, out _lastInput);

        return _lastInput != 0 && _newInput == 0;
    }

    public bool KeyDown(Key key)
    {
        GetKeyInputs(key, out _newInput, out _lastInput);

        return _lastInput == 0 && _newInput != 0;
    }

    public bool KeyHold(Key key)
    {
        GetKeyInputs(key, out _newInput, out _lastInput);

        return _newInput != 0 && _newInput == _lastInput;
    }

}
