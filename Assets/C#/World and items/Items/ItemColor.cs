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

        #region defaultColor

        float mass = GetComponent<Rigidbody>().mass;

        int shorter;
        if (_gameManager.itemMassTreshholds.Length > _gameManager.itemColorsByMass.Length)
            shorter = _gameManager.itemColorsByMass.Length;
        else
            shorter = _gameManager.itemMassTreshholds.Length;

        for (int i = 0; i < shorter; i++)
        {
            if (mass < _gameManager.itemMassTreshholds[i])
            {
                _defaultColor = _gameManager.itemColorsByMass[i];
                mass = -1;
                break;
            }
        }

        if (mass > 0)
            _defaultColor = _gameManager.itemModeColors[0];

        #endregion

        _mr.materials[materialIndex].color = _defaultColor;
    }
}
