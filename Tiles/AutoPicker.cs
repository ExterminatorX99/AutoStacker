using System.Linq;
using AutoStacker.Common;
using AutoStacker.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutoStacker.Tiles
{
	public class AutoPicker : ModTile
	{
		private static readonly Players.AutoPicker Player = new();

		public Point16 TopLeftReceiver = Point16.NegativeOne;
		public Point16 TopLeftPicker = Point16.NegativeOne;
		public Point16 Picker = Point16.NegativeOne;
		public int Direction = 1;

		private int _renda;

		public override void SetStaticDefaults()
		{
			Main.tileSpelunker[Type] = true;
			Main.tileContainer[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			//DustType = ModContent.DustType<Sparkle>();
			AdjTiles = new int[] { TileID.Containers };
			ChestDrop = ModContent.ItemType<Items.AutoPicker>();

			ContainerName.SetDefault("Auto Picker");

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom =
				new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Auto Picker");
			AddMapEntry(new Color(200, 200, 200), name, MapChestName);
		}

		public override bool HasSmartInteract() => true;

		public static string MapChestName(string name, int i, int j)
		{
			int left = i;
			int top = j;
			Tile tile = Main.tile[i, j];
			if (tile.frameX % 36 != 0)
				left--;

			if (tile.frameY != 0)
				top--;

			int chest = Chest.FindChest(left, top);
			if (chest < 0)
				return Language.GetTextValue("LegacyChestType.0");

			if (Main.chest[chest].name == "")
				return name;

			return name + ": " + Main.chest[chest].name;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 32, 32, ChestDrop);
			Chest.DestroyChest(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
				left--;
			if (tile.frameY != 0)
				top--;
			Item item = player.inventory[player.selectedItem];
			if (item.type == ModContent.ItemType<AutoPickerController>())
			{
				AutoPickerController autoPickerController = (AutoPickerController)item.ModItem;
				if (autoPickerController.TopLeft.X != -1 && autoPickerController.TopLeft.Y != -1)
				{
					player.tileInteractionHappened = true;

					TopLeftPicker = Common.AutoStacker.GetOrigin(Terraria.Player.tileTargetX, Terraria.Player.tileTargetY);
					TopLeftReceiver = autoPickerController.TopLeft;

					Picker = new Point16(TopLeftPicker.X < TopLeftReceiver.X ? TopLeftPicker.X + 3 : TopLeftPicker.X - 2,
						TopLeftPicker.Y > TopLeftReceiver.Y ? TopLeftReceiver.Y + 2 : TopLeftPicker.Y + 2);
					Direction = TopLeftPicker.X < TopLeftReceiver.X ? 1 : -1;
					Main.NewText("Auto Picker Selected to x:" + Picker.X + ", y:" + Picker.Y + " !");
				}
				else
				{
					Main.NewText("Select Reciever Chest before Select Auto Picker!");
				}
			}
			else
			{
				if (player.sign >= 0)
				{
					SoundEngine.PlaySound(SoundID.MenuClose);
					player.sign = -1;
					Main.editSign = false;
					Main.npcChatText = "";
				}

				if (Main.editChest)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
					Main.editChest = false;
					Main.npcChatText = "";
				}

				if (player.editedChestName)
				{
					NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
					player.editedChestName = false;
				}

				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					if (left == player.chestX && top == player.chestY && player.chest >= 0)
					{
						player.chest = -1;
						Recipe.FindRecipes();
						SoundEngine.PlaySound(SoundID.MenuClose);
					}
					else
					{
						NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
						Main.stackSplit = 600;
					}
				}
				else
				{
					int chest = Chest.FindChest(left, top);
					if (chest >= 0)
					{
						Main.stackSplit = 600;
						if (chest == player.chest)
						{
							player.chest = -1;
							SoundEngine.PlaySound(SoundID.MenuClose);
						}
						else
						{
							player.chest = chest;
							Main.playerInventory = true;
							Main.recBigList = false;
							player.chestX = left;
							player.chestY = top;
							SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
						}

						Recipe.FindRecipes();
					}
				}
			}

			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
				left--;
			if (tile.frameY != 0)
				top--;
			int chest = Chest.FindChest(left, top);
			player.cursorItemIconID = -1;
			if (chest < 0)
			{
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else
			{
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "Auto Picker";
				if (player.cursorItemIconText == "Auto Picker")
				{
					player.cursorItemIconID = ModContent.ItemType<Items.AutoPicker>();
					player.cursorItemIconText = "";
				}
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}

		public override void HitWire(int i, int j)
		{
			if (TopLeftReceiver.X != -1 && TopLeftReceiver.Y != -1)
			{
				Point16 origin = Common.AutoStacker.GetOrigin(Picker.X, Picker.Y);
				int fieldChest = Common.AutoStacker.FindChest(origin.X, origin.Y);
				if (fieldChest == -1 || !Main.chest[fieldChest].item.Any(chestItem => chestItem.stack > 0))
				{
					int pickerChest = Common.AutoStacker.FindChest(TopLeftPicker.X, TopLeftPicker.Y);
					int pickPower = Main.chest[pickerChest].item.Max(chestItem => chestItem.pick);
					if (pickPower != 0 && Main.tile[Picker.X, Picker.Y].IsActive && CanPick(Picker.X, Picker.Y, pickPower))
					{
						if (_renda <= 20)
						{
							Player.PickTile2(Picker.X, Picker.Y, pickPower, this);
							_renda += 1;
							if (!Main.tile[Picker.X, Picker.Y].IsActive)
							{
								MoveNext(pickPower);
								_renda = 0;
							}
						}
						else
						{
							MoveNext(pickPower);
							_renda = 0;
						}
					}
					else
					{
						MoveNext(pickPower);
						_renda = 0;
					}
				}
				else
				{
					Item item = Main.chest[fieldChest].item.First(chestItem => chestItem.stack > 0);
					if (!Deposit(item.Clone()))
						Item.NewItem(Picker.X * 16, Picker.Y * 16, 16, 16, item.type, item.stack, false, -1);
					item.SetDefaults(0, true);

					_renda = 0;
				}
			}
		}

		private void MoveNext(int pickPower)
		{
			// Main.NewText(picker.X +","+picker.Y);
			// int pickerChest = Common.AutoStacker.FindChest(topLeftPicker.X,topLeftPicker.Y);
			// int pickPower=Main.chest[pickerChest].item.Max(chestItem => chestItem.pick);

			short x = Picker.X;
			short y = Picker.Y;

			for (;;)
			{
				if (
					x + 4 * Direction < TopLeftReceiver.X && x + 4 * Direction < TopLeftPicker.X ||
					x + 3 * Direction > TopLeftReceiver.X && x + 3 * Direction > TopLeftPicker.X
				)
				{
					y += 1;
					Direction *= -1;

					if (Picker.Y >= Main.Map.MaxHeight)
						return;
				}
				else
				{
					x += (short)Direction;
				}

				Point16 origin = Common.AutoStacker.GetOrigin(x, y);
				int fieldChest = Common.AutoStacker.FindChest(origin.X, origin.Y);

				if (fieldChest == -1 || !Main.chest[fieldChest].item.Any(chestItem => chestItem.stack > 0))
				{
					if (pickPower != 0 && Main.tile[x, y].IsActive && CanPick(x, y, pickPower))
						break;
				}
				else
				{
					break;
				}
			}

			Picker = new Point16(x, y);
			// Main.NewText(picker.X +","+picker.Y);
		}

		private static bool CanPick(int x, int y, int pickPower)
		{
			Tile tile = Main.tile[x, y];
			if (tile.type == TileID.Chlorophyte && pickPower <= 200 ||
				(tile.type == TileID.Ebonstone || tile.type == TileID.Crimstone) && pickPower <= 65 ||
				tile.type == TileID.Pearlstone && pickPower <= 65 ||
				tile.type == TileID.Meteorite && pickPower <= 50 ||
				tile.type == TileID.DesertFossil && pickPower <= 65
//				|| ((tile.type == 22 || tile.type == 204) && (double)AY[index] > Main.worldSurface && pickPower < 55)
				||
				tile.type == TileID.Obsidian && pickPower <= 65 ||
				tile.type == TileID.Hellstone && pickPower <= 65 ||
				(tile.type == TileID.LihzahrdBrick || tile.type == TileID.LihzahrdAltar) && pickPower <= 210 ||
				Main.tileDungeon[tile.type] && pickPower <= 65
//				|| ((double)AX[index] < (double)Main.maxTilesX * 0.35 || (double)AX[index] > (double)Main.maxTilesX * 0.65)
				||
				tile.type == TileID.Cobalt && pickPower <= 100 ||
				tile.type == TileID.Mythril && pickPower <= 110 ||
				tile.type == TileID.Adamantite && pickPower <= 150 ||
				tile.type == TileID.Palladium && pickPower <= 100 ||
				tile.type == TileID.Orichalcum && pickPower <= 110 ||
				tile.type == TileID.Titanium && pickPower <= 150
			)
				return false;

			int check = 1;
			TileLoader.PickPowerCheck(tile, pickPower, ref check);
			if (check == 0)
				return false;

			Tile tileUpper = Main.tile[x, y - 1];
			return tileUpper.type != TileID.Containers && tileUpper.type != TileID.DemonAltar;
		}

		public bool Deposit(int x, int y, int width, int height, int type, int stack = 1, bool noBroadcast = false,
			int pfix = 0, bool noGrabDelay = false, bool reverseLookup = false) =>
			Deposit(new Item(type, stack));

		public bool Deposit(Item item)
		{
			Point16 topLeft = TopLeftReceiver;

			//chest
			int chestNo = Common.AutoStacker.FindChest(topLeft.X, topLeft.Y);
			if (chestNo != -1)
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
			//storage heart
			else if (AutoStacker.MagicStorageLoaded)
				if (MagicStorageConnecter.InjectItem(topLeft, item))
				{
					item.SetDefaults(0, true);
					return true;
				}

			return false;
		}
	}
}
