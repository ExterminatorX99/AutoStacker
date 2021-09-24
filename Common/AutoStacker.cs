using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace AutoStacker.Common
{
	public class AutoStacker
	{
		public static int FindChest(int originX, int originY)
		{
			Tile tile = Main.tile[originX, originY];
			if (tile == null || !tile.IsActive)
				return -1;

			if (!Chest.IsLocked(originX, originY))
				return Chest.FindChest(originX, originY);
			return -1;
		}

		public static Point16 GetOrigin(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			if (tile == null || !tile.IsActive)
				return new Point16(x, y);

			TileObjectData tileObjectData = TileObjectData.GetTileData(tile.type, 0);
			if (tileObjectData == null)
				return new Point16(x, y);

			//OneByOne
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			if (tileObjectData.Width == 1 && tileObjectData.Height == 1)
				return new Point16(x, y);

			//xOffset
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			int xOffset = tile.frameX % tileObjectData.CoordinateFullWidth / tileObjectData.CoordinateWidth;

			//yOffset
			//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			//Rectangle(single)
			int yOffset;
			if (tileObjectData.CoordinateHeights.Distinct().Count() == 1)
			{
				yOffset = tile.frameY % tileObjectData.CoordinateFullHeight / tileObjectData.CoordinateHeights[0];
			}

			//Rectangle(complex)
			else
			{
				yOffset = 0;
				int fullY = tile.frameY % tileObjectData.CoordinateFullHeight;
				for (int i = 0; i < tileObjectData.CoordinateHeights.Length && fullY >= tileObjectData.CoordinateHeights[i]; i++)
				{
					fullY -= tileObjectData.CoordinateHeights[i];
					yOffset++;
				}
			}

			return new Point16(x - xOffset, y - yOffset);
		}
	}
}
