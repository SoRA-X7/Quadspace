using System.Collections.Generic;

namespace Quadspace.Quadspace.TBP.Messages {
    public class TbpInfoMessage : TbpBotMessage {
        public string name;
        public string version;
        public string author;
        public List<string> features;
    }
}