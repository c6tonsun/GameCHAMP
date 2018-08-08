using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private float _newInput;
    private float _lastInput;

    private string[] _controllerNames;
    private string _firstController;

    [HideInInspector]
    public bool readGameInput, readMenuInput;

    private float _checkDelay = 1f;
    private float _lastCheck;

    private const string XBOX_CONTROLLER = "Xbox ";
    private const string PS_CONTROLLER = "PS ";
    private const string RP_CONTROLLER = "RumblePad ";
    
    private bool _isXboxController;
    private bool _isPSController;
    private bool _isRPController;

    private const int OLD = 0;
    private const int NEW = 1;

    private string[] _buttonAxes = new string[] { "Jump", "Activation", "Aim", "Throw", "Change Skill", "Freeze", "Pause"};
    private float[,] _buttonInputs;
    private string[] _axes = new string[] { "MoveX", "MoveZ", "LookX", "LookY", "Distance", "Rotation"}; 
    private float[] _axisInputs;
    private string[] _menuButtonAxes = new string[] { "SelectBack", "UpDown", "RightLeft" };
    private float[,] _menuButtonInputs;

    public enum Key
    {
        Jump = 0,
        Activation = 1,
        Aim = 2,
        Shoot = 3,
        Change = 4,
        Freeze = 5,
        Pause = 6
    }

    public enum Axis
    {
        MoveX = 0,
        MoveZ = 1,
        LookX = 2,
        LookY = 3,
        Distance = 4,
        Rotation = 5
    }

    public enum MenuButtonAxis
    {
        SelectBack = 0,
        UpDown = 1,
        RightLeft = 2
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CheckController();

        _buttonInputs = new float[_buttonAxes.Length, 2];
        _axisInputs = new float[_axes.Length];
        _menuButtonInputs = new float[_menuButtonAxes.Length, 2];
    }

    private void Update()
    {
#if UNITY_EDITOR

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

#endif

        if((_checkDelay + _lastCheck) < Time.realtimeSinceStartup)
        {
            CheckController();
            _lastCheck = Time.realtimeSinceStartup;
        }

        ReadInput("", true);

        if (_isXboxController)
        {
            ReadInput(XBOX_CONTROLLER, false);
        }
        else if (_isPSController)
        {
            ReadInput(PS_CONTROLLER, false);
        }
        else if (_isRPController)
        {
            ReadInput(RP_CONTROLLER, false);
        }
    }

    public void CheckController()
    {
        _firstController = "";
        _controllerNames = Input.GetJoystickNames();
        
        foreach(string str in _controllerNames)
        {
            if(str.Length > 0)
            {
                _firstController = str.ToLower();

                _isXboxController = _firstController.Contains("xbox");
                _isPSController = _firstController.Contains("wireless controller");
                _isRPController = _firstController.Contains("rumblepad");

                if (_isXboxController || _isPSController || _isRPController)
                    break;
            }
        }
    }

    private void ReadInput(string controller, bool isKeyboard)
    {
        // during transition drop inputs
        if (!readGameInput && !readMenuInput)
        {
            for (int i = 0; i < _axisInputs.Length; i++)
            {
                _axisInputs[i] = 0f;
            }
            for (int i = 0; i < _buttonInputs.Length / 2; i++)
            {
                if (i + 1 == _buttonInputs.Length / 2)
                {   // holding back button wont do ping pong
                    _buttonInputs[i, OLD] = -1f;
                    _buttonInputs[i, NEW] = -1f;
                }
                else
                {
                    _buttonInputs[i, OLD] = 0f;
                    _buttonInputs[i, NEW] = 0f;
                }
            }
            for (int i = 0; i < _menuButtonInputs.Length / 2; i++)
            {
                if (i == 0)
                {   // holding back button wont do ping pong
                    _menuButtonInputs[i, OLD] = -1f;
                    _menuButtonInputs[i, NEW] = -1f;
                }
                else
                {
                    _menuButtonInputs[i, OLD] = 0f;
                    _menuButtonInputs[i, NEW] = 0f;
                }
            }
            return;
        }

        // keyboard and mouse
        if(isKeyboard)
        {
            if (readGameInput)
            {
                for (int i = 0; i < _axes.Length; i++)
                {
                    if (i < 2)
                        _axisInputs[i] = Input.GetAxis(_axes[i]);
                    else
                        _axisInputs[i] = Input.GetAxisRaw(_axes[i]);
                }

                for (int i = 0; i < _buttonAxes.Length; i++)
                {
                    _buttonInputs[i, OLD] = _buttonInputs[i, NEW];
                    _buttonInputs[i, NEW] = Input.GetAxisRaw(_buttonAxes[i]);
                }
            }

            if (readMenuInput)
            {
                for (int i = 0; i < _menuButtonAxes.Length; i++)
                {
                    _menuButtonInputs[i, OLD] = _menuButtonInputs[i, NEW];
                    _menuButtonInputs[i, NEW] = Input.GetAxisRaw(_menuButtonAxes[i]);
                }
            }
        }
        else // joypad
        {
            if (readGameInput)
            {
                for (int i = 0; i < _axes.Length; i++)
                {
                    _axisInputs[i] = MathHelp.FartherFromZero(Input.GetAxisRaw(controller + _axes[i]), _axisInputs[i]);
                }

                for (int i = 0; i < _buttonAxes.Length; i++)
                {
                    // We do not asing old button input here because it is done when reading keyboard and mouse input.
                    // Doing it here would asing new keyboard and mouse input as old input breaking button input.
                    _buttonInputs[i, NEW] = MathHelp.FartherFromZero(Input.GetAxisRaw(controller + _buttonAxes[i]), _buttonInputs[i, NEW]);
                }
            }

            if (readMenuInput)
            {
                for (int i = 0; i < _menuButtonAxes.Length; i++)
                {
                    // We do not asing old button input here because it is done when reading keyboard and mouse input.
                    // Doing it here would asing new keyboard and mouse input as old input breaking button input.
                    _menuButtonInputs[i, NEW] = MathHelp.FartherFromZero(Input.GetAxisRaw(controller + _menuButtonAxes[i]), _menuButtonInputs[i, NEW]);
                }
            }
        }

    }

    private void GetKeyInputs(Key key)
    {
        _newInput = _buttonInputs[(int)key, NEW];
        _lastInput = _buttonInputs[(int)key, OLD];
    }

    private void GetMenuInputs(MenuButtonAxis axis)
    {
        _newInput = _menuButtonInputs[(int)axis, NEW];
        _lastInput = _menuButtonInputs[(int)axis, OLD];
    }

    public float GetAxisInput(Axis axis)
    {
        return _axisInputs[(int)axis];
    }

    public bool KeyUp(Key key)
    {
        GetKeyInputs(key);

        return _lastInput != 0 && _newInput == 0;
    }

    public bool KeyDown(Key key)
    {
        GetKeyInputs(key);

        return _lastInput == 0 && _newInput != 0;
    }

    public bool KeyHold(Key key)
    {
        GetKeyInputs(key);

        return _newInput != 0 && _newInput == _lastInput;
    }

    public bool KeyDown(MenuButtonAxis axis, bool positive)
    {
        GetMenuInputs(axis);

        if (positive)
            return _lastInput == 0 && _newInput > 0;
        else
            return _lastInput == 0 && _newInput < 0;
    }

    public bool KeyHold(MenuButtonAxis axis, bool positive)
    {
        GetMenuInputs(axis);

        if (positive)
            return _newInput > 0 && _lastInput > 0;
        else
            return _newInput < 0 && _lastInput < 0;
    }
}
