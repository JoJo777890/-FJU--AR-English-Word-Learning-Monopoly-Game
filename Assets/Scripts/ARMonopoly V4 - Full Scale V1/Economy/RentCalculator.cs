using ARMonopoly_V4___Full_Scale_V1.Board;

namespace ARMonopoly_V4___Full_Scale_V1.Economy
{
    public static class RentCalculator
    {
        public static int Compute (PropertyDef def, int house, int diceTotal)
        {
            int rent = 0; // tmp

            rent = def.rentTiers[house];
        
            return rent;
        }
    }
}
