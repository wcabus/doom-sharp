using DoomSharp.Core.Sound;

namespace DoomSharp.Core.GameLogic;

public struct MapObjectInfo
{
    private static readonly List<MapObjectInfo> _predefinedTypes = new();

    static MapObjectInfo()
    {
        AddPredefinedTypes();
    }

    public static MapObjectInfo? Find(int type)
    {
        var query = _predefinedTypes.Where(x => x.DoomedNum == type);
        if (query.Any())
        {
            return query.Single();
        }

        return null;
    }

    public MapObjectType Type { get; set; }

    public int DoomedNum { get; set; }
    public StateNum SpawnState { get; set; }
    public int SpawnHealth { get; set; }
    public StateNum SeeState { get; set; }
    public SoundType SeeSound { get; set; }
    public int ReactionTime { get; set; }
    public SoundType AttackSound { get; set; }
    public StateNum PainState { get; set; }
    public int PainChance { get; set; }
    public SoundType PainSound { get; set; }
    public StateNum MeleeState { get; set; }
    public StateNum MissileState { get; set; }
    public StateNum DeathState { get; set; }
    public StateNum XDeathState { get; set; }
    public SoundType DeathSound { get; set; }
    public int Speed { get; set; }
    public int Radius { get; set; }
    public int Height { get; set; }
    public int Mass { get; set; }
    public int Damage { get; set; }
    public SoundType ActiveSound { get; set; }
    public MapObjectFlag Flags { get; set; }
    public StateNum RaiseState { get; set; }

    private static void AddPredefinedTypes()
    {
        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_PLAYER,
            DoomedNum = -1,
            SpawnState = StateNum.S_PLAY,
            SpawnHealth = 100,
            SeeState = StateNum.S_PLAY_RUN1,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 0,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_PLAY_PAIN,
            PainChance = 255,
            PainSound = SoundType.sfx_plpain,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_PLAY_ATK1,
            DeathState = StateNum.S_PLAY_DIE1,
            XDeathState = StateNum.S_PLAY_XDIE1,
            DeathSound = SoundType.sfx_pldeth,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_PICKUP | MapObjectFlag.MF_NOTDMATCH,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_POSSESSED,
            DoomedNum = 3004,
            SpawnState = StateNum.S_POSS_STND,
            SpawnHealth = 20,
            SeeState = StateNum.S_POSS_RUN1,
            SeeSound = SoundType.sfx_posit1,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_pistol,
            PainState = StateNum.S_POSS_PAIN,
            PainChance = 200,
            PainSound = SoundType.sfx_popain,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_POSS_ATK1,
            DeathState = StateNum.S_POSS_DIE1,
            XDeathState = StateNum.S_POSS_XDIE1,
            DeathSound = SoundType.sfx_podth1,
            Speed = 8,
            Radius = 20 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_posact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_POSS_RAISE1
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_SHOTGUY,
            DoomedNum = 9,
            SpawnState = StateNum.S_SPOS_STND,
            SpawnHealth = 30,
            SeeState = StateNum.S_SPOS_RUN1,
            SeeSound = SoundType.sfx_posit2,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_SPOS_PAIN,
            PainChance = 170,
            PainSound = SoundType.sfx_popain,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_SPOS_ATK1,
            DeathState = StateNum.S_SPOS_DIE1,
            XDeathState = StateNum.S_SPOS_XDIE1,
            DeathSound = SoundType.sfx_podth2,
            Speed = 8,
            Radius = 20 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_posact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_SPOS_RAISE1
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_VILE,
            DoomedNum = 64,
            SpawnState = StateNum.S_VILE_STND,
            SpawnHealth = 700,
            SeeState = StateNum.S_VILE_RUN1,
            SeeSound = SoundType.sfx_vilsit,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_VILE_PAIN,
            PainChance = 10,
            PainSound = SoundType.sfx_vipain,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_VILE_ATK1,
            DeathState = StateNum.S_VILE_DIE1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_vildth,
            Speed = 15,
            Radius = 20 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 500,
            Damage = 0,
            ActiveSound = SoundType.sfx_vilact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_FIRE,
            DoomedNum = -1,
            SpawnState = StateNum.S_FIRE1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_UNDEAD,
            DoomedNum = 66,
            SpawnState = StateNum.S_SKEL_STND,
            SpawnHealth = 300,
            SeeState = StateNum.S_SKEL_RUN1,
            SeeSound = SoundType.sfx_skesit,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_SKEL_PAIN,
            PainChance = 100,
            PainSound = SoundType.sfx_popain,
            MeleeState = StateNum.S_SKEL_FIST1,
            MissileState = StateNum.S_SKEL_MISS1,
            DeathState = StateNum.S_SKEL_DIE1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_skedth,
            Speed = 10,
            Radius = 20 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 500,
            Damage = 0,
            ActiveSound = SoundType.sfx_skeact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_SKEL_RAISE1
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_TRACER,
            DoomedNum = -1,
            SpawnState = StateNum.S_TRACER,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_skeatk,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_TRACEEXP1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_barexp,
            Speed = 10 * Constants.FracUnit,
            Radius = 11 * Constants.FracUnit,
            Height = 8 * Constants.FracUnit,
            Mass = 100,
            Damage = 10,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_MISSILE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_SMOKE,
            DoomedNum = -1,
            SpawnState = StateNum.S_SMOKE1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_FATSO,
            DoomedNum = 67,
            SpawnState = StateNum.S_FATT_STND,
            SpawnHealth = 600,
            SeeState = StateNum.S_FATT_RUN1,
            SeeSound = SoundType.sfx_mansit,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_FATT_PAIN,
            PainChance = 80,
            PainSound = SoundType.sfx_mnpain,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_FATT_ATK1,
            DeathState = StateNum.S_FATT_DIE1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_mandth,
            Speed = 8,
            Radius = 48 * Constants.FracUnit,
            Height = 64 * Constants.FracUnit,
            Mass = 1000,
            Damage = 0,
            ActiveSound = SoundType.sfx_posact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_FATT_RAISE1
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_FATSHOT,
            DoomedNum = -1,
            SpawnState = StateNum.S_FATSHOT1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_firsht,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_FATSHOTX1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 20 * Constants.FracUnit,
            Radius = 6 * Constants.FracUnit,
            Height = 8 * Constants.FracUnit,
            Mass = 100,
            Damage = 8,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_MISSILE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_CHAINGUY,
            DoomedNum = 65,
            SpawnState = StateNum.S_CPOS_STND,
            SpawnHealth = 70,
            SeeState = StateNum.S_CPOS_RUN1,
            SeeSound = SoundType.sfx_posit2,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_CPOS_PAIN,
            PainChance = 170,
            PainSound = SoundType.sfx_popain,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_CPOS_ATK1,
            DeathState = StateNum.S_CPOS_DIE1,
            XDeathState = StateNum.S_CPOS_XDIE1,
            DeathSound = SoundType.sfx_podth2,
            Speed = 8,
            Radius = 20 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_posact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_CPOS_RAISE1
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_TROOP,
            DoomedNum = 3001,
            SpawnState = StateNum.S_TROO_STND,
            SpawnHealth = 60,
            SeeState = StateNum.S_TROO_RUN1,
            SeeSound = SoundType.sfx_bgsit1,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_TROO_PAIN,
            PainChance = 200,
            PainSound = SoundType.sfx_popain,
            MeleeState = StateNum.S_TROO_ATK1,
            MissileState = StateNum.S_TROO_ATK1,
            DeathState = StateNum.S_TROO_DIE1,
            XDeathState = StateNum.S_TROO_XDIE1,
            DeathSound = SoundType.sfx_bgdth1,
            Speed = 8,
            Radius = 20 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_bgact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_TROO_RAISE1
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_SERGEANT,
            DoomedNum = 3002,
            SpawnState = StateNum.S_SARG_STND,
            SpawnHealth = 150,
            SeeState = StateNum.S_SARG_RUN1,
            SeeSound = SoundType.sfx_sgtsit,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_sgtatk,
            PainState = StateNum.S_SARG_PAIN,
            PainChance = 180,
            PainSound = SoundType.sfx_dmpain,
            MeleeState = StateNum.S_SARG_ATK1,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_SARG_DIE1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_sgtdth,
            Speed = 10,
            Radius = 30 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 400,
            Damage = 0,
            ActiveSound = SoundType.sfx_dmact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_SARG_RAISE1
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_SHADOWS,
            DoomedNum = 58,
            SpawnState = StateNum.S_SARG_STND,
            SpawnHealth = 150,
            SeeState = StateNum.S_SARG_RUN1,
            SeeSound = SoundType.sfx_sgtsit,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_sgtatk,
            PainState = StateNum.S_SARG_PAIN,
            PainChance = 180,
            PainSound = SoundType.sfx_dmpain,
            MeleeState = StateNum.S_SARG_ATK1,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_SARG_DIE1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_sgtdth,
            Speed = 10,
            Radius = 30 * Constants.FracUnit,
            Height = 56 * Constants.FracUnit,
            Mass = 400,
            Damage = 0,
            ActiveSound = SoundType.sfx_dmact,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_SHADOW | MapObjectFlag.MF_COUNTKILL,
            RaiseState = StateNum.S_SARG_RAISE1
        });

        /*
         {		// MT_HEAD
	3005,		// doomednum
	S_HEAD_STND,		// spawnstate
	400,		// spawnhealth
	S_HEAD_RUN1,		// seestate
	sfx_cacsit,		// seesound
	8,		// reactiontime
	0,		// attacksound
	S_HEAD_PAIN,		// painstate
	128,		// painchance
	sfx_dmpain,		// painsound
	0,		// meleestate
	S_HEAD_ATK1,		// missilestate
	S_HEAD_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_cacdth,		// deathsound
	8,		// speed
	31*FRACUNIT,		// radius
	56*FRACUNIT,		// height
	400,		// mass
	0,		// damage
	sfx_dmact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_FLOAT|MF_NOGRAVITY|MF_COUNTKILL,		// flags
	S_HEAD_RAISE1		// raisestate
    },

    {		// MT_BRUISER
	3003,		// doomednum
	S_BOSS_STND,		// spawnstate
	1000,		// spawnhealth
	S_BOSS_RUN1,		// seestate
	sfx_brssit,		// seesound
	8,		// reactiontime
	0,		// attacksound
	S_BOSS_PAIN,		// painstate
	50,		// painchance
	sfx_dmpain,		// painsound
	S_BOSS_ATK1,		// meleestate
	S_BOSS_ATK1,		// missilestate
	S_BOSS_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_brsdth,		// deathsound
	8,		// speed
	24*FRACUNIT,		// radius
	64*FRACUNIT,		// height
	1000,		// mass
	0,		// damage
	sfx_dmact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_COUNTKILL,		// flags
	S_BOSS_RAISE1		// raisestate
    },

    {		// MT_BRUISERSHOT
	-1,		// doomednum
	S_BRBALL1,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_firsht,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_BRBALLX1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_firxpl,		// deathsound
	15*FRACUNIT,		// speed
	6*FRACUNIT,		// radius
	8*FRACUNIT,		// height
	100,		// mass
	8,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP|MF_MISSILE|MF_DROPOFF|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

    {		// MT_KNIGHT
	69,		// doomednum
	S_BOS2_STND,		// spawnstate
	500,		// spawnhealth
	S_BOS2_RUN1,		// seestate
	sfx_kntsit,		// seesound
	8,		// reactiontime
	0,		// attacksound
	S_BOS2_PAIN,		// painstate
	50,		// painchance
	sfx_dmpain,		// painsound
	S_BOS2_ATK1,		// meleestate
	S_BOS2_ATK1,		// missilestate
	S_BOS2_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_kntdth,		// deathsound
	8,		// speed
	24*FRACUNIT,		// radius
	64*FRACUNIT,		// height
	1000,		// mass
	0,		// damage
	sfx_dmact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_COUNTKILL,		// flags
	S_BOS2_RAISE1		// raisestate
    },

    {		// MT_SKULL
	3006,		// doomednum
	S_SKULL_STND,		// spawnstate
	100,		// spawnhealth
	S_SKULL_RUN1,		// seestate
	0,		// seesound
	8,		// reactiontime
	sfx_sklatk,		// attacksound
	S_SKULL_PAIN,		// painstate
	256,		// painchance
	sfx_dmpain,		// painsound
	0,		// meleestate
	S_SKULL_ATK1,		// missilestate
	S_SKULL_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_firxpl,		// deathsound
	8,		// speed
	16*FRACUNIT,		// radius
	56*FRACUNIT,		// height
	50,		// mass
	3,		// damage
	sfx_dmact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_FLOAT|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

    {		// MT_SPIDER
	7,		// doomednum
	S_SPID_STND,		// spawnstate
	3000,		// spawnhealth
	S_SPID_RUN1,		// seestate
	sfx_spisit,		// seesound
	8,		// reactiontime
	sfx_shotgn,		// attacksound
	S_SPID_PAIN,		// painstate
	40,		// painchance
	sfx_dmpain,		// painsound
	0,		// meleestate
	S_SPID_ATK1,		// missilestate
	S_SPID_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_spidth,		// deathsound
	12,		// speed
	128*FRACUNIT,		// radius
	100*FRACUNIT,		// height
	1000,		// mass
	0,		// damage
	sfx_dmact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_COUNTKILL,		// flags
	S_NULL		// raisestate
    },

    {		// MT_BABY
	68,		// doomednum
	S_BSPI_STND,		// spawnstate
	500,		// spawnhealth
	S_BSPI_SIGHT,		// seestate
	sfx_bspsit,		// seesound
	8,		// reactiontime
	0,		// attacksound
	S_BSPI_PAIN,		// painstate
	128,		// painchance
	sfx_dmpain,		// painsound
	0,		// meleestate
	S_BSPI_ATK1,		// missilestate
	S_BSPI_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_bspdth,		// deathsound
	12,		// speed
	64*FRACUNIT,		// radius
	64*FRACUNIT,		// height
	600,		// mass
	0,		// damage
	sfx_bspact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_COUNTKILL,		// flags
	S_BSPI_RAISE1		// raisestate
    },

    {		// MT_CYBORG
	16,		// doomednum
	S_CYBER_STND,		// spawnstate
	4000,		// spawnhealth
	S_CYBER_RUN1,		// seestate
	sfx_cybsit,		// seesound
	8,		// reactiontime
	0,		// attacksound
	S_CYBER_PAIN,		// painstate
	20,		// painchance
	sfx_dmpain,		// painsound
	0,		// meleestate
	S_CYBER_ATK1,		// missilestate
	S_CYBER_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_cybdth,		// deathsound
	16,		// speed
	40*FRACUNIT,		// radius
	110*FRACUNIT,		// height
	1000,		// mass
	0,		// damage
	sfx_dmact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_COUNTKILL,		// flags
	S_NULL		// raisestate
    },

    {		// MT_PAIN
	71,		// doomednum
	S_PAIN_STND,		// spawnstate
	400,		// spawnhealth
	S_PAIN_RUN1,		// seestate
	sfx_pesit,		// seesound
	8,		// reactiontime
	0,		// attacksound
	S_PAIN_PAIN,		// painstate
	128,		// painchance
	sfx_pepain,		// painsound
	0,		// meleestate
	S_PAIN_ATK1,		// missilestate
	S_PAIN_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_pedth,		// deathsound
	8,		// speed
	31*FRACUNIT,		// radius
	56*FRACUNIT,		// height
	400,		// mass
	0,		// damage
	sfx_dmact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_FLOAT|MF_NOGRAVITY|MF_COUNTKILL,		// flags
	S_PAIN_RAISE1		// raisestate
    },

    {		// MT_WOLFSS
	84,		// doomednum
	S_SSWV_STND,		// spawnstate
	50,		// spawnhealth
	S_SSWV_RUN1,		// seestate
	sfx_sssit,		// seesound
	8,		// reactiontime
	0,		// attacksound
	S_SSWV_PAIN,		// painstate
	170,		// painchance
	sfx_popain,		// painsound
	0,		// meleestate
	S_SSWV_ATK1,		// missilestate
	S_SSWV_DIE1,		// deathstate
	S_SSWV_XDIE1,		// xdeathstate
	sfx_ssdth,		// deathsound
	8,		// speed
	20*FRACUNIT,		// radius
	56*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_posact,		// activesound
	MF_SOLID|MF_SHOOTABLE|MF_COUNTKILL,		// flags
	S_SSWV_RAISE1		// raisestate
    },

    {		// MT_KEEN
	72,		// doomednum
	S_KEENSTND,		// spawnstate
	100,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_KEENPAIN,		// painstate
	256,		// painchance
	sfx_keenpn,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_COMMKEEN,		// deathstate
	S_NULL,		// xdeathstate
	sfx_keendt,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	72*FRACUNIT,		// height
	10000000,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID|MF_SPAWNCEILING|MF_NOGRAVITY|MF_SHOOTABLE|MF_COUNTKILL,		// flags
	S_NULL		// raisestate
    },

    {		// MT_BOSSBRAIN
	88,		// doomednum
	S_BRAIN,		// spawnstate
	250,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_BRAIN_PAIN,		// painstate
	255,		// painchance
	sfx_bospn,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_BRAIN_DIE1,		// deathstate
	S_NULL,		// xdeathstate
	sfx_bosdth,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	10000000,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID|MF_SHOOTABLE,		// flags
	S_NULL		// raisestate
    },

    {		// MT_BOSSSPIT
	89,		// doomednum
	S_BRAINEYE,		// spawnstate
	1000,		// spawnhealth
	S_BRAINEYESEE,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	32*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP|MF_NOSECTOR,		// flags
	S_NULL		// raisestate
    },

    {		// MT_BOSSTARGET
	87,		// doomednum
	S_NULL,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	32*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP|MF_NOSECTOR,		// flags
	S_NULL		// raisestate
    },

    {		// MT_SPAWNSHOT
	-1,		// doomednum
	S_SPAWN1,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_bospit,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_firxpl,		// deathsound
	10*FRACUNIT,		// speed
	6*FRACUNIT,		// radius
	32*FRACUNIT,		// height
	100,		// mass
	3,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP|MF_MISSILE|MF_DROPOFF|MF_NOGRAVITY|MF_NOCLIP,		// flags
	S_NULL		// raisestate
    },

    {		// MT_SPAWNFIRE
	-1,		// doomednum
	S_SPAWNFIRE1,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

         */

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_BARREL,
            DoomedNum = 2035,
            SpawnState = StateNum.S_BAR1,
            SpawnHealth = 20,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_BEXP,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_barexp,
            Speed = 0,
            Radius = 10 * Constants.FracUnit,
            Height = 42 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_NOBLOOD,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_TROOPSHOT,
            DoomedNum = -1,
            SpawnState = StateNum.S_TBALL1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_firsht,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_TBALLX1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 10 * Constants.FracUnit,
            Radius = 6 * Constants.FracUnit,
            Height = 8 * Constants.FracUnit,
            Mass = 100,
            Damage = 3,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_MISSILE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_HEADSHOT,
            DoomedNum = -1,
            SpawnState = StateNum.S_RBALL1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_firsht,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_RBALLX1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 10 * Constants.FracUnit,
            Radius = 6 * Constants.FracUnit,
            Height = 8 * Constants.FracUnit,
            Mass = 100,
            Damage = 5,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_MISSILE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_ROCKET,
            DoomedNum = -1,
            SpawnState = StateNum.S_ROCKET,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_rlaunc,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_EXPLODE1,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_barexp,
            Speed = 20 * Constants.FracUnit,
            Radius = 11 * Constants.FracUnit,
            Height = 8 * Constants.FracUnit,
            Mass = 100,
            Damage = 20,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_MISSILE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_PLASMA,
            DoomedNum = -1,
            SpawnState = StateNum.S_PLASBALL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_plasma,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_PLASEXP,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 25 * Constants.FracUnit,
            Radius = 13 * Constants.FracUnit,
            Height = 8 * Constants.FracUnit,
            Mass = 100,
            Damage = 5,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_MISSILE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_BFG,
            DoomedNum = -1,
            SpawnState = StateNum.S_BFGSHOT,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_BFGLAND,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_rxplod,
            Speed = 25 * Constants.FracUnit,
            Radius = 13 * Constants.FracUnit,
            Height = 8 * Constants.FracUnit,
            Mass = 100,
            Damage = 100,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_MISSILE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_ARACHPLAZ,
            DoomedNum = -1,
            SpawnState = StateNum.S_ARACH_PLAZ,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_plasma,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_ARACH_PLEX,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 25 * Constants.FracUnit,
            Radius = 13 * Constants.FracUnit,
            Height = 8 * Constants.FracUnit,
            Mass = 100,
            Damage = 5,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_MISSILE | MapObjectFlag.MF_DROPOFF | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_PUFF,
            DoomedNum = -1,
            SpawnState = StateNum.S_PUFF1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_BLOOD,
            DoomedNum = -1,
            SpawnState = StateNum.S_BLOOD1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_TFOG,
            DoomedNum = -1,
            SpawnState = StateNum.S_TFOG,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_IFOG,
            DoomedNum = -1,
            SpawnState = StateNum.S_IFOG,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_TELEPORTMAN,
            DoomedNum = 14,
            SpawnState = StateNum.S_NULL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_firxpl,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_NOBLOCKMAP | MapObjectFlag.MF_NOSECTOR,
            RaiseState = StateNum.S_NULL
        });

        /*
    {		// MT_EXTRABFG
	-1,		// doomednum
	S_BFGEXP,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

         */

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC0,
            DoomedNum = 2018,
            SpawnState = StateNum.S_ARM1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC1,
            DoomedNum = 2019,
            SpawnState = StateNum.S_ARM2,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC2,
            DoomedNum = 2014,
            SpawnState = StateNum.S_BON1,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC3,
            DoomedNum = 2015,
            SpawnState = StateNum.S_BON2,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC4,
            DoomedNum = 5,
            SpawnState = StateNum.S_BKEY,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_NOTDMATCH,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC5,
            DoomedNum = 13,
            SpawnState = StateNum.S_RKEY,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_NOTDMATCH,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC6,
            DoomedNum = 6,
            SpawnState = StateNum.S_YKEY,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_NOTDMATCH,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC7,
            DoomedNum = 39,
            SpawnState = StateNum.S_YSKULL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_NOTDMATCH,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC8,
            DoomedNum = 38,
            SpawnState = StateNum.S_RSKULL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_NOTDMATCH,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC9,
            DoomedNum = 40,
            SpawnState = StateNum.S_BSKULL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_NOTDMATCH,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC10,
            DoomedNum = 2011,
            SpawnState = StateNum.S_STIM,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC11,
            DoomedNum = 2012,
            SpawnState = StateNum.S_MEDI,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC12,
            DoomedNum = 2013,
            SpawnState = StateNum.S_SOUL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_INV,
            DoomedNum = 2022,
            SpawnState = StateNum.S_PINV,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC13,
            DoomedNum = 2023,
            SpawnState = StateNum.S_PSTR,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_INS,
            DoomedNum = 2024,
            SpawnState = StateNum.S_PINV,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC14,
            DoomedNum = 2025,
            SpawnState = StateNum.S_SUIT,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC15,
            DoomedNum = 2026,
            SpawnState = StateNum.S_PMAP,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC16,
            DoomedNum = 2045,
            SpawnState = StateNum.S_PVIS,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MEGA,
            DoomedNum = 83,
            SpawnState = StateNum.S_MEGA,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL | MapObjectFlag.MF_COUNTITEM,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_CLIP,
            DoomedNum = 2007,
            SpawnState = StateNum.S_CLIP,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC17,
            DoomedNum = 2048,
            SpawnState = StateNum.S_AMMO,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC18,
            DoomedNum = 2010,
            SpawnState = StateNum.S_ROCK,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC19,
            DoomedNum = 2046,
            SpawnState = StateNum.S_BROK,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC20,
            DoomedNum = 2047,
            SpawnState = StateNum.S_CELL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC21,
            DoomedNum = 17,
            SpawnState = StateNum.S_CELP,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC22,
            DoomedNum = 2008,
            SpawnState = StateNum.S_SHEL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC23,
            DoomedNum = 2049,
            SpawnState = StateNum.S_SBOX,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC24,
            DoomedNum = 8,
            SpawnState = StateNum.S_BPAK,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC25,
            DoomedNum = 2006,
            SpawnState = StateNum.S_BFUG,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_CHAINGUN,
            DoomedNum = 2002,
            SpawnState = StateNum.S_MGUN,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC26,
            DoomedNum = 2005,
            SpawnState = StateNum.S_CSAW,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC27,
            DoomedNum = 2003,
            SpawnState = StateNum.S_LAUN,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC28,
            DoomedNum = 2004,
            SpawnState = StateNum.S_PLAS,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_SHOTGUN,
            DoomedNum = 2001,
            SpawnState = StateNum.S_SHOT,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_SUPERSHOTGUN,
            DoomedNum = 82,
            SpawnState = StateNum.S_SHOT2,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPECIAL,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC29,
            DoomedNum = 85,
            SpawnState = StateNum.S_TECHLAMP,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC30,
            DoomedNum = 86,
            SpawnState = StateNum.S_TECH2LAMP,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC31,
            DoomedNum = 2028,
            SpawnState = StateNum.S_COLU,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC32,
            DoomedNum = 30,
            SpawnState = StateNum.S_TALLGRNCOL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC33,
            DoomedNum = 31,
            SpawnState = StateNum.S_SHRTGRNCOL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC34,
            DoomedNum = 32,
            SpawnState = StateNum.S_TALLREDCOL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC35,
            DoomedNum = 33,
            SpawnState = StateNum.S_SHRTREDCOL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC36,
            DoomedNum = 37,
            SpawnState = StateNum.S_SKULLCOL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC37,
            DoomedNum = 36,
            SpawnState = StateNum.S_HEARTCOL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC38,
            DoomedNum = 41,
            SpawnState = StateNum.S_EVILEYE,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC39,
            DoomedNum = 42,
            SpawnState = StateNum.S_FLOATSKULL,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC40,
            DoomedNum = 43,
            SpawnState = StateNum.S_TORCHTREE,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC41,
            DoomedNum = 44,
            SpawnState = StateNum.S_BLUETORCH,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC42,
            DoomedNum = 45,
            SpawnState = StateNum.S_GREENTORCH,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC43,
            DoomedNum = 46,
            SpawnState = StateNum.S_REDTORCH,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC44,
            DoomedNum = 55,
            SpawnState = StateNum.S_BTORCHSHRT,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC45,
            DoomedNum = 56,
            SpawnState = StateNum.S_GTORCHSHRT,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC46,
            DoomedNum = 57,
            SpawnState = StateNum.S_RTORCHSHRT,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC47,
            DoomedNum = 47,
            SpawnState = StateNum.S_STALAGTITE,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC48,
            DoomedNum = 48,
            SpawnState = StateNum.S_TECHPILLAR,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC49,
            DoomedNum = 34,
            SpawnState = StateNum.S_CANDLESTIK,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = 0,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC50,
            DoomedNum = 35,
            SpawnState = StateNum.S_CANDELABRA,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 16 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC51,
            DoomedNum = 49,
            SpawnState = StateNum.S_BLOODYTWITCH,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 68 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC52,
            DoomedNum = 50,
            SpawnState = StateNum.S_MEAT2,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 84 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC53,
            DoomedNum = 51,
            SpawnState = StateNum.S_MEAT3,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 84 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC54,
            DoomedNum = 52,
            SpawnState = StateNum.S_MEAT4,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 68 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC55,
            DoomedNum = 53,
            SpawnState = StateNum.S_MEAT5,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 16 * Constants.FracUnit,
            Height = 52 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SOLID | MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC56,
            DoomedNum = 59,
            SpawnState = StateNum.S_MEAT2,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 84 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC57,
            DoomedNum = 60,
            SpawnState = StateNum.S_MEAT4,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 68 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC58,
            DoomedNum = 61,
            SpawnState = StateNum.S_MEAT3,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 52 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC59,
            DoomedNum = 62,
            SpawnState = StateNum.S_MEAT5,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 52 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        _predefinedTypes.Add(new MapObjectInfo
        {
            Type = MapObjectType.MT_MISC60,
            DoomedNum = 63,
            SpawnState = StateNum.S_BLOODYTWITCH,
            SpawnHealth = 1000,
            SeeState = StateNum.S_NULL,
            SeeSound = SoundType.sfx_None,
            ReactionTime = 8,
            AttackSound = SoundType.sfx_None,
            PainState = StateNum.S_NULL,
            PainChance = 0,
            PainSound = SoundType.sfx_None,
            MeleeState = StateNum.S_NULL,
            MissileState = StateNum.S_NULL,
            DeathState = StateNum.S_NULL,
            XDeathState = StateNum.S_NULL,
            DeathSound = SoundType.sfx_None,
            Speed = 0,
            Radius = 20 * Constants.FracUnit,
            Height = 68 * Constants.FracUnit,
            Mass = 100,
            Damage = 0,
            ActiveSound = SoundType.sfx_None,
            Flags = MapObjectFlag.MF_SPAWNCEILING | MapObjectFlag.MF_NOGRAVITY,
            RaiseState = StateNum.S_NULL
        });

        /*
         {		// MT_MISC61
	22,		// doomednum
	S_HEAD_DIE6,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC62
	15,		// doomednum
	S_PLAY_DIE7,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC63
	18,		// doomednum
	S_POSS_DIE5,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC64
	21,		// doomednum
	S_SARG_DIE6,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC65
	23,		// doomednum
	S_SKULL_DIE6,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC66
	20,		// doomednum
	S_TROO_DIE5,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC67
	19,		// doomednum
	S_SPOS_DIE5,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC68
	10,		// doomednum
	S_PLAY_XDIE9,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC69
	12,		// doomednum
	S_PLAY_XDIE9,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC70
	28,		// doomednum
	S_HEADSONSTICK,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC71
	24,		// doomednum
	S_GIBS,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	0,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC72
	27,		// doomednum
	S_HEADONASTICK,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC73
	29,		// doomednum
	S_HEADCANDLES,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC74
	25,		// doomednum
	S_DEADSTICK,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC75
	26,		// doomednum
	S_LIVESTICK,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC76
	54,		// doomednum
	S_BIGTREE,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	32*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC77
	70,		// doomednum
	S_BBAR1,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC78
	73,		// doomednum
	S_HANGNOGUTS,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	88*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID|MF_SPAWNCEILING|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC79
	74,		// doomednum
	S_HANGBNOBRAIN,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	88*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID|MF_SPAWNCEILING|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC80
	75,		// doomednum
	S_HANGTLOOKDN,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	64*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID|MF_SPAWNCEILING|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC81
	76,		// doomednum
	S_HANGTSKULL,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	64*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID|MF_SPAWNCEILING|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC82
	77,		// doomednum
	S_HANGTLOOKUP,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	64*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID|MF_SPAWNCEILING|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC83
	78,		// doomednum
	S_HANGTNOBRAIN,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	16*FRACUNIT,		// radius
	64*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_SOLID|MF_SPAWNCEILING|MF_NOGRAVITY,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC84
	79,		// doomednum
	S_COLONGIBS,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC85
	80,		// doomednum
	S_SMALLPOOL,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP,		// flags
	S_NULL		// raisestate
    },

    {		// MT_MISC86
	81,		// doomednum
	S_BRAINSTEM,		// spawnstate
	1000,		// spawnhealth
	S_NULL,		// seestate
	sfx_None,		// seesound
	8,		// reactiontime
	sfx_None,		// attacksound
	S_NULL,		// painstate
	0,		// painchance
	sfx_None,		// painsound
	S_NULL,		// meleestate
	S_NULL,		// missilestate
	S_NULL,		// deathstate
	S_NULL,		// xdeathstate
	sfx_None,		// deathsound
	0,		// speed
	20*FRACUNIT,		// radius
	16*FRACUNIT,		// height
	100,		// mass
	0,		// damage
	sfx_None,		// activesound
	MF_NOBLOCKMAP,		// flags
	S_NULL		// raisestate
    }
         */
    }
}