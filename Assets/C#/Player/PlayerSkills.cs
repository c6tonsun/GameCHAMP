using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    private InputHandler _inputHandler;

    private PlayerMagnesis _playerMagnesis;
    private PlayerGravity _playerGravity;
    private CameraControl _camControl;
    private PlayerManipulationArea _playerManipulationArea;

    private Item _currentItem;
    private Item _lastItem;

    private float _distance;
    private RaycastHit _hit;

    private float _lerp = 0.1f;

    private bool _alreadyActivated = false;

    private bool _useAim = false;

    private float _minDistance = 3f;
    private float _maxDistance = 10f;

    public SkillMode currentSkillMode;
    private int lastSkillIndex = 1;

    public enum SkillMode
    {
        Area = 0,
        Single = 1
    }

    void Start()
    {
        _camControl = FindObjectOfType<CameraControl>();
        _playerMagnesis = GetComponent<PlayerMagnesis>();
        _playerGravity = GetComponent<PlayerGravity>();
        _inputHandler = FindObjectOfType<InputHandler>();
        _playerManipulationArea = FindObjectOfType<PlayerManipulationArea>();

        ChangeSkillMode();
        _playerManipulationArea.transform.parent = null;
    }
    
    void Update()
    {
        #region inputs
        
        // skill mode
        if (_inputHandler.KeyDown(InputHandler.Key.Change))
        {
            ChangeSkillMode();
            _alreadyActivated = false;
        }

        // aim toggle
        if (_inputHandler.KeyDown(InputHandler.Key.Aim) || !_playerGravity.isGrounded)
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
            }
        }

        // activation
        if (_inputHandler.KeyDown(InputHandler.Key.Activation) && _useAim)
        {
            _alreadyActivated = !_alreadyActivated;
        }

        // distance
        if (_useAim)
            _distance = MathHelp.Clamp(_distance + _inputHandler.distanceInput, _minDistance, _maxDistance);

        #endregion

        if (currentSkillMode == SkillMode.Single)
        {
            DoSingle();
        }
        else if(currentSkillMode == SkillMode.Area)
        {
            DoArea();
        }

    }

    private void DoSingle()
    {
        // check for new item
        if (!_alreadyActivated && _useAim)
        {
            RaycastHandling();
        }

        // drop item
        if (!_useAim && _currentItem != null)
        {
            _currentItem.SetGravityMode(Item.GravityMode.World);
            _currentItem = null;
        }

        if (_currentItem == null)
            return;

        #region activate item

        if (_alreadyActivated)
        {
            if (_currentItem.currentMode == Item.GravityMode.Self)
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

        #region throw item

        if (_inputHandler.KeyDown(InputHandler.Key.Shoot) && _alreadyActivated)
        {
            _currentItem.rb.AddForce(_camControl.transform.forward * 100, ForceMode.Impulse);
            _alreadyActivated = false;
        }

        #endregion

        // move
        if (_currentItem != null && _alreadyActivated)
            Move(_currentItem.transform, isItem: true);
    }

    private void DoArea()
    {
        if (_playerManipulationArea == null)
            return;

        // visibility
        _playerManipulationArea.SetVisible(_useAim);

        #region activate items in area

        if (_alreadyActivated)
        {
            if (!_playerManipulationArea.itemsActivated)
            {
                _playerManipulationArea.ActivateItems();
            }
        }
        else
        {
            _playerManipulationArea.DeactivateItems();
        }

        #endregion

        // move
        if (_playerManipulationArea != null)
            Move(_playerManipulationArea.transform, isItem: false);
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
    
    private void Move(Transform transform, bool isItem)
    {
        Vector3 lastPos = transform.position;
        Vector3 targetPos = _camControl.transform.position + (_camControl.transform.forward * (_distance + _camControl.currentDistance));
        transform.position = Vector3.Lerp(lastPos, targetPos, _lerp);

        #region item collision check

        if (isItem)
        {
            if (_currentItem.CanMoveCheck(transform.position - lastPos))
            {
                _currentItem.SetGravityMode(Item.GravityMode.Player);
            }
            else
            {
                transform.position = lastPos;
                _currentItem.SetGravityMode(Item.GravityMode.ERROR);
            }
        }

        #endregion
    }

    public void ChangeSkillMode()
    {
        int curIndex = (int)currentSkillMode + 1;

        if (curIndex > lastSkillIndex)
        {
            curIndex = 0;
        }

        currentSkillMode = (SkillMode)curIndex;

        _playerManipulationArea.SetVisible(currentSkillMode == SkillMode.Area);
    }
}