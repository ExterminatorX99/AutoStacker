using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class MountOfDiscord : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("MountOfDiscord.");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 30;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = 30000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item79;
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<Mounts.MountOfDiscord>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.Glass, 999)
				.Register();
		}
	}
}
