using UnityEngine;

public class UIMenuItem : MonoBehaviour
{
    public bool isDefault;
    public UIMenuItem upItem;
    public UIMenuItem downItem;
    [HideInInspector]
    public bool isHighlighted;
    
    [Space(15)]
    public bool isPlay;

    [Space(15)]
    public bool isReset;

    [Space(15)]
    public bool isQuit;

    [Space(15)]
    public bool isDeleteSaveFile;

    [Space(15)]
    public bool isMusicNoice;
    [Range(0f, 1f)]
    public float musicNoice;

    [Space(15)]
    public bool isSoundNoice;
    [Range(0f, 1f)]
    public float soundNoice;

    [Space(15)]
    public Transform mute;
    public Transform noice;
    public Transform text;
    private float _lerpTime;
    
    public void UpdateVolumeSlider(float speed)
    {
        if (isMusicNoice)
            _lerpTime = musicNoice;
        if (isSoundNoice)
            _lerpTime = soundNoice;

        _lerpTime = MathHelp.Clamp(_lerpTime + Time.unscaledDeltaTime * speed, 0f, 1f);

        transform.position = Vector3.Lerp(mute.position, noice.position, _lerpTime);
        transform.rotation = Quaternion.Lerp(mute.rotation, noice.rotation, _lerpTime);

        if (isMusicNoice)
        {
            musicNoice = _lerpTime;
            SaveLoad.Floats[SaveLoad.MUSIC_NOICE] = musicNoice;
        }
        if (isSoundNoice)
        {
            soundNoice = _lerpTime;
            SaveLoad.Floats[SaveLoad.SOUND_NOICE] = soundNoice;
        }

        SaveLoad.Save();
    }
}
