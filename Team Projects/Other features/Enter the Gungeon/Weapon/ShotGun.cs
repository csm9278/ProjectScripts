using UnityEngine;

public class ShotGun : Gun
{
    [Header("--- Gun Additive Setting ---")]
    public string bullName;
    public int shotCount;

    private void Start() => StartFunc();

    private void StartFunc()
    {
        InitGun(bullName);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        ManyShotUpdate(shotCount);
        ReloadUpdate();
    }
}