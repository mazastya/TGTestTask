using TestTaskTG.Models;

namespace TestTaskTG.Exceptions;

public class NewGameValidationException(string message) : Exception(message);

public static class NewGameValidator
{
    public static void ValidateWidth(NewGameRequest newGameRequest)
    {
        if (newGameRequest.width is < 2 or > 30)
            throw new NewGameValidationException("Error: Ширина поля должна быть не менее 2 и не более 30");
    }

    public static void ValidateHeight(NewGameRequest newGameRequest)
    {
        if (newGameRequest.height is < 2 or > 30)
            throw new NewGameValidationException("Error: Высота поля должна быть не менее 2 и не более 30");
    }

    public static void ValidateMinesCount(NewGameRequest newGameRequest)
    {
        if (newGameRequest.mines_count < 1 || newGameRequest.mines_count > newGameRequest.width * newGameRequest.height - 1)
            throw new NewGameValidationException($"Error: Количество мин должно быть не менее 1 и не более {newGameRequest.width * newGameRequest.height - 1}");
    }
}