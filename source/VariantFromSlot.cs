using AttributeRenderingLibrary;
using PlayerInventoryLib;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Backpacks;

public class VariantFromSlotConfig
{
    public Dictionary<string, string> SlotsToVariants { get; set; } = [];
    public string TargetVariant { get; set; } = "";
}

public class VariantFromSlotBehavior : CollectibleBehavior, IOnSlotModifiedListener
{
    public VariantFromSlotBehavior(CollectibleObject collObj) : base(collObj)
    {
    }

    public override void Initialize(JsonObject properties)
    {
        base.Initialize(properties);

        Config = properties.AsObject<VariantFromSlotConfig>() ?? new();
    }

    public virtual void OnSlotModified(ItemSlot slot)
    {
        if (slot is not IPlayerInventorySlot playerSlot || slot.Itemstack == null) return;

        string slotId = playerSlot.SlotId;
        if (!Config.SlotsToVariants.TryGetValue(slotId, out string? variantValue)) return;
        
        Variants variants = Variants.FromStack(slot.Itemstack);
        if (variants.Get(Config.TargetVariant) == variantValue) return;

        variants.Set(Config.TargetVariant, variantValue);
        variants.ToStack(slot.Itemstack);
        slot.MarkDirty();
    }

    protected VariantFromSlotConfig Config = new();
}