using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Worlds
{
	public class WitchsPot : ModSystem
	{
		public List<int> ChestNo = new();

		public override void PreUpdateWorld()
		{
			if (ChestNo.Count == 0)
				return;

			//Change Rondom Item
			Chest chest = Main.chest[ChestNo[0]];
			List<Item> items = new();

			foreach (Item item in chest.item)
			{
				//if item is nothing
				if (item.stack != 0)
					//Item Change
					do
					{
						item.SetDefaults(Main.rand.Next(TextureAssets.Item.Length - 1) + 1);
					} while (item.stack == 0);

				items.Add(item.Clone());
			}

			//Change Chest Type
			Terraria.WorldGen.PlaceChestDirect(Main.chest[ChestNo[0]].x, Main.chest[ChestNo[0]].y + 1, TileID.Containers, 0, ChestNo[0]);

			//Copy Item
			chest = Main.chest[ChestNo[0]];
			for (int itemNo = 0; itemNo < chest.item.Length; itemNo++)
				chest.item[itemNo] = items[itemNo].Clone();

			//Delete Que
			if (ChestNo.Count >= 2 && ChestNo[0] == ChestNo[1])
				ChestNo.RemoveAt(1);
			ChestNo.RemoveAt(0);
		}
	}
}
