using CombatOverhaul.Utils;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace Backpacks;

public class GenerateCreativeStacksConfig
{
    public bool Enabled { get; set; } = true;
    public Dictionary<string, List<string>> Attributes { get; set; } = [];
    public string[] CreativeTabs { get; set; } = [];
    public string Format { get; set; } = "";
}

public sealed class GenerateCreativeStacks : CollectibleBehavior
{
    private GenerateCreativeStacksConfig? _config;

    public GenerateCreativeStacks(CollectibleObject collObj) : base(collObj)
    {
    }

    public override void OnLoaded(ICoreAPI api)
    {
        base.OnLoaded(api);

        if (_config != null)
        {
            AddAllTypesToCreativeInventory(api, _config);
            _config = null;
        }
        else
        {
            LoggerUtil.Error(api, this, $"Failed to generate creative stacks for '{collObj?.Code}': missing config");
        }
    }

    public override void Initialize(JsonObject properties)
    {
        base.Initialize(properties);

        _config = properties.AsObject<GenerateCreativeStacksConfig>();
    }

    private void AddAllTypesToCreativeInventory(ICoreAPI api, GenerateCreativeStacksConfig config)
    {
        if (!config.Enabled)
        {
            return;
        }
        
        List<JsonItemStack> stacks = [];

        List<string> attributesCombinations = [""];
        foreach ((string attribute, List<string> attributeValues) in config.Attributes)
        {
            List<string> newCombinations = [];
            foreach (string oldCombination in attributesCombinations)
            {
                string newCombination = oldCombination;
                if (newCombination != "")
                {
                    newCombination += ",";
                }
                
                foreach (string attributeValue in attributeValues)
                {
                    newCombinations.Add(newCombination + $"\"{attribute}\":\"{attributeValue}\"");
                }
            }
            attributesCombinations = newCombinations;
        }

        foreach (string attributes in attributesCombinations)
        {
            stacks.Add(GenStackJson(api, string.Format(config.Format, attributes)));
        }

        JsonItemStack noAttributesStack = new()
        {
            Code = collObj.Code,
            Type = EnumItemClass.Item
        };
        noAttributesStack.Resolve(api.World, "GenerateCreativeStacks");

        if (collObj.CreativeInventoryStacks == null)
        {
            collObj.CreativeInventoryStacks = [
                new() { Stacks = stacks.ToArray(), Tabs = config.CreativeTabs },
                new() { Stacks = [noAttributesStack], Tabs = collObj.CreativeInventoryTabs }
            ];
            collObj.CreativeInventoryTabs = null;
        }
        else
        {
            collObj.CreativeInventoryStacks = collObj.CreativeInventoryStacks.Append(new CreativeTabAndStackList() { Stacks = stacks.ToArray(), Tabs = config.CreativeTabs });
        }
    }
    private JsonItemStack GenStackJson(ICoreAPI api, string json)
    {
        JsonItemStack stackJson = new()
        {
            Code = collObj.Code,
            Type = EnumItemClass.Item,
            Attributes = new JsonObject(JToken.Parse(json))
        };

        stackJson.Resolve(api.World, "GenerateCreativeStacks");

        return stackJson;
    }
}
