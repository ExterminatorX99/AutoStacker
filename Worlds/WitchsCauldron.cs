using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace AutoStacker.Worlds
{
	public class WitchsCauldron : ModSystem
	{
		private const int TimeStep = 10;
		private int _moonPhasePrev;
		private int _time2Prev;

		public override void PreUpdateWorld()
		{
			int moonPhase = Main.moonPhase;
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
					Main.tile[chest.x, chest.y].type == ModContent.TileType<Tiles.WitchsCauldron>()
			);


			foreach (Chest chest in chests)
			foreach (Item item in chest.item)
			{
				if (item.stack == 0)
					continue;

				//if change or add items

				//Item Change
				if (Main.rand.Next(60 * 60 * 24) <= passTime2)
					do
					{
						item.SetDefaults(Main.rand.Next(TextureAssets.Item.Length - 1) + 1);
					} while (item.stack == 0 || item.IsAir);
			}

			_time2Prev = time2;
			_moonPhasePrev = moonPhase;
		}
	}
}
