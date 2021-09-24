using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class MinionHouse : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Minion House");
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
			Item.createTile = ModContent.TileType<Tiles.MinionHouse>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Chest)
				.AddIngredient(ItemID.Wood, 999)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
