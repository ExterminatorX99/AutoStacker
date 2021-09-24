using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class WitchsPot : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Witch's Pot");
			Tooltip.SetDefault("This is a modded chest.");
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 22;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 500;
			Item.createTile = ModContent.TileType<Tiles.WitchsPot>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(null, "WitchsCauldron")
				.AddIngredient(ItemID.Switch)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
