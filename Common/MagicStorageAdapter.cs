using MagicStorage;
using MagicStorage.Components;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AutoStacker.Common
{
	public static class MagicStorageAdapter
	{
		public static void DepositItem(TileEntity tEStorageCenter, Item item)
		{
			int oldstack = item.stack;

			TEStorageHeart heart = ((TEStorageCenter)tEStorageCenter).GetHeart();
			heart.DepositItem(item);

			if (oldstack != item.stack)
			{
				if (Main.netMode == NetmodeID.Server)
					NetHelper.SendRefreshNetworkItems(heart.ID);
				else if (Main.netMode == NetmodeID.SinglePlayer)
					StorageGUI.RefreshItems();
			}
		}
	}
}
