using System.Collections.Generic;

namespace Quadspace.TBP.Randomizer {
    public class SevenBagRandomizerStart {
        public string type = "seven_bag";
        public List<string> bag_state;

        public SevenBagRandomizerStart(List<string> bagState) {
            bag_state = bagState;
        }
    }
}