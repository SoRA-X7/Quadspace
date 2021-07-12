namespace Quadspace.TBP.Messages {
    public class TbpRulesMessage : TbpFrontendMessage {
        public string randomizer = "seven_bag"; // TODO

        public TbpRulesMessage() : base(FrontendMessageType.rules) { }
    }
}