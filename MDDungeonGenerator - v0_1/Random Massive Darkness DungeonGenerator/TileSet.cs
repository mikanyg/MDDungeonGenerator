using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Xml;

namespace MassiveDarknessRandomDungeonGenerator
{
    public class TileSet
    {
        private string sName;
        private ArrayList lstTiles;
        private int iUniqueTileCount = 0;

        public TileSet(XmlNode tileSet)
        {
            lstTiles = new ArrayList();

            if (null != tileSet)
            {
                if ((null != tileSet.Attributes) && (null != tileSet.Attributes["Name"]))
                {
                    sName = tileSet.Attributes["Name"].Value;
                }
                if (null != tileSet.ChildNodes)
                {
                    foreach (XmlNode tile in tileSet.ChildNodes)
                    {
                        AddTile(tile);
                    }
                }
            }
        }

        private void AddTile(XmlNode xmlTile)
        {
            bool isUniqueTile = true;
            Tile tileToAdd = new Tile(xmlTile);

            // See if the reverse side of the tile is already in the tile collection
            foreach (Tile tIterator in lstTiles)
            {
                if (tIterator.Name == tileToAdd.ReverseSideName)
                {
                    isUniqueTile = false;
                    tileToAdd.ReverseSideTile = tIterator;
                    tIterator.ReverseSideTile = tileToAdd;
                    break;
                }
            }

            // Add the tile
            lstTiles.Add(tileToAdd);

            // Update the unique tile count
            if (isUniqueTile)
            {
                iUniqueTileCount++;
            }
        }

        public override string ToString()
        {
            return sName + " (" + iUniqueTileCount.ToString() + " Tiles)";
        }

        public ArrayList Tiles
        {
            get { return lstTiles; }
        }

        public int UniqueTileCount
        {
            get { return iUniqueTileCount; }
        }
    }

    public class Tile
    {
        private int[] ai90DegreeZoneRotation = { 0, 7, 4, 1, 8, 5, 2, 9, 6, 3 };
        private int[] ai180DegreeZoneRotation = { 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        private int[] ai270DegreeZoneRotation = { 0, 3, 6, 9, 2, 5, 8, 1, 4, 7 };
        private string sName;
        private int iWidth;
        private int iHeight;
        private string sReverseSideName;
        private ArrayList lstCorridorZones = null;
        private ArrayList lstCorridorZoneEdges = null;
        private string sBGMapEditorTilePath;
        private string sBGMapEditorFullColorTilePath;
        private int iLayoutTileWidth;
        private int iLayoutTileHeight;
        private string sBGMapEditorLayoutTilePath;
        private int iConnectedCorridorZone = 0;
        private int iConnectedCorridorZoneEdge = 0;
        private int iPreviousConnectedCorridorZone = 0;
        private int iPreviousConnectedCorridorZoneEdge = 0;
        private int iRotationSteps = 0;
        private Tile tReverseSideTile = null;
        private Chip cStart = null;
        private Chip cExit = null;
        private Chip cBridge = null;
        private Chip cLevel = null;
        private Chip cDoor = null;
        private int iDoors = 0;
        private Point ptDungeonLayoutLocation = new Point();
        private Point ptLayoutTileLayoutLocation = new Point();

        public Tile(Tile otherTile)
        {
            sName = otherTile.sName;
            iWidth = otherTile.iWidth;
            iHeight = otherTile.iHeight;
            sReverseSideName = otherTile.sReverseSideName;
            if (null != otherTile.lstCorridorZones)
            {
                lstCorridorZones = new ArrayList();
                lstCorridorZones.AddRange(otherTile.lstCorridorZones);
            }
            if (null != otherTile.lstCorridorZoneEdges)
            {
                lstCorridorZoneEdges = new ArrayList();
                lstCorridorZoneEdges.AddRange(otherTile.lstCorridorZoneEdges);
            }
            sBGMapEditorTilePath = otherTile.sBGMapEditorTilePath;
            sBGMapEditorFullColorTilePath = otherTile.sBGMapEditorFullColorTilePath;
            sBGMapEditorLayoutTilePath = otherTile.sBGMapEditorLayoutTilePath;
            iLayoutTileWidth = otherTile.iLayoutTileWidth;
            iLayoutTileHeight = otherTile.iLayoutTileHeight;
        }

        public Tile(XmlNode tile)
        {
            if (null == tile)
            {
                return;
            }

            if (null != tile.ChildNodes)
            {
                foreach (XmlNode infoElement in tile.ChildNodes)
                {
                    switch (infoElement.Name)
                    {
                        case "Name":
                            sName = infoElement.InnerText;
                            break;
                        case "Size":
                            string sTileSize = infoElement.InnerText;
                            string[] dimensions = sTileSize.Split("x".ToCharArray());
                            if (2 == dimensions.Length)
                            {
                                int.TryParse(dimensions[0], out iWidth);
                                int.TryParse(dimensions[1], out iHeight);
                            }
                            break;
                        case "ReverseSideName":
                            sReverseSideName = infoElement.InnerText;
                            break;
                        case "CorridorZones":
                            string sCorridorZones = infoElement.InnerText;
                            string[] asCorridors = sCorridorZones.Split(",".ToCharArray());
                            int iCorridor = 0;
                            if (asCorridors.Length > 0)
                            {
                                lstCorridorZones = new ArrayList();
                                foreach (string sCorridor in asCorridors)
                                {
                                    if (String.Empty != sCorridor.Trim())
                                    {
                                        if(int.TryParse(sCorridor.Trim(), out iCorridor))
                                        {
                                            lstCorridorZones.Add(iCorridor);
                                        }
                                    }
                                }
                            }
                            break;
                        case "CorridorZoneEdges":
                            string sCorridorZoneEdges = infoElement.InnerText;
                            string[] asCorridorZoneEdges = sCorridorZoneEdges.Split(",".ToCharArray());
                            int iCorridorZoneEdge = 0;
                            if (asCorridorZoneEdges.Length > 0)
                            {
                                lstCorridorZoneEdges = new ArrayList();
                                foreach (string sCorridorZoneEdge in asCorridorZoneEdges)
                                {
                                    if (String.Empty != sCorridorZoneEdge.Trim())
                                    {
                                        if (int.TryParse(sCorridorZoneEdge.Trim(), out iCorridorZoneEdge))
                                        {
                                            lstCorridorZoneEdges.Add(iCorridorZoneEdge);
                                        }
                                    }
                                }
                            }
                            break;
                        case "BGMapEditorTilePath":
                            sBGMapEditorTilePath = infoElement.InnerText;
                            break;
                        case "BGMapEditorFullColorTilePath":
                            sBGMapEditorFullColorTilePath = infoElement.InnerText;
                            break;
                        case "LayoutTileSize":
                            string sLayoutSize = infoElement.InnerText;
                            string[] tileDimensions = sLayoutSize.Split("x".ToCharArray());
                            if (2 == tileDimensions.Length)
                            {
                                int.TryParse(tileDimensions[0], out iLayoutTileWidth);
                                int.TryParse(tileDimensions[1], out iLayoutTileHeight);
                            }
                            break;
                        case "BGMapEditorLayoutTilePath":
                            sBGMapEditorLayoutTilePath = infoElement.InnerText;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void RotateTile(int iNumRotations)
        {
            if (0 >= iNumRotations)
            {
                // Nothing to do here
                return;
            }

            int iRotationsLeft = iNumRotations;
            if (3 < iRotationsLeft)
            {
                iRotationsLeft = 3;
            }

            iRotationSteps = iRotationsLeft;

            while (0 < iRotationsLeft)
            {
                // Rotate each zone edge once clockwise
                for(int iZoneEdgeIterator = 0; iZoneEdgeIterator < lstCorridorZoneEdges.Count; iZoneEdgeIterator++)
                {
                    lstCorridorZoneEdges[iZoneEdgeIterator] = (int)(lstCorridorZoneEdges[iZoneEdgeIterator]) + 3;
                    if ((int)(lstCorridorZoneEdges[iZoneEdgeIterator]) > 12)
                    {
                        lstCorridorZoneEdges[iZoneEdgeIterator] = (int)(lstCorridorZoneEdges[iZoneEdgeIterator]) - 12;
                    }
                }

                iRotationsLeft--;
            }

            string sNewTileImageFilename = String.Empty;
            switch (iNumRotations)
            {
                case 1:  // 90 degrees clockwise
                    sNewTileImageFilename = "r_90.png";
                    int iCurrent90Zone = 0;
                    // apply the corresponding mapping table translation for the corridor zones
                    for (int i90Counter = 0; i90Counter < lstCorridorZones.Count; i90Counter++)
                    {
                        iCurrent90Zone = (int)(lstCorridorZones[i90Counter]);
                        lstCorridorZones[i90Counter] = ai90DegreeZoneRotation[iCurrent90Zone];
                    }
                    break;
                case 2:  // 180 degrees clockwise
                    sNewTileImageFilename = "r_180.png";
                    int iCurrent180Zone = 0;
                    // apply the corresponding mapping table translation for the corridor zones
                    for (int i180Counter = 0; i180Counter < lstCorridorZones.Count; i180Counter++)
                    {
                        iCurrent180Zone = (int)(lstCorridorZones[i180Counter]);
                        lstCorridorZones[i180Counter] = ai180DegreeZoneRotation[iCurrent180Zone];
                    }
                    break;
                case 3:  // 270 degrees clockwise
                    sNewTileImageFilename = "r_270.png";
                    int iCurrent270Zone = 0;
                    // apply the corresponding mapping table translation for the corridor zones
                    for (int i270Counter = 0; i270Counter < lstCorridorZones.Count; i270Counter++)
                    {
                        iCurrent270Zone = (int)(lstCorridorZones[i270Counter]);
                        lstCorridorZones[i270Counter] = ai270DegreeZoneRotation[iCurrent270Zone];
                    }
                    break;
                default:
                    break;
            }

            // Replace the image filename with the corresponding rotation filename
            sBGMapEditorTilePath = sBGMapEditorTilePath.Replace("r_0.png", sNewTileImageFilename);
            sBGMapEditorFullColorTilePath = sBGMapEditorFullColorTilePath.Replace("r_0.png", sNewTileImageFilename);
            // Don't rotate the layout tile
            //sBGMapEditorLayoutTilePath = sBGMapEditorLayoutTilePath.Replace("r_0.png", sNewTileImageFilename);
        }

        public void PerformLayout(Graphics grDungeon, StringBuilder sbBGMapEditorFileContents, bool bUseFullColorTiles)
        {
            // Load the tile image
            Image iTileImage = TokenUtilities.GetTokenImage(this, bUseFullColorTiles);
            if (null != iTileImage)
            {
                grDungeon.DrawImage(iTileImage, new Rectangle(ptDungeonLayoutLocation.X, ptDungeonLayoutLocation.Y, iTileImage.Width, iTileImage.Height));
            }
            // Add the current tile to the MAP file
            if (true == bUseFullColorTiles)
            {
                sbBGMapEditorFileContents.AppendLine(sBGMapEditorFullColorTilePath + ";tile;" + ptDungeonLayoutLocation.X.ToString() + "x" + ptDungeonLayoutLocation.Y.ToString() + ";25");
            }
            else
            {
                sbBGMapEditorFileContents.AppendLine(sBGMapEditorTilePath + ";tile;" + ptDungeonLayoutLocation.X.ToString() + "x" + ptDungeonLayoutLocation.Y.ToString() + ";25");
            }

            // Add the bridge
            if (null != cBridge)
            {
                string sBridgeTokenPath = cBridge.BGMapEditorTilePath;
                Point ptBridgeLocation = new Point(ptDungeonLayoutLocation.X, ptDungeonLayoutLocation.Y);

                switch (iConnectedCorridorZoneEdge)
                {
                    case 1:
                        ptBridgeLocation.Offset(2 * (iWidth / 3), iHeight);
                        break;
                    case 2:
                        ptBridgeLocation.Offset(iWidth / 3, iHeight);
                        break;
                    case 3:
                        ptBridgeLocation.Offset(0, iHeight);
                        break;
                    case 4:
                        // It is rotated 90 degrees, so use the Height for the X-width offset
                        ptBridgeLocation.Offset(-1 * cBridge.Height, 2 * (iHeight / 3));
                        sBridgeTokenPath = sBridgeTokenPath.Replace("r_0.png", "r_90.png");
                        break;
                    case 5:
                        // It is rotated 90 degrees, so use the Height for the X-width offset
                        ptBridgeLocation.Offset(-1 * cBridge.Height, iHeight / 3);
                        sBridgeTokenPath = sBridgeTokenPath.Replace("r_0.png", "r_90.png");
                        break;
                    case 6:
                        // It is rotated 90 degrees, so use the Height for the X-width offset
                        ptBridgeLocation.Offset(-1 * cBridge.Height, 0);
                        sBridgeTokenPath = sBridgeTokenPath.Replace("r_0.png", "r_90.png");
                        break;
                    case 7:
                        ptBridgeLocation.Offset(0, -1 * cBridge.Height);
                        break;
                    case 8:
                        ptBridgeLocation.Offset(iWidth / 3, -1 * cBridge.Height);
                        break;
                    case 9:
                        ptBridgeLocation.Offset(2 * (iWidth / 3), -1 * cBridge.Height);
                        break;
                    case 10:
                        ptBridgeLocation.Offset(iWidth, 0);
                        sBridgeTokenPath = sBridgeTokenPath.Replace("r_0.png", "r_90.png");
                        break;
                    case 11:
                        ptBridgeLocation.Offset(iWidth, iHeight / 3);
                        sBridgeTokenPath = sBridgeTokenPath.Replace("r_0.png", "r_90.png");
                        break;
                    case 12:
                        ptBridgeLocation.Offset(iWidth, 2 * (iHeight / 3));
                        sBridgeTokenPath = sBridgeTokenPath.Replace("r_0.png", "r_90.png");
                        break;
                    default:
                        break;
                }
                sbBGMapEditorFileContents.AppendLine(sBridgeTokenPath + ";chip;" + ptBridgeLocation.X.ToString() + "x" + ptBridgeLocation.Y.ToString() + ";0");

                Image iBridgeImage = TokenUtilities.GetTokenImage(cBridge, false, sBridgeTokenPath);
                if (null != iBridgeImage)
                {
                    grDungeon.DrawImage(iBridgeImage, new Rectangle(ptBridgeLocation.X, ptBridgeLocation.Y, iBridgeImage.Width, iBridgeImage.Height));
                }
            }

            // Add the start marker
            int iStartZone = 0;
            if (null != cStart)
            {
                iStartZone = TokenUtilities.DetermineFarthestZone(iConnectedCorridorZone, lstCorridorZones);
                // Determine where the center of the start zone is, then offset the point so that the start token is centered in the zone
                Point ptStart = TokenUtilities.DetermineZoneCenter(this, iStartZone);
                ptStart.Offset(-1 * (cStart.Width / 2), -1 * (cStart.Height / 2));

                sbBGMapEditorFileContents.AppendLine(cStart.BGMapEditorTilePath + ";chip;" + ptStart.X.ToString() + "x" + ptStart.Y.ToString() + ";0");

                Image imgStartImage = TokenUtilities.GetTokenImage(cStart);
                if (null != imgStartImage)
                {
                    grDungeon.DrawImage(imgStartImage, new Rectangle(ptStart.X, ptStart.Y, cStart.Width, cStart.Height));
                }
            }

            // Add the exit marker
            int iExitZone = 0;
            if (null != cExit)
            {
                iExitZone = TokenUtilities.DetermineFarthestZone(iPreviousConnectedCorridorZone, lstCorridorZones);
                // Determine where the center of the exit zone is, then offset the point so that the exit token is centered in the zone
                Point ptExit = TokenUtilities.DetermineZoneCenter(this, iExitZone);
                ptExit.Offset(-1 * (cExit.Width / 2), -1 * (cExit.Height / 2));

                sbBGMapEditorFileContents.AppendLine(cExit.BGMapEditorTilePath + ";chip;" + ptExit.X.ToString() + "x" + ptExit.Y.ToString() + ";0");

                Image imgExitImage = TokenUtilities.GetTokenImage(cExit);
                if (null != imgExitImage)
                {
                    grDungeon.DrawImage(imgExitImage, new Rectangle(ptExit.X, ptExit.Y, cExit.Width, cExit.Height));
                }
            }

            // Add the level marker
            // ** The level marker must go in a cooridor
            // - Give preference to the center of the tile
            // - Do not put the level marker in the same zone as the start or Exit marker
            int iSelectedZone = 5;
            if (false == lstCorridorZones.Contains(5))
            {
                // randomly choose another zone, since the center of the tile is not a cooridor
                ArrayList lstPossibleLevelCorridorZones = new ArrayList(lstCorridorZones);
                if (null != cStart)
                {
                    // This is a START tile, exclude the start zone
                    lstPossibleLevelCorridorZones.Remove(iStartZone);
                }
                else if (null != cExit)
                {
                    // This is an Exit tile, exclude the start zone
                    lstPossibleLevelCorridorZones.Remove(iExitZone);
                }
                int iZoneIndex = TokenUtilities.rndGenerator.Next(0, lstPossibleLevelCorridorZones.Count);
                iSelectedZone = (int)(lstPossibleLevelCorridorZones[iZoneIndex]);
            }
            // Determine where the center of the selected zone is, then offset the point so that the level token is centered in the zone
            Point ptLevel = TokenUtilities.DetermineZoneCenter(this, iSelectedZone);
            ptLevel.Offset(- 1 * (cLevel.Width / 2), - 1 * (cLevel.Height / 2));

            sbBGMapEditorFileContents.AppendLine(cLevel.BGMapEditorTilePath + ";chip;" + ptLevel.X.ToString() + "x" + ptLevel.Y.ToString() + ";0");

            Image imgLevelImage = TokenUtilities.GetTokenImage(cLevel);
            if (null != imgLevelImage)
            {
                grDungeon.DrawImage(imgLevelImage, new Rectangle(ptLevel.X, ptLevel.Y, cLevel.Width, cLevel.Height));
            }

            // Add the doors
            if (0 < iDoors)
            {
                // Get a list of all possible door locations, based upon the corridors on the tile
                ArrayList lstAvailableDoorLocations = TokenUtilities.DetermineAvailableDoorLocations(lstCorridorZones);

                for (int iDoorIterator = 0; iDoorIterator < iDoors; iDoorIterator++)
                {
                    if (0 >= lstAvailableDoorLocations.Count)
                    {
                        // No available locations left
                        break;
                    }

                    // Determine door location
                    int iDoorIndex = TokenUtilities.rndGenerator.Next(0, lstAvailableDoorLocations.Count);
                    string sDoorToPlace = (string)(lstAvailableDoorLocations[iDoorIndex]);
                    // Remove the selected door placement so that it cannot be used again
                    lstAvailableDoorLocations.Remove(sDoorToPlace);
                    string[] asDoorToPlaceComponents = sDoorToPlace.Split("-".ToCharArray());
                    // Wastefully create a new point here, as we need to declare it outside of the If statement below
                    Point ptDoorLocation = new Point(ptDungeonLayoutLocation.X, ptDungeonLayoutLocation.Y);
                    int iDoorZone;
                    string sDoorTokenPath = cDoor.BGMapEditorTilePath;
                    if (true == int.TryParse(asDoorToPlaceComponents[0], out iDoorZone))
                    {
                        ptDoorLocation = TokenUtilities.DetermineZoneCenter(this, iDoorZone);
                        switch (asDoorToPlaceComponents[1])
                        {
                            case "Top":
                                sDoorTokenPath = sDoorTokenPath.Substring(0, sDoorTokenPath.LastIndexOf("r_0.png")) + "r_180.png";
                                //sDoorTokenPath = sDoorTokenPath.Replace("r_0.png", "r_180.png");
                                // Offset the point up so that it is at the top, middle of the current zone
                                ptDoorLocation.Offset(0, -1 * (this.Height / 6));
                                // Now offset the door location by half of the door width and height
                                ptDoorLocation.Offset(-1 * (cDoor.Width / 2), -1 * (cDoor.Height / 2));
                                break;
                            case "Bottom":
                                // Offset the point up so that it is at the bottom, middle of the current zone
                                ptDoorLocation.Offset(0, (this.Height / 6));
                                // Now offset the door location by half of the door width and height
                                ptDoorLocation.Offset(-1 * (cDoor.Width / 2), -1 * (cDoor.Height / 2));
                                break;
                            case "Left":
                                sDoorTokenPath = sDoorTokenPath.Substring(0, sDoorTokenPath.LastIndexOf("r_0.png")) + "r_90.png";
                                // Offset the point up so that it is at the left, middle of the current zone
                                ptDoorLocation.Offset(-1 * (this.Width / 6), 0);
                                // Now offset the door location by half of the door width and height (swap width/height since the door is to be rotated 90 degrees)
                                ptDoorLocation.Offset(-1 * (cDoor.Height / 2), -1 * (cDoor.Width / 2));
                                break;
                            case "Right":
                            default:
                                sDoorTokenPath = sDoorTokenPath.Substring(0, sDoorTokenPath.LastIndexOf("r_0.png")) + "r_270.png";
                                // Offset the point up so that it is at the right, middle of the current zone
                                ptDoorLocation.Offset((this.Height / 6), 0);
                                // Now offset the door location by half of the door width and height (swap width/height since the door is to be rotated 270 degrees)
                                ptDoorLocation.Offset(-1 * (cDoor.Height / 2), -1 * (cDoor.Width / 2));
                                break;
                        }
                    }

                    sbBGMapEditorFileContents.AppendLine(sDoorTokenPath + ";chip;" + ptDoorLocation.X.ToString() + "x" + ptDoorLocation.Y.ToString() + ";0");

                    Image imgDoorImage = TokenUtilities.GetTokenImage(cDoor, false, sDoorTokenPath);
                    if (null != imgDoorImage)
                    {
                        grDungeon.DrawImage(imgDoorImage, new Rectangle(ptDoorLocation.X, ptDoorLocation.Y, imgDoorImage.Width, imgDoorImage.Height));
                    }
                }
            }
        }

        public void PerformSummaryLayout(Graphics grDungeon, StringBuilder sbBGMapEditorFileContents)
        {
            // Load the summary tile image
            Image iTileImage = TokenUtilities.GetTokenImage(this, false, "", true);
            if (null != iTileImage)
            {
                grDungeon.DrawImage(iTileImage, new Rectangle(ptLayoutTileLayoutLocation.X, ptLayoutTileLayoutLocation.Y, iTileImage.Width, iTileImage.Height));
            }
            // Add the current summary tile to the MAP file
            sbBGMapEditorFileContents.AppendLine(sBGMapEditorLayoutTilePath + ";tile;" + ptLayoutTileLayoutLocation.X.ToString() + "x" + ptLayoutTileLayoutLocation.Y.ToString() + ";5");

            // Add the bridge
            if (null != cBridge)
            {
                Point ptBridgeLocation = new Point(ptLayoutTileLayoutLocation.X, ptLayoutTileLayoutLocation.Y);

                switch (iConnectedCorridorZoneEdge)
                {
                    case 1:
                        ptBridgeLocation.Offset(2 * (iLayoutTileWidth / 3), iLayoutTileHeight);
                        break;
                    case 2:
                        ptBridgeLocation.Offset(iLayoutTileWidth / 3, iLayoutTileHeight);
                        break;
                    case 3:
                        ptBridgeLocation.Offset(0, iLayoutTileHeight);
                        break;
                    case 4:
                        // It is rotated 90 degrees, so use the Height for the X-width offset
                        ptBridgeLocation.Offset(-1 * cBridge.LayoutTileHeight, 2 * (iLayoutTileHeight / 3));
                        break;
                    case 5:
                        // It is rotated 90 degrees, so use the Height for the X-width offset
                        ptBridgeLocation.Offset(-1 * cBridge.LayoutTileHeight, iLayoutTileHeight / 3);
                        break;
                    case 6:
                        // It is rotated 90 degrees, so use the Height for the X-width offset
                        ptBridgeLocation.Offset(-1 * cBridge.LayoutTileHeight, 0);
                        break;
                    case 7:
                        ptBridgeLocation.Offset(0, -1 * cBridge.LayoutTileHeight);
                        break;
                    case 8:
                        ptBridgeLocation.Offset(iLayoutTileWidth / 3, -1 * cBridge.LayoutTileHeight);
                        break;
                    case 9:
                        ptBridgeLocation.Offset(2 * (iLayoutTileWidth / 3), -1 * cBridge.LayoutTileHeight);
                        break;
                    case 10:
                        ptBridgeLocation.Offset(iLayoutTileWidth, 0);
                        break;
                    case 11:
                        ptBridgeLocation.Offset(iLayoutTileWidth, iLayoutTileHeight / 3);
                        break;
                    case 12:
                        ptBridgeLocation.Offset(iLayoutTileWidth, 2 * (iLayoutTileHeight / 3));
                        break;
                    default:
                        break;
                }
                sbBGMapEditorFileContents.AppendLine(cBridge.BGMapEditorLayoutTilePath + ";tile;" + ptBridgeLocation.X.ToString() + "x" + ptBridgeLocation.Y.ToString() + ";5");

                Image iBridgeImage = TokenUtilities.GetTokenImage(cBridge, false, "", true);
                if (null != iBridgeImage)
                {
                    grDungeon.DrawImage(iBridgeImage, new Rectangle(ptBridgeLocation.X, ptBridgeLocation.Y, iBridgeImage.Width, iBridgeImage.Height));
                }
            }
        }

        public ArrayList GetMatchingConnectingEdges(int iEdgeToMatch)
        {
            ArrayList arMatchingConnectingEdges = new ArrayList();

            switch (iEdgeToMatch)
            {
                case 1:
                case 2:
                case 3:
                    // Passed in edge is on the bottom of the other tile, see if this
                    // tile has any corridor edge zones on its top
                    if (lstCorridorZoneEdges.Contains(7))
                    {
                        arMatchingConnectingEdges.Add(7);
                    }
                    if (lstCorridorZoneEdges.Contains(8))
                    {
                        arMatchingConnectingEdges.Add(8);
                    }
                    if (lstCorridorZoneEdges.Contains(9))
                    {
                        arMatchingConnectingEdges.Add(9);
                    }
                    break;
                case 4:
                case 5:
                case 6:
                    // Passed in edge is on the left of the other tile, see if this
                    // tile has any corridor edge zones on its right
                    if (lstCorridorZoneEdges.Contains(10))
                    {
                        arMatchingConnectingEdges.Add(10);
                    }
                    if (lstCorridorZoneEdges.Contains(11))
                    {
                        arMatchingConnectingEdges.Add(11);
                    }
                    if (lstCorridorZoneEdges.Contains(12))
                    {
                        arMatchingConnectingEdges.Add(12);
                    }
                    break;
                case 7:
                case 8:
                case 9:
                    // Passed in edge is on the top of the other tile, see if this
                    // tile has any corridor edge zones on its bottom
                    if (lstCorridorZoneEdges.Contains(1))
                    {
                        arMatchingConnectingEdges.Add(1);
                    }
                    if (lstCorridorZoneEdges.Contains(2))
                    {
                        arMatchingConnectingEdges.Add(2);
                    }
                    if (lstCorridorZoneEdges.Contains(3))
                    {
                        arMatchingConnectingEdges.Add(4);
                    }
                    break;
                case 10:
                case 11:
                case 12:
                    // Passed in edge is on the right of the other tile, see if this
                    // tile has any corridor edge zones on its left
                    if (lstCorridorZoneEdges.Contains(4))
                    {
                        arMatchingConnectingEdges.Add(4);
                    }
                    if (lstCorridorZoneEdges.Contains(5))
                    {
                        arMatchingConnectingEdges.Add(5);
                    }
                    if (lstCorridorZoneEdges.Contains(6))
                    {
                        arMatchingConnectingEdges.Add(6);
                    }
                    break;
                default:
                    break;
            }

            return arMatchingConnectingEdges;
        }

        public string Name
        {
            get { return sName; }
        }
        public int Width
        {
            get { return iWidth; }
        }
        public int Height
        {
            get { return iHeight; }
        }
        public string ReverseSideName
        {
            get { return sReverseSideName; }
        }
        public Tile ReverseSideTile
        {
            get { return tReverseSideTile; }
            set { tReverseSideTile = value; }
        }
        public ArrayList CorridorZones
        {
            get { return lstCorridorZones; }
        }
        public ArrayList CorridorZoneEdges
        {
            get { return lstCorridorZoneEdges; }
        }
        public int PreviousConnectedCorridorZoneEdge
        {
            get { return iPreviousConnectedCorridorZoneEdge; }
            set
            {
                iPreviousConnectedCorridorZoneEdge = value;
                // Set the corresponding iPreviousConnectedCorridorZone
                switch (value)
                {
                    case 1:
                    case 12:
                        iPreviousConnectedCorridorZone = 3;
                        break;
                    case 2:
                        iPreviousConnectedCorridorZone = 2;
                        break;
                    case 3:
                    case 4:
                        iPreviousConnectedCorridorZone = 1;
                        break;
                    case 5:
                        iPreviousConnectedCorridorZone = 4;
                        break;
                    case 6:
                    case 7:
                        iPreviousConnectedCorridorZone = 7;
                        break;
                    case 8:
                        iPreviousConnectedCorridorZone = 8;
                        break;
                    case 9:
                    case 10:
                        iPreviousConnectedCorridorZone = 9;
                        break;
                    case 11:
                        iPreviousConnectedCorridorZone = 6;
                        break;
                    default:
                        break;
                }
            }
        }
        public int ConnectedCorridorZoneEdge
        {
            get { return iConnectedCorridorZoneEdge; }
            set
            {
                iConnectedCorridorZoneEdge = value;
                // Set the corresponding iConnectedCooridorZone
                switch (value)
                {
                    case 1:
                    case 12:
                        iConnectedCorridorZone = 3;
                        break;
                    case 2:
                        iConnectedCorridorZone = 2;
                        break;
                    case 3:
                    case 4:
                        iConnectedCorridorZone = 1;
                        break;
                    case 5:
                        iConnectedCorridorZone = 4;
                        break;
                    case 6:
                    case 7:
                        iConnectedCorridorZone = 7;
                        break;
                    case 8:
                        iConnectedCorridorZone = 8;
                        break;
                    case 9:
                    case 10:
                        iConnectedCorridorZone = 9;
                        break;
                    case 11:
                        iConnectedCorridorZone = 6;
                        break;
                    default:
                        break;
                }
            }
        }
        public int RotationSteps
        {
            get { return iRotationSteps; }
        }
        public Chip Bridge
        {
            get { return cBridge; }
            set { cBridge = value; }
        }
        public Chip Start
        {
            get { return cStart; }
            set { cStart = value; }
        }
        public Chip Exit
        {
            get { return cExit; }
            set { cExit = value; }
        }
        public int Doors
        {
            get { return iDoors; }
            set { iDoors = value; }
        }
        public Chip Door
        {
            get { return cDoor; }
            set { cDoor = value; }
        }
        public Chip Level
        {
            get { return cLevel; }
            set { cLevel = value; }
        }
        public Point DungeonLayoutLocation
        {
            get { return ptDungeonLayoutLocation; }
            set { ptDungeonLayoutLocation = value; }
        }
        public string BGMapEditorTilePath
        {
            get { return sBGMapEditorTilePath; }
        }
        // Layout Tile Specific Accessors
        public int LayoutTileWidth
        {
            get { return iLayoutTileWidth; }
        }
        public int LayoutTileHeight
        {
            get { return iLayoutTileHeight; }
        }
        public Point LayoutTileLayoutLocation
        {
            get { return ptLayoutTileLayoutLocation; }
            set { ptLayoutTileLayoutLocation = value; }
        }
        public string BGMapEditorLayoutTilePath
        {
            get { return sBGMapEditorLayoutTilePath; }
        }
    }
}