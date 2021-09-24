using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Items
{
	public class ItemVacuumer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Item Vacuumer");
			Tooltip.SetDefault("usage\nRight click this item : ON/OFF Vaccume ");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
		}

		// RightClick
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			if (Players.ItemVacuumer.VacuumSwitch)
			{
				Players.ItemVacuumer.VacuumSwitch = false;
				Main.NewText("Vacuume OFF!!");
			}
			else
			{
				Players.ItemVacuumer.VacuumSwitch = true;
				Main.NewText("Vacuume ON!!");
			}

			Item.stack++;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddTile(TileID.WorkBenches)
				.AddIngredient(ItemID.ReinforcedFishingPole)
				.Register();
		}
	}
}
