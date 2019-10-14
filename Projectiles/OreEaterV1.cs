using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;

namespace AutoStacker.Projectiles
{
	public class OreEaterV1 : OreEaterBase
	{
		string displayName="Ore Eater Ver.1";
		
		public OreEaterV1()
		{
			this.maxSerchNum= 10;
			this.speed=16 * 3;
			this.light = 0f;
		}
		
		public override void AI()
		{
			if(!Terraria.Program.LoadedEverything )
			{
				return;
			}
			
			Player player = Main.player[projectile.owner];
			Players.OreEater modPlayer = player.GetModPlayer<Players.OreEater>();
			
			if(modPlayer.pet == null)
			{
				modPlayer.pet = (PetBase)new PetV1();
			}
			AI2(player, modPlayer, (PetBase)modPlayer.pet);
			
		}
	}
	
	public class PetV1 : PetBase
	{
		public override bool checkCanMove(int index, int dX, int dY, int pickPower)
		{
			Tile tile = Main.tile[AX[index], AY[index]];
			
			if
			(
				(
					!petDictionaryA.ContainsKey(AX[index] + dX) 
					|| !petDictionaryA[AX[index] + dX].ContainsKey(AY[index] + dY) 
				)
				&& AX[index] + dX < Main.Map.MaxWidth
				&& AX[index] + dX > 1
				&& AY[index] + dY < Main.Map.MaxHeight
				&& AY[index] + dY > 1
				&& Main.Map.IsRevealed(AX[index] + dX,AY[index] + dY)
				&& 
				(
					tile.liquid == 0 
					|| tile.liquid == 1 
					|| tile.liquid == 2
				)
				&&
				(
					tile == null 
					||
					(
						tile != null 
						&&
						(
							!tile.active()
							||
							(
								tile.active()
								&& 
								(
									oreTile.ContainsKey(tile.type)
									&& oreTile[tile.type]
								)
							)
						)
						
					)
				)
			)
			{
			}
			else
			{
				return false;
			}

			if ((tile.type == 211 && pickPower < 200)
				|| ((tile.type == 25 || tile.type == 203) && pickPower < 65)
				|| (tile.type == 117 && pickPower < 65)
				|| (tile.type == 37 && pickPower < 50)
				|| (tile.type == 404 && pickPower < 65)
//				|| ((tile.type == 22 || tile.type == 204) && (double)AY[index] > Main.worldSurface && pickPower < 55)
				|| (tile.type == 56 && pickPower < 65)
				|| (tile.type == 58 && pickPower < 65)
				|| ((tile.type == 226 || tile.type == 237) && pickPower < 210)
				|| (Main.tileDungeon[tile.type] && pickPower < 65)
//				|| ((double)AX[index] < (double)Main.maxTilesX * 0.35 || (double)AX[index] > (double)Main.maxTilesX * 0.65)
				|| (tile.type == 107 && pickPower < 100)
				|| (tile.type == 108 && pickPower < 110)
				|| (tile.type == 111 && pickPower < 150)
				|| (tile.type == 221 && pickPower < 100)
				|| (tile.type == 222 && pickPower < 110)
				|| (tile.type == 223 && pickPower < 150)
			)
			{
				return false;
			}

			int check=1;
			TileLoader.PickPowerCheck(tile, pickPower, ref check);
			if(check == 0)
			{
				return false;
			}
			
			return true;

		}
	}
}
