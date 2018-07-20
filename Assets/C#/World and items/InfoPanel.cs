using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    private Material _infoMaterial;
    private Color _defaultColor;

    // fade
    [Range(0, 1)]
    public float minAlpha = 0.1f, maxAlpha = 1f;
    public float alphaSpeed = 2;
    private float _fadeTimer;
    private float _sinValue;
    private bool _stopFade = true;

    // show / hide info
    // Note: scale = tiling
    public float showHideSpeed = 2;
    private float _showHideLerp;
    private Vector2 hideScale = new Vector2(1f, 0f);
    private Vector2 showScale = new Vector2(1f, 1f);
    private Vector2 hideOffset = new Vector2(0f, 0.05f);
    private Vector2 showOffset = new Vector2(0f, 0f);

    private void Start()
    {
        _infoMaterial = GetComponent<MeshRenderer>().material;
        _defaultColor = _infoMaterial.color;

        HideInfo();
    }

    private void Update()
    {
        if (_stopFade && _sinValue > 0.9f)
        {
            #region show and hide

            _showHideLerp = MathHelp.Clamp(_showHideLerp + Time.deltaTime * showHideSpeed, 0f, 1f);
            
            _infoMaterial.mainTextureOffset = Vector2.Lerp(hideOffset, showOffset, _showHideLerp);
            _infoMaterial.mainTextureScale = Vector2.Lerp(hideScale, showScale, _showHideLerp);

            _stopFade = _showHideLerp != 1f;

            #endregion
        }
        else
        {
            #region alpha fade

            // timer
            _fadeTimer += Time.deltaTime * alphaSpeed;

            _sinValue = Mathf.Sin(_fadeTimer) * 0.5f + 0.5f;

            // set colors alpha value (between min and max)
            _defaultColor.a = minAlpha + _sinValue * (maxAlpha - minAlpha);
            // set color to material
            _infoMaterial.color = _defaultColor;

            #endregion
        }
    }

    public void ShowInfo()
    {
        if (showHideSpeed < 0) showHideSpeed = -showHideSpeed;
    }

    public void HideInfo()
    {
        if (showHideSpeed > 0) showHideSpeed = -showHideSpeed;
        _stopFade = true;
    }

    public bool IsShown()
    {
        return _showHideLerp == 1f;
    }

    public bool IsHidden()
    {
        return _showHideLerp == 0f;
    }

    public void SetAlbedo(Texture texture)
    {
        _infoMaterial.mainTexture = texture;
    }
}
