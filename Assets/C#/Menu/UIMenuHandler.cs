using UnityEngine;

public class UIMenuHandler : MonoBehaviour
{
    // menu
    public float sliderSpeed;
    public float scaleFactor, scaleSpeed;
    private float _scaleTimer, _sinValue;
    private Vector3 _activeItemScale;
    private Vector3 _activeItemTextScale;
    private UIMenuItem[] _menuItems;
    private UIMenuItem _activeItem;

    // game to menu transition
    public float transitionSpeed;
    private float _transitionTimer;
    private bool _isInTransition;
    private Camera _menuCamera;
    private Camera _mainCamera;
    private CameraControl _cameraControl;
    private Rect _menuRect = new Rect(0, 0, 1, 1);
    private Rect _mainRect = new Rect(1, 0, 1, 1);

    // input
    private InputHandler _inputHandler;
    private bool _readMenuInput;
    private bool _readGameInput;
    private bool _ignoreInitialUnpause = true;

    // music
    private AudioSource _musicSource;

    // reset
    private PlayerMove _playerMove;
    private Item[] _items;

    private void Start()
    {
        #region camera handling

        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.tag.Contains("Menu"))
                _menuCamera = cam;
            if (cam.tag.Contains("Main"))
                _mainCamera = cam;
        }

        _menuCamera.rect = _menuRect;
        _mainCamera.rect = _mainRect;

        #endregion

        #region menuitems and audio

        _menuItems = GetComponentsInChildren<UIMenuItem>();
        foreach (UIMenuItem item in _menuItems)
        {
            if (item.isMusicNoice)
            {
                item.musicNoice = SaveLoad.Floats[SaveLoad.MUSIC_NOICE];
                item.UpdateVolumeSlider(0f);
            }

            if (item.isSoundNoice)
            {
                item.soundNoice = SaveLoad.Floats[SaveLoad.SOUND_NOICE];
                item.UpdateVolumeSlider(0f);
            }

            if (item.isDefault)
                SetActiveItem(item);
        }

        #endregion

        _readMenuInput = true;
        _readGameInput = false;
        _inputHandler = FindObjectOfType<InputHandler>();
        _inputHandler.readMenuInput = _readMenuInput;
        _inputHandler.readGameInput = _readGameInput;

        Time.timeScale = 0f;

        #region music

        _musicSource = _mainCamera.GetComponent<AudioSource>();

        _musicSource.time = 2 * 60 + 20;
        _musicSource.Play();
        _musicSource.volume = SaveLoad.Floats[SaveLoad.MUSIC_NOICE];

        #endregion
    }

    private void Update()
    {
        #region inputs

        if (_readGameInput && _inputHandler.KeyDown(InputHandler.Key.Pause))
        {
            DoPause();
        }

        if (_readMenuInput)
        {
            if (_inputHandler.KeyDown(InputHandler.MenuButtonAxis.SelectBack, positive: true))
                DoSelect();
            if (_inputHandler.KeyDown(InputHandler.MenuButtonAxis.SelectBack, positive: false))
                DoUnpause();

            if (_inputHandler.KeyDown(InputHandler.MenuButtonAxis.UpDown, positive: true))
                DoUp();
            if (_inputHandler.KeyDown(InputHandler.MenuButtonAxis.UpDown, positive: false))
                DoDown();

            if (_activeItem.isSoundNoice || _activeItem.isMusicNoice)
            {
                if (_inputHandler.KeyHold(InputHandler.MenuButtonAxis.RightLeft, positive: true))
                    DoRight();
                if (_inputHandler.KeyHold(InputHandler.MenuButtonAxis.RightLeft, positive: false))
                    DoLeft();
            }
            else
            {
                if (_inputHandler.KeyDown(InputHandler.MenuButtonAxis.RightLeft, positive: true))
                    DoRight();
                if (_inputHandler.KeyDown(InputHandler.MenuButtonAxis.RightLeft, positive: false))
                    DoLeft();
            }
        }

        #endregion

        #region camera handling and read input logic

        if (_isInTransition)
        {
            _transitionTimer = MathHelp.Clamp(_transitionTimer + Time.unscaledDeltaTime * transitionSpeed, 0f, 1f);
            _menuRect.x = _transitionTimer;
            _mainRect.x = _transitionTimer - 1;

            _menuCamera.rect = _menuRect;
            _mainCamera.rect = _mainRect;

            if (_transitionTimer == 1f)
            {   // game
                _readGameInput = true;
                _menuCamera.enabled = false;
                _isInTransition = false;
                Time.timeScale = 1f;
            }

            if (_transitionTimer == 0f)
            {   // menu
                _readMenuInput = true;
                _mainCamera.enabled = false;
                _isInTransition = false;
            }

            _inputHandler.readMenuInput = _readMenuInput;
            _inputHandler.readGameInput = _readGameInput;
        }

        #endregion

        #region scale active

        _scaleTimer += Time.unscaledDeltaTime * scaleSpeed;
        _sinValue = Mathf.Sin(_scaleTimer) * scaleFactor + 1f + scaleFactor * 2;
        if (_activeItem != null)
        {
            _activeItem.transform.localScale = _activeItemScale * _sinValue;

            if (_activeItem.text != null)
                _activeItem.text.transform.localScale = _activeItemTextScale * _sinValue;
        }

        #endregion
    }
    
    private void DoSelect()
    {
        if (_activeItem.isPlay)
        {
            _ignoreInitialUnpause = false;
            DoUnpause();
        }

        if (_activeItem.isReset)
            DoReset();

        if (_activeItem.isQuit)
            Application.Quit();

        if (_activeItem.isDeleteSaveFile)
            SaveLoad.Delete();
    }

    private void DoUp()
    {
        if (_activeItem.upItem != null)
            SetActiveItem(_activeItem.upItem);
    }

    private void DoDown()
    {
        if (_activeItem.downItem != null)
            SetActiveItem(_activeItem.downItem);
    }

    private void DoLeft()
    {
        #region slider handling

        if (_activeItem.isMusicNoice)
        {
            _activeItem.UpdateVolumeSlider(-sliderSpeed);
            _musicSource.volume = SaveLoad.Floats[SaveLoad.MUSIC_NOICE];
            return;
        }

        if (_activeItem.isSoundNoice)
        {
            _activeItem.UpdateVolumeSlider(-sliderSpeed);
            return;
        }

        #endregion
    }

    private void DoRight()
    {
        #region slider handling

        if (_activeItem.isMusicNoice)
        {
            _activeItem.UpdateVolumeSlider(sliderSpeed);
            _musicSource.volume = SaveLoad.Floats[SaveLoad.MUSIC_NOICE];
            return;
        }

        if (_activeItem.isSoundNoice)
        {
            _activeItem.UpdateVolumeSlider(sliderSpeed);
            return;
        }

        #endregion
    }

    private void SetActiveItem(UIMenuItem item)
    {
        // clear old item
        if (_activeItem != null)
        {
            _activeItem.isHighlighted = false;
            _activeItem.transform.localScale = _activeItemScale;

            if (_activeItem.text != null)
                _activeItem.text.transform.localScale = _activeItemTextScale;
        }

        // swap
        _activeItem = item;

        // setup new item
        _activeItem.isHighlighted = true;
        _activeItemScale = _activeItem.transform.localScale;
        if (_activeItem.text != null)
            _activeItemTextScale = _activeItem.text.transform.localScale;
    }

    private void DoPause()
    {
        Time.timeScale = 0f;
        if (transitionSpeed > 0)
            transitionSpeed = -transitionSpeed;
        
        _readGameInput = false;
        _readMenuInput = false;
        _mainCamera.enabled = true;
        _menuCamera.enabled = true;

        _isInTransition = true;
    }

    private void DoUnpause()
    {
        if (_ignoreInitialUnpause)
            return;

        if (transitionSpeed < 0)
            transitionSpeed = -transitionSpeed;

        _readGameInput = false;
        _readMenuInput = false;
        _mainCamera.enabled = true;
        _menuCamera.enabled = true;

        _isInTransition = true;
    }

    private void DoReset()
    {
        _playerMove = FindObjectOfType<PlayerMove>();
        _playerMove.PlayerToLastCheckpoint();

        _items = FindObjectsOfType<Item>();
        foreach (Item item in _items)
            item.DoReset();
    }
}
