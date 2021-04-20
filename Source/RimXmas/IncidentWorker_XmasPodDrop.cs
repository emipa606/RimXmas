using RimWorld;
using Verse;

namespace RimXmas
{
    // Token: 0x02000005 RID: 5
    public class IncidentWorker_XmasPodDrop : IncidentWorker_ResourcePodCrash
    {
        private Map lastMap;

        // Token: 0x04000003 RID: 3
        private int lastYear;

        // Token: 0x06000007 RID: 7 RVA: 0x0000238C File Offset: 0x0000058C
        private bool Able(IncidentParms parms)
        {
            var map = (Map) parms.target;
            bool result;
            if (lastYear == GenLocalDate.Year(map) && lastMap == map)
            {
                result = false;
            }
            else
            {
                var num = GenLocalDate.DayOfYear(map);
                //Log.Message("day of year = " + num + ".");
                if (num < 45)
                {
                    result = false;
                }
                else if (GenCelestial.CurCelestialSunGlow(map) >= 0.3f)
                {
                    result = false;
                }
                else
                {
                    lastYear = GenLocalDate.Year(map);
                    lastMap = map;
                    result = true;
                }
            }

            return result;
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002418 File Offset: 0x00000618
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result;
            if (!Able(parms))
            {
                result = false;
            }
            else
            {
                var activeXmasPodDef = (ActiveXmasPodDef) ThingDef.Named("ActiveXmasPod");
                if (activeXmasPodDef == null)
                {
                    Log.ErrorOnce("[RimXmas]: ActiveXmasPod was not defined!", "ActiveXmasPod".GetHashCode());
                    result = false;
                }
                else if (activeXmasPodDef.xmasContents == null)
                {
                    Log.ErrorOnce("[RimXmas]: xmasContents was not defined in ActiveXmasPod!", "xmasContents".GetHashCode());
                    result = false;
                }
                else
                {
                    var things = activeXmasPodDef.xmasContents.root.Generate();
                    var map = (Map) parms.target;
                    var intVec = DropCellFinder.RandomDropSpot(map);
                    XmasPodUtility.DropThingsNear(intVec, map, things, 110, false, true, true);
                    Find.LetterStack.ReceiveLetter("LetterLabelXmasPodDrop".Translate(), "XmasPodDrop".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map));
                    result = true;
                }
            }

            return result;
        }
    }
}