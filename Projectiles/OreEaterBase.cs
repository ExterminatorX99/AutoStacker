using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoStacker.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Projectiles
{
	public class OreEaterBase : ModProjectile
	{
		private const int FadeInTicks = 30;
		private const int FullBrightTicks = 2;
		private const int FadeOutTicks = 30;
		private const int Range = 10;
		private const string _displayName = "Ore Eater Base";
		private int _rangeHypoteneus = (int)Math.Sqrt(Range * Range + Range * Range);

		private int _originX;
		private int _originY;

		private int _routeCount = -1;
		private int _routeCountShift;

		public int MaxSerchNum = 60;
		private int _prevLoop;
		private int _chestID = -1;
		private int _targetPrev = 4;

		public int Speed = 16 * 3;
		public float Light = 0f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(_displayName);
			Main.projFrames[Type] = 1;
			Main.projPet[Type] = true;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.penetrate = -1;
			Projectile.netImportant = true;
			Projectile.timeLeft *= 5;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.scale = 0.8f;
			Projectile.tileCollide = false;
		}

		public override ModProjectile Clone()
		{
			OreEaterBase newInstance = (OreEaterBase)MemberwiseClone();
			newInstance._originX = _originX;
			newInstance._originY = _originY;
			newInstance._routeCount = _routeCount;
			newInstance._routeCountShift = _routeCountShift;
			newInstance._prevLoop = _prevLoop;
			newInstance._targetPrev = _targetPrev;

			return newInstance;
		}

		//public override void AI()
		//{
		//	if(!Terraria.Program.LoadedEverything )//&& Terraria.Main.tilesLoaded))
		//	{
		//		return;
		//	}
		//	
		//	Player player = Main.player[projectile.owner];
		//	Players.OreEater modPlayer = player.GetModPlayer<Players.OreEater>(mod);
		//	
		//	if(modPlayer.pet == null)
		//	{
		//		modPlayer.pet = new PetBase();
		//	}
		//	AI2(player, modPlayer, modPlayer.pet);
		//	
		//}

		public void AI2(Player player, OreEater modPlayer, PetBase pet)
		{
			pet.DelayLoad();

			if (!player.active)
			{
				Main.npc[modPlayer.Index].active = false;
				Projectile.active = false;
				pet.InitListA();
				pet.RouteListX.Clear();
				pet.RouteListY.Clear();
				Projectile.position = player.position;
				return;
			}

			if (player.dead)
				modPlayer.OreEaterEnable = false;

			if (modPlayer.OreEaterEnable)
			{
				Projectile.timeLeft = 2;
			}
			else
			{
				modPlayer.ResetEffects();
				Main.npc[modPlayer.Index].active = false;
				pet.InitListA();
				pet.RouteListX.Clear();
				pet.RouteListY.Clear();
				Projectile.position = player.position;
			}

			//scan pickel
			int pickPower = Main.LocalPlayer.inventory.Max(item => item.pick);
			if (pickPower <= 1)
				pickPower = 1;

			int pickSpeed;
			if (!Main.LocalPlayer.inventory.Any(item => item.pick > 0))
				pickSpeed = 30;
			else
				pickSpeed = Main.LocalPlayer.inventory.Where(item => item.pick > 0).Min(item => item.useTime);
			if (pickSpeed <= 1)
				pickSpeed = 1;


			//light
			Lighting.AddLight(Projectile.position, 0.9f * Light, 0.1f * Light, 0.3f * Light);


			//ore scan & move & pick 
			if (!pet.StatusAIndex.ContainsKey(3) && !pet.StatusAIndex.ContainsKey(4))
			{
				if (pet.LatestLoop >= MaxSerchNum || _prevLoop == pet.LatestLoop || pet.StatusA.Count == 0)
				{
					if (pet.LatestLoop >= MaxSerchNum || _prevLoop == pet.LatestLoop)
						//if(prevLoop == pet.latestLoop)
					{
						Projectile.position = player.position;
						modPlayer.NPC.position = Projectile.position;
					}

					_originX = (int)Projectile.position.X / 16;
					_originY = (int)Projectile.position.Y / 16;
					_prevLoop = 0;
					_routeCountShift = 0;
					pet.SerchA(_originX, _originY, 12, 2, 3, pickPower, true);
				}
				else
				{
					_prevLoop = pet.LatestLoop;
					pet.SerchA(_originX, _originY, 1, 1, 3, pickPower);
					pet.SerchA(_originX, _originY, 2, 1, 3, pickPower);
				}
			}
			else
			{
				int target;
				if (pet.StatusAIndex.ContainsKey(3))
					target = 3;
				else if (pet.StatusAIndex.ContainsKey(4))
					target = 4;
				else
					return;

				//makeRoute
				if (_routeCount == -1)
				{
					pet.MakeRoute(target, 0, MaxSerchNum);
					_routeCount = pet.RouteListX.Count - 1;
					if (target == 4)
					{
						_routeCount -= _routeCountShift;
						_routeCountShift = _routeCount;
					}
				}

				//set velocity
				Projectile.velocity.X = pet.RouteListX[_routeCount] * 16 - Projectile.position.X - 8;
				Projectile.velocity.Y = pet.RouteListY[_routeCount] * 16 - Projectile.position.Y - 8;
				modPlayer.NPC.position = Projectile.position;

				//next cell
				if (_routeCount >= 0 && Main.rand.Next(_routeCount * pickSpeed) <= Speed)
					_routeCount -= 1;


				//end route
				if (_routeCount == -1)
				{
					Projectile.position.X = pet.RouteListX[0] * 16;
					Projectile.position.Y = pet.RouteListY[0] * 16;
					Projectile.velocity.X = 0;
					Projectile.velocity.Y = 0;
					if (target == 3)
					{
						modPlayer.Player.PickTile(pet.Ax[pet.StatusAIndex[3][0]], pet.Ay[pet.StatusAIndex[3][0]], pickPower);
						pet.InitListA();
						pet.RouteListX.Clear();
						pet.RouteListY.Clear();
						_targetPrev = 3;
					}
					else
					{
						if (_targetPrev == 3)
						{
							pet.InitListA();
							pet.RouteListX.Clear();
							pet.RouteListY.Clear();
						}
						else
						{
							pet.StatusA[pet.StatusAIndex[4][0]] = 5;
							pet.make_statusAIndex();
						}

						_targetPrev = 4;
					}
				}
			}

			modPlayer.NPC.life = modPlayer.NPC.lifeMax;
		}
	}

	public class PetBase
	{
		private static readonly Regex OreRegex = new("Ore$|OreTile$", RegexOptions.Compiled);
		private static Regex _gemRegex = new("^Large", RegexOptions.Compiled);
		private static int _tileId;
		private static Dictionary<int, bool> _oreTile = new();

		private readonly double _root2 = Math.Sqrt(2);
		private Item _item = new();
		public int LatestLoop;

		public List<int> RouteListX = new();
		public List<int> RouteListY = new();

		public List<List<int>> PetDictionaryAInv { get; } = new();

		public Dictionary<int, Dictionary<int, int>> PetDictionaryA { get; } = new();

		public List<int> Ax { get; } = new();

		public List<int> Ay { get; } = new();

		public List<int> StatusA { get; } = new();

		public List<int> RouteAx { get; } = new();

		public List<int> RouteAy { get; } = new();

		public List<double> NA { get; } = new();

		public Dictionary<int, List<int>> StatusAIndex { get; set; } = new();

		public Dictionary<int, bool> OreTile
		{
			get => _oreTile;
			set => _oreTile = value;
		}

		public PetBase()
		{
			InitListA();
			make_statusAIndex();
		}

		public void DelayLoad()
		{
			if (_tileId == 0)
			{
				Main.NewText("AutoStacker[Ore Eater]:Item data loading...");
				_oreTile[TileID.ExposedGems] = true;
				_oreTile[TileID.Sapphire] = true;
				_oreTile[TileID.Ruby] = true;
				_oreTile[TileID.Emerald] = true;
				_oreTile[TileID.Topaz] = true;
				_oreTile[TileID.Amethyst] = true;
				_oreTile[TileID.Diamond] = true;
				_oreTile[TileID.Crystals] = true;
				_oreTile[TileID.Heart] = true;
				_oreTile[TileID.LifeFruit] = true;
				_oreTile[TileID.Pots] = true;
				_oreTile[TileID.Cobweb] = true;
				_oreTile[TileID.Obsidian] = true;
			}

			if (_tileId < TextureAssets.Tile.Length)
			{
				ModTile tile = TileLoader.GetTile(_tileId);
				if (TileID.Sets.Ore[_tileId] || tile?.Name != null && OreRegex.IsMatch(tile.Name))
					_oreTile[_tileId] = true;

				_tileId += 1;
				if (_tileId == TextureAssets.Tile.Length)
					//Main.recipe.Where( recipe => recipe.createItem.modItem != null && recipe.createItem.modItem.DisplayName != null && gemRegex.IsMatch( recipe.createItem.modItem.DisplayName.GetDefault() )).SelectMany( recipe => recipe.requiredItem ).Where(item => item.createTile != null && item.createTile != -1 ).Any(item => _oreTile[item.createTile] = true );
					Main.NewText("AutoStacker[Ore Eater]: Item data loading Complete!");
			}
		}

		public void SerchA(int originX, int originY, int serchTiles, int resultMaxNum, int resultMaxStatus, int pickPower, bool reset = false)
		{
			if (reset)
			{
				InitListA();
				LatestLoop = 0;
			}

			if (serchTiles <= 0)
				return;
			if (!PetDictionaryA.ContainsKey(originX) || !PetDictionaryA[originX].ContainsKey(originY))
			{
				AddListA(originX, originY, 0, originX, originY, 0);
				make_statusAIndex();
			}

			if (!StatusAIndex.ContainsKey(0) && !StatusAIndex.ContainsKey(1))
				return;

			if (StatusAIndex.ContainsKey(resultMaxStatus) && StatusAIndex[resultMaxStatus].Count >= resultMaxNum)
				return;

			//fined next tile
			if (StatusAIndex.ContainsKey(0))
				foreach (int index in StatusAIndex[0])
				{
					int x = Ax[index];
					int y = Ay[index];
					for (int dX = -1; dX <= 1; dX++)
					for (int dY = -1; dY <= 1; dY++)
					{
						if (dY == 0 && dX == 0)
							continue;

						if (CheckCanMove(index, dX, dY, pickPower))
							AddListA(x + dX, y + dY, 0, x, y, int.MaxValue);
					}

					StatusA[index] = 1;
				}

			//find row cost route
			if (StatusAIndex.ContainsKey(1))
				foreach (int index in StatusAIndex[1])
				{
					double cur = NA[index];
					int x = Ax[index];
					int y = Ay[index];
					for (int dX = -1; dX <= 1; dX++)
					for (int dY = -1; dY <= 1; dY++)
					{
						if (
							dY == 0 && dX == 0 ||
							!PetDictionaryA.ContainsKey(x + dX) ||
							!PetDictionaryA[x + dX].ContainsKey(y + dY) ||
							StatusA[PetDictionaryA[x + dX][y + dY]] != 0 &&
							StatusA[PetDictionaryA[x + dX][y + dY]] != 1 &&
							StatusA[PetDictionaryA[x + dX][y + dY]] != 2
						)
							continue;
						double match;
						if (dX == 0 || dY == 0)
							match = NA[PetDictionaryA[x + dX][y + dY]] + 1d;
						else
							match = NA[PetDictionaryA[x + dX][y + dY]] + _root2;

						if (match < cur)
						{
							NA[index] = match;
							cur = match;
							RouteAx[index] = x + dX;
							RouteAy[index] = y + dY;
						}
					}

					StatusA[index] = 2;
				}

			//find player, ores...
			if (StatusAIndex.ContainsKey(2))
				foreach (int index in StatusAIndex[2])
					if (CheckCanPick(index, pickPower))
						StatusA[index] = 3;
					else if
					(
						Ax[index] == (int)(Main.LocalPlayer.position.X / 16) && Ay[index] == (int)(Main.LocalPlayer.position.Y / 16) //-4)
					)
						StatusA[index] = 4;
					else
						StatusA[index] = 5;

			make_statusAIndex();
			LatestLoop += 1;
			SerchA(originX, originY, serchTiles - 1, resultMaxNum, resultMaxStatus, pickPower);
		}

		public virtual bool CheckCanMove(int index, int dX, int dY, int pickPower)
		{
			Tile tile = Main.tile[Ax[index], Ay[index]];
			if (PetDictionaryA.ContainsKey(Ax[index] + dX) && PetDictionaryA[Ax[index] + dX].ContainsKey(Ay[index] + dY) ||
				Ax[index] + dX >= Main.Map.MaxWidth ||
				Ax[index] + dX <= 1 ||
				Ay[index] + dY >= Main.Map.MaxHeight ||
				Ay[index] + dY <= 1 ||
				!Main.Map.IsRevealed(Ax[index] + dX, Ay[index] + dY) ||
				tile != null && tile.IsActive && !_oreTile.ContainsKey(tile.type))
				return false;

			if (tile.type == TileID.Chlorophyte && pickPower <= 200 ||
				(tile.type == TileID.Ebonstone || tile.type == TileID.Crimstone) && pickPower <= 65 ||
				tile.type == TileID.Pearlstone && pickPower <= 65 ||
				tile.type == TileID.Meteorite && pickPower <= 50 ||
				tile.type == TileID.DesertFossil && pickPower <= 65
				//|| ((tile.type == 22 || tile.type == 204) && (double)_AY[index] > Main.worldSurface && pickPower <= 55)
				||
				tile.type == TileID.Obsidian && pickPower <= 65 ||
				tile.type == TileID.Hellstone && pickPower <= 65 ||
				(tile.type == TileID.LihzahrdBrick || tile.type == TileID.LihzahrdAltar) && pickPower <= 210 ||
				Main.tileDungeon[tile.type] && pickPower <= 65
				//|| ((double)_AX[index] < (double)Main.maxTilesX * 0.35 || (double)_AX[index] > (double)Main.maxTilesX * 0.65)
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

		private bool CheckCanPick(int index, int pickPower)
		{
			if (index >= Ax.Count || index >= Ay.Count || index <= -1)
				return false;

			Tile tile = Main.tile[Ax[index], Ay[index]];
			Tile tileUpper = Main.tile[Ax[index], Ay[index] - 1];

			if (tile == null)
				return false;

			//_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
			if (tile.type == TileID.Chlorophyte && pickPower <= 200 ||
				(tile.type == TileID.Ebonstone || tile.type == TileID.Crimstone) && pickPower <= 65 ||
				tile.type == TileID.Pearlstone && pickPower <= 65 ||
				tile.type == TileID.Meteorite && pickPower <= 50 ||
				tile.type == TileID.DesertFossil && pickPower <= 65
//				|| ((tile.type == 22 || tile.type == 204) && (double)_AY[index] > Main.worldSurface && pickPower < 55)
				||
				tile.type == TileID.Obsidian && pickPower <= 65 ||
				tile.type == TileID.Hellstone && pickPower <= 65 ||
				(tile.type == TileID.LihzahrdBrick || tile.type == TileID.LihzahrdAltar) && pickPower <= 210 ||
				Main.tileDungeon[tile.type] && pickPower <= 65
//				|| ((double)_AX[index] < (double)Main.maxTilesX * 0.35 || (double)_AX[index] > (double)Main.maxTilesX * 0.65)
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
			if (check == 0)
				return false;
			//_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

			return tile.IsActive &&
				   _oreTile.ContainsKey(tile.type) &&
				   (tileUpper == null ||
					tileUpper.type != TileID.Containers && tileUpper.type != TileID.DemonAltar);
		}

		public void MakeRoute(int status, int routeNo, int maxSerchNum)
		{
			RouteListX.Clear();
			RouteListY.Clear();
			if (StatusAIndex.ContainsKey(status) && StatusAIndex[status].Count > routeNo)
			{
				RouteListX.Add(Ax[StatusAIndex[status][routeNo]]);
				RouteListY.Add(Ay[StatusAIndex[status][routeNo]]);
				for (int count = 0; count <= maxSerchNum; count++)
				{
					int x = RouteListX[^1];
					int y = RouteListY[^1];

					if (
						!PetDictionaryA.ContainsKey(x) || !PetDictionaryA[x].ContainsKey(y)
					)
					{
						InitListA();
						RouteListX.Clear();
						RouteListY.Clear();
						break;
					}

					{
						RouteListX.Add(RouteAx[PetDictionaryA[x][y]]);
						RouteListY.Add(RouteAy[PetDictionaryA[x][y]]);

						if (NA[PetDictionaryA[RouteListX[^1]][RouteListY[^1]]] == 0)
							break;
					}
				}
			}
		}

		private void AddListA(int x, int y, int status, int routeX, int routeY, double n)
		{
			if (!PetDictionaryA.ContainsKey(x))
				PetDictionaryA.Add(x, new Dictionary<int, int>());

			if (!PetDictionaryA[x].ContainsKey(y))
			{
				PetDictionaryAInv.Insert(StatusA.Count, new List<int>());
				PetDictionaryAInv[StatusA.Count].Add(x);
				PetDictionaryAInv[StatusA.Count].Add(y);

				PetDictionaryA[x][y] = StatusA.Count;

				Ax.Add(x);
				Ay.Add(y);
				StatusA.Add(status);
				RouteAx.Add(routeX);
				RouteAy.Add(routeY);
				NA.Add(n);
			}
		}

		public void InitListA()
		{
			Ax.Clear();
			Ay.Clear();
			StatusA.Clear();
			RouteAx.Clear();
			RouteAy.Clear();
			NA.Clear();

			StatusAIndex.Clear();

			PetDictionaryA.Clear();
			PetDictionaryAInv.Clear();

			make_statusAIndex();
		}

		public void RemoveListA(int x, int y)
		{
			if (!PetDictionaryA.ContainsKey(x))
				return;

			if (!PetDictionaryA[x].ContainsKey(y))
				return;

			int removeIndexNo = PetDictionaryA[x][y];
			int rowNumber = StatusA.Count;

			PetDictionaryAInv.RemoveAt(removeIndexNo);
			PetDictionaryA[x].Remove(y);
			for (int indexNo = removeIndexNo; indexNo < rowNumber - 1; indexNo++)
				PetDictionaryA[PetDictionaryAInv[indexNo][0]][PetDictionaryAInv[indexNo][1]]--;

			Ax.RemoveAt(removeIndexNo);
			Ay.RemoveAt(removeIndexNo);
			StatusA.RemoveAt(removeIndexNo);
			RouteAx.RemoveAt(removeIndexNo);
			RouteAy.RemoveAt(removeIndexNo);
			NA.RemoveAt(removeIndexNo);
		}

		public void RemoveListA(int index)
		{
			if (index >= PetDictionaryAInv.Count)
				return;

			int x = PetDictionaryAInv[index][0];
			int y = PetDictionaryAInv[index][1];

			RemoveListA(x, y);
		}

		public void make_statusAIndex()
		{
			StatusAIndex.Clear();
			int rowNo = 0;
			foreach (int status in StatusA)
			{
				if (!StatusAIndex.ContainsKey(status))
					StatusAIndex[status] = new List<int>();
				StatusAIndex[status].Add(rowNo);
				rowNo += 1;
			}
		}
	}
}
