using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace AutoStacker.Worlds
{
	public class ItemGrowerChest : ModSystem
	{
		private const int TimeStep = 10;
		private readonly Dictionary<Chest, List<ChestItem>> _chestItems = new();
		private int _moonPhasePrev;
		private int _time2Prev;

		public override void PreUpdateWorld()
		{
			int moonPhase = Main.moonPhase;
			//time2 = Main.time + 16200 * Convert.ToInt32(Main.dayTime) + 70200 * (1-Convert.ToInt32(Main.dayTime))
			int time2 = (int)Main.time + 70200 - 54000 * Convert.ToInt32(Main.dayTime);
			int passTime2 = time2 - _time2Prev + (moonPhase == _moonPhasePrev ? 0 : 1) * 86400;

			if (passTime2 > 3600)
			{
				_time2Prev = time2 - time2 % TimeStep;
				_moonPhasePrev = moonPhase;
				return;
			}

			if (passTime2 < TimeStep)
				return;

			IEnumerable<Chest> chests = Main.chest.Where
			(
				chest =>
					chest != null &&
					Main.tile[chest.x, chest.y] != null &&
					Main.tile[chest.x, chest.y].IsActive &&
					Main.tile[chest.x, chest.y].type == ModContent.TileType<Tiles.ItemGrowerChest>()
			);

			foreach (Chest chest in chests)
			{
				if (!_chestItems.ContainsKey(chest))
				{
					int len = chest.item.Length;
					_chestItems[chest] = new List<ChestItem>(len);
					for (int itemNo = 0; itemNo < len; itemNo++)
						_chestItems[chest].Add(new ChestItem());
				}

				for (int itemNo = 0; itemNo < _chestItems[chest].Count; itemNo++)
					//if item is nothing
					if (chest.item[itemNo].stack == 0)
					{
						//skip
					}

					//if change or add items
					else if
					(
						_chestItems[chest][itemNo].Type != chest.item[itemNo].type ||
						_chestItems[chest][itemNo].Stack != chest.item[itemNo].stack ||
						_chestItems[chest][itemNo].Prefix != chest.item[itemNo].prefix
					)
					{
						//init growing data
						_chestItems[chest][itemNo]
							.Init
							(
								chest.item[itemNo].type
								, chest.item[itemNo].stack
								, chest.item[itemNo].prefix
								, chest.item[itemNo].maxStack
							);
					}

					//if over max time
					else if (_chestItems[chest][itemNo].TotalGrowPassTime2 > _chestItems[chest][itemNo].MaxGrowPassTime2)
					{
						//stack max
						chest.item[itemNo].stack = _chestItems[chest][itemNo].MaxStack;
						_chestItems[chest][itemNo].Stack = _chestItems[chest][itemNo].MaxStack;
					}

					//others
					else
					{
						//grow item
						long totalGrowPassTime2 = _chestItems[chest][itemNo].TotalGrowPassTime2 + passTime2;
						_chestItems[chest][itemNo].TotalGrowPassTime2 = totalGrowPassTime2;

						int nextStack = (int)Math.Pow(2, totalGrowPassTime2 * 0.0000115740740740741);

						chest.item[itemNo].stack = nextStack;
						_chestItems[chest][itemNo].Stack = nextStack;
					}
			}

			_time2Prev = time2;
			_moonPhasePrev = moonPhase;
		}

		private class ChestItem
		{
			public int Type;
			public int Stack;
			public int Prefix;
			public int MaxStack;

			public long MaxGrowPassTime2;
			public long TotalGrowPassTime2;

			public ChestItem()
			{
				Type = 0;
				Stack = 0;
				Prefix = 0;
				MaxStack = 0;

				MaxGrowPassTime2 = 0;
				TotalGrowPassTime2 = 0;
			}

			public void Init(int type, int stack, int prefix, int maxStack)
			{
				Type = type;
				Stack = stack;
				Prefix = prefix;
				MaxStack = maxStack;

				MaxGrowPassTime2 = (long)(Math.Log(maxStack, 2) * 86400);
				TotalGrowPassTime2 = (long)(Math.Log(stack, 2) * 86400);
			}
		}
	}
}
