// Terraria.WorldGen

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.WorldGen
{
	public class AutoPicker : Terraria.WorldGen
	{
		private static readonly bool StopDrops = false;

		public static void KillTile2(Tiles.AutoPicker autoPicker, int i, int j, bool fail = false, bool effectOnly = false, bool noItem = false)
		{
			if (i < 0 || j < 0 || i >= Main.maxTilesX || j >= Main.maxTilesY)
				return;
			Tile tile = Main.tile[i, j];
			if (tile == null)
			{
				tile = new Tile();
				Main.tile[i, j] = tile;
			}

			if (!tile.IsActive)
				return;
			if (j >= 1 && Main.tile[i, j - 1] == null)
				Main.tile[i, j - 1] = new Tile();
			if (j >= 1 &&
				Main.tile[i, j - 1].IsActive &&
				(Main.tile[i, j - 1].type == TileID.Trees && tile.type != TileID.Trees ||
				 Main.tile[i, j - 1].type == TileID.PalmTree && tile.type != TileID.PalmTree ||
				 TileID.Sets.BasicChest[Main.tile[i, j - 1].type] && !TileID.Sets.BasicChest[tile.type] ||
				 Main.tile[i, j - 1].type == TileID.PalmTree && tile.type != TileID.PalmTree ||
				 TileID.Sets.BasicDresser[Main.tile[i, j - 1].type] && !TileID.Sets.BasicDresser[tile.type] ||
				 Main.tile[i, j - 1].type == TileID.DemonAltar && tile.type != TileID.DemonAltar ||
				 Main.tile[i, j - 1].type == TileID.MushroomTrees && tile.type != TileID.MushroomTrees))
			{
				if (Main.tile[i, j - 1].type == TileID.Trees)
				{
					if ((Main.tile[i, j - 1].frameX != 66 || Main.tile[i, j - 1].frameY < 0 || Main.tile[i, j - 1].frameY > 44) &&
						(Main.tile[i, j - 1].frameX != 88 || Main.tile[i, j - 1].frameY < 66 || Main.tile[i, j - 1].frameY > 110) &&
						Main.tile[i, j - 1].frameY < 198)
						return;
				}
				else if (Main.tile[i, j - 1].type != TileID.PalmTree || Main.tile[i, j - 1].frameX == 66 || Main.tile[i, j - 1].frameX == 220)
				{
					return;
				}
			}

			switch (tile.type)
			{
				case 10 when tile.frameY >= 594 && tile.frameY <= 646:
					fail = true;
					break;
				case 138:
					fail = CheckBoulderChest(i, j);
					break;
				case 235:
				{
					int frameX = tile.frameX;
					int num53 = i - frameX % 54 / 18;
					for (int l = 0; l < 3; l++)
						if (Main.tile[num53 + l, j - 1].IsActive &&
							(TileID.Sets.BasicChest[Main.tile[num53 + l, j - 1].type] ||
							 TileID.Sets.BasicChestFake[Main.tile[num53 + l, j - 1].type] ||
							 TileID.Sets.BasicDresser[Main.tile[num53 + l, j - 1].type]))
						{
							fail = true;
							break;
						}

					break;
				}
			}

			TileLoader.KillTile(i, j, tile.type, ref fail, ref effectOnly, ref noItem);
			if (!effectOnly && !StopDrops)
			{
				if (!noItem && FixExploitManEaters.SpotProtected(i, j))
					return;
				if (TileLoader.KillSound(i, j, tile.type))
					switch (tile.type)
					{
						case 127:
							SoundEngine.PlaySound(SoundID.Item27, i * 16, j * 16);
							break;
						case 147:
						case 224:
						{
							if (genRand.Next(2) == 0)
								SoundEngine.PlaySound(SoundID.Item48, i * 16, j * 16);
							else
								SoundEngine.PlaySound(SoundID.Item49, i * 16, j * 16);

							break;
						}
						case 161:
						case 163:
						case 164:
						case 200:
							SoundEngine.PlaySound(SoundID.Item50, i * 16, j * 16);
							break;
						case 3:
						case 110:
						{
							SoundEngine.PlaySound(6, i * 16, j * 16);
							if (tile.frameX == 144)
								if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, 5))
									Item.NewItem(i * 16, j * 16, 16, 16, 5);

							break;
						}
						case 254:
							SoundEngine.PlaySound(6, i * 16, j * 16);
							break;
						case 24:
						{
							SoundEngine.PlaySound(6, i * 16, j * 16);
							if (tile.frameX == 144)
								if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, 60))
									Item.NewItem(i * 16, j * 16, 16, 16, 60);

							break;
						}
						default:
						{
							if (Main.tileAlch[tile.type] ||
								tile.type == TileID.LivingMahoganyLeaves ||
								tile.type == TileID.DyePlants ||
								tile.type == TileID.CorruptThorns ||
								tile.type == TileID.Cobweb ||
								tile.type == TileID.Vines ||
								tile.type == TileID.JunglePlants ||
								tile.type == TileID.JungleVines ||
								tile.type == TileID.JungleThorns ||
								tile.type == TileID.MushroomPlants ||
								tile.type == TileID.Plants2 ||
								tile.type == TileID.JunglePlants2 ||
								tile.type == TileID.HallowedPlants2 ||
								tile.type == TileID.HallowedVines ||
								tile.type == TileID.LongMoss ||
								tile.type == TileID.LeafBlock ||
								tile.type == TileID.CrimsonVines ||
								tile.type == TileID.PlantDetritus ||
								tile.type == TileID.CrimsonThorns ||
								tile.type == TileID.VineFlowers)
							{
								SoundEngine.PlaySound(6, i * 16, j * 16);
							}
							else if (tile.type == TileID.CrimsonPlants)
							{
								SoundEngine.PlaySound(6, i * 16, j * 16);
								if (tile.frameX == 270)
									if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, 2887))
										Item.NewItem(i * 16, j * 16, 16, 16, 2887);
							}
							else if (tile.type == TileID.Stone ||
									 tile.type == TileID.Iron ||
									 tile.type == TileID.Copper ||
									 tile.type == TileID.Gold ||
									 tile.type == TileID.Silver ||
									 tile.type == TileID.Demonite ||
									 tile.type == TileID.DemoniteBrick ||
									 tile.type == TileID.Ebonstone ||
									 tile.type == TileID.Meteorite ||
									 tile.type == TileID.GrayBrick ||
									 tile.type == TileID.RedBrick ||
									 tile.type == TileID.BlueDungeonBrick ||
									 tile.type == TileID.GreenDungeonBrick ||
									 tile.type == TileID.PinkDungeonBrick ||
									 tile.type == TileID.GoldBrick ||
									 tile.type == TileID.SilverBrick ||
									 tile.type == TileID.CopperBrick ||
									 tile.type == TileID.Spikes ||
									 tile.type == TileID.Obsidian ||
									 tile.type == TileID.Hellstone ||
									 tile.type == TileID.Sapphire ||
									 tile.type == TileID.Ruby ||
									 tile.type == TileID.Emerald ||
									 tile.type == TileID.Topaz ||
									 tile.type == TileID.Amethyst ||
									 tile.type == TileID.Diamond ||
									 tile.type == TileID.ObsidianBrick ||
									 tile.type == TileID.HellstoneBrick ||
									 tile.type == TileID.Cobalt ||
									 tile.type == TileID.Mythril ||
									 tile.type == TileID.Adamantite ||
									 tile.type == TileID.Pearlstone ||
									 tile.type == TileID.PearlstoneBrick ||
									 tile.type == TileID.IridescentBrick ||
									 tile.type == TileID.Mudstone ||
									 tile.type == TileID.CobaltBrick ||
									 tile.type == TileID.MythrilBrick ||
									 tile.type == TileID.AdamantiteBeam ||
									 tile.type == TileID.SandstoneBrick ||
									 tile.type == TileID.EbonstoneBrick ||
									 tile.type == TileID.RedStucco ||
									 tile.type == TileID.YellowStucco ||
									 tile.type == TileID.GreenStucco ||
									 tile.type == TileID.GrayStucco ||
									 tile.type == TileID.RainbowBrick ||
									 tile.type == TileID.IceBlock ||
									 tile.type == TileID.Tin ||
									 tile.type == TileID.Lead ||
									 tile.type == TileID.Tungsten ||
									 tile.type == TileID.Platinum ||
									 tile.type == TileID.TinBrick ||
									 tile.type == TileID.TungstenBrick ||
									 tile.type == TileID.PlatinumBrick ||
									 tile.type == TileID.Crimstone ||
									 tile.type == TileID.Sunplate ||
									 tile.type == TileID.Crimtane ||
									 tile.type == TileID.IceBrick ||
									 tile.type == TileID.Chlorophyte ||
									 tile.type == TileID.Palladium ||
									 tile.type == TileID.Orichalcum ||
									 tile.type == TileID.Titanium ||
									 tile.type == TileID.LihzahrdBrick ||
									 tile.type == TileID.PalladiumColumn ||
									 tile.type == TileID.BubblegumBlock ||
									 tile.type == TileID.Titanstone ||
									 tile.type == TileID.Cog ||
									 tile.type == TileID.StoneSlab ||
									 tile.type == TileID.SandStoneSlab ||
									 tile.type == TileID.CopperPlating ||
									 tile.type == TileID.TinPlating ||
									 tile.type == TileID.ChlorophyteBrick ||
									 tile.type == TileID.CrimtaneBrick ||
									 tile.type == TileID.ShroomitePlating ||
									 tile.type == TileID.MartianConduitPlating ||
									 tile.type == TileID.Marble ||
									 tile.type == TileID.MarbleBlock ||
									 tile.type == TileID.Granite ||
									 tile.type == TileID.GraniteBlock ||
									 tile.type == TileID.MeteoriteBrick ||
									 tile.type == TileID.FossilOre)
							{
								SoundEngine.PlaySound(21, i * 16, j * 16);
							}
							else if (tile.type == TileID.Larva || tile.type == TileID.FleshBlock)
							{
								SoundEngine.PlaySound(4, i * 16, j * 16);
							}
							else if (tile.type == TileID.DemonAltar && tile.frameX >= 54)
							{
								SoundEngine.PlaySound(4, i * 16, j * 16);
							}
							else if (tile.type == TileID.MinecartTrack)
							{
								SoundEngine.PlaySound(SoundID.Item52, i * 16, j * 16);
							}
							else if (tile.type >= TileID.CopperCoinPile && tile.type <= TileID.PlatinumCoinPile)
							{
								SoundEngine.PlaySound(18, i * 16, j * 16);
							}
							else if (tile.type != TileID.Boulder)
							{
								SoundEngine.PlaySound(0, i * 16, j * 16);
							}

							break;
						}
					}

				switch (tile.type)
				{
					case 162:
					case 385:
					case 129:
					{
						if (!fail)
							SoundEngine.PlaySound(SoundID.Item27, i * 16, j * 16);

						break;
					}
					case 165 when tile.frameX < 54:
					{
						if (!fail)
							SoundEngine.PlaySound(SoundID.Item27, i * 16, j * 16);

						break;
					}
				}
			}

			switch (tile.type)
			{
				case 128:
				case 269:
				{
					int num86 = i;
					int m = tile.frameX;
					int m2;
					for (m2 = tile.frameX; m2 >= 100; m2 -= 100)
					{
					}

					while (m2 >= 36)
						m2 -= 36;
					if (m2 == 18)
					{
						m = Main.tile[i - 1, j].frameX;
						num86--;
					}

					if (m >= 100)
					{
						int num89 = 0;
						while (m >= 100)
						{
							m -= 100;
							num89++;
						}

						int num90 = Main.tile[num86, j].frameY / 18;
						if (num90 == 0)
							if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, Item.headType[num89]))
								Item.NewItem(i * 16, j * 16, 16, 16, Item.headType[num89]);
						if (num90 == 1)
							if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, Item.bodyType[num89]))
								Item.NewItem(i * 16, j * 16, 16, 16, Item.bodyType[num89]);
						if (num90 == 2)
							if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, Item.legType[num89]))
								Item.NewItem(i * 16, j * 16, 16, 16, Item.legType[num89]);
						for (m = Main.tile[num86, j].frameX; m >= 100; m -= 100)
						{
						}

						Main.tile[num86, j].frameX = (short)m;
					}

					break;
				}
				case 334:
				{
					int num88 = i;
					int n = tile.frameX;
					int num87 = tile.frameX;
					int num85 = 0;
					while (num87 >= 5000)
					{
						num87 -= 5000;
						num85++;
					}

					if (num85 != 0)
						num87 = (num85 - 1) * 18;
					num87 %= 54;
					if (num87 == 18)
					{
						n = Main.tile[i - 1, j].frameX;
						num88--;
					}

					if (num87 == 36)
					{
						n = Main.tile[i - 2, j].frameX;
						num88 -= 2;
					}

					if (n >= 5000)
					{
						int num83 = n % 5000;
						num83 -= 100;
						int num81 = Main.tile[num88 + 1, j].frameX;
						num81 = num81 < 25000 ? num81 - 10000 : num81 - 25000;
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Item item = new();
							item.netDefaults(num83);
							item.Prefix(num81);
							if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, num83, 1, true))
							{
								int num79 = Item.NewItem(i * 16, j * 16, 16, 16, num83, 1, true);
								item.position = Main.item[num79].position;
								Main.item[num79] = item;
								NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num79);
							}
						}

						n = Main.tile[num88, j].frameX;
						int num78 = 0;
						while (n >= 5000)
						{
							n -= 5000;
							num78++;
						}

						if (num78 != 0)
							n = (num78 - 1) * 18;
						Main.tile[num88, j].frameX = (short)n;
						Main.tile[num88 + 1, j].frameX = (short)(n + 18);
					}

					break;
				}
				case 395:
				{
					int num77 = TEItemFrame.Find(i - tile.frameX % 36 / 18, j - tile.frameY % 36 / 18);
					if (num77 != -1 && ((TEItemFrame)TileEntity.ByID[num77]).item.stack > 0)
					{
						((TEItemFrame)TileEntity.ByID[num77]).DropItem();
						if (Main.netMode != NetmodeID.Server)
							Main.blockMouse = true;
						return;
					}

					break;
				}
			}

			int num76 = KillTile_GetTileDustAmount(fail, tile, i, j);
			for (int num75 = 0; num75 < num76; num75++)
				KillTile_MakeTileDust(i, j, tile);
			if (effectOnly)
				return;
			if (fail)
			{
				switch (tile.type)
				{
					case 2:
					case 23:
					case 109:
					case 199:
						tile.type = TileID.Dirt;
						break;
					case 60:
					case 70:
						tile.type = TileID.Mud;
						break;
				}

				if (Main.tileMoss[tile.type])
					tile.type = TileID.Stone;
				SquareTileFrame(i, j);
				return;
			}

			if (TileID.Sets.BasicChest[tile.type] && Main.netMode != NetmodeID.MultiplayerClient)
			{
				int num74 = tile.frameX / 18;
				int y3 = j - tile.frameY / 18;
				while (num74 > 1)
					num74 -= 2;
				num74 = i - num74;
				if (!Chest.DestroyChest(num74, y3))
					return;
			}

			if (TileID.Sets.BasicDresser[tile.type] && Main.netMode != NetmodeID.MultiplayerClient)
			{
				int num72 = tile.frameX / 18;
				int y2 = j - tile.frameY / 18;
				num72 %= 3;
				num72 = i - num72;
				if (!Chest.DestroyChest(num72, y2))
					return;
			}

			if (tile.type == TileID.Cobweb && tile.wall == 62 && genRand.Next(4) != 0)
				noItem = true;
			if (!noItem && !StopDrops && Main.netMode != NetmodeID.MultiplayerClient)
			{
				bool flag3 = false;
				int num69 = -1;
				int num68 = -1;
				int num67 = -1;
				switch (tile.type)
				{
					case 3:
					{
						num69 = 400;
						num68 = 100;
						if (tile.frameX >= 108)
						{
							num69 *= 3;
							num68 *= 3;
						}

						break;
					}
					case 73:
					{
						num69 = 200;
						num68 = 50;
						if (tile.frameX >= 108)
						{
							num69 *= 3;
							num68 *= 3;
						}

						break;
					}
					case 61:
					{
						num67 = 80;
						if (tile.frameX >= 108)
							num67 *= 3;

						break;
					}
					case 74:
					{
						num67 = 40;
						if (tile.frameX >= 108)
							num67 *= 3;

						break;
					}
					case 62:
						num67 = 250;
						break;
					case 185:
					{
						if (tile.frameY == 0 && tile.frameX < 214)
							num69 = 6;
						if (tile.frameY == 18 && (tile.frameX < 214 || tile.frameX >= 1368))
							num69 = 6;

						break;
					}
					case 186:
					{
						if (tile.frameX >= 378 && tile.frameX <= 700)
							num69 = 6;

						break;
					}
					case 187:
					{
						if (tile.frameX >= 756 && tile.frameX <= 916)
							num69 = 6;
						if (tile.frameX <= 322)
							num69 = 6;

						break;
					}
					case 233:
						num67 = 10;
						break;
				}

				TileLoader.DropCritterChance(i, j, tile.type, ref num69, ref num68, ref num67);
				if (num69 > 0 && NPC.CountNPCS(357) < 5 && genRand.Next(num69) == 0)
				{
					int type12 = 357;
					if (genRand.Next(NPC.goldCritterChance) == 0)
						type12 = 448;
					int num66 = NPC.NewNPC(i * 16 + 10, j * 16, type12);
					Main.npc[num66].TargetClosest();
					Main.npc[num66].velocity.Y = genRand.Next(-50, -21) * 0.1f;
					Main.npc[num66].velocity.X = genRand.Next(0, 26) * 0.1f * (0f - Main.npc[num66].direction);
					Main.npc[num66].direction *= -1;
					Main.npc[num66].netUpdate = true;
				}

				if (num68 > 0 && NPC.CountNPCS(377) < 5 && genRand.Next(num68) == 0)
				{
					int type11 = 377;
					if (genRand.Next(NPC.goldCritterChance) == 0)
						type11 = 446;
					int num65 = NPC.NewNPC(i * 16 + 10, j * 16, type11);
					Main.npc[num65].TargetClosest();
					Main.npc[num65].velocity.Y = genRand.Next(-50, -21) * 0.1f;
					Main.npc[num65].velocity.X = genRand.Next(0, 26) * 0.1f * (0f - Main.npc[num65].direction);
					Main.npc[num65].direction *= -1;
					Main.npc[num65].netUpdate = true;
				}

				if (num67 > 0 && NPC.CountNPCS(485) + NPC.CountNPCS(486) + NPC.CountNPCS(487) < 8 && genRand.Next(num67) == 0)
				{
					int type10 = 485;
					if (genRand.Next(4) == 0)
						type10 = 486;
					if (genRand.Next(12) == 0)
						type10 = 487;
					int num64 = NPC.NewNPC(i * 16 + 10, j * 16, type10);
					Main.npc[num64].TargetClosest();
					Main.npc[num64].velocity.Y = genRand.Next(-50, -21) * 0.1f;
					Main.npc[num64].velocity.X = genRand.Next(0, 26) * 0.1f * (0f - Main.npc[num64].direction);
					Main.npc[num64].direction *= -1;
					Main.npc[num64].netUpdate = true;
				}

				int num63 = 0;
				int num62 = 0;
				switch (tile.type)
				{
					case 0:
					case 2:
					case 109:
						num63 = 2;
						break;
					case 426:
						num63 = 3621;
						break;
					case 430:
						num63 = 3633;
						break;
					case 431:
						num63 = 3634;
						break;
					case 432:
						num63 = 3635;
						break;
					case 433:
						num63 = 3636;
						break;
					case 434:
						num63 = 3637;
						break;
					case 427:
						num63 = 3622;
						break;
					case 435:
						num63 = 3638;
						break;
					case 436:
						num63 = 3639;
						break;
					case 437:
						num63 = 3640;
						break;
					case 438:
						num63 = 3641;
						break;
					case 439:
						num63 = 3642;
						break;
					case 446:
						num63 = 3736;
						break;
					case 447:
						num63 = 3737;
						break;
					case 448:
						num63 = 3738;
						break;
					case 449:
						num63 = 3739;
						break;
					case 450:
						num63 = 3740;
						break;
					case 451:
						num63 = 3741;
						break;
					case 368:
						num63 = 3086;
						break;
					case 369:
						num63 = 3087;
						break;
					case 367:
						num63 = 3081;
						break;
					case 379:
						num63 = 3214;
						break;
					case 353:
						num63 = 2996;
						break;
					case 365:
						num63 = 3077;
						break;
					case 366:
						num63 = 3078;
						break;
					case 52 or 62 when genRand.Next(2) == 0 && Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16)].cordage:
						num63 = 2996;
						break;
					case 357:
						num63 = 3066;
						break;
					case 1:
						num63 = 3;
						break;
					case 3:
					case 73:
					{
						if (genRand.Next(2) == 0 &&
							(Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16)].HasItem(281) ||
							 Main.player[Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16)].HasItem(986)))
							num63 = 283;

						break;
					}
					case 227:
					{
						int num61 = tile.frameX / 34;
						num63 = 1107 + num61;
						if (num61 >= 8 && num61 <= 11)
							num63 = 3385 + num61 - 8;

						break;
					}
					case 4:
					{
						int num60 = tile.frameY / 22;
						num63 = num60 switch
						{
							0  => 8,
							8  => 523,
							9  => 974,
							10 => 1245,
							11 => 1333,
							12 => 2274,
							13 => 3004,
							14 => 3045,
							15 => 3114,
							_  => 426 + num60
						};

						break;
					}
					case 239:
					{
						int num91 = tile.frameX / 18;
						num63 = num91 switch
						{
							0  => 20,
							1  => 703,
							2  => 22,
							3  => 704,
							4  => 21,
							5  => 705,
							6  => 19,
							7  => 706,
							8  => 57,
							9  => 117,
							10 => 175,
							11 => 381,
							12 => 1184,
							13 => 382,
							14 => 1191,
							15 => 391,
							16 => 1198,
							17 => 1006,
							18 => 1225,
							19 => 1257,
							20 => 1552,
							21 => 3261,
							22 => 3467,
							_  => num63
						};

						break;
					}
					case 380:
					{
						int num59 = tile.frameY / 18;
						num63 = 3215 + num59;
						break;
					}
					case 442:
						num63 = 3707;
						break;
					case 383:
						num63 = 620;
						break;
					case 315:
						num63 = 2435;
						break;
					case 330:
						num63 = 71;
						break;
					case 331:
						num63 = 72;
						break;
					case 332:
						num63 = 73;
						break;
					case 333:
						num63 = 74;
						break;
					case 5:
					{
						if (tile.frameX >= 22 && tile.frameY >= 198)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								if (genRand.Next(2) == 0)
								{
									int num58;
									for (num58 = j;
										Main.tile[i, num58] != null &&
										(!Main.tile[i, num58].IsActive ||
										 !Main.tileSolid[Main.tile[i, num58].type] ||
										 Main.tileSolidTop[Main.tile[i, num58].type]);
										num58++)
									{
									}

									if (Main.tile[i, num58] != null)
									{
										if (Main.tile[i, num58].type == TileID.Grass ||
											Main.tile[i, num58].type == TileID.HallowedGrass ||
											Main.tile[i, num58].type == TileID.SnowBlock ||
											Main.tile[i, num58].type == TileID.CrimsonGrass ||
											Main.tile[i, num58].type == TileID.CorruptGrass ||
											TileLoader.CanDropAcorn(Main.tile[i, num58].type))
										{
											num63 = 9;
											num62 = 27;
										}
										else
										{
											num63 = 9;
										}
									}
								}
								else
								{
									num63 = 9;
								}
							}
						}
						else
						{
							num63 = 9;
						}

						if (num63 == 9)
						{
							int num57 = i;
							int num56 = j;
							if (tile.frameX == 66 && tile.frameY <= 45)
								num57++;
							if (tile.frameX == 88 && tile.frameY >= 66 && tile.frameY <= 110)
								num57--;
							if (tile.frameX == 22 && tile.frameY >= 132 && tile.frameY <= 176)
								num57--;
							if (tile.frameX == 44 && tile.frameY >= 132 && tile.frameY <= 176)
								num57++;
							if (tile.frameX == 44 && tile.frameY >= 198)
								num57++;
							if (tile.frameX == 66 && tile.frameY >= 198)
								num57--;
							for (; !Main.tile[num57, num56].IsActive || !Main.tileSolid[Main.tile[num57, num56].type]; num56++)
							{
							}

							if (Main.tile[num57, num56].IsActive)
							{
								ushort type9 = Main.tile[num57, num56].type;
								num63 = type9 switch
								{
									70  => 183,
									60  => 620,
									23  => 619,
									199 => 911,
									147 => 2503,
									109 => 621,
									_   => num63
								};
								TileLoader.DropTreeWood(type9, ref num63);
							}

							int num55 = Player.FindClosest(new Vector2(num57 * 16, num56 * 16), 16, 16);
							int axe = Main.player[num55].inventory[Main.player[num55].selectedItem].axe;
							if (genRand.Next(100) < axe || Main.rand.Next(3) == 0)
								flag3 = true;
						}

						break;
					}
					case 323:
					{
						num63 = 2504;
						if (tile.frameX <= 132 && tile.frameX >= 88)
							num62 = 27;
						int num54;
						for (num54 = j; !Main.tile[i, num54].IsActive || !Main.tileSolid[Main.tile[i, num54].type]; num54++)
						{
						}

						if (Main.tile[i, num54].IsActive)
						{
							ushort type8 = Main.tile[i, num54].type;
							num63 = type8 switch
							{
								234 => 911,
								116 => 621,
								112 => 619,
								_   => num63
							};
							TileLoader.DropPalmTreeWood(type8, ref num63);
						}

						break;
					}
					case 408:
						num63 = 3460;
						break;
					case 409:
						num63 = 3461;
						break;
					case 415:
						num63 = 3573;
						break;
					case 416:
						num63 = 3574;
						break;
					case 417:
						num63 = 3575;
						break;
					case 418:
						num63 = 3576;
						break;
					case >= 255 and <= 261:
						num63 = 1970 + tile.type - 255;
						break;
					case >= 262 and <= 268:
						num63 = 1970 + tile.type - 262;
						break;
					case 171:
					{
						if (tile.frameX >= 10)
						{
							dropXmasTree(i, j, 0);
							dropXmasTree(i, j, 1);
							dropXmasTree(i, j, 2);
							dropXmasTree(i, j, 3);
						}

						break;
					}
					case 324:
						num63 = (tile.frameY / 22) switch
						{
							0 => 2625,
							1 => 2626,
							_ => num63
						};

						break;
					case 421:
						num63 = 3609;
						break;
					case 422:
						num63 = 3610;
						break;
					case 419:
						num63 = (tile.frameX / 18) switch
						{
							0 => 3602,
							1 => 3618,
							2 => 3663,
							_ => num63
						};

						break;
					case 428:
						num63 = (tile.frameY / 18) switch
						{
							0 => 3630,
							1 => 3632,
							2 => 3631,
							3 => 3626,
							_ => num63
						};
						PressurePlateHelper.DestroyPlate(new Point(i, j));
						break;
					case 420:
						num63 = (tile.frameY / 18) switch
						{
							0 => 3603,
							1 => 3604,
							2 => 3605,
							3 => 3606,
							4 => 3607,
							5 => 3608,
							_ => num63
						};

						break;
					case 423:
						TELogicSensor.Kill(i, j);
						num63 = (tile.frameY / 18) switch
						{
							0 => 3613,
							1 => 3614,
							2 => 3615,
							3 => 3726,
							4 => 3727,
							5 => 3728,
							6 => 3729,
							_ => num63
						};

						break;
					case 424:
						num63 = 3616;
						break;
					case 445:
						num63 = 3725;
						break;
					case 429:
						num63 = 3629;
						break;
					case 272:
						num63 = 1344;
						break;
					case 273:
						num63 = 2119;
						break;
					case 274:
						num63 = 2120;
						break;
					case 460:
						num63 = 3756;
						break;
					case 326:
						num63 = 2693;
						SoundEngine.PlaySound(13, i * 16, j * 16);
						break;
					case 327:
						num63 = 2694;
						SoundEngine.PlaySound(13, i * 16, j * 16);
						break;
					case 458:
						num63 = 3754;
						SoundEngine.PlaySound(13, i * 16, j * 16);
						break;
					case 459:
						num63 = 3755;
						SoundEngine.PlaySound(13, i * 16, j * 16);
						break;
					case 345:
						num63 = 2787;
						SoundEngine.PlaySound(13, i * 16, j * 16);
						break;
					case 328:
						num63 = 2695;
						SoundEngine.PlaySound(13, i * 16, j * 16);
						break;
					case 329:
						num63 = 2697;
						SoundEngine.PlaySound(13, i * 16, j * 16);
						break;
					case 346:
						num63 = 2792;
						break;
					case 347:
						num63 = 2793;
						break;
					case 348:
						num63 = 2794;
						break;
					case 350:
						num63 = 2860;
						break;
					case 336:
						num63 = 2701;
						break;
					case 340:
						num63 = 2751;
						break;
					case 341:
						num63 = 2752;
						break;
					case 342:
						num63 = 2753;
						break;
					case 343:
						num63 = 2754;
						break;
					case 344:
						num63 = 2755;
						break;
					case 351:
						num63 = 2868;
						break;
					case 251:
						num63 = 1725;
						break;
					case 252:
						num63 = 1727;
						break;
					case 253:
						num63 = 1729;
						break;
					case 325:
						num63 = 2692;
						break;
					case 370:
						num63 = 3100;
						break;
					case 396:
						num63 = 3271;
						break;
					case 400:
						num63 = 3276;
						break;
					case 401:
						num63 = 3277;
						break;
					case 403:
						num63 = 3339;
						break;
					case 397:
						num63 = 3272;
						break;
					case 398:
						num63 = 3274;
						break;
					case 399:
						num63 = 3275;
						break;
					case 402:
						num63 = 3338;
						break;
					case 404:
						num63 = 3347;
						break;
					case 407:
						num63 = 3380;
						break;
					case 170:
						num63 = 1872;
						break;
					case 284:
						num63 = 2173;
						break;
					case 214:
						num63 = 85;
						break;
					case 213:
						num63 = 965;
						break;
					case 211:
						num63 = 947;
						break;
					case 6:
						num63 = 11;
						break;
					case 7:
						num63 = 12;
						break;
					case 8:
						num63 = 13;
						break;
					case 9:
						num63 = 14;
						break;
					case 202:
						num63 = 824;
						break;
					case 234:
						num63 = 1246;
						break;
					case 226:
						num63 = 1101;
						break;
					case 224:
						num63 = 1103;
						break;
					case 36:
						num63 = 1869;
						break;
					case 311:
						num63 = 2260;
						break;
					case 312:
						num63 = 2261;
						break;
					case 313:
						num63 = 2262;
						break;
					case 229:
						num63 = 1125;
						break;
					case 230:
						num63 = 1127;
						break;
					case 225 when genRand.Next(3) == 0:
						tile.LiquidType = LiquidID.Honey;
						tile.LiquidAmount = byte.MaxValue;
						break;
					case 225:
					{
						num63 = 1124;
						if (Main.netMode != NetmodeID.MultiplayerClient && genRand.Next(2) == 0)
						{
							int num52 = 1;
							if (genRand.Next(3) == 0)
								num52 = 2;
							for (int num51 = 0; num51 < num52; num51++)
							{
								int type7 = genRand.Next(210, 212);
								int num50 = NPC.NewNPC(i * 16 + 8, j * 16 + 15, type7, 1);
								Main.npc[num50].velocity.X = genRand.Next(-200, 201) * 0.002f;
								Main.npc[num50].velocity.Y = genRand.Next(-200, 201) * 0.002f;
								Main.npc[num50].netUpdate = true;
							}
						}

						break;
					}
					case 221:
						num63 = 1104;
						break;
					case 222:
						num63 = 1105;
						break;
					case 223:
						num63 = 1106;
						break;
					case 248:
						num63 = 1589;
						break;
					case 249:
						num63 = 1591;
						break;
					case 250:
						num63 = 1593;
						break;
					case 191:
						num63 = 9;
						break;
					case 203:
						num63 = 836;
						break;
					case 204:
						num63 = 880;
						break;
					case 166:
						num63 = 699;
						break;
					case 167:
						num63 = 700;
						break;
					case 168:
						num63 = 701;
						break;
					case 169:
						num63 = 702;
						break;
					case 123:
						num63 = 424;
						break;
					case 124:
						num63 = 480;
						break;
					case 157:
						num63 = 619;
						break;
					case 158:
						num63 = 620;
						break;
					case 159:
						num63 = 621;
						break;
					case 161:
						num63 = 664;
						break;
					case 206:
						num63 = 883;
						break;
					case 232:
						num63 = 1150;
						break;
					case 198:
						num63 = 775;
						break;
					case 314:
						num63 = Minecart.GetTrackItem(tile);
						break;
					case 189:
						num63 = 751;
						break;
					case 195:
						num63 = 763;
						break;
					case 194:
						num63 = 766;
						break;
					case 193:
						num63 = 762;
						break;
					case 196:
						num63 = 765;
						break;
					case 197:
						num63 = 767;
						break;
					case 178:
						num63 = (tile.frameX / 18) switch
						{
							0 => 181,
							1 => 180,
							2 => 177,
							3 => 179,
							4 => 178,
							5 => 182,
							6 => 999,
							_ => num63
						};
						break;
					case 149 when tile.frameX == 0 || tile.frameX == 54:
						num63 = 596;
						break;
					case 149 when tile.frameX == 18 || tile.frameX == 72:
						num63 = 597;
						break;
					case 149:
					{
						if (tile.frameX == 36 || tile.frameX == 90)
							num63 = 598;

						break;
					}
					case 13:
						SoundEngine.PlaySound(13, i * 16, j * 16);
						num63 = (tile.frameX / 18) switch
						{
							1 => 28,
							2 => 110,
							3 => 350,
							4 => 351,
							5 => 2234,
							6 => 2244,
							7 => 2257,
							8 => 2258,
							_ => 31
						};

						break;
					case 19:
					{
						int num49 = tile.frameY / 18;
						num63 = num49 switch
						{
							0  => 94,
							1  => 631,
							2  => 632,
							3  => 633,
							4  => 634,
							5  => 913,
							6  => 1384,
							7  => 1385,
							8  => 1386,
							9  => 1387,
							10 => 1388,
							11 => 1389,
							12 => 1418,
							13 => 1457,
							14 => 1702,
							15 => 1796,
							16 => 1818,
							17 => 2518,
							18 => 2549,
							19 => 2566,
							20 => 2581,
							21 => 2627,
							22 => 2628,
							23 => 2629,
							24 => 2630,
							25 => 2744,
							26 => 2822,
							27 => 3144,
							28 => 3146,
							29 => 3145,
							30 => 3903 + num49 - 30,
							31 => 3903 + num49 - 30,
							32 => 3903 + num49 - 30,
							33 => 3903 + num49 - 30,
							34 => 3903 + num49 - 30,
							35 => 3903 + num49 - 30,
							_  => num63
						};

						break;
					}
					case 22:
						num63 = 56;
						break;
					case 140:
						num63 = 577;
						break;
					case 23:
						num63 = 2;
						break;
					case 25:
						num63 = 61;
						break;
					case 30:
						num63 = 9;
						break;
					case 208:
						num63 = 911;
						break;
					case 33:
					{
						int num48 = tile.frameY / 22;
						num63 = num48 switch
						{
							1  => 1405,
							2  => 1406,
							3  => 1407,
							4  => 2045 + num48 - 4,
							5  => 2045 + num48 - 4,
							6  => 2045 + num48 - 4,
							7  => 2045 + num48 - 4,
							8  => 2045 + num48 - 4,
							9  => 2045 + num48 - 4,
							10 => 2045 + num48 - 4,
							11 => 2045 + num48 - 4,
							12 => 2045 + num48 - 4,
							13 => 2045 + num48 - 4,
							14 => 2153 + num48 - 14,
							15 => 2153 + num48 - 14,
							16 => 2153 + num48 - 14,
							17 => 2236,
							18 => 2523,
							19 => 2542,
							20 => 2556,
							21 => 2571,
							22 => 2648,
							23 => 2649,
							24 => 2650,
							25 => 2651,
							26 => 2818,
							27 => 3171,
							28 => 3173,
							29 => 3172,
							30 => 3890,
							_  => 105
						};
						break;
					}
					case 372:
						num63 = 3117;
						break;
					case 371:
						num63 = 3113;
						break;
					case 174:
						num63 = 713;
						break;
					case 37:
						num63 = 116;
						break;
					case 38:
						num63 = 129;
						break;
					case 39:
						num63 = 131;
						break;
					case 40:
						num63 = 133;
						break;
					case 41:
						num63 = 134;
						break;
					case 43:
						num63 = 137;
						break;
					case 44:
						num63 = 139;
						break;
					case 45:
						num63 = 141;
						break;
					case 46:
						num63 = 143;
						break;
					case 47:
						num63 = 145;
						break;
					case 48:
						num63 = 147;
						break;
					case 49:
						num63 = 148;
						break;
					case 51:
						num63 = 150;
						break;
					case 53:
						num63 = 169;
						break;
					case 151:
						num63 = 607;
						break;
					case 152:
						num63 = 609;
						break;
					case 54:
						num63 = 170;
						SoundEngine.PlaySound(13, i * 16, j * 16);
						break;
					case 56:
						num63 = 173;
						break;
					case 57:
						num63 = 172;
						break;
					case 58:
						num63 = 174;
						break;
					case 60:
					case 70:
						num63 = 176;
						break;
					case 75:
						num63 = 192;
						break;
					case 76:
						num63 = 214;
						break;
					case 78:
						num63 = 222;
						break;
					case 81:
						num63 = 275;
						break;
					case 80:
					case 188:
						num63 = 276;
						break;
					case 107:
						num63 = 364;
						break;
					case 108:
						num63 = 365;
						break;
					case 111:
						num63 = 366;
						break;
					case 150:
						num63 = 604;
						break;
					case 112:
						num63 = 370;
						break;
					case 116:
						num63 = 408;
						break;
					case 117:
						num63 = 409;
						break;
					case 129:
						num63 = 502;
						break;
					case 118:
						num63 = 412;
						break;
					case 119:
						num63 = 413;
						break;
					case 120:
						num63 = 414;
						break;
					case 121:
						num63 = 415;
						break;
					case 122:
						num63 = 416;
						break;
					case 136:
						num63 = 538;
						break;
					case 385:
						num63 = 3234;
						break;
					case 137:
					{
						int num92 = tile.frameY / 18;
						num63 = num92 switch
						{
							0 => 539,
							1 => 1146,
							2 => 1147,
							3 => 1148,
							4 => 1149,
							_ => num63
						};

						break;
					}
					case 141:
						num63 = 580;
						break;
					case 145:
						num63 = 586;
						break;
					case 146:
						num63 = 591;
						break;
					case 147:
						num63 = 593;
						break;
					case 148:
						num63 = 594;
						break;
					case 153:
						num63 = 611;
						break;
					case 154:
						num63 = 612;
						break;
					case 155:
						num63 = 613;
						break;
					case 156:
						num63 = 614;
						break;
					case 160:
						num63 = 662;
						break;
					case 175:
						num63 = 717;
						break;
					case 176:
						num63 = 718;
						break;
					case 177:
						num63 = 719;
						break;
					case 163:
						num63 = 833;
						break;
					case 164:
						num63 = 834;
						break;
					case 200:
						num63 = 835;
						break;
					case 210:
						num63 = 937;
						break;
					case 135:
					{
						int num93 = tile.frameY / 18;
						num63 = num93 switch
						{
							0 => 529,
							1 => 541,
							2 => 542,
							3 => 543,
							4 => 852,
							5 => 853,
							6 => 1151,
							_ => num63
						};

						break;
					}
					case 144:
					{
						num63 = tile.frameX switch
						{
							0  => 583,
							18 => 584,
							36 => 585,
							_  => num63
						};

						break;
					}
					case 130:
						num63 = 511;
						break;
					case 131:
						num63 = 512;
						break;
					case 61:
					case 74:
					{
						switch (tile.frameX)
						{
							case 144 when tile.type == TileID.JunglePlants:
							{
								if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, 331, genRand.Next(2, 4)))
									Item.NewItem(i * 16, j * 16, 16, 16, 331, genRand.Next(2, 4));
								break;
							}
							case 162 when tile.type == TileID.JunglePlants:
								num63 = 223;
								break;
							case >= 108 and <= 126 when tile.type == TileID.JunglePlants && genRand.Next(20) == 0:
								num63 = 208;
								break;
							default:
							{
								if (genRand.Next(100) == 0)
									num63 = 195;

								break;
							}
						}

						break;
					}
					case 59:
						num63 = 176;
						break;
					case 190:
						num63 = 183;
						break;
					case 71:
					case 72:
					{
						if (genRand.Next(50) == 0)
							num63 = 194;
						else if (genRand.Next(2) == 0)
							num63 = 183;
						break;
					}
					case >= 63 and <= 68:
						num63 = tile.type - 63 + 177;
						break;
					case 50:
						num63 = tile.frameX != 90 ? 149 : 165;
						break;
					default:
					{
						if (Main.tileAlch[tile.type])
						{
							if (tile.type > TileID.ImmatureHerbs)
							{
								int num47 = tile.frameX / 18;
								bool flag2 = false;
								num63 = 313 + num47;
								int type6 = 307 + num47;
								if (tile.type == TileID.BloomingHerbs)
									flag2 = true;
								if (num47 == 0 && Main.dayTime)
									flag2 = true;
								if (num47 == 1 && !Main.dayTime)
									flag2 = true;
								if (num47 == 3 && !Main.dayTime && (Main.bloodMoon || Main.moonPhase == 0))
									flag2 = true;
								if (num47 == 4 && (Main.raining || Main.cloudAlpha > 0f))
									flag2 = true;
								if (num47 == 5 && !Main.raining && Main.dayTime && Main.time > 40500.0)
									flag2 = true;
								if (num47 == 6)
								{
									num63 = 2358;
									type6 = 2357;
								}

								int num46 = Player.FindClosest(new Vector2(i * 16, j * 16), 16, 16);
								if (Main.player[num46].inventory[Main.player[num46].selectedItem].type == ItemID.StaffofRegrowth)
								{
									if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, type6, genRand.Next(1, 6)))
										Item.NewItem(i * 16, j * 16, 16, 16, type6, genRand.Next(1, 6));
									if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, num63, genRand.Next(1, 3)))
										Item.NewItem(i * 16, j * 16, 16, 16, num63, genRand.Next(1, 3));
									num63 = -1;
								}
								else if (flag2)
								{
									int stack = genRand.Next(1, 4);
									if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, type6, stack))
										Item.NewItem(i * 16, j * 16, 16, 16, type6, stack);
								}
							}
						}
						else if (tile.type == TileID.BorealWood)
						{
							num63 = 2503;
						}
						else if (tile.type == TileID.PalmWood)
						{
							num63 = 2504;
						}

						break;
					}
				}

				bool num94 = TileLoader.Drop(i, j, tile.type);
				if (num94 && num63 > 0)
				{
					int num45 = 1;
					if (flag3)
						num45++;
					if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, num63, num45, false, -1))
						Item.NewItem(i * 16, j * 16, 16, 16, num63, num45, false, -1);
				}

				if (num94 && num62 > 0)
					if (!autoPicker.Deposit(i * 16, j * 16, 16, 16, num62, 1, false, -1))
						Item.NewItem(i * 16, j * 16, 16, 16, num62, 1, false, -1);
			}

			if (Main.netMode != NetmodeID.Server)
				AchievementsHelper.NotifyTileDestroyed(Main.player[Main.myPlayer], tile.type);
			tile.IsActive = false;
			tile.IsHalfBlock = false;
			tile.frameX = -1;
			tile.frameY = -1;
			tile.Color = 0;
			tile.FrameNumber = 0;
			switch (tile.type)
			{
				case 58 when j > Main.maxTilesY - 200:
					tile.LiquidType = LiquidID.Lava;
					tile.LiquidAmount = 128;
					break;
				case 419:
					Wiring.PokeLogicGate(i, j + 1);
					break;
				case 54:
					SquareWallFrame(i, j);
					break;
			}

			tile.type = TileID.Dirt;
			tile.IsActuated = false;
			SquareTileFrame(i, j);
		}
	}
}
