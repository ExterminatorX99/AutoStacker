using AutoStacker.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Projectiles
{
	public class OreEaterV3 : OreEaterBase
	{
		private string _displayName = "Ore Eater Ver.3";

		public OreEaterV3()
		{
			MaxSerchNum = 30;
			Speed = 16 * 3;
			Light = 2f;
		}

		public override void AI()
		{
			if (!Program.LoadedEverything)
				return;

			Player player = Main.player[Projectile.owner];
			OreEater modPlayer = player.GetModPlayer<OreEater>();

			modPlayer.Pet ??= new PetV3();
			AI2(player, modPlayer, modPlayer.Pet);
		}
	}

	public class PetV3 : PetBase
	{
		public override bool CheckCanMove(int index, int dX, int dY, int pickPower)
		{
			Tile tile = Main.tile[Ax[index], Ay[index]];

			if (PetDictionaryA.ContainsKey(Ax[index] + dX) && PetDictionaryA[Ax[index] + dX].ContainsKey(Ay[index] + dY) ||
				Ax[index] + dX >= Main.Map.MaxWidth ||
				Ax[index] + dX <= 1 ||
				Ay[index] + dY >= Main.Map.MaxHeight ||
				Ay[index] + dY <= 1 ||
				!Main.Map.IsRevealed(Ax[index] + dX, Ay[index] + dY) ||
				tile != null && tile.IsActive && (!tile.IsActive || !OreTile.ContainsKey(tile.type) || !OreTile[tile.type]))
				return false;

			if (tile.type == TileID.Chlorophyte && pickPower <= 200 ||
				(tile.type == TileID.Ebonstone || tile.type == TileID.Crimstone) && pickPower <= 65 ||
				tile.type == TileID.Pearlstone && pickPower <= 65 ||
				tile.type == TileID.Meteorite && pickPower <= 50 ||
				tile.type == TileID.DesertFossil && pickPower <= 65
				//				|| ((tile.type == 22 || tile.type == 204) && (double)AY[index] > Main.worldSurface && pickPower < 55)
				||
				tile.type == TileID.Obsidian && pickPower <= 65 ||
				tile.type == TileID.Hellstone && pickPower <= 65 ||
				(tile.type == TileID.LihzahrdBrick || tile.type == TileID.LihzahrdAltar) && pickPower <= 210 ||
				Main.tileDungeon[tile.type] && pickPower <= 65
				//				|| ((double)AX[index] < (double)Main.maxTilesX * 0.35 || (double)AX[index] > (double)Main.maxTilesX * 0.65)
				||
				tile.type == TileID.Cobalt && pickPower <= 100 ||
				tile.type == TileID.Mythril && pickPower <= 110 ||
				tile.type == TileID.Adamantite && pickPower <= 150 ||
				tile.type == TileID.Palladium && pickPower <= 100 ||
				tile.type == TileID.Orichalcum && pickPower <= 110 ||
				tile.type == TileID.Titanium && pickPower <= 150
			)
				return false;

			int check = 1;
			TileLoader.PickPowerCheck(tile, pickPower, ref check);
			return check != 0;
		}
	}
}
