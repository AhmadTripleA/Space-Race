using Godot;
using System.Collections.Generic;

public partial class Storage : Node
{
    [Export] public int Capacity = 20; // Default inventory size
    private List<ItemStack> itemStacks = [];

    [Signal]
    public delegate void InventoryChangedEventHandler(); // Signal for UI updates

    public Storage() { itemStacks = [.. new ItemStack[Capacity]]; }

    public Storage(int capacity)
    {
        Capacity = capacity;
        itemStacks = [.. new ItemStack[Capacity]];
    }

    public override void _Ready()
    {
        // Initialize empty slots
        for (int i = 0; i < Capacity; i++) itemStacks[i] = null;
    }

    public static int TransferItem(Storage from, Storage to, Item item, int quantity)
    {
        int removed = from.RemoveItem(item, quantity);
        int remaining = to.AddItem(item, removed);
        if (remaining > 0) from.AddItem(item, remaining);
        return removed - remaining;
    }

    public int AddItem(Item item, int quantity)
    {
        // Try adding to existing stacks.
        foreach (var stack in itemStacks)
        {
            if (stack != null && stack.Item == item)
            {
                quantity = stack.AddToStack(item, quantity);
                if (quantity == 0) break; // If everything was added, exit the loop.
            }
        }

        // Add to an empty slot if necessary.
        for (int i = 0; i < itemStacks.Count; i++)
        {
            if (itemStacks[i] == null && quantity > 0) // Only if there is still something to add
            {
                int added = Mathf.Min(quantity, item.MaxStackSize);
                itemStacks[i] = new ItemStack(item, added);
                quantity -= added;
                if (quantity == 0) break; // Done adding items, exit loop.
            }
        }

        // If there is still remaining quantity, return it (items that couldn't fit)
        EmitSignal(SignalName.InventoryChanged);
        return quantity;
    }

    public int RemoveItem(Item item, int quantity)
    {
        int totalRemoved = 0;

        for (int i = 0; i < itemStacks.Count; i++)
        {
            if (itemStacks[i] != null && itemStacks[i].Item == item)
            {
                int removed = itemStacks[i].RemoveFromStack(quantity);
                totalRemoved += removed;
                quantity -= removed;

                if (itemStacks[i].IsEmpty()) itemStacks[i] = null;

                if (quantity <= 0)
                {
                    EmitSignal(SignalName.InventoryChanged);
                    return totalRemoved;
                }
            }
        }

        EmitSignal(SignalName.InventoryChanged);
        return totalRemoved;
    }

    public int GetItemCount(Item item)
    {
        int total = 0;
        foreach (var stack in itemStacks)
        {
            if (stack != null && stack.Item == item) total += stack.Quantity;
        }
        return total;
    }

    public bool CanStoreItem(Item item, int quantity)
    {
        int remaining = quantity;

        foreach (var stack in itemStacks)
        {
            if (stack != null && stack.Item == item)
            {
                remaining = stack.SpaceLeft();
                if (remaining == 0) return true;
            }
        }

        foreach (var slot in itemStacks)
        {
            if (slot == null) return true;
        }

        return false;
    }

    public List<ItemStack> GetAllStacks()
    {
        List<ItemStack> nonEmptyStacks = [];
        foreach (var stack in itemStacks)
        {
            if (stack != null) nonEmptyStacks.Add(stack);
        }
        return nonEmptyStacks;
    }

    public void PrintStorage()
    {
        GD.Print("Storage contents:");
        for (int i = 0; i < itemStacks.Count; i++)
        {
            if (itemStacks[i] != null)
                GD.Print($"{i}: {itemStacks[i].Item.Name} x{itemStacks[i].Quantity}");
            else
                GD.Print($"{i}: Empty");
        }
    }
}
