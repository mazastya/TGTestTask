using TestTaskTG.Exceptions;
using TestTaskTG.Models;

namespace TestTaskTG.Services
{
    public class MinesweeperService
    {
        private static readonly Dictionary<string, GameData> Games = new Dictionary<string, GameData>();

        public GameInfoResponse NewGame(NewGameRequest newGameRequest)
        {
            NewGameValidator.ValidateWidth(newGameRequest);
            NewGameValidator.ValidateHeight(newGameRequest);
            NewGameValidator.ValidateMinesCount(newGameRequest);

            var field = new char[newGameRequest.height, newGameRequest.width];

            for (var i = 0; i < newGameRequest.height; i++)
            for (var j = 0; j < newGameRequest.width; j++)
                field[i, j] = ' ';
            var gameId = Guid.NewGuid().ToString("D");

            var gameData = new GameData()
            {
                Response = new GameInfoResponse()
                {
                    GameId = gameId,
                    Width = newGameRequest.width,
                    Height = newGameRequest.height,
                    MinesCount = newGameRequest.mines_count,
                    IsCompleted = false,
                    Field = field
                },
                MemoryArray = new bool[newGameRequest.height, newGameRequest.width],
                MinesCount = newGameRequest.mines_count,
                HiddenField = new char[newGameRequest.height, newGameRequest.width]
            };

            Games.Add(gameId, gameData);

            return gameData.Response;
        }

        public GameInfoResponse Turn(GameTurnRequest gameTurnRequest)
        {
            var game = Games[gameTurnRequest.game_id];
            var col = gameTurnRequest.col;
            var row = gameTurnRequest.row;

            if (game.Response.IsCompleted)
            {
                throw new Exception("Игра завершена");
            }

            if (game.HiddenField[row, col] == 'X')
            {
                game.Response.IsCompleted = true;
                EndGame(game);
                game.Response.Field = game.HiddenField;
                return game.Response;
            }

            if (game.IsNewGame)
            {
                GenerateBombs(game.Response.Height, game.Response.Width, game, row, col);
                game.IsNewGame = false;
            }

            if (game.Response.Field[row, col] != ' ')
            {
                throw new Exception("Эта ячейка уже открыта, выберите другую!");
            }

            if (gameTurnRequest.game_id != game.Response.GameId) throw new Exception("GameId Error");
            Turn(row, col, game);

            Array.Copy(game.HiddenField, game.Response.Field, game.HiddenField.Length);
            for (var i = 0; i < game.Response.Height; i++)
            for (var j = 0; j < game.Response.Width; j++)
            {
                if (game.Response.Field[i, j] == 'X')
                    game.Response.Field[i, j] = ' ';
            }

            CheckWinGame(game);

            return game.Response;

        }

        private static void Turn(int row, int col, GameData game)
        {
            while (true)
            {
                if (row < 0 || col < 0 || row > game.Response.Height - 1 || col > game.Response.Width - 1) return;
                if (game.MemoryArray[row, col]) return;

                game.MemoryArray[row, col] = true;

                if (game.HiddenField[row, col] == 'X') return;

                var bombsCount = CalculateNearbyBombs(row, col, game);
                if (bombsCount == 0)
                {
                    game.HiddenField[row, col] = Convert.ToChar(bombsCount.ToString());
                    Turn(row, col + 1, game);
                    Turn(row + 1, col, game);
                    Turn(row - 1, col, game);
                    Turn(row, col - 1, game);
                    Turn(row - 1, col - 1, game);
                    Turn(row + 1, col + 1, game);
                    Turn(row + 1, col - 1, game);
                    row = row - 1;
                    col = col + 1;
                    continue;
                }

                game.HiddenField[row, col] = Convert.ToChar(bombsCount.ToString());
                break;
            }
        }

        private static int CalculateNearbyBombs(int row, int col, GameData game)
        {
            var bombQty = 0;

            if (CheckBomb(row, col + 1, game))
                bombQty++;
            if (CheckBomb(row + 1, col, game))
                bombQty++;
            if (CheckBomb(row - 1, col, game))
                bombQty++;
            if (CheckBomb(row, col - 1, game))
                bombQty++;
            if (CheckBomb(row - 1, col - 1, game))
                bombQty++;
            if (CheckBomb(row + 1, col + 1, game))
                bombQty++;
            if (CheckBomb(row + 1, col - 1, game))
                bombQty++;
            if (CheckBomb(row - 1, col + 1, game))
                bombQty++;

            return bombQty;
        }

        private static bool CheckBomb(int row, int col, GameData gameData)
        {
            return row >= 0 && col >= 0 && row <= gameData.Response.Height - 1 && col <= gameData.Response.Width - 1 &&
                   gameData.HiddenField[row, col] == 'X';
        }

        private static void GenerateBombs(int height, int width, GameData game, int row, int col)
        {
            var counter = 0;

            var randomArray = new int[width * height];

            for (var i = 0; i < width * height; i++)
            {
                randomArray[i] = i;
            }

            var rnd = new Random();
            rnd.Shuffle(randomArray);

            var randomBombsIndexes = new HashSet<int>();
            var minesCount = game.MinesCount;

            for (var i = 0; i < minesCount; i++)
            {
                randomBombsIndexes.Add(randomArray[i]);
            }

            if (randomBombsIndexes.Contains(col + row * width))
            {
                randomBombsIndexes.Remove(col + row * width);
                randomBombsIndexes.Add(randomArray[minesCount++]);
            }

            for (var i = 0; i < height; i++)
            for (var j = 0; j < width; j++)
            {
                game.MemoryArray[i, j] = false;
                if (randomBombsIndexes.Contains(counter))
                {
                    game.HiddenField[i, j] = 'X';
                }

                if (game.HiddenField[i, j] != 'X')
                    game.HiddenField[i, j] = ' ';
                counter++;
            }
        }

        private void EndGame(GameData gameData)
        {
            for (var i = 0; i < gameData.Response.Height; i++)
            for (var j = 0; j < gameData.Response.Width; j++)
            {
                if (i < 0 || j < 0 || i > gameData.Response.Height - 1 || j > gameData.Response.Width - 1)
                    continue;
                if (gameData.MemoryArray[i, j])
                    continue;

                gameData.MemoryArray[i, j] = true;
                var bombsCount = CalculateNearbyBombs(i, j, gameData);
                if (gameData.HiddenField[i, j] != 'X')
                    gameData.HiddenField[i, j] = Convert.ToChar(bombsCount.ToString());
            }
        }

        private void CheckWinGame(GameData gameData)
        {
            var endGame = true;
            for (var i = 0; i < gameData.Response.Height; i++)
            for (var j = 0; j < gameData.Response.Width; j++)
            {
                if (gameData.HiddenField[i, j] == ' ')
                    endGame = false;
            }

            if (!endGame) return;
            {
                EndGame(gameData);

                for (var i = 0; i < gameData.Response.Height; i++)
                for (var j = 0; j < gameData.Response.Width; j++)
                {
                    if (gameData.HiddenField[i, j] == 'X')
                        gameData.HiddenField[i, j] = 'M';
                }

                gameData.Response.Field = gameData.HiddenField;
                gameData.Response.IsCompleted = true;
            }
        }
    }
}