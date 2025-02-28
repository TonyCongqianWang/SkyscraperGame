using SkyscraperGameEngine;

namespace SkyscraperGameGui;

class ConstraintCheckHandler(MainWindow mainWindow, GameEngine gameEngine)
{
    public Action CreateButtonCallback(string constraintPosition, int size)
    {
        int constraintIndex;
        if (constraintPosition.Contains("top"))
            constraintIndex = 0;
        else if (constraintPosition.Contains("bottom"))
            constraintIndex = size;
        else if (constraintPosition.Contains("left"))
            constraintIndex = 2 * size;
        else if (constraintPosition.Contains("right"))
            constraintIndex = 3 * size;
        else
            return () => { };
        constraintIndex += int.Parse(constraintPosition.Split('_')[^1]);
        return () => ExecuteCheck(constraintIndex);
    }

    public void ExecuteCheckAll()
    {
        GameStateViewModel gameStateModel = gameEngine.GetState();
        int size = gameStateModel.Size;
        int cIdx = 0;
        for (; cIdx < 1 * size; cIdx++)
        {
            if (gameStateModel.TopValueNeedsCheckArray[cIdx % size])
                ExecuteCheck(cIdx);
        }
        for (; cIdx < 2 * size; cIdx++)
        {
            if (gameStateModel.BottomValueNeedsCheckArray[cIdx % size])
                ExecuteCheck(cIdx);
        }
        for (; cIdx < 3 * size; cIdx++)
        {
            if (gameStateModel.LeftValueNeedsCheckArray[cIdx % size])
                ExecuteCheck(cIdx);
        }
        for (; cIdx < 4 * size; cIdx++)
        {
            if (gameStateModel.RightValueNeedsCheckArray[cIdx % size])
                ExecuteCheck(cIdx);
        }
    }

    private void ExecuteCheck(int constraintIndex)
    {
        gameEngine.TryCheckConstraint(constraintIndex);
        mainWindow.RenderGame();
    }


}
