using System.Collections.Generic;
using System.Linq;
using AutoStacker.Common;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.GlobalItems
{
	public class RecieverChestSelector : GlobalItem
	{
		public static readonly short[] ExcludeItemList =
		{
			ItemID.CopperCoin,
			ItemID.SilverCoin,
			ItemID.GoldCoin,
			ItemID.PlatinumCoin,
			ItemID.Heart,
			ItemID.CandyApple,
			ItemID.CandyCane,
			ItemID.Star,
			ItemID.SugarPlum,
			ItemID.SoulCake
		};

		private static readonly Dictionary<int, List<int>> DeleteQue = new();

		public override bool OnPickup(Item item, Player player)
		{
			Players.RecieverChestSelector modPlayer = Main.LocalPlayer.GetModPlayer<Players.RecieverChestSelector>();
			Point16 topLeft = modPlayer.TopLeft;
			if (modPlayer.ActiveItem == null ||
				topLeft.X == -1 && topLeft.Y == -1 ||
				!modPlayer.AutoSendEnabled ||
				ExcludeItemList.Any(x => x == item.type) ||
				item.stack <= 0 ||
				item.IsAir)
				return true;

			//Item depositItem=item.Clone();
			if (Deposit(item, player))
			{
				item.SetDefaults(0, true);
				if (!DeleteQue.ContainsKey(item.type))
					DeleteQue[item.type] = new List<int>();
				DeleteQue[item.type].Add(item.stack);
			}

			return true;
		}

		public override void UpdateInventory(Item item, Player player)
		{
			if (DeleteQue.ContainsKey(item.type))
			{
				item.stack -= DeleteQue[item.type].Sum(stack => stack);
				if (item.stack <= 0)
					item.SetDefaults(0, true);
				DeleteQue.Remove(item.type);
			}
		}

		public static bool Deposit(Item item, Player player)
		{
			Players.RecieverChestSelector modPlayer = Main.LocalPlayer.GetModPlayer<Players.RecieverChestSelector>();
			Point16 topLeft = modPlayer.TopLeft;

			//chest
			int chestNo = Common.AutoStacker.FindChest(topLeft.X, topLeft.Y);
			if (chestNo != -1)
			{
				//stack item
				for (int slot = 0; slot < Main.chest[chestNo].item.Length; slot++)
				{
					if (Main.chest[chestNo].item[slot].stack == 0)
					{
						Main.chest[chestNo].item[slot] = item.Clone();
						item.SetDefaults(0, true);
						Wiring.TripWire(topLeft.X, topLeft.Y, 2, 2);
						return true;
					}

					Item chestItem = Main.chest[chestNo].item[slot];
					if (item.IsTheSameAs(chestItem) && chestItem.stack < chestItem.maxStack)
					{
						int spaceLeft = chestItem.maxStack - chestItem.stack;
						if (spaceLeft >= item.stack)
						{
							chestItem.stack += item.stack;
							item.SetDefaults(0, true);
							Wiring.TripWire(topLeft.X, topLeft.Y, 2, 2);
							return true;
						}

						item.stack -= spaceLeft;
						chestItem.stack = chestItem.maxStack;
					}
				}
			}

			//storage heart
			else if (AutoStacker.MagicStorageLoaded)
			{
				MagicStorageConnecter.InjectItem(topLeft, item);
				if (item.stack == 0)
				{
					item.SetDefaults(0, true);
					return true;
				}
			}

			return false;
		}
	}
}
