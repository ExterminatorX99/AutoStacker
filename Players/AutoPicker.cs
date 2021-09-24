using System.Reflection;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;

#nullable enable

namespace AutoStacker.Players
{
	public class AutoPicker : Player
	{
		public delegate int GetPickaxeDamage(int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget);

		public delegate bool DoesPickTargetTransformOnKill(HitTile hitCounter, int damage, int x, int y, int pickPower, int bufferIndex, Tile tileTarget);

		public delegate void ClearMiningCacheAt(int x, int y, int hitTileCacheType);

		private static readonly GetPickaxeDamage CallGetPickaxeDamage;
		private static readonly DoesPickTargetTransformOnKill CallDoesPickTargetTransformOnKill;
		private static readonly ClearMiningCacheAt CallClearMiningCacheAt;

		static AutoPicker()
		{
			MethodInfo methodGetPickaxeDamage = typeof(Player).GetMethod("GetPickaxeDamage", BindingFlags.NonPublic | BindingFlags.Instance)!;
			MethodInfo methodDoesPickTargetTransformOnKill =
				typeof(Player).GetMethod("DoesPickTargetTransformOnKill", BindingFlags.NonPublic | BindingFlags.Instance)!;
			MethodInfo methodClearMiningCacheAt = typeof(Player).GetMethod("ClearMiningCacheAt", BindingFlags.NonPublic | BindingFlags.Instance)!;

			CallGetPickaxeDamage = methodGetPickaxeDamage.CreateDelegate<GetPickaxeDamage>();
			CallDoesPickTargetTransformOnKill = methodDoesPickTargetTransformOnKill.CreateDelegate<DoesPickTargetTransformOnKill>();
			CallClearMiningCacheAt = methodClearMiningCacheAt.CreateDelegate<ClearMiningCacheAt>();
		}

		public void PickTile2(int x, int y, int pickPower, Tiles.AutoPicker autoPicker)
		{
			int num = hitTile.HitObject(x, y, 1);
			Tile tile = Main.tile[x, y];
			if (tile.type == TileID.MysticSnakeRope)
				return;

			int num2 = CallGetPickaxeDamage(x, y, pickPower, num, tile);
			if (!Terraria.WorldGen.CanKillTile(x, y))
				num2 = 0;

			if (Main.getGoodWorld)
				num2 *= 2;

			if (CallDoesPickTargetTransformOnKill(hitTile, num2, x, y, pickPower, num, tile))
				num2 = 0;

			if (hitTile.AddDamage(num, num2) >= 100)
			{
				AchievementsHelper.CurrentlyMining = true;
				CallClearMiningCacheAt(x, y, 1);
				if (Main.netMode == NetmodeID.MultiplayerClient && Main.tileContainer[Main.tile[x, y].type])
				{
					if (Main.tile[x, y].type == TileID.DisplayDoll || Main.tile[x, y].type == TileID.HatRack)
					{
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 20, x, y);
					}
					else
					{
						WorldGen.AutoPicker.KillTile2(autoPicker, x, y, true);
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y, 1f);
					}

					if (Main.tile[x, y].type == TileID.Containers || Main.tile[x, y].type < TileID.Count && TileID.Sets.BasicChest[Main.tile[x, y].type])
						NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 1, x, y);

					if (Main.tile[x, y].type == TileID.Containers2)
						NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 5, x, y);

					if (Main.tile[x, y].type == TileID.Dressers)
						NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 3, x, y);

					if (Main.tile[x, y].type >= TileID.Count)
					{
						if (TileID.Sets.BasicChest[Main.tile[x, y].type])
							NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 101, x, y, 0f, 0, Main.tile[x, y].type);

						if (TileID.Sets.BasicDresser[Main.tile[x, y].type])
							NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, 103, x, y, 0f, 0, Main.tile[x, y].type);
					}
				}
				else
				{
					bool num3 = Main.tile[x, y].IsActive;
					WorldGen.AutoPicker.KillTile2(autoPicker, x, y, true);

					if (!Main.dedServ && num3 && !Main.tile[x, y].IsActive)
						AchievementsHelper.HandleMining();

					if (Main.netMode == NetmodeID.MultiplayerClient)
						NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
				}

				AchievementsHelper.CurrentlyMining = false;
			}
			else
			{
				WorldGen.AutoPicker.KillTile2(autoPicker, x, y, true);
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y, 1f);
					NetMessage.SendData(MessageID.SyncTilePicking, -1, -1, null, Main.myPlayer, x, y, num2);
				}
			}

			if (num2 != 0)
				hitTile.Prune();
		}
	}
}
