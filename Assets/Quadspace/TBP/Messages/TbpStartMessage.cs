using System.Collections.Generic;

namespace Quadspace.Quadspace.TBP.Messages {
    public class TbpStartMessage : TbpFrontendMessage {
        public string hold;
        public List<string> queue;
        public int combo;
        public bool back_to_back;
        public List<string[]> board;
        public TbpStartMessage() : base(FrontendMessageType.start) { }
    }
}