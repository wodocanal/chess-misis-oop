namespace Model.Core;

public enum MoveExecutionResult {
    Success,
    CancelledSelection,
    InvalidSource,
    InvalidTarget,
    WrongTurn,
    GameFinished,
}
