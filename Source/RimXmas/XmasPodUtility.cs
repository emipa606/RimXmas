using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RimXmas
{
    // Token: 0x02000002 RID: 2
    public static class XmasPodUtility
    {
        // Token: 0x04000001 RID: 1
        private static readonly List<List<Thing>> tempList = new List<List<Thing>>();

        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        private static void MakeDropPodAt(IntVec3 c, Map map, ActiveDropPodInfo info)
        {
            var activeDropPod = (ActiveDropPod) ThingMaker.MakeThing(ThingDef.Named("ActiveXmasPod"));
            activeDropPod.Contents = info;
            SkyfallerMaker.SpawnSkyfaller(ThingDef.Named("XmasPodIncoming"), activeDropPod, c, map);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002090 File Offset: 0x00000290
        public static void DropThingsNear(IntVec3 dropCenter, Map map, IEnumerable<Thing> things, int openDelay,
            bool canInstaDropDuringInit, bool leaveSlag, bool canRoofPunch)
        {
            tempList.Clear();
            foreach (var item in things)
            {
                var list = new List<Thing> {item};
                tempList.Add(list);
            }

            DropThingGroupsNear(dropCenter, map, tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch);
            tempList.Clear();
        }

        // Token: 0x06000003 RID: 3 RVA: 0x0000214C File Offset: 0x0000034C
        private static void DropThingGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups,
            int openDelay, bool instaDrop, bool leaveSlag, bool canRoofPunch)
        {
            foreach (var list in thingsGroups)
            {
                if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out var intVec, true, canRoofPunch))
                {
                    Log.Warning(string.Concat("DropThingsNear failed to find a place to drop ", list.FirstOrDefault(),
                        " near ", dropCenter, ". Dropping on random square instead."));
                    intVec = CellFinderLoose.RandomCellWith(c => c.Walkable(map), map);
                }

                foreach (var thing in list)
                {
                    thing.SetForbidden(true, false);
                }

                if (instaDrop)
                {
                    foreach (var thing in list)
                    {
                        GenPlace.TryPlaceThing(thing, intVec, map, ThingPlaceMode.Near);
                    }
                }
                else
                {
                    var activeDropPodInfo = new ActiveDropPodInfo();
                    foreach (var item in list)
                    {
                        activeDropPodInfo.innerContainer.TryAdd(item);
                    }

                    activeDropPodInfo.openDelay = openDelay;
                    activeDropPodInfo.leaveSlag = leaveSlag;
                    MakeDropPodAt(intVec, map, activeDropPodInfo);
                }
            }
        }
    }
}