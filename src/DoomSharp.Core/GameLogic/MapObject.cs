using DoomSharp.Core.Graphics;

namespace DoomSharp.Core.GameLogic;

//
// NOTES: mobj_t
//
// mobj_ts are used to tell the refresh where to draw an image,
// tell the world simulation when objects are contacted,
// and tell the sound driver how to position a sound.
//
// The refresh uses the next and prev links to follow
// lists of things in sectors as they are being drawn.
// The sprite, frame, and angle elements determine which patch_t
// is used to draw the sprite if it is visible.
// The sprite and frame values are allmost allways set
// from state_t structures.
// The statescr.exe utility generates the states.h and states.c
// files that contain the sprite/frame numbers from the
// statescr.txt source file.
// The xyz origin point represents a point at the bottom middle
// of the sprite (between the feet of a biped).
// This is the default origin position for patch_ts grabbed
// with lumpy.exe.
// A walking creature will have its z equal to the floor
// it is standing on.
//
// The sound code uses the x,y, and subsector fields
// to do stereo positioning of any sound effited by the mobj_t.
//
// The play simulation uses the blocklinks, x,y,z, radius, height
// to determine when mobj_ts are touching each other,
// touching lines in the map, or hit by trace lines (gunshots,
// lines of sight, etc).
// The mobj_t->flags element has various bit flags
// used by the simulation.
//
// Every mobj_t is linked into a single sector
// based on its origin coordinates.
// The subsector_t is found with R_PointInSubsector(x,y),
// and the sector_t can be found with subsector->sector.
// The sector links are only used by the rendering code,
// the play simulation does not care about them at all.
//
// Any mobj_t that needs to be acted upon by something else
// in the play world (block movement, be shot, etc) will also
// need to be linked into the blockmap.
// If the thing has the MF_NOBLOCK flag set, it will not use
// the block links. It can still interact with other things,
// but only as the instigator (missiles will run into other
// things, but nothing can run into a missile).
// Each block in the grid is 128*128 units, and knows about
// every line_t that it contains a piece of, and every
// interactable mobj_t that has its origin contained.  
//
// A valid mobj_t is a mobj_t that has the proper subsector_t
// filled in for its xy coordinates and is linked into the
// sector from which the subsector was made, or has the
// MF_NOSECTOR flag set (the subsector_t needs to be valid
// even if MF_NOSECTOR is set), and is linked into a blockmap
// block or has the MF_NOBLOCKMAP flag set.
// Links should only be modified by the P_[Un]SetThingPosition()
// functions.
// Do not change the MF_NO? flags while a thing is valid.
//
// Any questions?
//

public class MapObject : Thinker
{
    // Info for drawing: position.
    public Fixed X { get; set; } = Fixed.Zero;
    public Fixed Y { get; set; } = Fixed.Zero;
    public Fixed Z { get; set; } = Fixed.Zero;

    // More list: links in sector (if needed)
    public MapObject? SectorNext { get; set; }
    public MapObject? SectorPrev { get; set; }

    // More drawing info: to determine current sprite.
    public uint Angle { get; set; } // orientation
    public SpriteNum Sprite { get; set; } // used to find patch_t and flip value
    public int Frame { get; set; } // might be ORed with FF_FULLBRIGHT

    // Interaction info, by BLOCKMAP.
    // Links in blocks (if needed).
    public MapObject? BlockNext { get; set; }
    public MapObject? BlockPrev { get; set; }

    public SubSector? SubSector { get; set; }

    // The closest interval over all contacted Sectors.
    public Fixed FloorZ { get; set; } = Fixed.Zero;
    public Fixed CeilingZ { get; set; } = Fixed.Zero;

    // For movement checking.
    public Fixed Radius { get; set; } = Fixed.Zero;
    public Fixed Height { get; set; } = Fixed.Zero;

    // Momentums, used to update position.
    public Fixed MomX { get; set; } = Fixed.Zero;
    public Fixed MomY { get; set; } = Fixed.Zero;
    public Fixed MomZ { get; set; } = Fixed.Zero;

    // If == validcount, already checked.
    public int ValidCount { get; set; }

    public MapObjectType Type { get; set; }
    public MapObjectInfo Info { get; set; }   // &mobjinfo[mobj->type]

    public int Tics { get; set; }	// state tic counter
    public State? State { get; set; }
    public int Flags { get; set; }
    public int Health { get; set; }

    // Movement direction, movement generation (zig-zagging).
    public int MoveDir { get; set; }	// 0-7
    public int MoveCount { get; set; }	// when 0, select a new dir

    // Thing being chased/attacked (or NULL),
    // also the originator for missiles.
    public MapObject? Target { get; set; }

    // Reaction time: if non 0, don't attack yet.
    // Used by player to freeze a bit after teleporting.
    public int ReactionTime { get; set; }

    // If >0, the target will be chased
    // no matter what (even if shot)
    public int Threshold { get; set; }

    // Additional info record for player avatars only.
    // Only valid if type == MT_PLAYER
    public Player? Player { get; set; }

    // Player number last looked for.
    public int LastLook { get; set; }

    // For nightmare respawn.
    public MapThing Spawnpoint { get; set; }

    // Thing being chased/attacked for tracers.
    public MapObject? Tracer { get; set; }
}