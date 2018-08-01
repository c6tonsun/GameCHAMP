using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenuHandler : MonoBehaviour
{
    // menu
    public float sliderSpeed;
    public GameObject highlight;
    public Vector3 highlightScaler = new Vector3(1.2f, 1.2f, 0.8f);
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
                _activeItem = item;
        }

        #endregion

        _readMenuInput = true;
        _readGameInput = false;
        _inputHandler = FindObjectOfType<InputHandler>();
        _inputHandler.readMenuInput = _readMenuInput;
        _inputHandler.readGameInput = _readGameInput;

        Time.timeScale = 0f;

        UpdateHighlightItem();

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

                UpdateHighlightItem();
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
    }
    
    private void DoSelect()
    {
        if (_activeItem.isPlay)
        {
            _ignoreInitialUnpause = false;
            DoUnpause();
        }

        if (_activeItem.isReset)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (_activeItem.isQuit)
            Application.Quit();

        if (_activeItem.isDeleteSaveFile)
            SaveLoad.Delete();
    }

    private void DoUp()
    {
        if (_activeItem.upItem == null)
            return;

        _activeItem.isHighlighted = false;

        _activeItem = _activeItem.upItem;
        _activeItem.isHighlighted = true;

        UpdateHighlightItem();
    }

    private void DoDown()
    {
        if (_activeItem.downItem == null)
            return;

        _activeItem.isHighlighted = false;

        _activeItem = _activeItem.downItem;
        _activeItem.isHighlighted = true;

        UpdateHighlightItem();
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

        if (_activeItem.leftItem == null)
            return;

        _activeItem.isHighlighted = false;

        _activeItem = _activeItem.leftItem;
        _activeItem.isHighlighted = true;

        UpdateHighlightItem();
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

        if (_activeItem.rightItem == null)
            return;

        _activeItem.isHighlighted = false;

        _activeItem = _activeItem.rightItem;
        _activeItem.isHighlighted = true;

        UpdateHighlightItem();
    }
    
    public void UpdateHighlightItem()
    {
        highlight.GetComponent<MeshFilter>().mesh = _activeItem.GetComponent<MeshFilter>().mesh;

        highlight.transform.position = _activeItem.transform.position;
        highlight.transform.rotation = _activeItem.transform.rotation;
        highlight.transform.localScale = MathHelp.MultiplyVector3(_activeItem.transform.localScale, highlightScaler);
    }

    public void DoPause()
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

    public void DoUnpause()
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
}
