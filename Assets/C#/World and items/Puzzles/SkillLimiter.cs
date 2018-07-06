using UnityEngine;

public class SkillLimiter : VisualizedOverlaps
{
    private PlayerSkills _playerSkills;
    private int _playerLayer;

    private void Start()
    {
        _playerSkills = FindObjectOfType<PlayerSkills>();
        _playerLayer = LayerMask.NameToLayer("Player");
    }

    private new void Update()
    {
        base.Update();

        foreach (Collider col in _colliders)
        {
            if (col.gameObject.layer == _playerLayer)
            {
                _playerSkills.forceAimToFalse = true;
                return;
            }
        }
        _playerSkills.forceAimToFalse = false;
    }
}
