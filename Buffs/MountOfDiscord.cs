using Terraria;
using Terraria.ModLoader;

namespace AutoStacker.Buffs
{
	public class MountOfDiscord : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("MountOfDiscord");
			Description.SetDefault("");
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.mount.SetMount(ModContent.MountType<Mounts.MountOfDiscord>(), player);
			player.buffTime[buffIndex] = 10;

			player.velocity.X = 0.0f;
			player.velocity.Y = 0.0000001f;

			const float speed = 16f;

			if (!player.releaseUp)
				player.position.Y -= speed;

			if (!player.releaseDown)
				player.position.Y += speed;

			if (!player.releaseRight)
			{
				player.position.X += speed;
				player.direction = 1;
			}

			if (!player.releaseLeft)
			{
				player.position.X -= speed;
				player.direction = -1;
			}
		}
	}
}
