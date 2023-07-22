using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 100000 };

    Board board;

    //Relative to whose move it is. So if it's black's move, it will return + if black is winning
    private int getBoardValue()
    {
        int value = 0;

        PieceList[] piecelists = board.GetAllPieceLists();
        
        foreach (PieceList piece_list in piecelists)
        {
            int multiplier = piece_list.IsWhitePieceList ? 1 : -1;

            foreach (Piece piece in piece_list)
            {
                value += multiplier * pieceValues[(int)piece.PieceType];
            }
        }
        return value * (board.IsWhiteToMove ? 1 : -1);
    }

    private int quiesce(int alpha, int beta)
    {
        int stand_pat = getBoardValue();

        if (stand_pat >= beta) return beta;
        if (alpha < stand_pat) alpha = stand_pat;

        Move[] moves = board.GetLegalMoves(true);

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = -quiesce(-beta, -alpha);
            board.UndoMove(move);

            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }

        return alpha;
    }

    private int alphaBeta(int alpha, int beta, uint depth)
    {
        if (depth == 0) return quiesce(alpha, beta);

        Move[] moves = board.GetLegalMoves();

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = -alphaBeta(-beta, -alpha, depth - 1);
            board.UndoMove(move);


            if (score >= beta) return beta;
            if (score > alpha) alpha = score;

        }

        return alpha;
    }

 

    private Move rootNegaMax(uint depth)
    {
        Move[] moves = board.GetLegalMoves();
        Move best_move = moves[0];

        int score = -100000;
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int move_score = -alphaBeta(-100000, 100000, depth);
            board.UndoMove(move);

            if (move_score > score)
            {
                score = move_score;
                best_move = move;
            }
        }

        return best_move;
    }


    public Move Think(Board board, Timer timer)
    {
        this.board = board;
        
        //Low on time
        if (timer.MillisecondsRemaining < 10000)
        {
            return rootNegaMax(2);
        } else
        {
            return rootNegaMax(3);
        }

    }
}
