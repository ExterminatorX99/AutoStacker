using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutoStacker.Mounts
{
	public class MountOfDiscord : ModMount
	{
		public override void SetStaticDefaults()
		{
			MountData.spawnDust = 226;
			MountData.spawnDustNoGravity = true;
			MountData.buff = ModContent.BuffType<Buffs.MountOfDiscord>();
			MountData.heightBoost = 0;
			MountData.flightTimeMax = int.MaxValue;
			MountData.fatigueMax = int.MaxValue;
			MountData.fallDamage = 0f;
			MountData.usesHover = true;
			MountData.runSpeed = 0;
			MountData.dashSpeed = 0;
			MountData.acceleration = 0;
			MountData.swimSpeed = 0;
			MountData.jumpHeight = 0;
			MountData.jumpSpeed = 0;
			MountData.totalFrames = 1;
			MountData.playerYOffsets = new int[MountData.totalFrames];
			MountData.xOffset = 0;
			MountData.bodyFrame = 5;
			MountData.yOffset = 0;
			MountData.playerHeadOffset = 18;
			MountData.standingFrameCount = 0;
			MountData.standingFrameDelay = 0;
			MountData.standingFrameStart = 0;
			MountData.runningFrameCount = 0;
			MountData.runningFrameDelay = 0;
			MountData.runningFrameStart = 0;
			MountData.flyingFrameCount = 0;
			MountData.flyingFrameDelay = 0;
			MountData.flyingFrameStart = 0;
			MountData.inAirFrameCount = 0;
			MountData.inAirFrameDelay = 0;
			MountData.inAirFrameStart = 0;
			MountData.idleFrameCount = 0;
			MountData.idleFrameDelay = 0;
			MountData.idleFrameStart = 0;
			MountData.idleFrameLoop = true;
			MountData.swimFrameCount = 0;
			MountData.swimFrameDelay = 0;
			MountData.swimFrameStart = 0;
			if (Main.netMode != NetmodeID.Server)
			{
				MountData.textureWidth = MountData.backTexture.Width();
				MountData.textureHeight = MountData.backTexture.Height();
			}
		}
	}
}
