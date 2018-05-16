using System;
using System.Collections;
using System.Drawing;
using System.Text;

namespace MassiveDarknessRandomDungeonGenerator
{
    public class DungeonMap
    {
        private ArrayList lstTiles;
        private StringBuilder sbBGMapEditorFileContents = null;
        private Image imgDungeonLayout = null;

        public DungeonMap()
        {
            lstTiles = new ArrayList();
            sbBGMapEditorFileContents = new StringBuilder();
        }

        public void AddTile(Tile newTile)
        {
            lstTiles.Add(newTile);
        }

        public void RandomlyRotateFlipTiles(Die dieRed, Chip chpBridge)
        {
            int iBridgesUsed = 0;
            // Iterate through each tile, manipulating it within the list
            for (int iIndex = 0; iIndex < lstTiles.Count; iIndex++)
            {
                // Roll the die
                int iFace = TokenUtilities.rndGenerator.Next(0, dieRed.Faces.Count);
                DieFace dFace = (DieFace)(dieRed.Faces[iFace]);
                // Act on the results
                Tile currentTile = (Tile)(lstTiles[iIndex]);
                if ((null != currentTile.ReverseSideTile) && (dFace.Bams > 0))
                {
                    // Flip the tile and replaced it in the list
                    currentTile = currentTile.ReverseSideTile;
                    lstTiles[iIndex] = currentTile;
                }
                // Add a bridge if it is not the last tile
                if ((dFace.Diamonds > 0) && (iBridgesUsed < 2) && (iIndex < (lstTiles.Count - 1)))
                {
                    currentTile.Bridge = chpBridge;
                    iBridgesUsed++;
                }
                // Rotate the tile clockwise, based on the number of swords rolled
                currentTile.RotateTile(dFace.Swords);
            }
        }

        public void AddChips(Die dieGreen, Chip chpStart, Chip chpExit, Chip chpDoor)
        {
            // Add the start chip to the first tile
            ((Tile)(lstTiles[0])).Start = chpStart;
            // Add the exit chip to the last tile
            ((Tile)(lstTiles[lstTiles.Count - 1])).Exit = chpExit;

            // Iterate through each tile, adding the appropriate number of doors
            int iTotalDoors = 0;
            for (int iIndex = 0; iIndex < lstTiles.Count; iIndex++)
            {
                // Roll the die
                int iFace = TokenUtilities.rndGenerator.Next(0, dieGreen.Faces.Count);
                DieFace dFace = (DieFace)(dieGreen.Faces[iFace]);
                // Act on the results
                Tile currentTile = (Tile)(lstTiles[iIndex]);
                int iDoorsForThisTile = (0 == dFace.Shields)? 1 : dFace.Shields;
                currentTile.Doors = iDoorsForThisTile;
                iTotalDoors += iDoorsForThisTile;
                currentTile.Door = chpDoor;
            }

            // Can only have a maximum of 15 doors spread across all the tiles.  Randomly remove any extra doors.
            if (15 < iTotalDoors)
            {
                int iDoorsLeftToRemove = iTotalDoors - 15;

                while (0 < iDoorsLeftToRemove)
                {
                    int iTileIndexToRemoveDoor = TokenUtilities.rndGenerator.Next(0, lstTiles.Count);
                    Tile tTileToExamine = (Tile)(lstTiles[iTileIndexToRemoveDoor]);
                    if (1 < tTileToExamine.Doors) // Must leave at least one door
                    {
                        tTileToExamine.Doors -= 1;
                        iDoorsLeftToRemove--;
                    }
                }
            }
        }

        public void AssignLevels(bool bIsEasyDifficulty, Chip cLevel1, Chip cLevel2, Chip cLevel3, Chip cLevel4, Chip cLevel5)
        {
            if (lstTiles.Count < 6)
            {
                for(int iCurrentLevel = 0; iCurrentLevel < lstTiles.Count; iCurrentLevel++)
                {
                    Tile tCurrent = (Tile)(lstTiles[iCurrentLevel]);
                    switch (iCurrentLevel)
                    {
                        case 0:
                            tCurrent.Level = cLevel1;
                            break;
                        case 1:
                            tCurrent.Level = cLevel2;
                            break;
                        case 2:
                            tCurrent.Level = cLevel3;
                            break;
                        case 3:
                            tCurrent.Level = cLevel4;
                            break;
                        case 4:
                            tCurrent.Level = cLevel5;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                int[] aiTileLevels = new int[6] { 0, 1, 1, 1, 1, 1 } ;
                int totalTiles = lstTiles.Count - 5;  // Ignore the first 5 tiles, as we already initialized the array with one level per tile

                // We can have only 1 Level1 in Hard mode, and only 1 Level5 in Easy mode
                int iConstrainedLevel = 1;
                if (bIsEasyDifficulty)
                {
                    iConstrainedLevel = 5;
                }

                // Generate the spread of the levels
                while (totalTiles > 0)
                {
                    // Randomly generate a level between 1 and 5
                    int iLevelIndex = TokenUtilities.rndGenerator.Next(1, 6);
                    // See if we can use it
                    if (aiTileLevels[iLevelIndex] < 2)
                    {
                        if ((iLevelIndex != iConstrainedLevel) || ((iLevelIndex == iConstrainedLevel) && (aiTileLevels[iLevelIndex] < 1)))
                        {
                            aiTileLevels[iLevelIndex]++;
                            totalTiles--;
                        }
                    }
                }

                // Assign the levels to the tiles
                int iLevelToAssign = 0;
                foreach (Tile tCurrentTile in lstTiles)
                {
                    // get the next available level
                    for(iLevelToAssign = 1; iLevelToAssign < aiTileLevels.Length; iLevelToAssign++)
                    {
                        if (aiTileLevels[iLevelToAssign] > 0)
                        {
                            aiTileLevels[iLevelToAssign]--;
                            break;
                        }
                    }
                    if (iLevelToAssign > 5)
                    {
                        iLevelToAssign = 5;
                    }
                    switch (iLevelToAssign)
                    {
                        case 1:
                            tCurrentTile.Level = cLevel1;
                            break;
                        case 2:
                            tCurrentTile.Level = cLevel2;
                            break;
                        case 3:
                            tCurrentTile.Level = cLevel3;
                            break;
                        case 4:
                            tCurrentTile.Level = cLevel4;
                            break;
                        case 5:
                            tCurrentTile.Level = cLevel5;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void PerformLayout(bool bUseFullColorTiles, bool bDrawSummaryLayout, Chip chpBridge, Chip chpLevel1, Chip chpLevel2, Chip chpLevel3, Chip chpLevel4, Chip chpLevel5)
        {
            if ((null == lstTiles) || (0 == lstTiles.Count))
            {
                // No tiles to lay out; return.
                return;
            }

            bool bMoveToNextTile = false;
            Point ptSummaryLayoutLocation = new Point(0, 0);

            // Get the first tile, and set it at 0,0 (no constraints on its placement)
            Tile tCurrentTile = (Tile)(lstTiles[0]);
            tCurrentTile.DungeonLayoutLocation = new Point(0, 0);
            tCurrentTile.LayoutTileLayoutLocation = new Point(0,0);

            // Iterate through each remaining tile, trying to lay it out
            for (int iIndex = 1; iIndex < lstTiles.Count; iIndex++)
            {
                bMoveToNextTile = false;
                Tile tNextTile = (Tile)(lstTiles[iIndex]);

                // Get the list of all available Zone Edges for connecting the next tile
                ArrayList arPossibleZoneEdges = new ArrayList(tCurrentTile.CorridorZoneEdges);
                // Randomly try each one
                while (0 < arPossibleZoneEdges.Count)
                {
                    int iRandomIndex = TokenUtilities.rndGenerator.Next(0, arPossibleZoneEdges.Count);
                    int iZoneEdgeToTry = (int)(arPossibleZoneEdges[iRandomIndex]);

                    // Check if the next tile has an edge zone that matches up
                    // (if the current zone to try is at the BOTTOM of the tile, then the
                    //  next time MUST have a possible connection zone edge at its TOP, etc.)
                    ArrayList arMatchingConnectingEdges = tNextTile.GetMatchingConnectingEdges(iZoneEdgeToTry);
                    if (0 < arMatchingConnectingEdges.Count)
                    {
                        // The next tile has at least one available edge to connect to.
                        // Get the list of all available Zone Edges for connecting the next tile
                        ArrayList arPossibleConnectingZoneEdges = new ArrayList(arMatchingConnectingEdges);
                        // Randomly try each one
                        while (0 < arPossibleConnectingZoneEdges.Count)
                        {
                            int iPossibleConnectingZoneEdgeIndex = TokenUtilities.rndGenerator.Next(0, arPossibleConnectingZoneEdges.Count);
                            int iNextZoneEdgeToTry = (int)(arPossibleConnectingZoneEdges[iPossibleConnectingZoneEdgeIndex]);
                            Point ptZoneEdgeLocation = DetermineEdgeZoneLocation(tCurrentTile, iZoneEdgeToTry);
                            Point ptLayoutTileZoneEdgeLocation = DetermineEdgeZoneLocation(tCurrentTile, iZoneEdgeToTry, false, true);
                            Rectangle recPotentialTileLocation = DetermineBoundingRectangle(tNextTile, ptZoneEdgeLocation, iNextZoneEdgeToTry);
                            Rectangle recPotentialLayoutTileLocation = DetermineBoundingRectangle(tNextTile, ptLayoutTileZoneEdgeLocation, iNextZoneEdgeToTry, true);

                            // The Rectangle now dictates where the 'Next' tile is to be drawn with the selected Zones.
                            // Now see if it fits on the Layout (doesn't overlay any previous tiles already layed out)
                            if (true == DoesTilePlacementFit(tNextTile, recPotentialTileLocation))
                            {
                                // The tile placement fits, so set the connecting zone edge of the current tile and then set the layout location of the 'Next' tile
                                tCurrentTile.ConnectedCorridorZoneEdge = iZoneEdgeToTry;
                                tNextTile.DungeonLayoutLocation = new Point(recPotentialTileLocation.Location.X, recPotentialTileLocation.Location.Y);
                                tNextTile.LayoutTileLayoutLocation = new Point(recPotentialLayoutTileLocation.X, recPotentialLayoutTileLocation.Y);
                                tNextTile.PreviousConnectedCorridorZoneEdge = iNextZoneEdgeToTry;
                                tCurrentTile = tNextTile;
                                bMoveToNextTile = true;
                            }

                            if (true == bMoveToNextTile)
                            {
                                break;
                            }
                            // remove this edge from the list of options, as we just tried it
                            arPossibleConnectingZoneEdges.Remove(iNextZoneEdgeToTry);
                        }
                    }

                    if (true == bMoveToNextTile)
                    {
                        break;
                    }
                    // remove this edge from the list of options, as we just tried it
                    arPossibleZoneEdges.Remove(iZoneEdgeToTry);
                }

                if (0 == arPossibleZoneEdges.Count)
                {
                    // We have exhausted all possible zone edges for the current tile, and we were unable place the next token!
                    throw new Exception("Tile placement failed while laying out dungeon.  Try to generate another dungeon.");
                }
            }

            // Layout successful, but some tiles may be in the negative X or Y space.  Shift the entire dungeon into the positive X and Y space
            int iSmallestX = 0, iLargestX = 0;
            int iSmallestY = 0, iLargestY = 0;
            int iSmallestLayoutX = 0, iLargestLayoutX = 0;
            int iSmallestLayoutY = 0, iLargestLayoutY = 0;
            foreach (Tile tShiftTile in lstTiles)
            {
                if (iSmallestX > tShiftTile.DungeonLayoutLocation.X)
                {
                    iSmallestX = tShiftTile.DungeonLayoutLocation.X;
                }
                if (iSmallestY > tShiftTile.DungeonLayoutLocation.Y)
                {
                    iSmallestY = tShiftTile.DungeonLayoutLocation.Y;
                }
                if (iLargestX < tShiftTile.DungeonLayoutLocation.X)
                {
                    iLargestX = tShiftTile.DungeonLayoutLocation.X;
                }
                if (iLargestY < tShiftTile.DungeonLayoutLocation.Y)
                {
                    iLargestY = tShiftTile.DungeonLayoutLocation.Y;
                }
                // Do the Layout Tiles
                if (iSmallestLayoutX > tShiftTile.LayoutTileLayoutLocation.X)
                {
                    iSmallestLayoutX = tShiftTile.LayoutTileLayoutLocation.X;
                }
                if (iSmallestLayoutY > tShiftTile.LayoutTileLayoutLocation.Y)
                {
                    iSmallestLayoutY = tShiftTile.LayoutTileLayoutLocation.Y;
                }
                if (iLargestLayoutX < tShiftTile.LayoutTileLayoutLocation.X)
                {
                    iLargestLayoutX = tShiftTile.LayoutTileLayoutLocation.X;
                }
                if (iLargestLayoutY < tShiftTile.LayoutTileLayoutLocation.Y)
                {
                    iLargestLayoutY = tShiftTile.LayoutTileLayoutLocation.Y;
                }
            }
            // Shift the tiles
            if ((0 > iSmallestX) || (0 > iSmallestY))
            {
                int iXShift = Math.Abs(iSmallestX);
                int iYShift = Math.Abs(iSmallestY);
                foreach (Tile tTileToShift in lstTiles)
                {
                    tTileToShift.DungeonLayoutLocation = new Point(tTileToShift.DungeonLayoutLocation.X + iXShift, tTileToShift.DungeonLayoutLocation.Y + iYShift);
                }
                iLargestX += iXShift;
                iLargestY += iYShift;
            }
            // Shift the layout tiles
            if ((0 > iSmallestLayoutX) || (0 > iSmallestLayoutY))
            {
                int iXShift = Math.Abs(iSmallestLayoutX);
                int iYShift = Math.Abs(iSmallestLayoutY);
                foreach (Tile tTileToShift in lstTiles)
                {
                    tTileToShift.LayoutTileLayoutLocation = new Point(tTileToShift.LayoutTileLayoutLocation.X + iXShift, tTileToShift.LayoutTileLayoutLocation.Y + iYShift);
                }
                iLargestLayoutX += iXShift;
                iLargestLayoutY += iYShift;
            }
            // Add the size of a tile to the X/Y extents, since the calculation above was only for the upper left corner of each tile
            iLargestX += ((Tile)(lstTiles[0])).Width;
            iLargestY += ((Tile)(lstTiles[0])).Height;
            // Add the size of a layout tile to the X/Y extents, since the calculation above was only for the upper left corner of each tile
            iLargestLayoutX += ((Tile)(lstTiles[0])).LayoutTileWidth;
            iLargestLayoutY += ((Tile)(lstTiles[0])).LayoutTileHeight;

            // The main dungeon layout manipulation is complete.  Now place the summary layout tiles, if requested
            if (true == bDrawSummaryLayout)
            {
                // Leave the layout in the upper left corner, and shift the dungeon down (Y-Axis) to accomodate
                int iDungeonShiftY = iLargestLayoutY + 25; // Add 25 pixels for a space buffer

                // Shift the tiles
                foreach (Tile tTileToShift in lstTiles)
                {
                    tTileToShift.DungeonLayoutLocation = new Point(tTileToShift.DungeonLayoutLocation.X, tTileToShift.DungeonLayoutLocation.Y + iDungeonShiftY);
                }

                // Increase the overall Bitmap height to accomodate this shift for the layout
                iLargestY += iDungeonShiftY;
            }

            // The Summary Layout manipulation is complete.  Now draw everything.
            if (null != imgDungeonLayout)
            {
                imgDungeonLayout.Dispose();
                imgDungeonLayout = null;
            }

            imgDungeonLayout = new Bitmap(iLargestX, iLargestY, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var gr = Graphics.FromImage(imgDungeonLayout))
            {
                gr.Clear(Color.DeepPink); // Flood the image with our background color that we want to be transparent

                // Draw the summary layout tiles, if desired
                if (true == bDrawSummaryLayout)
                {
                    foreach (Tile tDrawTile in lstTiles)
                    {
                        tDrawTile.PerformSummaryLayout(gr, sbBGMapEditorFileContents);
                    }
                }

                // Draw the main tiles
                foreach (Tile tDrawTile in lstTiles)
                {
                    tDrawTile.PerformLayout(gr, sbBGMapEditorFileContents, bUseFullColorTiles);
                }

                // TODO - transparency works, but is not being carried over to clipboard
                ((Bitmap)imgDungeonLayout).MakeTransparent(Color.DeepPink);
            }
        }

        private bool DoesTilePlacementFit(Tile tTileToEvaluate, Rectangle recPotentialTileLocation)
        {
            bool bTileIsClear = true;

            // Get the index of the Tile to Evaluate, as we want to check all tiles that come before it
            //  (the layout is progressing sequentially, starting with the first tile)
            int iStopIndex = lstTiles.IndexOf(tTileToEvaluate);

            if (0 <= iStopIndex)
            {
                for (int iCurrentIndex = 0; iCurrentIndex < iStopIndex; iCurrentIndex++)
                {
                    Tile tCurrentTile = (Tile)(lstTiles[iCurrentIndex]);

                    // Check the current tile separate from the bridge token, so that the dead space created by the bridge is allowed
                    // Step 1 - Does the upper left corner of the rectangle fall within the current tile?
                    if ((recPotentialTileLocation.Left >= tCurrentTile.DungeonLayoutLocation.X) && (recPotentialTileLocation.Left < (tCurrentTile.DungeonLayoutLocation.X + tCurrentTile.Width)) &&
                        (recPotentialTileLocation.Top >= tCurrentTile.DungeonLayoutLocation.Y) && (recPotentialTileLocation.Top < (tCurrentTile.DungeonLayoutLocation.Y + tCurrentTile.Height)))
                    {
                        bTileIsClear = false;
                        break;
                    }
                    // Step 2 - Does the upper right corner of the rectangle fall within the current tile?
                    if ((recPotentialTileLocation.Right >= tCurrentTile.DungeonLayoutLocation.X) && (recPotentialTileLocation.Right < (tCurrentTile.DungeonLayoutLocation.X + tCurrentTile.Width)) &&
                        (recPotentialTileLocation.Top >= tCurrentTile.DungeonLayoutLocation.Y) && (recPotentialTileLocation.Top < (tCurrentTile.DungeonLayoutLocation.Y + tCurrentTile.Height)))
                    {
                        bTileIsClear = false;
                        break;
                    }
                    // Step 3 - Does the lower right corner of the rectangle fall within the current tile?
                    if ((recPotentialTileLocation.Right >= tCurrentTile.DungeonLayoutLocation.X) && (recPotentialTileLocation.Right < (tCurrentTile.DungeonLayoutLocation.X + tCurrentTile.Width)) &&
                        (recPotentialTileLocation.Bottom >= tCurrentTile.DungeonLayoutLocation.Y) && (recPotentialTileLocation.Bottom < (tCurrentTile.DungeonLayoutLocation.Y + tCurrentTile.Height)))
                    {
                        bTileIsClear = false;
                        break;
                    }
                    // Step 4 - Does the lower left corner of the rectangle fall within the current tile?
                    if ((recPotentialTileLocation.Left >= tCurrentTile.DungeonLayoutLocation.X) && (recPotentialTileLocation.Left < (tCurrentTile.DungeonLayoutLocation.X + tCurrentTile.Width)) &&
                        (recPotentialTileLocation.Bottom >= tCurrentTile.DungeonLayoutLocation.Y) && (recPotentialTileLocation.Bottom < (tCurrentTile.DungeonLayoutLocation.Y + tCurrentTile.Height)))
                    {
                        bTileIsClear = false;
                        break;
                    }

                    // Now check the bridge
                    if (null != tCurrentTile.Bridge)
                    {
                        // Figure out the rectangular bounding box for the bridge
                        // * Start with the Edge Zone location
                        Point ptBridgeLocation = DetermineEdgeZoneLocation(tCurrentTile, tCurrentTile.ConnectedCorridorZoneEdge, true);

                        // * Determine where the bridge will start
                        int iXOffset = 0;
                        int iYOffset = 0;
                        int iBridgeHeight = tCurrentTile.Bridge.Height;
                        int iBridgeWidth = tCurrentTile.Bridge.Width;
                        switch(tCurrentTile.ConnectedCorridorZoneEdge)
                        {
                            case 1:
                            case 2:
                            case 3:
                                // The bridge is pointing down, so the edge zone location is the bridge start location [do nothing]
                                break;
                            case 4:
                            case 5:
                            case 6:
                                // The bridge is pointing left, subract the width (use height for now, as we have not rotated the bridge)
                                iXOffset = -1 * tCurrentTile.Bridge.Height;
                                // The bridge needs to be rotated 90 degrees to point left; switch the width/height
                                iBridgeHeight = tCurrentTile.Bridge.Width;
                                iBridgeWidth = tCurrentTile.Bridge.Height;
                                break;
                            case 7:
                            case 8:
                            case 9:
                                // The bridge is pointing up, subtract the height
                                iYOffset = -1 * tCurrentTile.Bridge.Height;
                                break;
                            case 10:
                            case 11:
                            case 12:
                                // The bridge is pointing right, so the edge zone location is the bridge start location [do nothing]
                                // The bridge needs to be rotated 90 degrees to point right; switch the width/height
                                iBridgeHeight = tCurrentTile.Bridge.Width;
                                iBridgeWidth = tCurrentTile.Bridge.Height;
                                break;
                            default:
                                break;
                        }
                        // Move the bridge start point based on the offsets
                        ptBridgeLocation.Offset(iXOffset, iYOffset);

                        // * Determine if the new tile overlaps the bridge
                        // Step 1 - Does the upper left corner of the rectangle fall within the bridge tile?
                        if ((recPotentialTileLocation.Left >= ptBridgeLocation.X) && (recPotentialTileLocation.Left < (ptBridgeLocation.X + iBridgeWidth)) &&
                            (recPotentialTileLocation.Top >= ptBridgeLocation.Y) && (recPotentialTileLocation.Top < (ptBridgeLocation.Y + iBridgeHeight)))
                        {
                            bTileIsClear = false;
                            break;
                        }
                        // Step 2 - Does the upper right corner of the rectangle fall within the bridge tile?
                        if ((recPotentialTileLocation.Right >= ptBridgeLocation.X) && (recPotentialTileLocation.Right < (ptBridgeLocation.X + iBridgeWidth)) &&
                            (recPotentialTileLocation.Top >= ptBridgeLocation.Y) && (recPotentialTileLocation.Top < (ptBridgeLocation.Y + iBridgeHeight)))
                        {
                            bTileIsClear = false;
                            break;
                        }
                        // Step 3 - Does the lower right corner of the rectangle fall within the bridge tile?
                        if ((recPotentialTileLocation.Right >= ptBridgeLocation.X) && (recPotentialTileLocation.Right < (ptBridgeLocation.X + iBridgeWidth)) &&
                            (recPotentialTileLocation.Bottom >= ptBridgeLocation.Y) && (recPotentialTileLocation.Bottom < (ptBridgeLocation.Y + iBridgeHeight)))
                        {
                            bTileIsClear = false;
                            break;
                        }
                        // Step 4 - Does the lower left corner of the rectangle fall within the bridge tile?
                        if ((recPotentialTileLocation.Left >= ptBridgeLocation.X) && (recPotentialTileLocation.Left < (ptBridgeLocation.X + iBridgeWidth)) &&
                            (recPotentialTileLocation.Bottom >= ptBridgeLocation.Y) && (recPotentialTileLocation.Bottom < (ptBridgeLocation.Y + iBridgeHeight)))
                        {
                            bTileIsClear = false;
                            break;
                        }
                    }
                }
            }
            return bTileIsClear;
        }

        private bool DoesSummaryLayoutPlacementFit(Rectangle recPotentialTileLocation)
        {
            bool bTileIsClear = true;

            // Go through every tile in the dungeion
            int iStopIndex = lstTiles.Count;

            if (0 <= iStopIndex)
            {
                for (int iCurrentIndex = 0; iCurrentIndex < iStopIndex; iCurrentIndex++)
                {
                    Tile tCurrentTile = (Tile)(lstTiles[iCurrentIndex]);

                    // Check the current tile separate from the bridge token, so that the dead space created by the bridge is allowed
                    // Step 1 - Does the upper left corner of the rectangle fall within the current tile?
                    if ((recPotentialTileLocation.Left >= tCurrentTile.DungeonLayoutLocation.X) && (recPotentialTileLocation.Left < (tCurrentTile.DungeonLayoutLocation.X + tCurrentTile.Width)) &&
                        (recPotentialTileLocation.Top >= tCurrentTile.DungeonLayoutLocation.Y) && (recPotentialTileLocation.Top < (tCurrentTile.DungeonLayoutLocation.Y + tCurrentTile.Height)))
                    {
                        bTileIsClear = false;
                        break;
                    }
                    // Step 2 - Does the upper right corner of the rectangle fall within the current tile?
                    if ((recPotentialTileLocation.Right >= tCurrentTile.DungeonLayoutLocation.X) && (recPotentialTileLocation.Right < (tCurrentTile.DungeonLayoutLocation.X + tCurrentTile.Width)) &&
                        (recPotentialTileLocation.Top >= tCurrentTile.DungeonLayoutLocation.Y) && (recPotentialTileLocation.Top < (tCurrentTile.DungeonLayoutLocation.Y + tCurrentTile.Height)))
                    {
                        bTileIsClear = false;
                        break;
                    }
                    // Step 3 - Does the lower right corner of the rectangle fall within the current tile?
                    if ((recPotentialTileLocation.Right >= tCurrentTile.DungeonLayoutLocation.X) && (recPotentialTileLocation.Right < (tCurrentTile.DungeonLayoutLocation.X + tCurrentTile.Width)) &&
                        (recPotentialTileLocation.Bottom >= tCurrentTile.DungeonLayoutLocation.Y) && (recPotentialTileLocation.Bottom < (tCurrentTile.DungeonLayoutLocation.Y + tCurrentTile.Height)))
                    {
                        bTileIsClear = false;
                        break;
                    }
                    // Step 4 - Does the lower left corner of the rectangle fall within the current tile?
                    if ((recPotentialTileLocation.Left >= tCurrentTile.DungeonLayoutLocation.X) && (recPotentialTileLocation.Left < (tCurrentTile.DungeonLayoutLocation.X + tCurrentTile.Width)) &&
                        (recPotentialTileLocation.Bottom >= tCurrentTile.DungeonLayoutLocation.Y) && (recPotentialTileLocation.Bottom < (tCurrentTile.DungeonLayoutLocation.Y + tCurrentTile.Height)))
                    {
                        bTileIsClear = false;
                        break;
                    }

                    // Now check the bridge
                    if (null != tCurrentTile.Bridge)
                    {
                        // Figure out the rectangular bounding box for the bridge
                        // * Start with the Edge Zone location
                        Point ptBridgeLocation = DetermineEdgeZoneLocation(tCurrentTile, tCurrentTile.ConnectedCorridorZoneEdge, true);

                        // * Determine where the bridge will start
                        int iXOffset = 0;
                        int iYOffset = 0;
                        int iBridgeHeight = tCurrentTile.Bridge.Height;
                        int iBridgeWidth = tCurrentTile.Bridge.Width;
                        switch (tCurrentTile.ConnectedCorridorZoneEdge)
                        {
                            case 1:
                            case 2:
                            case 3:
                                // The bridge is pointing down, so the edge zone location is the bridge start location [do nothing]
                                break;
                            case 4:
                            case 5:
                            case 6:
                                // The bridge is pointing left, subract the width (use height for now, as we have not rotated the bridge)
                                iXOffset = -1 * tCurrentTile.Bridge.Height;
                                // The bridge needs to be rotated 90 degrees to point left; switch the width/height
                                iBridgeHeight = tCurrentTile.Bridge.Width;
                                iBridgeWidth = tCurrentTile.Bridge.Height;
                                break;
                            case 7:
                            case 8:
                            case 9:
                                // The bridge is pointing up, subtract the height
                                iYOffset = -1 * tCurrentTile.Bridge.Height;
                                break;
                            case 10:
                            case 11:
                            case 12:
                                // The bridge is pointing right, so the edge zone location is the bridge start location [do nothing]
                                // The bridge needs to be rotated 90 degrees to point right; switch the width/height
                                iBridgeHeight = tCurrentTile.Bridge.Width;
                                iBridgeWidth = tCurrentTile.Bridge.Height;
                                break;
                            default:
                                break;
                        }
                        // Move the bridge start point based on the offsets
                        ptBridgeLocation.Offset(iXOffset, iYOffset);

                        // * Determine if the new tile overlaps the bridge
                        // Step 1 - Does the upper left corner of the rectangle fall within the bridge tile?
                        if ((recPotentialTileLocation.Left >= ptBridgeLocation.X) && (recPotentialTileLocation.Left < (ptBridgeLocation.X + iBridgeWidth)) &&
                            (recPotentialTileLocation.Top >= ptBridgeLocation.Y) && (recPotentialTileLocation.Top < (ptBridgeLocation.Y + iBridgeHeight)))
                        {
                            bTileIsClear = false;
                            break;
                        }
                        // Step 2 - Does the upper right corner of the rectangle fall within the bridge tile?
                        if ((recPotentialTileLocation.Right >= ptBridgeLocation.X) && (recPotentialTileLocation.Right < (ptBridgeLocation.X + iBridgeWidth)) &&
                            (recPotentialTileLocation.Top >= ptBridgeLocation.Y) && (recPotentialTileLocation.Top < (ptBridgeLocation.Y + iBridgeHeight)))
                        {
                            bTileIsClear = false;
                            break;
                        }
                        // Step 3 - Does the lower right corner of the rectangle fall within the bridge tile?
                        if ((recPotentialTileLocation.Right >= ptBridgeLocation.X) && (recPotentialTileLocation.Right < (ptBridgeLocation.X + iBridgeWidth)) &&
                            (recPotentialTileLocation.Bottom >= ptBridgeLocation.Y) && (recPotentialTileLocation.Bottom < (ptBridgeLocation.Y + iBridgeHeight)))
                        {
                            bTileIsClear = false;
                            break;
                        }
                        // Step 4 - Does the lower left corner of the rectangle fall within the bridge tile?
                        if ((recPotentialTileLocation.Left >= ptBridgeLocation.X) && (recPotentialTileLocation.Left < (ptBridgeLocation.X + iBridgeWidth)) &&
                            (recPotentialTileLocation.Bottom >= ptBridgeLocation.Y) && (recPotentialTileLocation.Bottom < (ptBridgeLocation.Y + iBridgeHeight)))
                        {
                            bTileIsClear = false;
                            break;
                        }
                    }
                }
            }
            return bTileIsClear;
        }

        private Point DetermineEdgeZoneLocation(Tile tTileToExamine, int iZoneEdgeToExamine, bool bIgnoreBridge = false, bool bExamineLayoutTile = false)
        {
            Point ptRetVal = new Point(tTileToExamine.DungeonLayoutLocation.X, tTileToExamine.DungeonLayoutLocation.Y);
            int iWidthToUse = tTileToExamine.Width;
            int iHeightToUse = tTileToExamine.Height;

            if (true == bExamineLayoutTile)
            {
                ptRetVal = new Point(tTileToExamine.LayoutTileLayoutLocation.X, tTileToExamine.LayoutTileLayoutLocation.Y);
                iWidthToUse = tTileToExamine.LayoutTileWidth;
                iHeightToUse = tTileToExamine.LayoutTileHeight;
            }

            switch (iZoneEdgeToExamine)
            {
                case 1:
                    ptRetVal.Offset(2 * (iWidthToUse / 3), iHeightToUse);
                    break;
                case 2:
                    ptRetVal.Offset(iWidthToUse / 3, iHeightToUse);
                    break;
                case 3:
                    ptRetVal.Offset(0, iHeightToUse);
                    break;
                case 4:
                    ptRetVal.Offset(0, 2 * (iHeightToUse / 3));
                    break;
                case 5:
                    ptRetVal.Offset(0, iHeightToUse / 3);
                    break;
                case 6:
                case 7:
                    // No change, location is upper left corner
                    break;
                case 8:
                    ptRetVal.Offset(iWidthToUse / 3, 0);
                    break;
                case 9:
                    ptRetVal.Offset(2 * (iWidthToUse / 3), 0);
                    break;
                case 10:
                    ptRetVal.Offset(iWidthToUse, 0);
                    break;
                case 11:
                    ptRetVal.Offset(iWidthToUse, iHeightToUse / 3);
                    break;
                case 12:
                    ptRetVal.Offset(iWidthToUse, 2 * (iHeightToUse / 3));
                    break;
                default:
                    break;
            }

            if ((false == bIgnoreBridge) && (null != tTileToExamine.Bridge))
            {
                int iBridgeHeightToUse = tTileToExamine.Bridge.Height;
                if (true == bExamineLayoutTile)
                {
                    iBridgeHeightToUse = tTileToExamine.Bridge.LayoutTileHeight;
                }

                switch (iZoneEdgeToExamine)
                {
                    case 1:
                    case 2:
                    case 3:
                        // The bridge is going to extend down
                        ptRetVal.Offset(0, iBridgeHeightToUse);
                        break;
                    case 4:
                    case 5:
                    case 6:
                        // The bridge is going to extend left (use height here, as the bridge would be rotated in this case
                        ptRetVal.Offset(-1 * iBridgeHeightToUse, 0);
                        break;
                    case 7:
                    case 8:
                    case 9:
                        // The bridge is going to extend up
                        ptRetVal.Offset(0, -1 * iBridgeHeightToUse);
                        break;
                    case 10:
                    case 11:
                    case 12:
                        // The bridge is going to extend right (use height here, as the bridge would be rotated in this case
                        ptRetVal.Offset(iBridgeHeightToUse, 0);
                        break;
                    default:
                        break;
                }
            }

            return ptRetVal;
        }

        private Rectangle DetermineBoundingRectangle(Tile tTileToExamine, Point ptZoneEdgeLocationToStartFrom, int iZoneEdgeToExamine, bool bExamineLayoutTile = false)
        {
            int iWidthToUse = tTileToExamine.Width;
            int iHeightToUse = tTileToExamine.Height;

            if (true == bExamineLayoutTile)
            {
                iWidthToUse = tTileToExamine.LayoutTileWidth;
                iHeightToUse = tTileToExamine.LayoutTileHeight;
            }
            Rectangle recRetVal = new Rectangle(ptZoneEdgeLocationToStartFrom.X, ptZoneEdgeLocationToStartFrom.Y, iWidthToUse, iHeightToUse);

            switch (iZoneEdgeToExamine)
            {
                case 1:
                    recRetVal.Offset(-2 * (iWidthToUse / 3), -1 * iHeightToUse);
                    break;
                case 2:
                    recRetVal.Offset(-1 * (iWidthToUse / 3), -1 * iHeightToUse);
                    break;
                case 3:
                    recRetVal.Offset(0, -1 * iHeightToUse);
                    break;
                case 4:
                    recRetVal.Offset(0, -2 * (iHeightToUse / 3));
                    break;
                case 5:
                    recRetVal.Offset(0, -1 * (iHeightToUse / 3));
                    break;
                case 6:
                case 7:
                    // No change, location is correct
                    break;
                case 8:
                    recRetVal.Offset(-1 * (iWidthToUse / 3), 0);
                    break;
                case 9:
                    recRetVal.Offset(-2 * (iWidthToUse / 3), 0);
                    break;
                case 10:
                    recRetVal.Offset(-1 * iWidthToUse, 0);
                    break;
                case 11:
                    recRetVal.Offset(-1 * iWidthToUse, -1 * (iHeightToUse / 3));
                    break;
                case 12:
                    recRetVal.Offset(-1 * iWidthToUse, -2 * (iHeightToUse / 3));
                    break;
                default:
                    break;
            }

            return recRetVal;
        }

        public string BGMapEditorFileAsString
        {
            get { return sbBGMapEditorFileContents.ToString(); }
        }

        public Image DungeonImage
        {
            get { return imgDungeonLayout; }
        }
    }
}
