using System.Collections.Generic;
using Quadspace.TBP.Randomizer;

namespace Quadspace.TBP.Messages {
    public class TbpStartMessage : TbpFrontendMessage {
        public string hold;
        public List<string> queue;
        public int combo;
        public bool back_to_back;
        public List<string[]> board;
        public SevenBagRandomizerStart randomizer; //TODO
        public TbpStartMessage() : base(FrontendMessageType.start) { }
    }
}