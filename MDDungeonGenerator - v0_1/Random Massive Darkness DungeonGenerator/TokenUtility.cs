using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace MassiveDarknessRandomDungeonGenerator
{
    public static class TokenUtilities
    {
        public static Random rndGenerator = new Random();

        public static Image GetTokenImage(object objToken, bool bIsDark = false, string sFilePathOverride = "", bool bGetLayoutTile = false)
        {
            Image retVal = null;
            Assembly _assembly = Assembly.GetExecutingAssembly();
            Stream _imageStream = null;

            // DEBUG - Get a list of all resources embedded in the assembly (helps figure out resource path)
            //string[] sEmbeddedResources = _assembly.GetManifestResourceNames();
            try
            {
                if (objToken.GetType() == typeof(Tile))
                {
                    Tile tToken = (Tile)objToken;
                    string sResourceName = String.Empty;
                    string sTokenFilename = "r_0.png"; // Start with the unrotated tile
                    if (String.Empty != sFilePathOverride)
                    {
                        sTokenFilename = Path.GetFileName(sFilePathOverride);
                    }

                    // Get the image out of the embedded assembly
                    if (true == bIsDark)
                    {
                        sResourceName = "MassiveDarknessRandomDungeonGenerator.Images.dark_tiles._" + tToken.Name + "." + sTokenFilename;
                    }
                    else if (true == bGetLayoutTile)
                    {
                        sResourceName = "MassiveDarknessRandomDungeonGenerator.Images.layout._" + tToken.Name + "." + sTokenFilename;
                    }
                    else
                    {
                        sResourceName = "MassiveDarknessRandomDungeonGenerator.Images.tiles._" + tToken.Name + "." + sTokenFilename;
                    }

                    _imageStream = _assembly.GetManifestResourceStream(sResourceName);
                    if (null != _imageStream)
                    {
                        retVal = new Bitmap(_imageStream);

                        if (false == bGetLayoutTile)
                        {
                            switch (tToken.RotationSteps)
                            {
                                case 1:
                                    // Rotate the tile 90 degrees
                                    retVal.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                                case 2:
                                    // Rotate the tile 180 degrees
                                    retVal.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    break;
                                case 3:
                                    // Rotate the tile 270 degrees
                                    retVal.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                                default:
                                    // do nothing
                                    break;
                            }
                        }
                    }
                }
                else if (objToken.GetType() == typeof(Chip))
                {
                    Chip cToken = (Chip)objToken;
                    string sResourceName = String.Empty;
                    string sTokenFilename = String.Empty;
                    if (String.Empty != sFilePathOverride)
                    {
                        sTokenFilename = Path.GetFileName(sFilePathOverride);
                    }
                    else
                    {
                        sTokenFilename = Path.GetFileName(cToken.BGMapEditorTilePath);
                    }
                    // Get the image out of the embedded assembly
                    if (true == bGetLayoutTile)
                    {
                        _imageStream = _assembly.GetManifestResourceStream("MassiveDarknessRandomDungeonGenerator.Images.layout." + cToken.Name.ToLower() + "." + sTokenFilename);
                    }
                    else
                    {
                        _imageStream = _assembly.GetManifestResourceStream("MassiveDarknessRandomDungeonGenerator.Images.chips." + cToken.Name.ToLower() + "." + sTokenFilename);
                    }
                    if (null != _imageStream)
                    {
                        retVal = new Bitmap(_imageStream);
                    }
                }
            }
            catch (Exception exGetTokenImage)
            {
                // just swallow all exceptions here
                if (null != retVal)
                {
                    retVal.Dispose();
                    retVal = null;
                }
            }
            finally
            {
                if (null != _imageStream)
                {
                    _imageStream.Close();
                    _imageStream.Dispose();
                }
            }
            return retVal;
        }

        public static string GetEmbeddedText(string sEmbeddedResourcePathName)
        {
            string sEmbeddedText = String.Empty;
            StreamReader srEmbeddedTextContents = null;

            try
            {
                srEmbeddedTextContents = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(sEmbeddedResourcePathName));
                sEmbeddedText = srEmbeddedTextContents.ReadToEnd();
            }
            catch (Exception exEmbeddedText)
            {
                // rethrow the exception so the caller knows why it failed
                throw exEmbeddedText;
            }
            finally
            {
                // Use a try block here, which rethrows the exception, to force this finally block to execute and clean up.
                if (null != srEmbeddedTextContents)
                {
                    srEmbeddedTextContents.Close();
                    srEmbeddedTextContents.Dispose();
                }
            }
            return sEmbeddedText;
        }

        public static Point DetermineZoneCenter(Tile tTile, int iZone)
        {
            Point retVal = new Point(tTile.DungeonLayoutLocation.X, tTile.DungeonLayoutLocation.Y);

            switch (iZone)
            {
                case 1:
                    retVal.Offset((tTile.Width / 6), (5 * tTile.Height / 6));
                    break;
                case 2:
                    retVal.Offset((tTile.Width / 2), (5 * tTile.Height / 6));
                    break;
                case 3:
                    retVal.Offset((5 * tTile.Width / 6), (5 * tTile.Height / 6));
                    break;
                case 4:
                    retVal.Offset((tTile.Width / 6), (tTile.Height / 2));
                    break;
                case 5:
                    retVal.Offset((tTile.Width / 2), (tTile.Height / 2));
                    break;
                case 6:
                    retVal.Offset((5 * tTile.Width / 6), (tTile.Height / 2));
                    break;
                case 7:
                    retVal.Offset((tTile.Width / 6), (tTile.Height / 6));
                    break;
                case 8:
                    retVal.Offset((tTile.Width / 2), (tTile.Height / 6));
                    break;
                case 9:
                    retVal.Offset((5 * tTile.Width / 6), (tTile.Height / 6));
                    break;
                default:
                    break;
            }

            return retVal;
        }

        public static int DetermineFarthestZone(int iConnectedCorridorZone, ArrayList lstCorridorZones)
        {
            int iRetVal = 0;

            // The start marker should be in the zone that is furthest from the connecting zone edge
            switch (iConnectedCorridorZone)
            {
                case 1:
                    if (lstCorridorZones.Contains(9))
                    {
                        iRetVal = 9;
                    }
                    else if ((lstCorridorZones.Contains(6)) || (lstCorridorZones.Contains(8)))
                    {
                        if (lstCorridorZones.Contains(8))
                        {
                            iRetVal = 8;
                        }
                        else
                        {
                            iRetVal = 6;
                        }
                    }
                    else if ((lstCorridorZones.Contains(3)) || (lstCorridorZones.Contains(7)))
                    {
                        if (lstCorridorZones.Contains(7))
                        {
                            iRetVal = 7;
                        }
                        else
                        {
                            iRetVal = 3;
                        }
                    }
                    else
                    {
                        if (lstCorridorZones.Contains(4))
                        {
                            iRetVal = 4;
                        }
                        else
                        {
                            iRetVal = 2;
                        }
                    }
                    break;
                case 2:
                    if ((lstCorridorZones.Contains(7)) || (lstCorridorZones.Contains(9)))
                    {
                        if (lstCorridorZones.Contains(7))
                        {
                            iRetVal = 7;
                        }
                        else
                        {
                            iRetVal = 9;
                        }
                    }
                    else if ((lstCorridorZones.Contains(4)) || (lstCorridorZones.Contains(6)) || (lstCorridorZones.Contains(8)))
                    {
                        if (lstCorridorZones.Contains(8))
                        {
                            iRetVal = 8;
                        }
                        else if (lstCorridorZones.Contains(4))
                        {
                            iRetVal = 4;
                        }
                        else
                        {
                            iRetVal = 6;
                        }
                    }
                    else
                    {
                        if (lstCorridorZones.Contains(1))
                        {
                            iRetVal = 1;
                        }
                        else
                        {
                            iRetVal = 3;
                        }
                    }
                    break;
                case 3:
                    if (lstCorridorZones.Contains(7))
                    {
                        iRetVal = 7;
                    }
                    else if ((lstCorridorZones.Contains(4)) || (lstCorridorZones.Contains(8)))
                    {
                        if (lstCorridorZones.Contains(8))
                        {
                            iRetVal = 8;
                        }
                        else
                        {
                            iRetVal = 4;
                        }
                    }
                    else if ((lstCorridorZones.Contains(1)) || (lstCorridorZones.Contains(9)))
                    {
                        if (lstCorridorZones.Contains(1))
                        {
                            iRetVal = 1;
                        }
                        else
                        {
                            iRetVal = 9;
                        }
                    }
                    else
                    {
                        if (lstCorridorZones.Contains(6))
                        {
                            iRetVal = 6;
                        }
                        else
                        {
                            iRetVal = 2;
                        }
                    }
                    break;
                case 4:
                    if ((lstCorridorZones.Contains(3)) || (lstCorridorZones.Contains(9)))
                    {
                        if (lstCorridorZones.Contains(3))
                        {
                            iRetVal = 3;
                        }
                        else
                        {
                            iRetVal = 9;
                        }
                    }
                    else if ((lstCorridorZones.Contains(2)) || (lstCorridorZones.Contains(6)) || (lstCorridorZones.Contains(8)))
                    {
                        if (lstCorridorZones.Contains(6))
                        {
                            iRetVal = 6;
                        }
                        else if (lstCorridorZones.Contains(8))
                        {
                            iRetVal = 8;
                        }
                        else
                        {
                            iRetVal = 2;
                        }
                    }
                    else
                    {
                        if (lstCorridorZones.Contains(7))
                        {
                            iRetVal = 7;
                        }
                        else
                        {
                            iRetVal = 1;
                        }
                    }
                    break;
                case 5:
                    // This should never be
                    break;
                case 6:
                    if ((lstCorridorZones.Contains(1)) || (lstCorridorZones.Contains(7)))
                    {
                        if (lstCorridorZones.Contains(7))
                        {
                            iRetVal = 7;
                        }
                        else
                        {
                            iRetVal = 1;
                        }
                    }
                    else if ((lstCorridorZones.Contains(2)) || (lstCorridorZones.Contains(4)) || (lstCorridorZones.Contains(8)))
                    {
                        if (lstCorridorZones.Contains(4))
                        {
                            iRetVal = 4;
                        }
                        else if (lstCorridorZones.Contains(8))
                        {
                            iRetVal = 8;
                        }
                        else
                        {
                            iRetVal = 2;
                        }
                    }
                    else
                    {
                        if (lstCorridorZones.Contains(9))
                        {
                            iRetVal = 9;
                        }
                        else
                        {
                            iRetVal = 3;
                        }
                    }
                    break;
                case 7:
                    if (lstCorridorZones.Contains(3))
                    {
                        iRetVal = 3;
                    }
                    else if ((lstCorridorZones.Contains(2)) || (lstCorridorZones.Contains(6)))
                    {
                        if (lstCorridorZones.Contains(2))
                        {
                            iRetVal = 2;
                        }
                        else
                        {
                            iRetVal = 6;
                        }
                    }
                    else if ((lstCorridorZones.Contains(1)) || (lstCorridorZones.Contains(9)))
                    {
                        if (lstCorridorZones.Contains(1))
                        {
                            iRetVal = 1;
                        }
                        else
                        {
                            iRetVal = 9;
                        }
                    }
                    else
                    {
                        if (lstCorridorZones.Contains(4))
                        {
                            iRetVal = 4;
                        }
                        else
                        {
                            iRetVal = 8;
                        }
                    }
                    break;
                case 8:
                    if ((lstCorridorZones.Contains(1)) || (lstCorridorZones.Contains(3)))
                    {
                        if (lstCorridorZones.Contains(1))
                        {
                            iRetVal = 1;
                        }
                        else
                        {
                            iRetVal = 3;
                        }
                    }
                    else if ((lstCorridorZones.Contains(2)) || (lstCorridorZones.Contains(4)) || (lstCorridorZones.Contains(6)))
                    {
                        if (lstCorridorZones.Contains(2))
                        {
                            iRetVal = 2;
                        }
                        else if (lstCorridorZones.Contains(4))
                        {
                            iRetVal = 4;
                        }
                        else
                        {
                            iRetVal = 6;
                        }
                    }
                    else
                    {
                        if (lstCorridorZones.Contains(7))
                        {
                            iRetVal = 7;
                        }
                        else
                        {
                            iRetVal = 9;
                        }
                    }
                    break;
                case 9:
                    if (lstCorridorZones.Contains(1))
                    {
                        iRetVal = 1;
                    }
                    else if ((lstCorridorZones.Contains(2)) || (lstCorridorZones.Contains(4)))
                    {
                        if (lstCorridorZones.Contains(2))
                        {
                            iRetVal = 2;
                        }
                        else
                        {
                            iRetVal = 4;
                        }
                    }
                    else if ((lstCorridorZones.Contains(3)) || (lstCorridorZones.Contains(7)))
                    {
                        if (lstCorridorZones.Contains(3))
                        {
                            iRetVal = 3;
                        }
                        else
                        {
                            iRetVal = 7;
                        }
                    }
                    else
                    {
                        if (lstCorridorZones.Contains(6))
                        {
                            iRetVal = 6;
                        }
                        else
                        {
                            iRetVal = 8;
                        }
                    }
                    break;
                default:
                    break;
            }

            return iRetVal;
        }

        public static ArrayList DetermineAvailableDoorLocations(ArrayList lstCorridorZones)
        {
            ArrayList lstAvailableDoorLocations = new ArrayList();

            foreach (int iCorridorZone in lstCorridorZones)
            {
                switch (iCorridorZone)
                {
                    case 1:
                        if (false == lstCorridorZones.Contains(2))
                        {
                            lstAvailableDoorLocations.Add("1-Right");
                        }
                        if (false == lstCorridorZones.Contains(4))
                        {
                            lstAvailableDoorLocations.Add("1-Top");
                        }
                        break;
                    case 2:
                        if (false == lstCorridorZones.Contains(1))
                        {
                            lstAvailableDoorLocations.Add("2-Left");
                        }
                        if (false == lstCorridorZones.Contains(3))
                        {
                            lstAvailableDoorLocations.Add("2-Right");
                        }
                        if (false == lstCorridorZones.Contains(5))
                        {
                            lstAvailableDoorLocations.Add("2-Top");
                        }
                        break;
                    case 3:
                        if (false == lstCorridorZones.Contains(2))
                        {
                            lstAvailableDoorLocations.Add("3-Left");
                        }
                        if (false == lstCorridorZones.Contains(6))
                        {
                            lstAvailableDoorLocations.Add("3-Top");
                        }
                        break;
                    case 4:
                        if (false == lstCorridorZones.Contains(1))
                        {
                            lstAvailableDoorLocations.Add("4-Bottom");
                        }
                        if (false == lstCorridorZones.Contains(5))
                        {
                            lstAvailableDoorLocations.Add("4-Right");
                        }
                        if (false == lstCorridorZones.Contains(7))
                        {
                            lstAvailableDoorLocations.Add("4-Top");
                        }
                        break;
                    case 5:
                        if (false == lstCorridorZones.Contains(2))
                        {
                            lstAvailableDoorLocations.Add("5-Bottom");
                        }
                        if (false == lstCorridorZones.Contains(4))
                        {
                            lstAvailableDoorLocations.Add("5-Left");
                        }
                        if (false == lstCorridorZones.Contains(6))
                        {
                            lstAvailableDoorLocations.Add("5-Right");
                        }
                        if (false == lstCorridorZones.Contains(8))
                        {
                            lstAvailableDoorLocations.Add("5-Top");
                        }
                        break;
                    case 6:
                        if (false == lstCorridorZones.Contains(3))
                        {
                            lstAvailableDoorLocations.Add("6-Bottom");
                        }
                        if (false == lstCorridorZones.Contains(5))
                        {
                            lstAvailableDoorLocations.Add("6-Left");
                        }
                        if (false == lstCorridorZones.Contains(9))
                        {
                            lstAvailableDoorLocations.Add("6-Top");
                        }
                        break;
                    case 7:
                        if (false == lstCorridorZones.Contains(8))
                        {
                            lstAvailableDoorLocations.Add("7-Right");
                        }
                        if (false == lstCorridorZones.Contains(4))
                        {
                            lstAvailableDoorLocations.Add("7-Bottom");
                        }
                        break;
                    case 8:
                        if (false == lstCorridorZones.Contains(7))
                        {
                            lstAvailableDoorLocations.Add("8-Left");
                        }
                        if (false == lstCorridorZones.Contains(9))
                        {
                            lstAvailableDoorLocations.Add("8-Right");
                        }
                        if (false == lstCorridorZones.Contains(5))
                        {
                            lstAvailableDoorLocations.Add("8-Bottom");
                        }
                        break;
                    case 9:
                        if (false == lstCorridorZones.Contains(8))
                        {
                            lstAvailableDoorLocations.Add("9-Left");
                        }
                        if (false == lstCorridorZones.Contains(6))
                        {
                            lstAvailableDoorLocations.Add("9-Bottom");
                        }
                        break;
                    default:
                        break;
                }
            }
            return lstAvailableDoorLocations;
        }
    }
}
