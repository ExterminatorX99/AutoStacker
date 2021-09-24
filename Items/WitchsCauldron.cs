using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class WitchsCauldron : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Witch's Cauldron");
			Tooltip.SetDefault("This is a modded chest.");
		}

		public override void SetDefaults()
		{
			Item.width = 39;
			Item.height = 22;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 500;
			Item.createTile = ModContent.TileType<Tiles.WitchsCauldron>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Chest)
				.AddIngredient(ItemID.BlackCounterweight)
				.AddTile(TileID.WorkBenches)
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.Chest)
				.AddIngredient(ItemID.BlueCounterweight)
				.AddTile(TileID.WorkBenches)
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.Chest)
				.AddIngredient(ItemID.GreenCounterweight)
				.AddTile(TileID.WorkBenches)
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.Chest)
				.AddIngredient(ItemID.PurpleCounterweight)
				.AddTile(TileID.WorkBenches)
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.Chest)
				.AddIngredient(ItemID.RedCounterweight)
				.AddTile(TileID.WorkBenches)
				.Register();

			CreateRecipe()
				.AddIngredient(ItemID.Chest)
				.AddIngredient(ItemID.YellowCounterweight)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
