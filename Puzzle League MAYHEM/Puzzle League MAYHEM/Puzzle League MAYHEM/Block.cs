using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puzzle_League_MAYHEM {
    class Block: GameObject {
        Random rand = new Random();

        enum BlockColor {
            Blank,      // 0
            Red,        // 1
            Yellow,     // 2
            Purple,     // 3
            Green,      // 4
            LightBlue,  // 5
            DarkBlue,   // 6
            Exclamation,// 7
            Garbage     // 8
        }
        BlockColor thisColor;

        public Block() {
            thisColor = (BlockColor)rand.Next(4);

            switch(thisColor) {
                case BlockColor.Red: texturePath += "Temp/RedPixel"; break;
                case BlockColor.Yellow: texturePath += "Temp/YellowPixel"; break;
                case BlockColor.Purple: texturePath += "Temp/PurplePixel"; break;
                case BlockColor.Green: texturePath += "Temp/GreenPixel"; break;
                case BlockColor.LightBlue: texturePath += "Temp/LightBluePixel"; break;
                default: texturePath += "Temp/WhitePixel"; break;
            }
        }

        public Block(bool darkBlueOn) {
            if (darkBlueOn)
                thisColor = (BlockColor)rand.Next(5);
            else
                thisColor = (BlockColor)rand.Next(4);

            switch (thisColor) {
                case BlockColor.Red: texturePath += "Temp/RedPixel"; break;
                case BlockColor.Yellow: texturePath += "Temp/YellowPixel"; break;
                case BlockColor.Purple: texturePath += "Temp/PurplePixel"; break;
                case BlockColor.Green: texturePath += "Temp/GreenPixel"; break;
                case BlockColor.LightBlue: texturePath += "Temp/LightBluePixel"; break;
                case BlockColor.DarkBlue: texturePath += "Temp/DarkBluePixel"; break;
                default: texturePath += "Temp/WhitePixel"; break;
            }
        }
    }
}
