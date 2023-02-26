using Revanche.GameObjects;

namespace Revanche.Extensions
{
    internal static class ElementalEffectivenessExtensions
    {
        private const float Inf = 0.7f; // was 0.8f
        private const float Sup = 2.1f; // was 1.5f

        public static float ElementalEffectiveness(this ElementType atkElement, ElementType defElement)
        {
            if (atkElement == defElement || atkElement == ElementType.Neutral || defElement == ElementType.Neutral)
            {
                return 1;
            }

            return atkElement switch
            {
                ElementType.Fire => defElement is ElementType.Lightning or ElementType.Magic ? Sup : Inf,
                ElementType.Ghost => defElement is ElementType.Fire or ElementType.Water ? Sup : Inf,
                ElementType.Lightning => defElement is ElementType.Water or ElementType.Ghost ? Sup : Inf,
                ElementType.Magic => defElement is ElementType.Lightning or ElementType.Ghost ? Sup : Inf,
                ElementType.Water => defElement is ElementType.Fire or ElementType.Magic ? Sup : Inf,
                _ => 1
            };
        }

        public static bool ElementIsEffective(this ElementType atkElement, ElementType defElement)
        {
            if (atkElement == defElement || atkElement == ElementType.Neutral || defElement == ElementType.Neutral)
            {
                return false;
            }

            return atkElement switch
            {
                ElementType.Fire => defElement is ElementType.Lightning or ElementType.Magic,
                ElementType.Ghost => defElement is ElementType.Fire or ElementType.Water,
                ElementType.Lightning => defElement is ElementType.Water or ElementType.Ghost,
                ElementType.Magic => defElement is ElementType.Lightning or ElementType.Ghost,
                ElementType.Water => defElement is ElementType.Fire or ElementType.Magic,
                _ => false
            };
        }
    }
}
