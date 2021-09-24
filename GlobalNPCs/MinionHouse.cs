using Terraria;
using Terraria.ModLoader;

namespace AutoStacker.GlobalNPCs
{
	public class MinionHouse : GlobalNPC
	{
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (player.name == "MinionHouse")
				spawnRate *= 2;
		}

		public override void EditSpawnRange(Player player, ref int spawnRangeX, ref int spawnRangeY, ref int safeRangeX, ref int safeRangeY)
		{
			if (player.name == "MinionHouse")
			{
				spawnRangeX /= 4;
				spawnRangeY /= 4;
				safeRangeX = 0;
				safeRangeY = 0;
			}
		}

		//public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		//{
		//	int x = Main.maxTilesX*16/2;
		//	int y = Main.maxTilesY*16/2;
		//	
		//	int x = Main.maxTilesX*4;
		//	int y = Main.maxTilesY*4;
		//	int dy=0;
		//	int dx=1;
		//	int ddy=1;
		//	int ddx=1;
		//	
		//	Main.NewText(spawnInfo.player.name + "," + spawnInfo.player.position.X + "," + spawnInfo.player.position.Y );
		//	
		//	
		//	for(int round=1;round <= 20;round++)
		//	{
		//		int edgeLength = round;
		//		for(int edge = 0; edge <= 1; edge ++)
		//		{
		//			for(int point = 0;point <= edgeLength; point++)
		//			{
		//				if(Main.tile[x,y] == null)
		//				{
		//					spawnInfo.spawnTileX = x;
		//					spawnInfo.spawnTileY = y;
		//					Main.NewText(spawnInfo.spawnTileX +","+ spawnInfo.spawnTileY);
		//					break;
		//				}
		//				
		//				x += dx;
		//				y += dy;
		//			}
		//			
		//			if(Math.Abs(dx) >= 1)
		//			{
		//				ddx *= -1;
		//			}
		//			if(Math.Abs(dy) >= 1)
		//			{
		//				ddy *= -1;
		//			}
		//			dx += ddx;
		//			dy += ddy;
		//		}
		//	}
		//	
		//	
		//	foreach (int current in pool.Keys)
		//	{
		//		pool[current] = 0f;
		//		Main.NewText(pool[current]);
		//	}
		//	
		//	
		//}

		//public override bool CheckActive(NPC npc)
		//{
		//	return true;
		//}
	}
}
