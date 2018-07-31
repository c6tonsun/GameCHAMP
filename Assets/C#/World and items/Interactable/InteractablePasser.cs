using UnityEngine;

public class InteractablePasser : MonoBehaviour, IInteractable
{
    public GameObject interactable;
    private IInteractable _interactable;

    [Header("Color stuff")]
    public int materialIndex;
    private Material _material;
    private Color _defaultColor;
    private Color _otherColor;
    private bool _isUnderCursor;

    private void Start()
    {
        #region color

        _material = GetComponent<MeshRenderer>().materials[materialIndex];
        _defaultColor = _material.color;
        _otherColor = FindObjectOfType<GameManager>().itemModeColors[(int)Item.GravityMode.Self - 1];

        #endregion

        _interactable = interactable.GetComponent<IInteractable>();

        if (_interactable == null)
            Debug.LogError("Wrong gameobject reference");
    }

    private void Update()
    {
        #region color

        if (_isUnderCursor)
            _isUnderCursor = false;
        else
            _material.color = _defaultColor;

        #endregion
    }

    public void Interact()
    {
        _interactable.Interact();
    }

    public void OnCursorHover()
    {
        _isUnderCursor = true;
        _material.color = _otherColor;

        _interactable.OnCursorHover();
    }
}
