using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ItemDB
{
    public static event Action OnItemsLoaded;
    private static Dictionary<int, Item> _items = [];

    public static void LoadItems()
    {
        _items.Clear();

        string itemsFolder = "res://Assets/Items/";
        var dir = DirAccess.Open(itemsFolder);

        if (dir != null)
        {
            dir.ListDirBegin();

            string fileName;
            while ((fileName = dir.GetNext()) != "")
            {
                if (fileName.EndsWith(".tres"))
                {
                    string filePath = itemsFolder + fileName;
                    Item item = ResourceLoader.Load<Item>(filePath);

                    if (item != null)
                    {
                        _items[item.Id] = item;
                        GD.Print($"Loaded item: {item.Name} (ID: {item.Id})");
                    }
                    else
                    {
                        GD.PrintErr($"Failed to load item: {filePath}");
                    }
                }
            }
            OnItemsLoaded?.Invoke();
        }
        else
        {
            GD.PrintErr($"Failed to open items directory: {itemsFolder}");
        }
    }

    public static Item GetItemById(int id)
    {
        return _items.TryGetValue(id, out var item) ? item : null;
    }

    public static Item[] GetAllItems()
    {
        return _items.Values.ToArray();
    }
}
