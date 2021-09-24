using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace AutoStacker.Worlds
{
	public class MinionHouse : ModSystem
	{
		private Dictionary<int, Player> _minionHousePlayer = new();
		private Dictionary<int, int> _minionHousePlayerNo = new();
		private Dictionary<int, int> _minionHousePlayerAi = new();

		public void Init()
		{
			_minionHousePlayer = new Dictionary<int, Player>();
			_minionHousePlayerNo = new Dictionary<int, int>();
			_minionHousePlayerAi = new Dictionary<int, int>();
		}

		public override void PreUpdateWorld()
		{
			if (!Program.LoadedEverything)
				return;

			int minionHouseChestType = ModContent.TileType<Tiles.MinionHouse>();

			for (int chestNo = 0; chestNo < Main.chest.Length; chestNo++)
			{
				Chest chest = Main.chest[chestNo];
				if (
					chest == null ||
					Main.tile[chest.x, chest.y] == null ||
					!Main.tile[chest.x, chest.y].IsActive ||
					Main.tile[chest.x, chest.y].type != minionHouseChestType
				)
					continue;

				//spawn player
				if (!_minionHousePlayer.ContainsKey(chestNo))
					for (int playerNo = 0; playerNo < Main.player.Length; playerNo++)
						if (Main.player[playerNo] == null || !Main.player[playerNo].active)
						{
							Main.player[playerNo] = (Player)Main.player[Main.myPlayer].clientClone();

							_minionHousePlayer[chestNo] = Main.player[playerNo];
							_minionHousePlayerNo[chestNo] = playerNo;
							_minionHousePlayerAi[chestNo] = 0;

							Main.player[playerNo].active = true;
							Main.player[playerNo].dead = false;
							Main.player[playerNo].nearbyActiveNPCs = 1f;
							Main.player[playerNo].townNPCs = 1f;
							Main.player[playerNo].name = "Minion House Keeper";
							Main.player[playerNo].velocity.X = 0f;
							Main.player[playerNo].velocity.Y = 0f;
							Main.player[playerNo].releaseDown = false;
							Main.player[playerNo].releaseRight = false;
							Main.player[playerNo].releaseHook = false;
							Main.player[playerNo].releaseInventory = false;
							Main.player[playerNo].releaseJump = false;
							Main.player[playerNo].releaseLeft = false;
							Main.player[playerNo].releaseMapFullscreen = false;
							Main.player[playerNo].releaseMapStyle = false;
							Main.player[playerNo].releaseMount = false;
							Main.player[playerNo].releaseQuickHeal = false;
							Main.player[playerNo].releaseQuickMana = false;
							Main.player[playerNo].releaseRight = false;
							Main.player[playerNo].releaseSmart = false;
							Main.player[playerNo].releaseThrow = false;
							Main.player[playerNo].releaseUp = false;
							Main.player[playerNo].releaseUseItem = false;
							Main.player[playerNo].releaseUseTile = false;
							Main.player[playerNo].selectedItem = 0;

							Main.player[playerNo].position.X = chest.x * 16 - 16 * 2;
							Main.player[playerNo].position.Y = chest.y * 16 - 16 * 4;

							break;
						}

				//use item
				if (_minionHousePlayerAi[chestNo] > 0)
				{
					_minionHousePlayerAi[chestNo] -= 1;
					continue;
				}

				foreach (Item item in chest.item)
				{
					if (item == null || item.IsAir || !item.CountsAsClass(DamageClass.Summon))
						continue;

					_minionHousePlayerAi[chestNo] = 2 * 1000;

					int myPlayer = Main.myPlayer;
					Main.myPlayer = _minionHousePlayerNo[chestNo];

					Item playerItem = _minionHousePlayer[chestNo].inventory[0].Clone();
					_minionHousePlayer[chestNo].inventory[0] = item.Clone();

					_minionHousePlayer[chestNo].controlUseItem = true;
					_minionHousePlayer[chestNo].releaseUseItem = true;
					_minionHousePlayer[chestNo].ItemCheck(_minionHousePlayerNo[chestNo]);
					_minionHousePlayer[chestNo].controlUseItem = false;
					_minionHousePlayer[chestNo].releaseUseItem = false;

					Main.myPlayer = myPlayer;
				}

				_minionHousePlayer[chestNo].controlUseItem = false;
				_minionHousePlayer[chestNo].releaseUseItem = false;
			}


			//take damage
			for (int projectileNo = 0; projectileNo < Main.maxProjectiles; projectileNo++)
			{
				Projectile projectile = Main.projectile[projectileNo];

				if (!_minionHousePlayerNo.ContainsValue(projectile.owner))
					continue;
				Player player = Main.player[projectile.owner];

				for (int npcNo = 0; npcNo < Main.maxNPCs; npcNo++)
				{
					NPC npc = Main.npc[npcNo];
					if (projectile.position.X + projectile.width > npc.position.X &&
						projectile.position.X < npc.position.X + npc.width &&
						projectile.position.Y + projectile.height > npc.position.Y &&
						projectile.position.Y < npc.position.Y + npc.height &&
						!npc.friendly)
						npc.StrikeNPC(projectile.damage, projectile.knockBack, (int)projectile.rotation);
				}
			}

			//check destroyed chest -> player delete
			if (_minionHousePlayer.Keys.Any(chestNo => Main.chest[chestNo] == null))
			{
				foreach (int chestNo2 in _minionHousePlayer.Keys)
				{
					Main.player[_minionHousePlayerNo[chestNo2]] = new Player();
					Main.player[_minionHousePlayerNo[chestNo2]].active = false;
				}

				Init();
			}
		}
	}
}
