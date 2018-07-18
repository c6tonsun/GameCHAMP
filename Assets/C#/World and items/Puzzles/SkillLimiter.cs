using UnityEngine;

public class SkillLimiter : VisualizedOverlaps
{
    private PlayerSkills _playerSkills;
    private int _playerLayer;

    private void Start()
    {
        _playerSkills = FindObjectOfType<PlayerSkills>();
    }

    private new void Update()
    {
        base.Update();
        
        _playerSkills.forceAimToFalse = _colliders.Length > 0;
    }
}
