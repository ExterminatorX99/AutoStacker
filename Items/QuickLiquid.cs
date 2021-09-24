using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class QuickLiquid : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quick Liquid");
			Tooltip.SetDefault("usage\nRight click this item : ON/OFF Quick Liquid");
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
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			if (Worlds.QuickLiquid.QuickSwitch)
			{
				Worlds.QuickLiquid.QuickSwitch = false;
				Main.NewText("Quick Liquid OFF!!");
			}
			else
			{
				Worlds.QuickLiquid.QuickSwitch = true;
				Main.NewText("Quick Liquid ON!!");
			}

			Item.stack++;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
