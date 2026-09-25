using System.Drawing;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TicTacToe;

/// <summary>
/// Numerical Tic Tac Toe on an n x n board. The players share the numbers
/// 1..n^2 and play them in order; whoever completes a line of n numbers adding
/// up to the board's target sum wins, no matter who played the other numbers.
/// </summary>
public sealed class NumericalTTTGame : Game, IGame
{
    public NumberLists Numbers { get; }
    public int _selectedNumber;
    public NumericalTTTGame(IPlayer playerOne, IPlayer playerTwo, int size = 3)
        : base(playerOne, playerTwo, new Board(size))
    {
        Numbers = new NumberLists(Boards[0].Size, playerOne);
        Console.WriteLine("You are playing NEW Numerical TTT");

        IPlayer currentPlayer = MoveCount % 2 == 0 ? PlayerOne : PlayerTwo;
        int number = AskForNumber(currentPlayer, Boards[0]);
    }


public int AskForNumber(IPlayer player, Board board)
    {
        int turnValue; 

        Console.WriteLine("What number value do you want?");
        while (true)
        {
            Console.WriteLine("Available numbers are " + string.Join(", ", Numbers.GetPlayerList(player)));
            string input = Console.ReadLine();

            if (int.TryParse(input, out turnValue) && turnValue > 0 && Numbers.UsedNumbers(player, turnValue))  // Check whether is an int, and whether in correct range // 
                break; //The loop ends on true
            Console.WriteLine("Incorrect value, try again.");
        }
        _selectedNumber = turnValue;
        return turnValue;
    }
    

    public override GameType Type => GameType.NumericalTicTacToe;
    public bool IsLegal(Placement p)
    {
        Board board = Boards[p.BoardIndex];
        return board.GetCell(p.Row, p.Column) is null; // true ( legal)  only if the cell is empty

    }

    public MoveOutcome PlayMove(Placement p)
    {
        throw new NotImplementedException();
    }

    public void Apply(Placement p)
    {
        Board board = Boards[p.BoardIndex];
        board.PlacePiece(p.Row, p.Column, new Piece(_selectedNumber));
    }

    public MoveOutcome Evaluate(Placement p)
    {
        Board board = Boards[p.BoardIndex];

        if (Lines(board, board.Size).Any(line => IsMatch(board, line)))
        {
            return MoveOutcome.CurrentPlayerWins;
        }

        // The numbers run out exactly when the board fills.
        return board.IsFull() ? MoveOutcome.Draw : MoveOutcome.Continue;
    }

    /// <summary>True if every cell in the line is filled and they add up to the target sum.</summary>
    private static bool IsMatch(Board board, (int Row, int Column)[] line)
    {
        int sum = 0;

        foreach ((int row, int column) in line)
        {
            Piece? piece = board.GetCell(row, column);
            if (piece is null)
            {
                return false;
            }

            sum += piece.Value;
        }

        return sum == board.TargetSum;
    }
}

public class NumberLists // Number Lists Class //
{
    private readonly IPlayer _playerOne;
    public List<int> PlayerOneList { get; } //player one list (evens)
    public List<int> PlayerTwoList { get; } //player two list (odds)
    public List<int> GetPlayerList(IPlayer player) => player == _playerOne ? PlayerOneList : PlayerTwoList;
    public NumberLists(int boardSize, IPlayer playerOne)
    {
        _playerOne = playerOne;

        PlayerOneList = new List<int>(); //list constructors//
        PlayerTwoList = new List<int>();
        for (int i = 1; i <= (boardSize * boardSize); i++)
        {
            if (i % 2 == 0)
                PlayerTwoList.Add(i); // Adds evens to PlayerTwoList
            else
                PlayerOneList.Add(i); // Adds odds to PlayerOneList
        }
    }
    public bool UsedNumbers(IPlayer player, int number) // Method to check and remove the number from numlist //
    {
        List<int> list = GetPlayerList(player);
        if (list.Contains(number))
        {
            list.Remove(number);
            return true;  // Able to use this number, and remove
        }
        else return false; // Not able to use this number ( already used)
    }

    void ShowHelp()
    {
        Console.WriteLine();
        // Console.WriteLine("How to play Numerical Tic Tac Toe");
        // Console.WriteLine("---------------------------------");
        // Console.WriteLine("The board is an n x n grid played with the numbers 1 to n^2.");
        // Console.WriteLine("Player One has Odd numbers, Player 2 has even Numbers. Player 1 moves first.");
        // Console.WriteLine();
        // Console.WriteLine("Players take turns placing available numbers in an empty cell. The");
        // Console.WriteLine("first to complete a row, column or diagonal whose numbers add up");
        // Console.WriteLine("to n(n^2 + 1) / 2 (15 on a 3x3 board) wins — no matter who played");
        // Console.WriteLine("the other numbers in that line.");
        // Console.WriteLine();
        // Console.WriteLine("On your turn:");

    }
}

