namespace Quadspace.TBP.Messages {
    public class TbpNewPieceMessage : TbpFrontendMessage {
        public string piece;

        public TbpNewPieceMessage(string piece) : base(FrontendMessageType.new_piece) {
            this.piece = piece;
        }
    }
}