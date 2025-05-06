using FLS;
using FLS.Rules;

namespace fuzzy_logic_model_generator
{
    public class FuzzyLogicModelGenerator
    {
        private const int ON_VALUE = 40;

        private IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();
        public FuzzyLogicModelGenerator()
        {
            var modelYearsOld = new LinguisticVariable("ModelYearsOld");
            var young = modelYearsOld.MembershipFunctions.AddTrapezoid("Young", 0, 0, 20, 40);
            var medium = modelYearsOld.MembershipFunctions.AddTriangle("Medium", 30, 50, 70);
            var old = modelYearsOld.MembershipFunctions.AddTrapezoid("Old", 50, 80, 10000, 10000);

            var pieces = new LinguisticVariable("Pieces");
            var small = pieces.MembershipFunctions.AddTrapezoid("Small", 0, 0, 200, 400);
            var huge = pieces.MembershipFunctions.AddTrapezoid("Huge", 500, 800, 5000, 5000);


            var power = new LinguisticVariable("Generator");
            var off = power.MembershipFunctions.AddTriangle("Off", 0, 10, 20);
            var on = power.MembershipFunctions.AddTriangle("On", ON_VALUE - 10, ON_VALUE, ON_VALUE + 10);

            var rule1 = Rule.If(modelYearsOld.Is(young).And(pieces.Is(small))).Then(power.Is(off));
            //var rule2 = Rule.If(modelYearsOld.Is(medium).And(pieces.Is(small))).Then(power.Is(off));
            var rule3 = Rule.If(modelYearsOld.Is(old).Or(pieces.Is(huge))).Then(power.Is(on));
            //var rule4 = Rule.If(pieces.Is(huge)).Then(power.Is(on));

            fuzzyEngine.Rules.Add(rule1, rule3);
        }
       public bool main(int modelYearsOldInMinutes, int amountOfModelPieces)
        {
            var result = fuzzyEngine.Defuzzify(
                new {
                    ModelYearsOld = modelYearsOldInMinutes,
                    Pieces = amountOfModelPieces});

            return (result / ON_VALUE) > 0.5d;
        }
    }
}
