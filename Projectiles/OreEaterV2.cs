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
	public class OreEaterV2 : OreEaterBase
	{
		string displayName="Ore Eater Ver.2";
		
		public OreEaterV2()
		{
			this.maxSerchNum= 40;
			this.speed=16 * 3;
			this.light = 1f;
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
				modPlayer.pet = (PetBase)new PetV2();
			}
			AI2(player, modPlayer, (PetBase)modPlayer.pet);
			
		}
	}
	
	public class PetV2 : PetBase
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
								//&& 
								//(
								//	(
								//		oreTile.ContainsKey(tile.type)
								//		&& oreTile[tile.type]
								//	)
								//	|| tile.type == TileID.ExposedGems
								//	|| tile.type == TileID.Sapphire
								//	|| tile.type == TileID.Ruby
								//	|| tile.type == TileID.Emerald
								//	|| tile.type == TileID.Topaz
								//	|| tile.type == TileID.Amethyst
								//	|| tile.type == TileID.Diamond
//									|| tile.type == TileID.Crystals
								//	|| tile.type == TileID.Heart
								//	|| tile.type == TileID.LifeFruit
								//	|| tile.type == TileID.Pots
								//)
							)
						)
					)
				)
			)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
