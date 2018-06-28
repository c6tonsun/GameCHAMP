using UnityEngine;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
    private float _newInput;
    private float _lastInput;

    private string[] _controllerNames;
    private string _firstController;

    private float _checkDelay = 5f;
    private float _lastCheck;

    private const string XBOX_CONTROLLER = "Xbox ";
    private const string PS_CONTROLLER = "PS ";
    private const string RP_CONTROLLER = "RumblePad ";

    private const int OLD = 0;
    private const int NEW = 1;

    private string[] _buttonAxes = new string[] { "Jump", "Activation", "Aim", "Push", "Pull", "Shoot", "Change Skill", "Freeze"};
    private float[,] _buttonInputs;
    private string[] _axes = new string[] { "MoveX", "MoveZ", "LookX", "LookY", "Distance", "Rotation"};
    private float[] _axisInputs; 

    private float _tempInput;

    public enum Key
    {
        Jump = 0,
        Activation = 1,
        Aim = 2,
        Push = 3,
        Pull = 4,
        Shoot = 5,
        Change = 6,
        Freeze = 7
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

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CheckController();
        _buttonInputs = new float[_buttonAxes.Length, 2];
        _axisInputs = new float[_axes.Length];
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

        ReadInput("", true);

        if (_firstController.Length > 0)
        {
            if (_firstController.Contains("xbox"))
            {
                ReadInput(XBOX_CONTROLLER, false);
            }
            else if (_firstController.Contains("wireless controller"))
            {
                ReadInput(PS_CONTROLLER, false);
            }
            else if (_firstController.Contains("rumblepad"))
            {
                ReadInput(RP_CONTROLLER, false);
            }

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

    private void ReadInput(string controller, bool isKeyboard)
    {
        // key mouse
        if(isKeyboard)
        {
            for(int i = 0; i < _axes.Length; i++)
            {
                _axisInputs[i] = Input.GetAxisRaw(controller + _axes[i]);
            }

            for(int i = 0; i < _buttonAxes.Length; i++)
            {
                _buttonInputs[i, OLD] = _buttonInputs[i, NEW];
                _buttonInputs[i, NEW] = Input.GetAxisRaw(controller + _buttonAxes[i]);
            }
        }
        else // joy pad
        {
            for (int i = 0; i < _axes.Length; i++)
            {
                _axisInputs[i] = MathHelp.AbsBiggest(Input.GetAxisRaw(controller + _axes[i]), _axisInputs[i]);
            }

            for (int i = 0; i < _buttonAxes.Length; i++)
            {
                _buttonInputs[i, OLD] = _buttonInputs[i, NEW];

                _buttonInputs[i, NEW] = MathHelp.AbsBiggest(Input.GetAxisRaw(controller + _buttonAxes[i]), _buttonInputs[i, NEW]);
            }
        }

    }

    private void GetKeyInputs(Key key, out float newInput, out float lastInput)
    {
        newInput = _buttonInputs[(int)key, NEW];
        lastInput = _buttonInputs[(int)key, OLD];
    }

    public float GetAxisInput(Axis axis)
    {
        return _axisInputs[(int)axis];
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
