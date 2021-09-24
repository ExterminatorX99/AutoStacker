using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace AutoStacker.Worlds
{
	public class QuickLiquidV2 : ModSystem
	{
		public static Liquid2[] Liquid2;
		public static bool QuickSwitch = false;
		private int _count;

		public QuickLiquidV2()
		{
			Liquid2 = new Liquid2[1000000];
			for (int i = 0; i < Liquid2.Length; i++)
				Liquid2[i] = new Liquid2();
			Worlds.Liquid2.ReInit();
		}

		public override void LoadWorldData(TagCompound tag)
		{
			Worlds.Liquid2.ReInit();
		}

		public override void PreUpdateWorld()
		{
			if (QuickSwitch)
			{
				_count++;
				if (_count >= 100)
					_count = 0;
				//Main.NewText(Liquid.numLiquid +","+ LiquidBuffer.numLiquidBuffer +","+ Liquid2.numLiquid);

				Liquid.cycles = 1;
				Liquid.panicCounter = 0;
				Liquid.UpdateLiquid();

				while (LiquidBuffer.numLiquidBuffer > 0 && Worlds.Liquid2.NumLiquid != 1000000 - 1)
				{
					//Liquid2.AddWater(Main.liquidBuffer[LiquidBuffer.numLiquidBuffer -1].x,Main.liquidBuffer[LiquidBuffer.numLiquidBuffer -1].y);
					//LiquidBuffer.DelBuffer(LiquidBuffer.numLiquidBuffer -1);
					Main.tile[Main.liquidBuffer[0].x, Main.liquidBuffer[0].y].CheckingLiquid = false;
					Worlds.Liquid2.AddWater(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y);
					LiquidBuffer.DelBuffer(0);
				}

				Worlds.Liquid2.Cycles = 1;
				Worlds.Liquid2.PanicCounter = 0;
				Worlds.Liquid2.UpdateLiquid();
			}
		}
	}

	public class Liquid2 : Liquid
	{
		public static int SkipCount { get; set; }
		public static int StuckCount { get; set; }
		public static int StuckAmount { get; set; }
		public static int Cycles { get; set; } = 1;
		public static int ResLiquid { get; set; } = 1000000;
		public static int MaxLiquid { get; set; } = 1000000;
		public static int NumLiquid { get; set; }
		public static bool Stuck { get; set; }
		public static bool QuickFall { get; set; }
		public static bool QuickSettle { get; set; }
		private static int WetCounter { get; set; }
		public static int PanicCounter { get; set; }
		public static bool PanicMode { get; set; }
		public static int PanicY { get; set; }
		private static HashSet<int> NetChangeSet { get; set; } = new();
		private static HashSet<int> SwapNetChangeSet { get; set; } = new();

		public new static void AddWater(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			if (Main.tile[x, y] == null)
				return;
			if (tile.CheckingLiquid)
				return;
			if (x >= Main.maxTilesX - 5 || y >= Main.maxTilesY - 5)
				return;
			if (x < 5 || y < 5)
				return;
			if (tile.LiquidAmount == 0)
				return;
			if (NumLiquid >= MaxLiquid - 1)
			{
				LiquidBuffer.AddBuffer(x, y);
				return;
			}

			tile.CheckingLiquid = true;
			QuickLiquidV2.Liquid2[NumLiquid].kill = 0;
			QuickLiquidV2.Liquid2[NumLiquid].x = x;
			QuickLiquidV2.Liquid2[NumLiquid].y = y;
			QuickLiquidV2.Liquid2[NumLiquid].delay = 0;
			tile.SkipLiquid = false;
			NumLiquid++;
			if (Main.netMode == NetmodeID.Server)
				NetSendLiquid(x, y);
			if (tile.IsActive && !Terraria.WorldGen.gen)
			{
				bool flag = false;
				if (tile.LiquidType == LiquidID.Lava)
				{
					if (TileObjectData.CheckLavaDeath(tile))
						flag = true;
				}
				else if (TileObjectData.CheckWaterDeath(tile))
				{
					flag = true;
				}

				if (flag)
				{
					Terraria.WorldGen.KillTile(x, y);
					if (Main.netMode == NetmodeID.Server)
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
				}
			}
		}

		public new static void DelWater(int l)
		{
			int num = QuickLiquidV2.Liquid2[l].x;
			int num2 = QuickLiquidV2.Liquid2[l].y;
			Tile tile = Main.tile[num - 1, num2];
			Tile tile2 = Main.tile[num + 1, num2];
			Tile tile3 = Main.tile[num, num2 + 1];
			Tile tile4 = Main.tile[num, num2];
			byte b = 2;
			if (tile4.LiquidAmount < b)
			{
				tile4.LiquidAmount = 0;
				if (tile.LiquidAmount < b)
					tile.LiquidAmount = 0;
				else
					AddWater(num - 1, num2);
				if (tile2.LiquidAmount < b)
					tile2.LiquidAmount = 0;
				else
					AddWater(num + 1, num2);
			}
			else if (tile4.LiquidAmount < 20)
			{
				if (tile.LiquidAmount < tile4.LiquidAmount && (!tile.IsActiveUnactuated || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]) ||
					tile2.LiquidAmount < tile4.LiquidAmount && (!tile2.IsActiveUnactuated || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type]) ||
					tile3.LiquidAmount < 255 && (!tile3.IsActiveUnactuated || !Main.tileSolid[tile3.type] || Main.tileSolidTop[tile3.type]))
					tile4.LiquidAmount = 0;
			}
			else if (tile3.LiquidAmount < 255 && (!tile3.IsActiveUnactuated || !Main.tileSolid[tile3.type] || Main.tileSolidTop[tile3.type]) && !stuck)
			{
				QuickLiquidV2.Liquid2[l].kill = 0;
				return;
			}

			if (tile4.LiquidAmount < 250 && Main.tile[num, num2 - 1].LiquidAmount > 0)
				AddWater(num, num2 - 1);
			if (tile4.LiquidAmount == 0)
			{
				tile4.LiquidType = LiquidID.Water;
			}
			else
			{
				if (tile2.LiquidAmount > 0 && Main.tile[num + 1, num2 + 1].LiquidAmount < 250 && !Main.tile[num + 1, num2 + 1].IsActive ||
					tile.LiquidAmount > 0 && Main.tile[num - 1, num2 + 1].LiquidAmount < 250 && !Main.tile[num - 1, num2 + 1].IsActive)
				{
					AddWater(num - 1, num2);
					AddWater(num + 1, num2);
				}

				if (tile4.LiquidType == LiquidID.Lava)
				{
					LavaCheck(num, num2);
					for (int i = num - 1; i <= num + 1; i++)
					for (int j = num2 - 1; j <= num2 + 1; j++)
					{
						Tile tile5 = Main.tile[i, j];
						if (tile5.IsActive)
						{
							if (tile5.type == TileID.Grass ||
								tile5.type == TileID.CorruptGrass ||
								tile5.type == TileID.HallowedGrass ||
								tile5.type == TileID.CrimsonGrass)
							{
								tile5.type = TileID.Dirt;
								Terraria.WorldGen.SquareTileFrame(i, j);
								if (Main.netMode == NetmodeID.Server)
									NetMessage.SendTileSquare(-1, num, num2, 3);
							}
							else if (tile5.type == TileID.JungleGrass || tile5.type == TileID.MushroomGrass)
							{
								tile5.type = TileID.Mud;
								Terraria.WorldGen.SquareTileFrame(i, j);
								if (Main.netMode == NetmodeID.Server)
									NetMessage.SendTileSquare(-1, num, num2, 3);
							}
						}
					}
				}
				else if (tile4.LiquidType == LiquidID.Honey)
				{
					HoneyCheck(num, num2);
				}
			}

			if (Main.netMode == NetmodeID.Server)
				NetSendLiquid(num, num2);
			NumLiquid--;
			Main.tile[QuickLiquidV2.Liquid2[l].x, QuickLiquidV2.Liquid2[l].y].CheckingLiquid = false;
			QuickLiquidV2.Liquid2[l].x = QuickLiquidV2.Liquid2[NumLiquid].x;
			QuickLiquidV2.Liquid2[l].y = QuickLiquidV2.Liquid2[NumLiquid].y;
			QuickLiquidV2.Liquid2[l].kill = QuickLiquidV2.Liquid2[NumLiquid].kill;
			if (Main.tileAlch[tile4.type])
				Terraria.WorldGen.CheckAlch(num, num2);
		}

		public new static void ReInit()
		{
			SkipCount = 0;
			StuckCount = 0;
			StuckAmount = 0;
			Cycles = 10;
			ResLiquid = 1000000;
			MaxLiquid = 1000000;
			NumLiquid = 0;
			Stuck = false;
			QuickFall = false;
			QuickSettle = false;
			WetCounter = 0;
			PanicCounter = 0;
			PanicMode = false;
			PanicY = 0;
		}

		public new static void UpdateLiquid()
		{
			int netMode = Main.netMode;
			if (QuickSettle || NumLiquid > 2000)
				QuickFall = true;
			else
				QuickFall = false;
			WetCounter++;
			int num7 = MaxLiquid / Cycles;
			int num2 = num7 * (WetCounter - 1);
			int num3 = num7 * WetCounter;
			if (WetCounter == Cycles)
				num3 = NumLiquid;
			if (num3 > NumLiquid)
			{
				num3 = NumLiquid;
				int netMode2 = Main.netMode;
				WetCounter = Cycles;
			}

			if (QuickFall)
				for (int l = num2; l < num3; l++)
				{
					QuickLiquidV2.Liquid2[l].delay = 10;
					QuickLiquidV2.Liquid2[l].Update();
					Main.tile[QuickLiquidV2.Liquid2[l].x, QuickLiquidV2.Liquid2[l].y].SkipLiquid = false;
				}
			else
				for (int m = num2; m < num3; m++)
					if (!Main.tile[QuickLiquidV2.Liquid2[m].x, QuickLiquidV2.Liquid2[m].y].SkipLiquid)
						QuickLiquidV2.Liquid2[m].Update();
					else
						Main.tile[QuickLiquidV2.Liquid2[m].x, QuickLiquidV2.Liquid2[m].y].SkipLiquid = false;

			if (WetCounter >= Cycles)
			{
				WetCounter = 0;
				for (int n = NumLiquid - 1; n >= 0; n--)
					if (QuickLiquidV2.Liquid2[n].kill > 4)
						DelWater(n);
				int num4 = MaxLiquid - (MaxLiquid - NumLiquid);
				if (num4 > LiquidBuffer.numLiquidBuffer)
					num4 = LiquidBuffer.numLiquidBuffer;
				for (int num5 = 0; num5 < num4; num5++)
				{
					Main.tile[Main.liquidBuffer[0].x, Main.liquidBuffer[0].y].CheckingLiquid = false;
					AddWater(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y);
					LiquidBuffer.DelBuffer(0);
				}

				if (NumLiquid > 0 && NumLiquid > StuckAmount - 50 && NumLiquid < StuckAmount + 50)
				{
					StuckCount++;
					if (StuckCount >= 10000)
					{
						Stuck = true;
						for (int num6 = NumLiquid - 1; num6 >= 0; num6--)
							DelWater(num6);
						Stuck = false;
						StuckCount = 0;
					}
				}
				else
				{
					StuckCount = 0;
					StuckAmount = NumLiquid;
				}
			}
			/*
			if (!Terraria.WorldGen.gen && Main.netMode == 2 && Liquid2._netChangeSet.Count > 0)
			{
				Utils.Swap<HashSet<int>>(ref Liquid2._netChangeSet, ref Liquid2._swapNetChangeSet);
				NetManager.Instance.Broadcast(NetLiquid2Module.Serialize(Liquid2._swapNetChangeSet), -1);
				Liquid2._swapNetChangeSet.Clear();
			}
			*/
		}
	}
}
