namespace Quadspace.Quadspace.TBP.Messages {
    public class TbpFrontendMessage {
        public FrontendMessageType type;

        public TbpFrontendMessage(FrontendMessageType type) {
            this.type = type;
        }
    }
}