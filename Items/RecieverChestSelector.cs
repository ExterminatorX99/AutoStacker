using System.Collections.Generic;
using AutoStacker.Common;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutoStacker.Items
{
	public class RecieverChestSelector : ModItem
	{
		public Point16 TopLeft = Point16.NegativeOne;
		public bool Active;

		public override ModItem Clone(Item item)
		{
			RecieverChestSelector newItem = (RecieverChestSelector)base.Clone(item);
			newItem.TopLeft = TopLeft;
			newItem.Active = Active;
			return newItem;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Reciever Chest Selector");

			const string tooltipStr = "usage \n" +
									  "  Click chest           : Select chest\n" +
									  "  Right click           : Open selected chest\n" +
									  "  Right click this item : ON/OFF auto stack \n";
			Tooltip.SetDefault(tooltipStr);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (Active)
			{
				TooltipLine lineH1 = new(Mod, "head1", "Switch [ *** ON *** ]");
				tooltips.Insert(1, lineH1);
			}
			else
			{
				TooltipLine lineH1 = new(Mod, "head1", "Switch [ ]");
				tooltips.Insert(1, lineH1);
			}

			if (TopLeft.X != -1 && TopLeft.Y != -1)
			{
				TooltipLine lineH2 = new(Mod, "head2", "Chest [" + TopLeft.X + "," + TopLeft.Y + "]\n ");
				tooltips.Insert(2, lineH2);
			}
			else
			{
				TooltipLine lineH2 = new(Mod, "head2", "Chest [ none ]\n ");
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
			tag.Set("active", Active);
			tag.Set("topLeftX", TopLeft.X);
			tag.Set("topLeftY", TopLeft.Y);
		}

		public override void LoadData(TagCompound tag)
		{
			if (tag.ContainsKey("active"))
				Active = tag.GetBool("active");
			if (tag.ContainsKey("topLeftX") && tag.ContainsKey("topLeftY"))
				TopLeft = new Point16(tag.GetShort("topLeftX"), tag.GetShort("topLeftY"));
		}

		// UseItem
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public override bool AltFunctionUse(Player player) => true;

		public override bool? UseItem(Player player)
		{
			Players.RecieverChestSelector modPlayer = Main.LocalPlayer.GetModPlayer<Players.RecieverChestSelector>();
			if (player.altFunctionUse == 0)
			{
				Point16 origin = Common.AutoStacker.GetOrigin(Player.tileTargetX, Player.tileTargetY);

				if (Common.AutoStacker.FindChest(origin.X, origin.Y) != -1 || AutoStacker.MagicStorageLoaded && CallMagicStorageFindHeart(origin))
				{
					modPlayer.AutoSendEnabled = true;

					Active = true;
					if (modPlayer.ActiveItem != null && modPlayer.ActiveItem.ModItem != null)
						if (!modPlayer.ActiveItem.Equals(Item))
							((RecieverChestSelector)modPlayer.ActiveItem.ModItem).Active = false;
					modPlayer.ActiveItem = Item;

					TopLeft = origin;
					modPlayer.TopLeft = origin;
					Main.NewText("Reciever Chest Selected to x:" + origin.X + ", y:" + origin.Y + " !");
				}
				else
				{
					Main.NewText("No chest to be found.");
				}
			}
			else
			{
				int chestNo = Common.AutoStacker.FindChest(TopLeft.X, TopLeft.Y);
				if (chestNo != -1)
				{
					player.chest = chestNo;
					Main.playerInventory = true;
					Main.recBigList = false;
					player.chestX = TopLeft.X;
					player.chestY = TopLeft.Y;

					modPlayer.NotSmartCursor = true;

					Main.SmartCursorEnabled = false;
					Player.tileRangeX = Main.Map.MaxWidth;
					Player.tileRangeY = Main.Map.MaxHeight;

					SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
				}
			}

			return true;
		}

		private bool CallMagicStorageFindHeart(Point16 origin)
		{
			if (MagicStorageConnecter.FindHeart(origin) == null)
				return false;
			return true;
		}

		// RightClick
		//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			Players.RecieverChestSelector modPlayer = Main.LocalPlayer.GetModPlayer<Players.RecieverChestSelector>();
			if (modPlayer.AutoSendEnabled && TopLeft.X == -1 && TopLeft.Y == -1)
			{
				Main.NewText("Reciever chest is not set.Click chest before use.");
			}
			else if (modPlayer.AutoSendEnabled && !(TopLeft.X == -1 && TopLeft.Y == -1))
			{
				if (Item.Equals(modPlayer.ActiveItem))
				{
					modPlayer.AutoSendEnabled = false;

					Active = false;
					Main.NewText("Reciever Chest Deselected!");
				}
				else
				{
					Active = true;
					if (modPlayer.ActiveItem != null && modPlayer.ActiveItem.ModItem != null)
						((RecieverChestSelector)modPlayer.ActiveItem.ModItem).Active = false;
					modPlayer.ActiveItem = Item;

					modPlayer.TopLeft = TopLeft;
					Main.NewText("Reciever Chest Selected to x:" + modPlayer.TopLeft.X + ", y:" + modPlayer.TopLeft.Y + " !");
				}
			}
			else if (!modPlayer.AutoSendEnabled && TopLeft.X == -1 && TopLeft.Y == -1)
			{
				Main.NewText("Reciever chest is not set.Click chest before use.");
			}
			else if (!modPlayer.AutoSendEnabled && !(TopLeft.X == -1 && TopLeft.Y == -1))
			{
				modPlayer.AutoSendEnabled = true;

				Active = true;
				modPlayer.ActiveItem = Item;

				modPlayer.TopLeft = TopLeft;
				Main.NewText("Reciever Chest Selected to x:" + modPlayer.TopLeft.X + ", y:" + modPlayer.TopLeft.Y + " !");
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
