using Vintagestory.API.Common;

namespace Backpacks;

public sealed class BackpacksSystem : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        api.RegisterCollectibleBehaviorClass("Backpacks:ShapeReplacement", typeof(ShapeReplacement));
        api.RegisterCollectibleBehaviorClass("Backpacks:VariantFromSlot", typeof(VariantFromSlotBehavior));
    }
}