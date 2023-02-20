using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Item[] Items;

    private void Start() => StartFunc();

    private void StartFunc()
    {
         
    }

    private void Update() => UpdateFunc();

    private void UpdateFunc()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < Items.Length; i++)
            {
                Items[i].ResetItem();
            }
        }
    }
}