using System.Text.RegularExpressions;
using Terraria;
using Terraria.DataStructures;

namespace AutoStacker.Common
{
	public static class MagicStorageConnecter
	{
		private static readonly Regex RegexMagicStorage = new("^MagicStorage(?!Extra)", RegexOptions.Compiled);

		public static TileEntity FindHeart(Point16 origin)
		{
			TileEntity tEStorageCenter = TileEntity.ByPosition[origin];
			if (tEStorageCenter == null || tEStorageCenter.GetType().Name != "TEStorageHeart")
				return null;

			return tEStorageCenter;
		}

		public static bool InjectItem(Point16 origin, Item item)
		{
			TileEntity tEStorageCenter = FindHeart(origin);
			if (tEStorageCenter == null)
				return false;

			if (RegexMagicStorage.IsMatch(tEStorageCenter.GetType().Assembly.GetName().Name))
			{
				MagicStorageAdapter.DepositItem(tEStorageCenter, item);
				return true;
			}

			return false;
		}
	}
}
