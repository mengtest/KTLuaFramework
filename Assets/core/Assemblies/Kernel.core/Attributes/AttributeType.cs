
namespace Kernel.core
{

	public enum AttributeType
	{
		[Comment("无")] NONE = 0,
		[Comment("攻击")] ATTACK,
		[Comment("防御")] DEFENCE,
		[Comment("生命")] HP,
		[Comment("伤害加深")] DAMAGE_ADD,
		[Comment("伤害减免")] DAMAGE_REDUCE,

		[Comment("对羽加深")] PLUME_ADD,
		[Comment("对甲加深")] ARMOR_ADD,
		[Comment("对鳞加深")] SQUAMA_ADD,
		[Comment("对尾加深")] TAIL_ADD,
		[Comment("受羽减免")] PLUME_REDUCE,
		[Comment("受甲减免")] ARMOR_REDUCE,
		[Comment("受鳞减免")] SQUAMA_REDUCE,
		[Comment("受尾减免")] TAIL_REDUCE,
		[Comment("对攻击型加深")] ATTACK_ADD,
		[Comment("对防御型加深")] DEFENCE_ADD,
		[Comment("对辅助型加深")] SUP_ADD,
		[Comment("受攻击型减免")] ATTACK_REDUCE,
		[Comment("受防御型减免")] DEFENCE_REDUCE,
		[Comment("受辅助型减免")] SUP_REDUCE,
		[Comment("对远程加深")] RANGED_ADD,
		[Comment("对近战加深")] MELEE_ADD,
		[Comment("受远程减免")] RANGED_REDUCE,
		[Comment("受近战减免")] MELEE_REDUCE,
		
		[Comment("普攻加深")] ATTACK_DAMAGE_ADD,
		[Comment("技能加深")] SKILL_DAMAGE_ADD,
		[Comment("大招加深")] RAGE_SKILL_DAMAGE_ADD,
		[Comment("普攻减免")] ATTACK_DAMAGE_REDUCE,
		[Comment("技能减免")] SKILL_DAMAGE_REDUCE,
		[Comment("大招减免")] RAGE_SKILL_DAMAGE_REDUCE,
		[Comment("自损加深")] SELF_LOSE_HP_DAMAGE_ADD,
		[Comment("自损减免")] SELF_LOSE_HP_DAMAGE_REDUCE,
		[Comment("敌损加深")] ENEMY_LOSE_HP_DAMAGE_ADD,
		[Comment("距离加深")] DISTANCE_DAMAGE_ADD,
		[Comment("距离减免")] DISTANCE_DAMAGE_REDUCE,
		[Comment("额外加深")] EXTRA_DAMAGE_ADD,
		[Comment("额外减免")] EXTRA_DAMAGE_REDUCE,

		[Comment("环境连乘加深")] ENVIRONMENT_MULTIPLE_ADD,
		[Comment("环境连乘减免")] ENVIRONMENT_MULTIPLE_REDUCE,

		[Comment("定位伤害加深")] CAREER_DAMAGE_ADD,
		[Comment("定位伤害减免")] CAREER_DAMAGE_REDUCE,

		[Comment("无战力加深")] NFIGHTPOWER_DAMAGE_ADD,
		[Comment("无战力减免")] NFIGHTPOWER_DAMAGE_REDUCE,
		[Comment("额外连乘加深")] EXTRA_MULTIPLE_ADD,
		[Comment("额外连乘减免")] EXTRA_MULTIPLE_REDUCE,


		[Comment("场上回血")] FIGHT_HP_RECOVER,
		[Comment("场下回血")] REST_HP_RECOVER,
		[Comment("治疗效果")] HEAL,
		[Comment("被治疗效果")] HEALED,
		[Comment("初始怒气")] RAGE,
		[Comment("受伤总怒气")] HURT_HP_RAGE_RECOVER_RATE,
		[Comment("场上回怒")] FIGHT_RAGE_RECOVER,
		//[Comment("场下回怒--废弃")] REST_RAGE_RECOVER,//废弃
		[Comment("死亡掉落怒气")] DEAD_RAGE,
		//[Comment("放大留怒--废弃")] RAGE_COST_RATE,//废弃
		//[Comment("击杀额外回怒率--废弃")] KILL_RAGE_RECOVER_RATE,//废弃
		[Comment("技能额外回怒率")] SKILL_RAGE_RECOVER_RATE,
		[Comment("受伤额外回怒率")] HURT_RAGE_RECOVER_RATE,
		[Comment("闪避率")] MISS_RATE,
		[Comment("命中率")] HIT_RATE,
		[Comment("暴击率")] CRITICAL_RATE,
		[Comment("抗暴率")] CRITICAL_RESIST_RATE,
		[Comment("额外闪避率")] EXTRA_MISS_RATE,
		[Comment("额外命中率")] EXTRA_HIT_RATE,
		[Comment("额外暴击率")] EXTRA_CRITICAL_RATE,
		[Comment("额外抗暴率")] EXTRA_CRITICAL_RESIST_RATE,
		[Comment("暴击伤害")] CRITICAL_DAMAGE,
		[Comment("抗暴伤害")] CRITICAL_RESIST_DAMAGE,
		[Comment("效果命中")] EFFECT_HIT,
		[Comment("效果抵抗")] EFFECT_RESIST,
		[Comment("吸血率")] SUCK_RATE,
		[Comment("输血率")] TRANSFUSION_RATE,
		[Comment("反弹率")] ANTI_RATE,
		
		[Comment("效果时长增加")] EFFECT_TIMER_ADD,
		[Comment("效果时长减少")] EFFECT_TIMER_REDUCE,

		[Comment("每段附加伤害")] SKILL_BLOCK_DAMAGE,
		[Comment("每段格挡伤害")] SKILL_BLOCK_BLOCK_DAMAGE,
		[Comment("强韧值")] TOUGH_MAX_VALUE,
		[Comment("强韧冷却速度")] TOUGH_CD_RECOVER_SPEED,
		[Comment("强韧冷却初始值")] TOUGH_CD_INITIAL_VALUE,
		[Comment("强韧恢复速度")] TOUGH_RECOVER_SPEED,
		[Comment("破强韧百分比")] TOUGH_IGNORE_RATE,
		[Comment("抗破强韧百分比")] TOUGH_CRITICAL_IGNORE_RATE,
		[Comment("无视防御固定值")] IGNORE_DEFENCE,
		[Comment("无视防御百分比")] IGNORE_DEFENCE_RATE,
		[Comment("无视防御概率")] IGNORE_ALL_DEFENCE_RATE,
		[Comment("攻击速度")] ATTACK_SPEED,
		[Comment("冷却缩减")] COOLDOWN_REDUCE_RATE,
		
		[Comment("战斗移动速度")] FIGHT_MOVE_SPEED,

		[Comment("掠夺怒气")] PILLAGE_RAGE_RECOVER_RATE,

		[Comment("背后加深伤害")] ROLE_BACK_DAMAGE_ADD,
		[Comment("背后减免伤害")] ROLE_BACK_DAMAGE_REDUCE,
		[Comment("两侧加深伤害")] ROLE_SIDE_DAMAGE_ADD,
		[Comment("两侧减免伤害")] ROLE_SIDE_DAMAGE_REDUCE,
		[Comment("前方加深伤害")] ROLE_FRONT_DAMAGE_ADD,
		[Comment("前方减免伤害")] ROLE_FRONT_DAMAGE_REDUCE,
		//按顺序往后添加，千万不要在中间插入枚举值
		COUNT//总在最后
	}
}