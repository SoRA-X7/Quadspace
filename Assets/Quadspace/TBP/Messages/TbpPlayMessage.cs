namespace Quadspace.TBP.Messages {
    public class TbpPlayMessage : TbpFrontendMessage {
        public TbpMove move;

        public TbpPlayMessage(TbpMove move) : base(FrontendMessageType.play) {
            this.move = move;
        }
    }
}