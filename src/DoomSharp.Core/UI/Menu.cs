namespace DoomSharp.Core.UI;

public class Menu
{
    public Menu()
    {
        /*
         https://github.com/id-Software/DOOM/blob/master/linuxdoom-1.10/m_menu.c
        currentMenu = &MainDef;
        itemOn = currentMenu->lastOn;
        whichSkull = 0;
        skullAnimCounter = 10;
        screenSize = screenblocks - 3;
        messageToPrint = 0;
        messageString = NULL;
        messageLastMenuActive = menuactive;
        quickSaveSlot = -1;

        // Here we could catch other version dependencies,
        //  like HELP1/2, and four episodes.

      
        switch ( gamemode )
        {
              case commercial:
	        // This is used because DOOM 2 had only one HELP
                //  page. I use CREDIT as second page now, but
	        //  kept this hack for educational purposes.
	        MainMenu[readthis] = MainMenu[quitdoom];
	        MainDef.numitems--;
	        MainDef.y += 8;
	        NewDef.prevMenu = &MainDef;
	        ReadDef1.routine = M_DrawReadThis1;
	        ReadDef1.x = 330;
	        ReadDef1.y = 165;
	        ReadMenu1[0].routine = M_FinishReadThis;
	        break;
              case shareware:
	        // Episode 2 and 3 are handled,
	        //  branching to an ad screen.
              case registered:
	        // We need to remove the fourth episode.
	        EpiDef.numitems--;
	        break;
              case retail:
	        // We are fine.
              default:
	        break;
        }
         
         */

        switch (DoomGame.Instance.GameMode)
        {
            case GameMode.Commercial:
                break;

            case GameMode.Shareware:
                break;

            case GameMode.Registered:
                break;

            case GameMode.Retail:
                break;

            default:
                break;
        }
    }

    public bool IsActive { get; private set; } = false;
}