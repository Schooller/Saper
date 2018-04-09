using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;


namespace Saper
{
    public struct Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public enum FieldState
    {
        Closed,
        MarkedAsBomb,
        Explosed,
        Opened,
    }

    public class Cell
    {
        public FieldState State { get; set; }
        public bool HasBomb { get; set; }
        public int NeighborBobmsCount { get; set; }
    }

    public class Field
    {
        Field() { }
        List<Cell> _Cells = new List<Cell>();
        public int Colunms { get; private set; }
        public int Rows { get; private set; }

        public List<Coordinate> Neighbors(Coordinate coordinate)
        {
            List<Coordinate> list = new List<Coordinate>();
            if (coordinate.X > 0)
                list.Add(new Coordinate { X = coordinate.X - 1, Y = coordinate.Y });
            if (coordinate.X < (Colunms - 1))
                list.Add(new Coordinate { X = coordinate.X + 1, Y = coordinate.Y });
            if (coordinate.Y > 0)
                list.Add(new Coordinate { X = coordinate.X, Y = coordinate.Y - 1 });
            if (coordinate.Y < (Rows - 1) )
                list.Add(new Coordinate { X = coordinate.X, Y = coordinate.Y + 1 });
            if (coordinate.X > 0 && coordinate.Y > 0)
                list.Add(new Coordinate { X = coordinate.X - 1, Y = coordinate.Y - 1 });
            if (coordinate.X > 0 && coordinate.Y < (Rows - 1))
                list.Add(new Coordinate { X = coordinate.X - 1, Y = coordinate.Y + 1 });
            if (coordinate.X < (Colunms - 1) && coordinate.Y > 0 )
                list.Add(new Coordinate { X = coordinate.X + 1, Y = coordinate.Y - 1 });
            if (coordinate.X < (Colunms - 1) && coordinate.Y < (Rows - 1) )
                list.Add(new Coordinate { X = coordinate.X + 1, Y = coordinate.Y + 1 });
            return list;
        }


        public int ClosedCellsNumber
        {
            get
            {
                return _Cells.Count((cell) => 
                {
                    return cell.State == FieldState.Closed; 
                });
            }
        }

        public int MarkedAsBombCellsNumber
        {
            get
            {
                return _Cells.Count((cell) =>
                {
                    return cell.State == FieldState.MarkedAsBomb;
                });
            }
        }


        public static Field Create(Options options)
        {
            return Create(options.Columns, options.Rows, options.Bombs);
        }

        public static Field Create(int colunms, int rows, int bombs)
        {
            if (colunms < 1 || rows < 1)
                throw new ArgumentException();
            Field instance = new Field
            {
                Colunms = colunms,
                Rows = rows
            };
            instance.createCells();
            instance.putBombs(bombs);
            instance.calcNeighborBombs();
            return instance;
        }

        void calcNeighborBombs()
        {
            for(int i = 0; i < _Cells.Count; i++)
            {
                fillNeighborBombsNumber(index2Coordinate(i));
            }
        }

        void fillNeighborBombsNumber(Coordinate coordinate)
        {
            Cell(coordinate).NeighborBobmsCount = Neighbors(coordinate).Count((neighbor)=> 
            {
                return Cell(neighbor).HasBomb;
            });
        }

        void putBombs(int bombs)
        {
            if (bombs < 1 || bombs >= Colunms * Rows)
                throw new ArgumentException();

            Random rand = new Random(DateTime.Now.Millisecond);

            do
            {
                int index = rand.Next(_Cells.Count);
                if (!_Cells[index].HasBomb)
                {
                    _Cells[index].HasBomb = true;
                    bombs--;
                }
            }
            while (bombs > 0);
        }

        void createCells()
        {
            _Cells.Clear();
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Colunms; x++)
                {
                    _Cells.Add(new Cell());
                }
            }
        }

        public Cell Cell(int x, int y)
        {
            return _Cells[coordinate2Index(new Coordinate { X = x, Y = y })];
        }

        public Cell Cell(Coordinate coordinate)
        {
            if (coordinate.X < 0 || coordinate.X >= Colunms || coordinate.Y < 0 || coordinate.Y >= Rows)
                throw new ArgumentException("invalid coordinates");
            return _Cells[coordinate2Index(coordinate)];
        }

        int coordinate2Index(Coordinate coordinate)
        {
            return coordinate.X * Colunms + coordinate.Y;
        }
        Coordinate index2Coordinate(int index)
        {
            return new Coordinate { X = index % Colunms, Y = index / Colunms };
        }
    }

    public class Options
    {
        public int Columns { get; set; }
        public int Rows { get; set; }
        public int Bombs { get; set; }
        public static Options Beginner
        {
            get
            {
                return new Options { Bombs = 10, Columns = 10, Rows = 10 };
            }
        }
        public static Options Intermediate
        {
            get
            {
                return new Options { Bombs = 40, Columns = 16, Rows = 16 };
            }
        }
        public static Options Expert
        {
            get
            {
                return new Options { Bombs = 99, Columns = 31, Rows = 16 };
            }
        }
    }

    public enum GameState
    {
        Inited,
        Playing,
        Win,
        Lost
    }

    public class MinesweeperGame
    {
        public MinesweeperGame()
        {
            Options = Options.Beginner;
            New();
        }

        public event Action<bool> GameOver = null;
        public event Action<int, int> CellStateChange = null;
        public event Action<int> Tick = null;

        Field _Field = null;
        
        public Options Options { get; private set; }
        public GameState State { get; private set; }
        int _Ticks = 0;
        Timer _Timer = null;

        public int Time
        {
            get
            {
                return _Ticks;
            }
        }
        
        public void New()
        {
            stopTimer();
            _Field = Field.Create(Options);
            _Ticks = 0;
            State = GameState.Inited;
        }

        public void UpdateOptions(Options options)
        {
            if (options.Bombs < 1 || options.Columns < 1 || options.Bombs >= options.Rows * options.Columns)
                throw new ArgumentException();
            Options = options;
            New();
        }

        public Cell Cell(int x, int y)
        {
            return _Field.Cell(x, y);
        }

        private void TimerTick(Object state)
        {
            _Ticks++;
            if (Tick != null)
            {
                Tick.Invoke(Time);
            }
        }

        void startTimer()
        {
            stopTimer();
            _Timer = new Timer(TimerTick, null, 0, 1000);
        }

        void stopTimer()
        {
            if (_Timer != null)
            {
                _Timer.Dispose();
                _Timer = null;
            }
        }

        public void MarkAsBomb(int x, int y)
        {
            Cell cell = getClickedCell(x, y);
            if (cell == null || cell.State != FieldState.Closed)
                return;

            cell.State = FieldState.MarkedAsBomb;
            onCellStateChange(x, y);

            checkGameOver();
        }

        public void UnMarkAsBomb(int x, int y)
        {
            Cell cell = getClickedCell(x, y);
            if (cell == null || cell.State != FieldState.MarkedAsBomb)
                return;

            cell.State = FieldState.Closed;
            onCellStateChange(x, y);
        }

        Cell getClickedCell(int x, int y)
        {
            if (State == GameState.Win || State == GameState.Lost)
                return null;

            if (State == GameState.Inited)
            {
                State = GameState.Playing;
                startTimer();
            }
            return _Field.Cell(x, y);
        }

        public void OpenCell(int x, int y)
        {
            Cell cell = getClickedCell(x, y);
            if (cell == null || cell.State != FieldState.Closed)
                return;

            if (cell.HasBomb)
            {
                cell.State = FieldState.Explosed;
                onCellStateChange(x, y);
                onGameOver(false);
            }
            else
            {
                cell.State = FieldState.Opened;
                onCellStateChange(x, y);
                if (!checkGameOver() && (cell.NeighborBobmsCount == 0))
                {
                    foreach(Coordinate neighbor in _Field.Neighbors(new Coordinate { X = x, Y = y }))
                    {
                        OpenCell(neighbor.X, neighbor.Y);
                    }
                }
            }
        }

        bool checkGameOver()
        {
            if (_Field.ClosedCellsNumber + _Field.MarkedAsBombCellsNumber == Options.Bombs)
            {
                onGameOver(true);
                return true;
            }
            return false;
        }

        void onCellStateChange(int x, int y)
        {
            if (CellStateChange != null)
                CellStateChange.Invoke(x, y);
        }

        void onGameOver(bool isWin)
        {
            stopTimer();
            if (isWin)
            {
                State = GameState.Win;
            }
            else
            {
                State = GameState.Lost;
            }
            if (GameOver != null)
                GameOver.Invoke(isWin);
        }
    }
}
