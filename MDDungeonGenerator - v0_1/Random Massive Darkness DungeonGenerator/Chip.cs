using System;
using System.Xml;

namespace MassiveDarknessRandomDungeonGenerator
{
    public class Chip
    {
        private string sName;
        private int iWidth;
        private int iHeight;
        private string sBGMapEditorTilePath;
        private int iLayoutTileWidth;
        private int iLayoutTileHeight;
        private string sBGMapEditorLayoutTilePath;

        public Chip(XmlNode xmlChip)
        {
            if (null != xmlChip.ChildNodes)
            {
                foreach (XmlNode infoElement in xmlChip.ChildNodes)
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
                            } break;
                        case "BGMapEditorTilePath":
                            sBGMapEditorTilePath = infoElement.InnerText;
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
        public string BGMapEditorTilePath
        {
            get { return sBGMapEditorTilePath; }
        }
        public int LayoutTileWidth
        {
            get { return iLayoutTileWidth; }
        }
        public int LayoutTileHeight
        {
            get { return iLayoutTileHeight; }
        }
        public string BGMapEditorLayoutTilePath
        {
            get { return sBGMapEditorLayoutTilePath; }
        }
    }
}
