using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper
{
    interface IView
    {
        void Draw();
        bool ProcessUserInput(string input);
    }

    class OptionsView : IView
    {
        int _Columns = 0;
        int _Rows = 0;
        int _Bombs = 0;
        public void Draw()
        {
            if (_Columns == 0)
                Console.Write("Lines (2 - 100):");
            else if (_Rows == 0)
                Console.Write("Rows (2 - 100):");
            else if (_Bombs == 0)
                Console.Write(String.Format("Bombs (1 - {0}):", _Rows * _Columns - 1));
        }

        public bool ProcessUserInput(string input)
        {
            try
            {
                int value = Convert.ToInt32(input);
                if (value < 2 || value > 100)
                    throw new Exception();
                if (_Columns == 0)
                {
                    _Columns = value;
                }
                else if (_Rows == 0)
                {
                    _Rows = value;
                }
                else
                {
                    _Bombs = value;
                    setOptions();
                    ViewManager.Instance.Remove(this);
                    return true;
                }
            }
            catch
            {
                Console.WriteLine("Invalid value, try again.");
                return false;
            }
            return false;
        }

        void setOptions()
        {
            MinesweeperConsoleApp.Game.UpdateOptions(new Options { Bombs = _Bombs, Columns = _Columns, Rows = _Rows });
        }
    }

    class GameView : IView
    {
        public GameView()
        {

        }

        public void Draw()
        {
            Console.WriteLine("State:" + MinesweeperConsoleApp.Game.State);
            Console.WriteLine("Time: " +MinesweeperConsoleApp.Game.Time);

            for (int y = -2; y < MinesweeperConsoleApp.Game.Options.Rows; y++)
            {
                Console.Write(((y > -1) ? y.ToString() + "|" : "  "));

                for (int x = 0; x < MinesweeperConsoleApp.Game.Options.Columns; x++)
                {
                    if (y == -2)
                    {
                        Console.Write(x + " ");
                    }
                    else if (y == -1)
                    {
                        Console.Write(x == MinesweeperConsoleApp.Game.Options.Columns - 1 ? "_" : "__");
                    }
                    else
                    {
                        Cell cell = MinesweeperConsoleApp.Game.Cell(x, y);
                        drawCell(cell);
                    }
                }
                Console.WriteLine();
            }

            if (MinesweeperConsoleApp.Game.State == GameState.Inited || MinesweeperConsoleApp.Game.State == GameState.Playing)
                Console.WriteLine("Your turn (format is [<action_char> <x> <y>]. <action_char> is 'O' (Open) or 'B' (Mark as bomb / unmark). E.g. 'O 2 3' or 'B 6 7'): ");
        }

        public void drawCell(Cell cell)
        {
            char cellChar = ' ';
            switch(cell.State)
            {
                case FieldState.Closed:
                    cellChar = 'H';
                    break;
                case FieldState.Opened:
                    cellChar = cell.HasBomb ? 'B' : (cell.NeighborBobmsCount == 0 ? ' ' : cell.NeighborBobmsCount.ToString().ElementAt(0));
                    break;
                case FieldState.Explosed:
                    cellChar = 'X';
                    break;
                case FieldState.MarkedAsBomb:
                    cellChar = 'M';
                    break;
            }
            Console.Write(cellChar + " ");
        }

        public bool ProcessUserInput(string input)
        {
            try
            {
                var arr = input.Split(' ');

                int x = Convert.ToInt16(arr[1]);
                int y = Convert.ToInt16(arr[2]);
                char action = Convert.ToChar(arr[0].ToUpper());
                switch(action)
                {
                    case 'O':
                        MinesweeperConsoleApp.Game.OpenCell(x, y);
                        break;
                    case 'B':
                        Cell cell = MinesweeperConsoleApp.Game.Cell(x, y);
                        if (cell.State == FieldState.Closed)
                            MinesweeperConsoleApp.Game.MarkAsBomb(x, y);
                        else if (cell.State == FieldState.MarkedAsBomb)
                            MinesweeperConsoleApp.Game.UnMarkAsBomb(x, y);
                        break;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return true;
        }
    }

    class Menu
    {
        public void Draw()
        {
            Console.WriteLine("[N]ew game o[P]tions [E]xit");
        }

        public bool ProcessUserInput(string input)
        {
            switch(input.ToUpper())
            {
                case "P":
                    ViewManager.Instance.Add(new OptionsView());
                    return true;
                case "E":
                    ViewManager.Instance.CloseAllViews();
                    return true;
                case "N":
                    MinesweeperConsoleApp.Game.New();
                    return true;
            }
            return false;
        }
    }

    class ViewManager
    {
        ViewManager() { }
        static ViewManager _Instance;
        public static ViewManager Instance
        {
            get { return _Instance ?? (_Instance = new ViewManager()); }
        }
        List<IView> _Viewes = new List<IView>();
        public void Add(IView view)
        {
            _Viewes.Add(view);
        }
        public void Remove(IView view)
        {
            if (_Viewes.Contains(view))
                _Viewes.Remove(view);
        }
        public IView CurrentView
        {
            get
            {
                return _Viewes.Count > 0 ? _Viewes.Last() : null;
            }
        }
        public void CloseAllViews()
        {
            _Viewes.Clear();
        }
    }

    class MinesweeperConsoleApp
    {
        static void Main(string[] args)
        {
            MinesweeperConsoleApp app = new MinesweeperConsoleApp();
            while (app.Run())
            {

            }
        }
        static MinesweeperGame _Game = null;
        public static MinesweeperGame Game
        {
            get { return _Game ?? (_Game = new MinesweeperGame()); }
        }
        private void Game_Tick(int time)
        {
            Debug.WriteLine("Time: " + time);
            IView curView = ViewManager.Instance.CurrentView as GameView;
            if (curView == null)
                return;

            //todoL update counter
        }

        private void Game_GameOver(bool isWin)
        {
            Debug.WriteLine(String.Format("Game over: You {0}!", isWin ? "win" : "lose"));
        }

        private void Game_CellStateChange(int x, int y)
        {
            Cell cell = _Game.Cell(x, y);
            Debug.WriteLine(String.Format("Game_CellStateChange: new state is {0}.", cell.State));
        }

        Options _Options = null;
        Menu _Menu = null;
        MinesweeperConsoleApp()
        {
            _Menu = new Menu();
            _Options = Options.Beginner;
            _Game = new MinesweeperGame();
            _Game.CellStateChange += Game_CellStateChange;
            _Game.GameOver += Game_GameOver;
            _Game.Tick += Game_Tick;
            ViewManager.Instance.Add(new GameView());
            _Game.New();
        }


        bool _NeedClearScreen = true;

        void draw()
        {
            IView curView = ViewManager.Instance.CurrentView;
            if (_NeedClearScreen)
            {
                Console.Clear();
                _Menu.Draw();
            }
            curView.Draw();

        }
        public bool Run()
        {
            IView curView = ViewManager.Instance.CurrentView;
            if (curView == null)
                return false;
            draw();
            string userInput = Console.ReadLine();
            if (!_Menu.ProcessUserInput(userInput))
                _NeedClearScreen = curView.ProcessUserInput(userInput);
            return true;
        }
    }
}
