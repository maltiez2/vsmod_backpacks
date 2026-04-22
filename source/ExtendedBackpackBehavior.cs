using AttributeRenderingLibrary;
using PlayerInventoryLib;
using PlayerInventoryLib.Backpacks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Backpacks;

/*public class ExtendedBackpackConfig
{
    public string RightHandVariant { get; set; } = "right_slot";
    public string LeftHandVariant { get; set; } = "left_slot";
    public string RightHandStateVariant { get; set; } = "right_slot_state";
    public string LeftHandStateVariant { get; set; } = "left_slot_state";
    public string EmptyStateCode { get; set; } = "empty";
    public string FullStateCode { get; set; } = "full";
    public string RightWeaponMetalVariant { get; set; } = "right_metal";
    public string RightWeaponLeatherVariant { get; set; } = "right_leather";
    public string RightWeaponWoodVariant { get; set; } = "right_wood";
    public string LeftWeaponMetalVariant { get; set; } = "left_metal";
    public string LeftWeaponLeatherVariant { get; set; } = "left_leather";
    public string LeftWeaponWoodVariant { get; set; } = "left_wood";


}

public class ExtendedBackpackBehavior : BackpackBehavior
{
    public ExtendedBackpackBehavior(CollectibleObject collObj) : base(collObj)
    {
    }

    public ExtendedBackpackConfig ExtendedConfig { get; set; } = new();


    public override void Initialize(JsonObject properties)
    {
        base.Initialize(properties);

        ExtendedConfig = properties.AsObject<ExtendedBackpackConfig>() ?? new();
    }

    public override void OnBackpackSlotModified(IBackpackSlot backpackSlot)
    {
        base.OnBackpackSlotModified(backpackSlot);

        BackpackInventory backpackInventory = (backpackSlot as ItemSlot)?.Inventory as BackpackInventory ?? throw new Exception();
        CharacterInventory characterInventory = backpackInventory.Player?.InventoryManager.GetOwnInventory(GlobalConstants.characterInvClassName) as CharacterInventory ?? throw new Exception();

        IEnumerable<string> variantCodes = backpackInventory
            .OfType<ItemSlotBagContentWithWildcardMatch>()
            .Where(slot => slot.Config.SetVariants)
            .Select(slot => slot.Config.SlotVariant)
            .Distinct();

        foreach (string variantCode in variantCodes)
        {
            ItemSlotBagContentWithWildcardMatch quiverSlot = backpackInventory
                .OfType<ItemSlotBagContentWithWildcardMatch>()
                .First(slot => slot.Config.SlotVariant == variantCode);

            ItemSlotBagContentWithWildcardMatch? quiverNotEmptySlot = backpackInventory
                .OfType<ItemSlotBagContentWithWildcardMatch>()
                .Where(slot => !slot.Empty && slot.Config.SlotVariant == variantCode)
                .FirstOrDefault((ItemSlotBagContentWithWildcardMatch?)null);

            string stateVariantCode = quiverSlot.Config.SlotStateVariant;

            Variants? variants;

            if (quiverNotEmptySlot == null)
            {
                variants = Variants.FromStack(sheathSlot.Itemstack);
                if (variants.Get(stateVariantCode) != quiverSlot.Config.EmptyStateCode)
                {
                    variants.Set(stateVariantCode, quiverSlot.Config.EmptyStateCode);
                    variants.ToStack(sheathSlot.Itemstack);
                    sheathSlot.MarkDirty();
                }
                continue;
            }

            variants = Variants.FromStack(sheathSlot.Itemstack);
            if (variants.Get(stateVariantCode) != quiverSlot.Config.FullStateCode)
            {
                variants.Set(stateVariantCode, quiverSlot.Config.FullStateCode);
                variants.ToStack(sheathSlot.Itemstack);
                sheathSlot.MarkDirty();
            }

            if (quiverSlot.Itemstack?.Collectible?.Attributes == null) continue;

            string metalVariantCode = quiverSlot.Config.SlotMetalVariant;
            string leatherVariantCode = quiverSlot.Config.SlotLeatherVariant;
            string woodVariantCode = quiverSlot.Config.SlotWoodVariant;

            SheathableStats stats = quiverNotEmptySlot.Itemstack.Collectible.Attributes.AsObject<SheathableStats>();

            variants = Variants.FromStack(sheathSlot.Itemstack);
            if (variants.Get(variantCode) != stats.InSheathVariantCode)
            {
                variants.Set(variantCode, stats.InSheathVariantCode);
                variants.ToStack(sheathSlot.Itemstack);
                sheathSlot.MarkDirty();
            }

            if (quiverSlot.Config.SetMaterialVariants)
            {
                Variants inQuiverVariants = Variants.FromStack(quiverSlot.Itemstack);

                if (inQuiverVariants.Get(metalVariantCode) != null)
                {
                    if (variants.Get(metalVariantCode) != inQuiverVariants.Get(metalVariantCode))
                    {
                        variants.Set(metalVariantCode, inQuiverVariants.Get(metalVariantCode));
                        variants.ToStack(sheathSlot.Itemstack);
                        sheathSlot.MarkDirty();
                    }
                }
                else
                {
                    if (variants.Get(metalVariantCode) != stats.MetalVariantCode)
                    {
                        variants.Set(metalVariantCode, stats.MetalVariantCode);
                        variants.ToStack(sheathSlot.Itemstack);
                        sheathSlot.MarkDirty();
                    }
                }

                if (inQuiverVariants.Get(leatherVariantCode) != null)
                {
                    if (variants.Get(leatherVariantCode) != inQuiverVariants.Get(leatherVariantCode))
                    {
                        variants.Set(leatherVariantCode, inQuiverVariants.Get(leatherVariantCode));
                        variants.ToStack(sheathSlot.Itemstack);
                        sheathSlot.MarkDirty();
                    }
                }
                else
                {
                    if (variants.Get(leatherVariantCode) != stats.LeatherVariantCode)
                    {
                        variants.Set(leatherVariantCode, stats.LeatherVariantCode);
                        variants.ToStack(sheathSlot.Itemstack);
                        sheathSlot.MarkDirty();
                    }
                }

                if (inQuiverVariants.Get(woodVariantCode) != null)
                {
                    if (variants.Get(woodVariantCode) != inQuiverVariants.Get(woodVariantCode))
                    {
                        variants.Set(woodVariantCode, inQuiverVariants.Get(woodVariantCode));
                        variants.ToStack(sheathSlot.Itemstack);
                        sheathSlot.MarkDirty();
                    }
                }
                else
                {
                    if (variants.Get(woodVariantCode) != stats.WoodVariantCode)
                    {
                        variants.Set(woodVariantCode, stats.WoodVariantCode);
                        variants.ToStack(sheathSlot.Itemstack);
                        sheathSlot.MarkDirty();
                    }
                }
            }
        }
    }
}*/