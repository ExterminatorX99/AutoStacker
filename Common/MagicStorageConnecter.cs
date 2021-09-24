using MagicStorage;
using MagicStorage.Components;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AutoStacker.Common
{
	public static class MagicStorageConnecter
	{
		public static TEStorageHeart FindHeart(Point16 origin)
		{
			if (TileEntity.ByPosition.TryGetValue(origin, out TileEntity tileEntity))
				if (tileEntity is TEStorageCenter center)
					return center.GetHeart();
			return null;
		}

		public static bool InjectItem(Point16 origin, Item item)
		{
			TEStorageHeart heart = FindHeart(origin);
			if (heart == null)
				return false;

			int oldStack = item.stack;

			heart.DepositItem(item);

			if (oldStack == item.stack)
				return false;

			HandleStorageItemChange(heart);
			return true;
		}

		private static void HandleStorageItemChange(TEStorageHeart heart)
		{
			if (Main.netMode == NetmodeID.Server)
				NetHelper.SendRefreshNetworkItems(heart.ID);
			else if (Main.netMode == NetmodeID.SinglePlayer)
				StorageGUI.RefreshItems();
		}
	}
}
