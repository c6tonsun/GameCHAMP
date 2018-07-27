using UnityEngine;

public class Checkpoint : VisualizedOverlaps
{
    public int ID;
    private PlayerMove _playerMove;

    private void Start()
    {
        _playerMove = FindObjectOfType<PlayerMove>();

        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
        foreach (Checkpoint c in checkpoints)
        {
            if (c == this)
                continue;

            if (c.ID == ID)
                Debug.LogError("Two checkpoints have same ID! Please make IDs unique.");
        }
    }

    private new void Update()
    {
        base.Update();

        if (_colliders.Length > 0)
            _playerMove.SetCheckpoint(transform.GetChild(0), ID);
    }
}
