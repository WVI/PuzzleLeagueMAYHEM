using System;

namespace Puzzle_League_MAYHEM {
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (PuzzleLeagueGame game = new PuzzleLeagueGame())
            {
                game.Run();
            }
        }
    }
#endif
}

