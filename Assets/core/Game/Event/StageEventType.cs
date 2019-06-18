namespace Alche.FightCore
{
	public enum StageEventType
	{
		NONE,
		FIRST_COMPLETE_STAGE,

		//怪物的
		MONSTER_SPAWN,
		MONSTER_DEAD,
		MONSTER_DISAPPEAR,
        MONSTER_SHOWINFO,

		DAMAGE,

		//队伍的
		SWITCH_TEAM,
		SWITCH_AUTO_ATTACK,
		SWITCH_LEADER,
		//IS_FIGHT,

		//Character的
		HEAL,

		//英雄的
		HERO_DEAD,
		HERO_SPAWN,
		HERO_DISPOSE,
		HERO_SWITCH_FIGHT,
		HERO_LEAVE_FIGHT,
		CHARACTER_CHANGE_PRESENT,//Present是外部，发出这个事件的时候，模型已经改变
		CHARACTER_CHANGE_SKIN,//Skin是core里的，发出这个事件的时候，模型还没有变
		RAGE_SKILL_CASTED,

		//一次战斗的
		BATTLE_START,
		BATTLE_WIN,
		BATTLE_LOSE,
		BATTLE_STAR,

		CAST_SKILL,
		WEAKNESS_OPEN,
		WEAKNESS_CLOSE,
		MONSTER_NOTICE_SHOW,
		MONSTER_NOTICE_HIDE,
		MONSTER_INDICATOR_SHOW,
		MONSTER_INDICATOR_HIDE,
		MONSTER_DROP_GOLD,
		CHARACTER_COLLIDE_SIDES,
		SHOW_FIGHT_TIP,
		PICKUP_LOOT,
		ON_INTERACT,
		ON_TASK_INTERACT_GADGET,
		PLAYER_NOTIFY_MOVE_COMPLETE,
		STAGE_COMPLETED_CHANGED,
		HOST_PLAYER_DEAD,
		FIGHT_STATE_CHANGED,
		ADD_BUFF,
		STAGE_FINALIZED,
		STAGE_INITED,
		ARENA_ATTACKER_GIVE_UP,
		ARENA_ATTACKER_SKIP,
		RESIST,
		HOST_PLAYER_SELECT_SHOW_BOSS_BLOOD_TARGET,
		HOST_PLAYER_MOVE_TO_TARGET_TO_CAST_SKILL,
		NEW_MONSTER_SHOW,
		NEW_AFFIX_SHOW,
		//-----------------//
		SUCK_DAMAGE,
		STAGE_MISSION_CHANGED,
		VAMPIRISM,
		MONSTER_ENTER_FIGHT,
		FIGHT_COUNTDOWN_INCREASE,
		FIGHT_WAVE_CHANGE,
		FIGHT_WAVE_INCREASE,
		WEAKNESS_DAMAGE,
		WEAKNESS_BREAK,
		HOST_PLAYER_FIRST_MOVE,
		ON_BUFF_STATUS_SHOW,
		ON_BUFF_STATUS_HIDE,
		HOST_PLAYER_CHANGE_AVATAR,
		WORMHOLE_UNIT_SPAWN,
		WORMHOLE_UNIT_DESTROY,
		MAIN_CITY_CHANGE_STAGE,
		HOST_PLAYER_ADD_PASSIVE_SKILL,
		PASSIVE_SKILL_TRIGGERED,
		CHANGE_SKILL,
		FROZE_TO_DEATH,
		CHANGE_HP,
		SKILL_INTERRUPTED,
		SKILL_FINISHED,

		WORLD_BOSS_OTHER_PLAYER_ENTER,
		//AI节点角色信息改变
		CHARACTER_DATA_CHANGED,

		WARM_UP_SUBSTITUTE_LEADER,
		RANKING_LIST_CHANGED,
		CREATE_SKILL,
		TRY_TO_ADD_STAGE_BUFF,
		REMOVE_BUFF,

		HOST_PLAYER_SKIN_NEED_CHANGE,//主角变更模型
		CHARACTER_SKIN_CHANGED,

		PEAK_ONE_BATTLE_FINISHED,
		MONSTER_ACTIVE,
		MONSTER_DEACTIVE,
		SHOW_PET_SKILL,

		//物件的那堆消息
		GADGET_SPAWN,
		GADGET_DEAD,
		GADGET_OPEN_SUCCESS,
		GADGET_OPEN_START,
		GADGET_OPEN_INTERRUPT,
		GADGET_CHANGE_STATE,
		GADGET_CLICK,

		//炼金物件的那堆消息
		ALCHEMIC_TRIGGER_ENTER,
		ALCHEMIC_TRIGGER_EXIST,
		ALCHEMIC_TRIGGERED,
		ALCHEMIC_ENERGY_FULL,

		//NPC的
		NPC_SPAWN,
		NPC_CLICK_ONMODEL,
		NPC_AUTOSHOUT,
		NPC_BEGINTALK,
		NPC_TASK_SHOW,
		NPC_SHOW,
		SHOW_STARINFO,

		GUIDE_CHECK,

		PLAY_SCREEN_ANIMATION,//播放切屏动画

		CHOOSE_PLOT_OPTION,
		COMMAND_PLOT,//指挥官对话战斗内用
		NPC_AUTOSHOUTFINISH,

		PLAYER_GET_GOLD,

		MONSTER_CLICK_ONMODEL, // 怪物被单击事件

		//关卡任务
		STAGE_TASK_CREATE,
		STAGE_TASK_PROGRESS_CHANGE,
		STAGE_TASK_FINISH,

		//主操作英雄进入区域
		HOST_PLAYER_ENTER_AREA,
		HOST_PLAYER_EXIST_AREA,
		AREA_SHOW,

		TOGGLE_BOSS_WARNING,
		TOGGLE_TIMESPACE_AMBIENT,
		

		SHOWISNAVIGATION,
		HIDEISNAVIGATION,

		//称号
		SHOWTITLE,
		HIDETITLE,

		//UI
		SCENEWINDOWSHOW,
		SCENEWINDOWHIDE,
		REGENERATIONTOSCALEVAL,//再生血量恢复到某个刻度值

		//
		SHOW_BUFF_TIP,

		//触发器
		TRANSFER_STAGE_TRIGGER_ACTION,
        STAGE_SCENE_EFFECT,

		//AI
		TRANSFER_AI_ACTION,

		//相机
		TRANSFER_SMART_CAMERA_ACTION,
        FIXED_CAMERA_SWITCH_POINT,//定点相机切换点
        FIXED_CAMERA_OPEN,//打开定点相机开关

		//大招立绘
		FIGHTRAGESKILLIMAGE,

		//觉醒释放或结束
		AWAKING_START_OR_END,
    }
}