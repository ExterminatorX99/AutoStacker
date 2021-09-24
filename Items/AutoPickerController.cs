using System.Collections.Generic;
using AutoStacker.Common;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutoStacker.Items
{
	public class AutoPickerController : ModItem
	{
		public Point16 TopLeft = Point16.NegativeOne;

		public override ModItem Clone(Item item)
		{
			AutoPickerController newItem = (AutoPickerController)base.Clone(item);
			newItem.TopLeft = TopLeft;
			return newItem;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Auto Picker Controller");

			const string tooltip = "usage \n" +
								   "  Click chest           : Select Receiver chest\n" +
								   "  Right click AutoPicker: Select AutoPicker\n" +
								   "  Right click this item : ON/OFF auto pick \n";

			Tooltip.SetDefault(tooltip);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (TopLeft != Point16.NegativeOne)
			{
				TooltipLine lineH2 = new(Mod, "head2", $"ReceiverChest {TopLeft}\n ");
				tooltips.Insert(2, lineH2);
			}
			else
			{
				TooltipLine lineH2 = new(Mod, "head2", "Chest { none }\n ");
				tooltips.Insert(2, lineH2);
			}
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 28;
			Item.useTime = 28;
		}

		public override void SaveData(TagCompound tag)
		{
			tag["topLeftX"] = TopLeft.X;
			tag["topLeftY"] = TopLeft.Y;
		}

		public override void LoadData(TagCompound tag)
		{
			short x = tag.GetShort("topLeftX");
			short y = tag.GetShort("topLeftY");
			TopLeft = new Point16(x, y);
		}

		public override bool? UseItem(Player player)
		{
			//Players.AutoPicker modPlayer = (Players.AutoPicker)Main.LocalPlayer.GetModPlayer<Players.AutoPicker>();
			Point16 origin = Common.AutoStacker.GetOrigin(Player.tileTargetX, Player.tileTargetY);
			if (player.altFunctionUse == 0)
			{
				if (
					Common.AutoStacker.FindChest(origin.X, origin.Y) != -1 && Main.tile[origin.X, origin.Y].type != ModContent.TileType<Tiles.AutoPicker>() ||
					AutoStacker.MagicStorageLoaded && CallMagicStorageFindHeart(origin)
				)
				{
					TopLeft = origin;
					Main.NewText("Reciever Chest Selected to x:" + origin.X + ", y:" + origin.Y + " !");
				}
				else
				{
					Main.NewText("No chest to be found.");
				}
			}

			return true;
		}

		private static bool CallMagicStorageFindHeart(Point16 origin) => MagicStorageConnecter.FindHeart(origin) != null;

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(null, "RecieverChestSelector")
				.AddIngredient(ItemID.Wire)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
