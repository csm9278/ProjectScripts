using UnityEngine;

public class Pistol : Gun
{
    public string bullName;

    private void Awake()
    {
        InitGun(bullName);
    }

    private void Start() => StartFunc();

    private void StartFunc()
    {
        //InitGun(bullName);
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        OneShotUpdate();
        ReloadUpdate();
    }
}