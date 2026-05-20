namespace Model.Core;

public sealed class Move
{
	public Move(PieceType pieceType, PieceColor pieceColor, Position from, Position to, PieceType? capturedPieceType = null)
	{
		PieceType = pieceType;
		PieceColor = pieceColor;
		From = from;
		To = to;
		CapturedPieceType = capturedPieceType;
	}

	public PieceType PieceType { get; }

	public PieceColor PieceColor { get; }

	public Position From { get; }

	public Position To { get; }

	public PieceType? CapturedPieceType { get; }

	public bool IsReverseOf(Move other)
	{
		return PieceType == other.PieceType
			&& PieceColor == other.PieceColor
			&& From == other.To
			&& To == other.From
			&& CapturedPieceType is null
			&& other.CapturedPieceType is null;
	}
}
