using UnityEngine;
using System.Collections;

public class PlayerSkills : MonoBehaviour
{
    private InputHandler _inputHandler;
    private CameraControl _camControl;
    private PlayerManipulationArea _playerManipulationArea;

    private PlayerAim _playerAim;
    private PlayerGravity _playerGravity;
    private PlayerAnimation _playerAnimation;

    private Item _currentItem;
    private Item _lastItem;
    private SlideDoor _slideDoor;

    private float _distance;
    private RaycastHit _hit;

    [HideInInspector]
    public bool forceAimToFalse;
    private bool _useAim = false;
    private bool _alreadyActivated;
    private bool _doActivation;
    private bool _doFreeze;
    private bool _doShoot;

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
        _inputHandler = FindObjectOfType<InputHandler>();
        _playerManipulationArea = FindObjectOfType<PlayerManipulationArea>();
        
        _playerAim = GetComponent<PlayerAim>();
        _playerGravity = GetComponent<PlayerGravity>();
        _playerAnimation = GetComponentInChildren<PlayerAnimation>();

        ChangeSkillMode();
    }
    
    void Update()
    {
        #region inputs
        
        // skill mode
        if (!_alreadyActivated && _inputHandler.KeyDown(InputHandler.Key.Change))
        {
            ChangeSkillMode();
        }

        // aim toggle
        if (forceAimToFalse || !_playerGravity.isGrounded)
            _useAim = false;
        else if (_inputHandler.KeyDown(InputHandler.Key.Aim))
            _useAim = !_useAim;

        if (_useAim)
        {
            _playerAim.AimOn();
        }
        else
        {
            _alreadyActivated = false;
            _playerAim.AimOff();
        }

        // activation
        _doActivation = _inputHandler.KeyDown(InputHandler.Key.Activation);
        if (_doActivation && _useAim)
            _alreadyActivated = !_alreadyActivated;

        // freeze
        if (_inputHandler.KeyDown(InputHandler.Key.Freeze) && _alreadyActivated)
            _doFreeze = true;

        // shoot
        if (_inputHandler.KeyDown(InputHandler.Key.Shoot))
            _doShoot = true;

        // distance
        if (_useAim)
        {
            if(Physics.Raycast(_camControl.transform.position, _camControl.transform.forward, out _hit, float.MaxValue, LayerMask.NameToLayer("Item")))
            {
                if(_hit.distance - _camControl.currentDistance > _maxDistance)
                {
                    _distance = MathHelp.Clamp(_distance + _inputHandler.GetAxisInput(InputHandler.Axis.Distance), _minDistance, _maxDistance);
                }
                else
                {
                    _distance = MathHelp.Clamp(_distance + _inputHandler.GetAxisInput(InputHandler.Axis.Distance), _minDistance, _hit.distance - _camControl.currentDistance);
                }
            }
            else
            {
                _distance = MathHelp.Clamp(_distance + _inputHandler.GetAxisInput(InputHandler.Axis.Distance), _minDistance, _maxDistance);
            }
        }

        #endregion

        if (!_useAim && Physics.Raycast(_camControl.transform.position, _camControl.transform.forward, out _hit, float.MaxValue))
        {
            _slideDoor = _hit.collider.GetComponent<SlideDoor>();
            if (_slideDoor != null && _slideDoor.isInteractable && _doActivation)
                _slideDoor.Interact();
            
            if (_currentItem != null)
            {
                _currentItem.SetGravityMode(Item.GravityMode.World);
                _currentItem = null;
            }

            _playerManipulationArea.SetVisible(false);
            _playerAnimation.doLookAt = false;
        }
        else if (currentSkillMode == SkillMode.Single)
        {
            DoSingle();
        }
        else if (currentSkillMode == SkillMode.Area)
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

        // player look at + return
        if (_currentItem == null)
        {
            _alreadyActivated = false;
            _playerAnimation.doLookAt = false;
            return;
        }
        else
        {
            _playerAnimation.target = _currentItem.transform;
            _playerAnimation.doLookAt = true;
        }

        // activate item
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
            return;
        }

        // shoot item
        if (_doShoot)
        {
            _currentItem.SetGravityMode(Item.GravityMode.World);
            StartCoroutine(Shoot());
            _doShoot = false;
            return;
        }

        // freeze item
        if (_doFreeze)
        {
            _currentItem.SetGravityMode(Item.GravityMode.Freeze);
            _alreadyActivated = false;
            _doFreeze = false;
            return;
        }

        // move item
        Move(_currentItem.transform, 0.1f, isItem: true);
    }

    private void DoArea()
    {
        // head look at
        _playerAnimation.target = _playerManipulationArea.transform;
        _playerAnimation.doLookAt = _useAim;

        #region activate items in area

        if (_alreadyActivated)
        {
            if (!_playerManipulationArea.itemsActivated)
            {
                if(_playerManipulationArea.HasItems())
                {
                    _playerManipulationArea.ActivateItems();
                }
                else
                {
                    _alreadyActivated = false;
                }
            }
        }
        else
        {
            _playerManipulationArea.DeactivateItems();
        }

        #endregion

        // move
        if (_playerManipulationArea != null)
            Move(_playerManipulationArea.transform, 0.1f, isItem: false);
    }

    private void RaycastHandling()
    {
        _lastItem = _currentItem;

        if (Physics.Raycast(_camControl.transform.position, _camControl.transform.forward, out _hit, float.MaxValue))
        {
            _currentItem = _hit.collider.GetComponent<Item>();

            if(_currentItem != null)
            {
                _currentItem.SetGravityMode(Item.GravityMode.Self);
            }

            if (_lastItem != null && _currentItem != _lastItem)
            {
                _lastItem.SetGravityMode(Item.GravityMode.World);
            }
        }
        else
        {
            _currentItem = null;
        }
    }
    
    private void Move(Transform toMove, float lerp, bool isItem)
    {
        Vector3 targetPos = _camControl.transform.position + (_camControl.transform.forward * (_distance + _camControl.currentDistance));

        Vector3 oldPos = toMove.position;
        Vector3 newPos = Vector3.Lerp(oldPos, targetPos, lerp);
        
        if (isItem)
        {
            _currentItem.DoRotate(_inputHandler.GetAxisInput(InputHandler.Axis.Rotation));
            
            if (_currentItem.CanMoveCheck(newPos - oldPos))
            {
                _currentItem.SetGravityMode(Item.GravityMode.Player);
                toMove.position = newPos;
            }
            else
            {
                _currentItem.SetGravityMode(Item.GravityMode.ERROR);
                toMove.position = oldPos;
            }
        }
        else
        {
            toMove.position = newPos;
        }
    }

    public void ChangeSkillMode()
    {
        int curIndex = (int)currentSkillMode + 1;

        if (curIndex > lastSkillIndex)
        {
            curIndex = 0;
        }

        currentSkillMode = (SkillMode)curIndex;
        
        if (currentSkillMode == SkillMode.Area)
        {
            _playerManipulationArea.SetVisible(true);
            Move(_playerManipulationArea.transform, 1f, isItem: false);
        }
        else
        {
            _playerManipulationArea.SetVisible(false);
        }
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForFixedUpdate();
        _currentItem.rb.AddForce(_camControl.transform.forward * 100, ForceMode.Impulse);
        _alreadyActivated = false;
    }
}