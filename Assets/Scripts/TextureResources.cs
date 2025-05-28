using UnityEngine;

public class TextureResources : MonoBehaviour
{
    // public Sprite[] ListItem;
    public Texture[] ListBlockTexture;
    public Sprite[] ListBlockUI;

    void Awake()
    {
        // ListItem = Resources.LoadAll<Sprite>("Items");
        ListBlockTexture = Resources.LoadAll<Texture>("Texture");
        ListBlockUI = Resources.LoadAll<Sprite>("UIBlock");

        // LoadImageAsync();
    }

    
    // private async void LoadImageAsync()
    // {
    //     await LoadSpriteFromResourcesAsync("Items");
    //     Debug.Log("Items loaded");
    //     await LoadSpriteFromResourcesAsync("ListMiniGameItem");
    //     Debug.Log("ListMiniGameItem loaded");
    //     await LoadSpriteFromResourcesAsync("ListNewArea");
    //     Debug.Log("ListNewArea loaded");
    // }

    // private async Task LoadSpriteFromResourcesAsync(string path)
    // {
    //     ResourceRequest request = Resources.LoadAsync<Sprite>(path);
        
    //     while (!request.isDone)
    //     {
    //         Debug.Log("??");
    //         await Task.Yield();
    //     }
    // }

    // void TakeImagesFromResources(Sprite[] list, string path) => list = Resources.LoadAll<Sprite>(path); 

    // public Texture TakeTexture(Texture[] list, string name)
    // {
    //     Texture itemNeed = null;
    //     foreach (Texture item in list){
    //         if(item.name.ToLower() == name.ToLower()) 
    //         {
    //             itemNeed = item;
    //             break;
    //         }
    //     }
    //     return itemNeed;
    // }
}