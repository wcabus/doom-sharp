using DoomSharp.Core.Networking;
using DoomSharp.Core.Data;
using DoomSharp.Core.Graphics;
using DoomSharp.Core.Input;

namespace DoomSharp.Core.GameLogic;

public class Player
{
    public Player()
    {
        for (var i = 0; i < PlayerSprites.Length; i++)
        {
            PlayerSprites[i] = new PlayerSprite();
        }
    }

    public PlayerState PlayerState { get; set; } = PlayerState.NotSet;
    public TicCommand Command { get; set; } = new();

    public MapObject? MapObject { get; set; }

    // Determine POV,
    //  including viewpoint bobbing during movement.
    // Focal origin above r.z
    public Fixed ViewZ { get; set; }
    // Base height above floor for viewz.
    public Fixed ViewHeight { get; set; }
    // Bob/squat speed.
    public Fixed DeltaViewHeight { get; set; }
    public bool OnGround { get; set; }
    // bounded/scaled total momentum.
    public Fixed Bob { get; set; }

    // This is only used between levels,
    // mo->health is used during levels.
    public int Health { get; set; }
    public int ArmorPoints { get; set; }
    // Armor type is 0-2.
    public int ArmorType { get; set; }

    // Power ups. invinc and invis are tic counters.
    public int[] Powers { get; } = new int[(int)PowerUpType.NumberOfPowerUps];
    public bool[] Cards { get; } = new bool[(int)KeyCardType.NumberOfKeyCards];
    public bool Backpack { get; set; }

    // Frags, kills of other players.
    public int[] Frags { get; set; } = new int[Constants.MaxPlayers];
    public WeaponType ReadyWeapon { get; set; }

    // Is wp_nochange if not changing.
    public WeaponType PendingWeapon { get; set; }

    public bool[] WeaponOwned { get; } = new bool[(int)WeaponType.NumberOfWeapons];
    public int[] Ammo { get; } = new int[(int)AmmoType.NumAmmo];
    public int[] MaxAmmo { get; } = new int[(int)AmmoType.NumAmmo];

    // True if button down last tic.
    public bool AttackDown { get; set; }
    public bool UseDown { get; set; }

    // Bit flags, for cheats and debug.
    // See cheat_t, above.
    public Cheat Cheats { get; set; }

    // Refired shots are less accurate.
    public int Refire { get; set; }

    // For intermission stats.
    public int KillCount { get; set; }
    public int ItemCount { get; set; }
    public int SecretCount { get; set; }

    // Hint messages.
    public string? Message { get; set; }

    // For screen flashing (red or bright).
    public int DamageCount { get; set; }
    public int BonusCount { get; set; }

    // Who did damage (NULL for floors/ceilings).
    public MapObject? Attacker { get; set; }

    // So gun flashes light up areas.
    public int ExtraLight { get; set; }

    // Current PLAYPAL, ???
    //  can be set to REDCOLORMAP for pain, etc.
    public int FixedColorMap { get; set; }

    // Player skin colorshift,
    //  0-3 for which color to draw player.
    public int ColorMap { get; set; }

    // Overlay view sprites (gun, etc).
    public PlayerSprite[] PlayerSprites { get; } = new PlayerSprite[(int)PlayerSpriteType.NumPlayerSprites];

    // True if secret level has been done.
    public bool DidSecret { get; set; }

    public void Think()
    {
        // fixme: do this in the cheat code
        if ((Cheats & Cheat.NoClip) != 0)
        {
            MapObject!.Flags |= MapObjectFlag.MF_NOCLIP;
        }
        else
        {
            MapObject!.Flags &= ~MapObjectFlag.MF_NOCLIP;
        }

        // chain saw run forward
        if ((MapObject!.Flags & MapObjectFlag.MF_JUSTATTACKED) != 0)
        {
            Command.AngleTurn = 0;
            Command.ForwardMove = 0xc800 / 512;
            Command.SideMove = 0;
            MapObject.Flags &= ~MapObjectFlag.MF_JUSTATTACKED;
        }

        if (PlayerState == PlayerState.Dead)
        {
            DeathThink();
            return;
        }

        // Move around.
        // Reactiontime is used to prevent movement
        //  for a bit after a teleport.
        if (MapObject.ReactionTime != 0)
        {
            MapObject.ReactionTime--;
        }
        else
        {
            Move();
        }

        CalcHeight();

        if (MapObject?.SubSector?.Sector != null && MapObject.SubSector.Sector.Special != 0)
        {
            PlayerInSpecialSector();
        }

        // Check for weapon change.

        // A special event has no other buttons.
        if ((Command.Buttons & ButtonCode.Special) != 0)
        {
            Command.Buttons = 0;
        }

        if ((Command.Buttons & ButtonCode.Change) != 0)
        {
            // The actual changing of the weapon is done
            //  when the weapon psprite can do it
            //  (read: not in the middle of an attack).
            var newWeapon = (WeaponType)((int)(Command.Buttons & ButtonCode.WeaponMask) >> (int)ButtonCode.WeaponShift);

            if (newWeapon == (int)WeaponType.Fist && 
                WeaponOwned[(int)WeaponType.Chainsaw] &&
                !(ReadyWeapon == WeaponType.Chainsaw &&
                  Powers[(int)PowerUpType.Strength] != 0)
            )
            {
                newWeapon = WeaponType.Chainsaw;
            }

            if (DoomGame.Instance.GameMode == GameMode.Commercial && 
                newWeapon == WeaponType.Shotgun && 
                WeaponOwned[(int)WeaponType.SuperShotgun] &&
                ReadyWeapon != WeaponType.SuperShotgun)
            {
                newWeapon = WeaponType.SuperShotgun;
            }
            
            if (WeaponOwned[(int)newWeapon] && newWeapon != ReadyWeapon)
            {
                // Do not go to plasma or BFG in shareware,
                //  even if cheated.
                if ((newWeapon != WeaponType.Plasma && newWeapon != WeaponType.Bfg) || 
                    DoomGame.Instance.GameMode != GameMode.Shareware)
                {
                    PendingWeapon = newWeapon;
                }
            }
        }

        // check for use
        if ((Command.Buttons & ButtonCode.Use) != 0)
        {
            if (!UseDown)
            {
                UseLines();
                UseDown = true;
            }
        }
        else
        {
            UseDown = false;
        }

        // cycle psprites
        DoomGame.Instance.Game.P_MovePlayerSprites(this);

        // Counters, time dependend power ups.

        // Strength counts up to diminish fade.
        if (Powers[(int)PowerUpType.Strength] > 0)
        {
            Powers[(int)PowerUpType.Strength]--;
        }

        if (Powers[(int)PowerUpType.Invulnerability] > 0)
        {
            Powers[(int)PowerUpType.Invulnerability]--;
        }

        if (Powers[(int)PowerUpType.Invisibility] != 0)
        {
            if (--Powers[(int)PowerUpType.Invisibility] == 0)
            {
                MapObject!.Flags &= ~MapObjectFlag.MF_SHADOW;
            }
        }

        if (Powers[(int)PowerUpType.InfraRed] > 0)
        {
            Powers[(int)PowerUpType.InfraRed]--;
        }

        if (Powers[(int)PowerUpType.IronFeet] > 0)
        {
            Powers[(int)PowerUpType.IronFeet]--;
        }

        if (DamageCount > 0)
        {
            DamageCount--;
        }

        if (BonusCount > 0)
        {
            BonusCount--;
        }

        // Handling colormaps.
        if (Powers[(int)PowerUpType.Invulnerability] != 0)
        {
            if (Powers[(int)PowerUpType.Invulnerability] > 4 * 32
                || (Powers[(int)PowerUpType.Invulnerability] & 8) != 0)
            {
                FixedColorMap = 32; //INVERSECOLORMAP;
            }
            else
            {
                FixedColorMap = 0;
            }
        }
        else if (Powers[(int)PowerUpType.InfraRed] != 0)
        {
            if (Powers[(int)PowerUpType.InfraRed] > 4 * 32
                || (Powers[(int)PowerUpType.InfraRed] & 8) != 0)
            {
                // almost full bright
                FixedColorMap = 1;
            }
            else
            {
                FixedColorMap = 0;
            }
        }
        else
        {
            FixedColorMap = 0;
        }
    }

    /// <summary>
    /// Looks for special lines in front of the player to activate.
    /// </summary>
    private void UseLines()
    {
        var angle = MapObject!.Angle >> RenderEngine.AngleToFineShift;
        var x1 = MapObject.X;
        var y1 = MapObject.Y;
        var x2 = x1 + (Constants.UseRange >> Constants.FracBits) * RenderEngine.FineCosine[angle];
        var y2 = y1 + (Constants.UseRange >> Constants.FracBits) * RenderEngine.FineSine[angle];

        // TODO P_PathTraverse(x1, y1, x2, y2, PT_ADDLINES, PTR_UseTraverse);
    }

    private const uint Angle5 = RenderEngine.Angle90 / 18;

    /// <summary>
    /// Fall on your face when dying. Decrease POV height to floor height.
    /// </summary>
    public void DeathThink()
    {
        DoomGame.Instance.Game.P_MovePlayerSprites(this);

        // fall to the ground
        if (ViewHeight > 6 * Constants.FracUnit)
        {
            ViewHeight -= Constants.FracUnit;
        }

        if (ViewHeight < 6 * Constants.FracUnit)
        {
            ViewHeight = 6 * Constants.FracUnit;
        }

        DeltaViewHeight = 0;
        OnGround = (MapObject!.Z <= MapObject.FloorZ);
        CalcHeight();

        if (Attacker != null && Attacker != MapObject)
        {
            var angle = DoomGame.Instance.Renderer.PointToAngle2(MapObject.X, MapObject.Y, Attacker.X, Attacker.Y);
            var delta = angle - MapObject.Angle;

            var inverseAngle5 = Angle5;
            if (delta < Angle5 || delta > (uint)-inverseAngle5)
            {
                // Looking at killer,
                //  so fade damage flash down.
                MapObject.Angle = angle;

                if (DamageCount != 0)
                {
                    DamageCount--;
                }
            }
            else if (delta < RenderEngine.Angle180)
            {
                MapObject.Angle += Angle5;
            }
            else
            {
                MapObject.Angle -= Angle5;
            }
        }
        else if (DamageCount != 0)
        {
            DamageCount--;
        }

        if ((Command.Buttons & ButtonCode.Use) != 0)
        {
            PlayerState = PlayerState.Reborn;
        }
    }

    private const int MaxBob = 0x100000;

    /// <summary>
    /// Calculate the walking / running height adjustment
    /// </summary>
    public void CalcHeight()
    {
        // Regular movement bobbing
        // (needs to be calculated for gun swing
        // even if not on ground)
        // OPTIMIZE: tablify angle
        // Note: a LUT allows for effects
        //  like a ramp with low health.
        Bob = (MapObject!.MomX * MapObject.MomX) + (MapObject.MomY * MapObject.MomY);

        Bob >>= 2;

        if (Bob > MaxBob)
        {
            Bob = MaxBob;
        }

        if ((Cheats & Cheat.NoMomentum) != 0 || !OnGround)
        {
            ViewZ = MapObject.Z + Constants.ViewHeight;

            if (ViewZ > MapObject.CeilingZ - 4 * Constants.FracUnit)
            {
                ViewZ = MapObject.CeilingZ - 4 * Constants.FracUnit;
            }

            ViewZ = MapObject.Z + ViewHeight;
            return;
        }

        var angle = (RenderEngine.FineAngles / 20 * DoomGame.Instance.Game.LevelTime) & RenderEngine.FineMask;
        var bob = (Fixed)((int)Bob / 2) * RenderEngine.FineSine[angle];

        // move viewheight
        if (PlayerState == PlayerState.Alive)
        {
            ViewHeight += DeltaViewHeight;

            if (ViewHeight > Constants.ViewHeight)
            {
                ViewHeight = Constants.ViewHeight;
                DeltaViewHeight = 0;
            }

            if (ViewHeight < Constants.ViewHeight / 2)
            {
                ViewHeight = Constants.ViewHeight / 2;
                if (DeltaViewHeight <= 0)
                {
                    DeltaViewHeight = 1;
                }
            }

            if (DeltaViewHeight != 0)
            {
                DeltaViewHeight += Constants.FracUnit / 4;
                if (DeltaViewHeight == 0)
                {
                    DeltaViewHeight = 1;
                }
            }
        }

        ViewZ = MapObject.Z + ViewHeight + bob;

        if (ViewZ > MapObject.CeilingZ - 4 * Constants.FracUnit)
        {
            ViewZ = MapObject.CeilingZ - 4 * Constants.FracUnit;
        }
    }

    public void Move()
    {
        MapObject!.Angle += (uint)(Command.AngleTurn << 16);

        // Do not let the player control movement
        //  if not onground.
        OnGround = (MapObject.Z <= MapObject.FloorZ);

        if (Command.ForwardMove != 0 && OnGround)
        {
            Thrust(MapObject.Angle, Command.ForwardMove * 2048);
        }

        if (Command.SideMove != 0 && OnGround)
        {
            Thrust(MapObject.Angle - RenderEngine.Angle90, Command.SideMove * 2048);
        }

        if ((Command.ForwardMove != 0 || Command.SideMove != 0) && 
            MapObject.State == State.GetSpawnState(StateNum.S_PLAY))
        {
            MapObject.SetState(StateNum.S_PLAY_RUN1);
        }
    }

    /// <summary>
    /// Moves the given origin along a given angle.
    /// </summary>
    public void Thrust(uint angle, Fixed move)
    {
        angle >>= RenderEngine.AngleToFineShift;

        MapObject!.MomX += (move * RenderEngine.FineCosine[angle]);
        MapObject!.MomY += (move * RenderEngine.FineSine[angle]);
    }

    /// <summary>
    /// Called every tic frame
    ///  that the player origin is in a special sector
    /// </summary>
    private void PlayerInSpecialSector()
    {
        var sector = MapObject?.SubSector?.Sector;
        if (sector is null)
        {
            return;
        }

        // Falling, not all the way down yet?
        if (MapObject!.Z != sector.FloorHeight)
        {
            return;
        }

        // Has hitten ground.
        switch (sector.Special)
        {
            case 5:
                // HELLSLIME DAMAGE
                if (Powers[(int)PowerUpType.IronFeet] == 0)
                {
                    if ((DoomGame.Instance.Game.LevelTime & 0x1f) == 0)
                    {
                        DamageMapObject(MapObject, null, null, 10);
                    }
                }
                break;

            case 7:
                // NUKAGE DAMAGE
                if (Powers[(int)PowerUpType.IronFeet] == 0)
                {
                    if ((DoomGame.Instance.Game.LevelTime & 0x1f) == 0)
                    {
                        DamageMapObject(MapObject, null, null, 5);
                    }
                }
                break;

            case 16:
            // SUPER HELLSLIME DAMAGE
            case 4:
                // STROBE HURT
                if (Powers[(int)PowerUpType.IronFeet] == 0 || DoomRandom.P_Random() < 5)
                {
                    if ((DoomGame.Instance.Game.LevelTime & 0x1f) == 0)
                    {
                        DamageMapObject(MapObject, null, null, 20);
                    }
                }
                break;

            case 9:
                // SECRET SECTOR
                SecretCount++;
                sector.Special = 0;
                break;

            case 11:
                // EXIT SUPER DAMAGE! (for E1M8 finale)
                Cheats &= ~Cheat.GodMode;

                if ((DoomGame.Instance.Game.LevelTime & 0x1f) == 0)
                {
                    DamageMapObject(MapObject, null, null, 20);
                }

                if (Health <= 10)
                {
                    DoomGame.Instance.Game.ExitLevel();
                }
                break;

            default:
                DoomGame.Error("P_PlayerInSpecialSector: unknown special {sector.Special");
                break;
        }
    }

    /// <summary>
    /// Damages both enemies and players
    /// "inflictor" is the thing that caused the damage
    ///  creature or missile, can be NULL (slime, etc)
    /// "source" is the thing to target after taking damage
    ///  creature or NULL
    /// Source and inflictor are the same for melee attacks.
    /// Source can be NULL for slime, barrel explosions
    /// and other environmental stuff.
    /// </summary>
    public void DamageMapObject(MapObject target, MapObject? inflictor, MapObject? source, int damage)
    {
        if ((target.Flags & MapObjectFlag.MF_SHOOTABLE) == 0)
        {
            return; // shouldn't happen...
        }

        if (target.Health <= 0)
        {
            return;
        }

        if ((target.Flags & MapObjectFlag.MF_SKULLFLY) != 0)
        {
            target.MomX = target.MomY = target.MomZ = 0;
        }

        var player = target.Player;
        if (player != null && DoomGame.Instance.Game.GameSkill == SkillLevel.Baby)
        {
            damage >>= 1;   // take half damage in trainer mode
        }
        
        // Some close combat weapons should not
        // inflict thrust and push the victim out of reach,
        // thus kick away unless using the chainsaw.
        if (inflictor != null && 
            (target.Flags & MapObjectFlag.MF_NOCLIP) == 0 && 
            (source?.Player == null || source.Player.ReadyWeapon != WeaponType.Chainsaw))
        {
            var ang = DoomGame.Instance.Renderer.PointToAngle2(inflictor.X, inflictor.Y, target.X, target.Y);
            var thrust = new Fixed(damage * (Constants.FracUnit >> 3) * 100 / target.Info.Mass);

            // make fall forwards sometimes
            if (damage < 40 && 
                damage > target.Health && 
                target.Z - inflictor.Z > 64 * Constants.FracUnit && 
                (DoomRandom.P_Random() & 1) != 0)
            {
                ang += RenderEngine.Angle180;
                thrust = (int)thrust * 4;
            }

            ang >>= RenderEngine.AngleToFineShift;
            target.MomX += (thrust * RenderEngine.FineCosine[ang]);
            target.MomY += (thrust * RenderEngine.FineSine[ang]);
        }

        // player specific
        if (player != null)
        {
            // end of game hell hack
            if (target.SubSector?.Sector != null && target.SubSector.Sector.Special == 11 && damage >= target.Health)
            {
                damage = target.Health - 1;
            }
            
            // Below certain threshold,
            // ignore damage in GOD mode, or with INVUL power.
            if (damage < 1000 && 
                ((player.Cheats & Cheat.GodMode) != 0 || 
                 player.Powers[(int)PowerUpType.Invulnerability] != 0))
            {
                return;
            }

            var saved = 0;
            if (player.ArmorType != 0)
            {
                if (player.ArmorType == 1)
                {
                    saved = damage / 3;
                }
                else
                {
                    saved = damage / 2;
                }

                if (player.ArmorPoints <= saved)
                {
                    // armor is used up
                    saved = player.ArmorPoints;
                    player.ArmorType = 0;
                }
                player.ArmorPoints -= saved;
                damage -= saved;
            }

            player.Health -= damage;   // mirror mobj health here for Dave
            if (player.Health < 0)
            {
                player.Health = 0;
            }

            player.Attacker = source;
            player.DamageCount += damage;  // add damage after armor / invuln

            if (player.DamageCount > 100)
            {
                player.DamageCount = 100;  // teleport stomp does 10k points...
            }

            var temp = damage < 100 ? damage : 100;

            //if (player == DoomGame.Instance.Game.Players[DoomGame.Instance.Game.ConsolePlayer])
            //{
            //    I_Tactile(40, 10, 40 + temp * 2); // unused
            //}
        }

        // do the damage	
        target.Health -= damage;
        if (target.Health <= 0)
        {
            KillMapObject(source, target);
            return;
        }

        if (DoomRandom.P_Random() < target.Info.PainChance && 
            (target.Flags & MapObjectFlag.MF_SKULLFLY) == 0)
        {
            target.Flags |= MapObjectFlag.MF_JUSTHIT;    // fight back!
            target.SetState(target.Info.PainState);
        }

        target.ReactionTime = 0;       // we're awake now...	

        if ((target.Threshold == 0 || target.Type == MapObjectType.MT_VILE) &&
            source != null &&
            source != target &&
            source.Type != MapObjectType.MT_VILE)
        {
            // if not intent on another player,
            // chase after this one
            target.Target = source;
            target.Threshold = Constants.BaseThreshold;
            if (target.State == State.GetSpawnState(target.Info.SpawnState) &&
                target.Info.SeeState != StateNum.S_NULL)
            {
                target.SetState(target.Info.SeeState);
            }
        }
    }

    private void KillMapObject(MapObject? source, MapObject target)
    {
        target.Flags &= ~(MapObjectFlag.MF_SHOOTABLE | MapObjectFlag.MF_FLOAT | MapObjectFlag.MF_SKULLFLY);

        if (target.Type != MapObjectType.MT_SKULL)
        {
            target.Flags &= ~MapObjectFlag.MF_NOGRAVITY;
        }

        target.Flags |= MapObjectFlag.MF_CORPSE | MapObjectFlag.MF_DROPOFF;
        target.Height >>= 2;

        if (source?.Player != null)
        {
            // count for intermission
            if ((target.Flags & MapObjectFlag.MF_COUNTKILL) != 0)
            {
                source.Player.KillCount++;
            }

            if (target.Player != null)
            {
                var playerIndex = DoomGame.Instance.Game.GetPlayerIndex(target.Player);
                if (playerIndex != -1)
                {
                    source.Player.Frags[playerIndex]++;
                }
            }
        }
        else if (!DoomGame.Instance.Game.NetGame && 
                 (target.Flags & MapObjectFlag.MF_COUNTKILL) != 0)
        {
            // count all monster deaths,
            // even those caused by other monsters
            DoomGame.Instance.Game.Players[0].KillCount++;
        }

        if (target.Player != null)
        {
            // count environment kills against you
            if (source != null)
            {
                var playerIndex = DoomGame.Instance.Game.GetPlayerIndex(target.Player);
                if (playerIndex != -1)
                {
                    target.Player.Frags[playerIndex]++;
                }
            }

            target.Flags &= ~MapObjectFlag.MF_SOLID;
            target.Player.PlayerState = PlayerState.Dead;
            DropWeapon(target.Player);

            if (target.Player == DoomGame.Instance.Game.Players[DoomGame.Instance.Game.ConsolePlayer] && 
                DoomGame.Instance.AutoMapActive)
            {
                // don't die in auto map,
                // switch view prior to dying
                // TODO AM_Stop();
            }
        }

        if (target.Health < -target.Info.SpawnHealth && target.Info.XDeathState != StateNum.S_NULL)
        {
            target.SetState(target.Info.XDeathState);
        }
        else
        {
            target.SetState(target.Info.DeathState);
        }
        
        target.Tics -= DoomRandom.P_Random() & 3;

        if (target.Tics < 1)
        {
            target.Tics = 1;
        }

        //	I_StartSound (&actor->r, actor->info->deathsound);

        // Drop stuff.
        // This determines the kind of object spawned
        // during the death frame of a thing.
        MapObjectType item;
        switch (target.Type)
        {
            case MapObjectType.MT_WOLFSS:
            case MapObjectType.MT_POSSESSED:
                item = MapObjectType.MT_CLIP;
                break;

            case MapObjectType.MT_SHOTGUY:
                item = MapObjectType.MT_SHOTGUN;
                break;

            case MapObjectType.MT_CHAINGUY:
                item = MapObjectType.MT_CHAINGUN;
                break;

            default:
                return;
        }

        var mo = DoomGame.Instance.Game.P_SpawnMapObject(target.X, target.Y, Constants.OnFloorZ, item);
        mo.Flags |= MapObjectFlag.MF_DROPPED; // special versions of items
    }

    private void DropWeapon(Player player)
    {
        DoomGame.Instance.Game.P_SetPlayerSprite(
            player,
            PlayerSpriteType.Weapon, 
            WeaponInfo.GetByType(player.ReadyWeapon).DownState);
    }
}