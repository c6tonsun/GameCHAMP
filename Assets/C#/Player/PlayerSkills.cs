using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    private InputHandler _inputHandler;

    private PlayerMagnesis _playerMagnesis;
    private PlayerGravity _playerGravity;
    private CameraControl _camControl;

    private Item _currentItem;
    private Item _lastItem;

    private float _distance;
    private RaycastHit _hit;

    private float _lerp = 0.1f;

    private bool _alreadyActivated = false;

    private bool _useAim = false;

    private float _minDistance = 2f;
    private float _maxDistance = 8f;

    // Use this for initialization
    void Start()
    {
        _camControl = FindObjectOfType<CameraControl>();
        _playerMagnesis = GetComponent<PlayerMagnesis>();
        _playerGravity = GetComponent<PlayerGravity>();
        _inputHandler = FindObjectOfType<InputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_alreadyActivated && _useAim)
        {
            RaycastHandling();
        }

        #region aim
        
        if(_inputHandler.KeyDown(InputHandler.Key.Aim) || !_playerGravity.isGrounded)
        {
            _useAim = !_useAim;

            if (!_playerGravity.isGrounded)
                _useAim = false;

            if (_useAim)
            {
                _playerMagnesis.MagnesisOn();
            }
            else
            {
                _alreadyActivated = false;
                _playerMagnesis.MagnesisOff();
                if (_currentItem != null)
                {
                    _currentItem.SetGravityMode(Item.GravityMode.World);
                    _currentItem = null;
                }
            }
        }

        #endregion

        if (_currentItem == null)
        {
            return;
        }

        #region activation

        if (_inputHandler.KeyDown(InputHandler.Key.Activation) && _useAim)
        {
            _alreadyActivated = !_alreadyActivated;
        }

        if (_alreadyActivated)
        {
            if(_currentItem.currentMode == Item.GravityMode.World || _currentItem.currentMode == Item.GravityMode.Self)
            {
                _currentItem.SetGravityMode(Item.GravityMode.Player);
                _distance = _hit.distance - _camControl.currentDistance;
            }
        }
        else
        {
            _currentItem.SetGravityMode(Item.GravityMode.Self);
        }
        
        #endregion

        MoveItem();
    }

    private void RaycastHandling()
    {
        if (Physics.Raycast(_camControl.transform.position, _camControl.transform.forward, out _hit, float.MaxValue))
        {
            _lastItem = _currentItem;
            _currentItem = _hit.collider.GetComponent<Item>();

            if(_currentItem != null && !_alreadyActivated)
            {
                _currentItem.SetGravityMode(Item.GravityMode.Self);
            }

            if (_lastItem != null && _currentItem != _lastItem && !_alreadyActivated)
            {
                _lastItem.SetGravityMode(Item.GravityMode.World);
            }
        }
    }

    private void MoveItem()
    {
        if(_currentItem == null || !_alreadyActivated)
        {
            return;
        }

        Vector3 lastPos = _currentItem.transform.position;

        _distance += _inputHandler.distanceInput;

        if (_distance < _minDistance)
            _distance = _minDistance;
        else if (_distance > _maxDistance)
            _distance = _maxDistance;

        Vector3 pointerPos = _camControl.transform.position + (_camControl.transform.forward * (_distance + _camControl.currentDistance));
        _currentItem.transform.position = Vector3.Lerp(_currentItem.transform.position, pointerPos, _lerp);

        Vector3 movement = _currentItem.transform.position - lastPos;

        if (_currentItem.CanMoveCheck(movement))
        {
            _currentItem.SetGravityMode(Item.GravityMode.Player);
        }
        else
        {
            _currentItem.transform.position = lastPos;
            _currentItem.SetGravityMode(Item.GravityMode.ERROR);
        }

    }
}