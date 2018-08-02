using UnityEngine;

public class ItemColor : MonoBehaviour
{
    private GameManager _gameManager;
    private MeshRenderer _mr;

    public int materialIndex;
    private Color _defaultColor;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _mr = GetComponent<MeshRenderer>();

        _defaultColor = _mr.materials[materialIndex].color;
    }

    public void SetModeColor(int mode)
    {
        if (mode == 0)
            _mr.materials[materialIndex].color = _defaultColor;
        else
            _mr.materials[materialIndex].color = _gameManager.itemModeColors[mode - 1];
    }
}
