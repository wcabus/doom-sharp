using System.Dynamic;

namespace DoomSharp.Core.UI
{
    public static class Messages
    {
        public const int NumberOfQuitMessages = 22;

        public static string GenericQuitMessage =>
            DoomGame.Instance.Language == GameLanguage.English 
                ? $"are you sure you want to{Environment.NewLine}quit this great game?"
                : $"VOUS VOULEZ VRAIMENT{Environment.NewLine}QUITTER CE SUPER JEU?";

        public static string DosY =>
            DoomGame.Instance.Language == GameLanguage.English
                ? "(press y to quit)"
                : "(APPUYEZ SUR Y POUR REVENIR AU OS.)";

        public static IEnumerable<string> QuitMessages 
        {
            get
            {
                // DOOM1
                yield return GenericQuitMessage;
                yield return $"please don't leave, there's more{Environment.NewLine}demons to toast!";
                yield return $"let's beat it -- this is turning{Environment.NewLine}into a bloodbath!";
                yield return $"i wouldn't leave if i were you.{Environment.NewLine}dos is much worse.";
                yield return $"you're trying to say you like dos{Environment.NewLine}better than me, right?";
                yield return $"don't leave yet -- there's a{Environment.NewLine}demon around that corner!";
                yield return $"ya know, next time you come in here{Environment.NewLine}i'm gonna toast ya.";
                yield return "go ahead and leave. see if i care.";

                // QuitDOOM II messages
                yield return $"you want to quit?{Environment.NewLine}then, thou hast lost an eighth!";
                yield return $"don't go now, there's a {Environment.NewLine}dimensional shambler waiting{Environment.NewLine}at the dos prompt!";
                yield return $"get outta here and go back{Environment.NewLine}to your boring programs.";
                yield return $"if i were your boss, i'd {Environment.NewLine} deathmatch ya in a minute!";
                yield return $"look, bud. you leave now{Environment.NewLine}and you forfeit your body count!";
                yield return $"just leave. when you come{Environment.NewLine}back, i'll be waiting with a bat.";
                yield return $"you're lucky i don't smack{Environment.NewLine}you for thinking about leaving.";

                // FinalDOOM?
                yield return $"fuck you, pussy!{Environment.NewLine}get the fuck out!";
                yield return $"you quit and i'll jizz{Environment.NewLine}in your cystholes!";
                yield return $"if you leave, i'll make{Environment.NewLine}the lord drink my jizz.";
                yield return $"hey, ron! can we say{Environment.NewLine}'fuck' in the game?";
                yield return $"i'd leave: this is just{Environment.NewLine}more monsters and levels.{Environment.NewLine}what a load.";
                yield return $"suck it down, asshole!{Environment.NewLine}you're a fucking wimp!";
                yield return $"don't quit now! we're {Environment.NewLine}still spending your money!";

                // Internal debug. Different style, too.
                yield return $"THIS IS NO MESSAGE!{Environment.NewLine}Page intentionally left blank.";
            }
        }
    }
}