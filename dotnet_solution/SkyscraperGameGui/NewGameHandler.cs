using SkyscraperGameEngine;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;

namespace SkyscraperGameGui;

internal class NewGameHandler(
    GameInterface gameEngine,
    TextBox rngSeedBox,
    TextBox gridSizeBox,
    TextBox gridFillPercentBox,
    TextBox constrFillPercentBox,
    CheckBox allowInFeasibleCheckbox)
{
    readonly MD5 md5 = MD5.Create();

    public void SendNewGameRequest()
    {
        byte[] seedBytes = Encoding.UTF8.GetBytes(rngSeedBox.Text);
        int seed = Math.Abs(BitConverter.ToInt32(md5.ComputeHash(seedBytes), 0));
        if (rngSeedBox.Text == "")
            seed = -1;
        InstanceGenerationOptions options = new()
        {
            RandomSeed = seed,
            AllowInfeasible = allowInFeasibleCheckbox.IsChecked == true
        };
        if (int.TryParse(gridSizeBox.Text, out int size))
            options.Size = size;
        if (double.TryParse(gridFillPercentBox.Text, out double gridFillPercent))
            options.GridFillRate = gridFillPercent / 100;
        if (double.TryParse(constrFillPercentBox.Text, out double constrFillPercent))
            options.ConstraintFillRate = constrFillPercent / 100;
        gameEngine.StartNewGame(options);
    }
}