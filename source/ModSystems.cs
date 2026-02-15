using Vintagestory.API.Common;

namespace Backpacks;

public sealed class BackpacksSystem : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviorClass("Backpacks:ShapeTexturesFromAttributes", typeof(ShapeTexturesFromAttributes));
        api.RegisterCollectibleBehaviorClass("Backpacks:ShapeReplacement", typeof(ShapeReplacement));
        api.RegisterCollectibleBehaviorClass("Backpacks:Sheath", typeof(SheathBehavior));
        api.RegisterCollectibleBehaviorClass("Backpacks:Quiver", typeof(QuiverBehavior));
        api.RegisterCollectibleBehaviorClass("Backpacks:VariantFromSlot", typeof(VariantFromSlotBehavior));
    }
}