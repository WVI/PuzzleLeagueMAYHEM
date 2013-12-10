using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puzzle_League_MAYHEM {
	static class RNG {
		private static Random rand = new Random();

		public static int Next() {
			return rand.Next();
		}

		public static int Next(int maxValue) {
			return rand.Next(maxValue);
		}

		public static int Next(int minValue, int maxValue) {
			return rand.Next(minValue, maxValue);
		}
	}
}
