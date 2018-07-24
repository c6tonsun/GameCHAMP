public class Checkpoint : VisualizedOverlaps
{
    private PlayerMove _playerMove;

    private void Start()
    {
        _playerMove = FindObjectOfType<PlayerMove>();
    }

    private new void Update()
    {
        base.Update();

        if (_colliders.Length > 0)
            _playerMove.SetCheckPoint(transform.GetChild(0));
    }
}
