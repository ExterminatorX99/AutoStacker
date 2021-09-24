using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutoStacker.Players
{
	internal class RecieverChestSelector : ModPlayer
	{
		public bool AutoSendEnabled;
		public Item ActiveItem;
		public Point16 TopLeft = Point16.NegativeOne;

		public bool NotSmartCursor;

		public override void SaveData(TagCompound tag)
		{
			tag.Set("autoSendEnabled", AutoSendEnabled);

			int index = Array.IndexOf(Player.inventory, ActiveItem);
			tag.Set("activeItem", index);

			tag.Set("topLeftX", TopLeft.X);
			tag.Set("topLeftY", TopLeft.Y);
		}

		public override void LoadData(TagCompound tag)
		{
			if (tag.ContainsKey("autoSendEnabled"))
				AutoSendEnabled = tag.GetBool("autoSendEnabled");
			if (tag.ContainsKey("activeItem"))
			{
				int itemNo = tag.GetInt("activeItem");

				if (itemNo >= 0 && itemNo < Player.inventory.Length && Player.inventory[itemNo].type == ModContent.ItemType<Items.RecieverChestSelector>())
					ActiveItem = Player.inventory[itemNo];
			}

			if (tag.ContainsKey("topLeftX") && tag.ContainsKey("topLeftY"))
				TopLeft = new Point16(tag.GetShort("topLeftX"), tag.GetShort("topLeftY"));
		}

		public override void ResetEffects()
		{
			Item item = Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem];

			if (!Main.playerInventory || item.type != ModContent.ItemType<Items.RecieverChestSelector>())
				NotSmartCursor = false;

			if (NotSmartCursor)
			{
				Main.SmartCursorEnabled = false;
				Player.tileRangeX = Main.Map.MaxWidth;
				Player.tileRangeY = Main.Map.MaxHeight;
			}
		}
	}
}
