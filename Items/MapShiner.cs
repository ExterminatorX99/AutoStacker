using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class MapShiner : ModItem
	{
		public static bool Active;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Map Shiner");
			Tooltip.SetDefault("usage\nRight click this item : Map Shine");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 28;
			Item.useTime = 28;
		}

		// RightClick
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			if (Active)
			{
				Active = false;
				Main.NewText("Map Shine OFF!!");
			}
			else
			{
				Active = true;
				Main.NewText("Map Shine ON!!");
			}

			Item.stack++;
		}

		public override void UpdateInventory(Player player)
		{
			if (Active)
				for (int x = Main.mapMinX; x < Main.mapMaxX; x++)
				for (int y = Main.mapMinY; y < Main.mapMaxY; y++)
					Main.Map.Update(x, y, byte.MaxValue);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
