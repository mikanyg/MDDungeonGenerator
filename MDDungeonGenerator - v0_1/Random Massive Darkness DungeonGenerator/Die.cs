using System;
using System.Collections;
using System.Xml;

namespace MassiveDarknessRandomDungeonGenerator
{
    public class Die
    {
        private string sName;
        private ArrayList lstFaces;

        public Die(XmlNode xmlDie)
        {
            lstFaces = new ArrayList();

            if (null == xmlDie)
            {
                // Nothing to parse
                return;
            }

            if ((null != xmlDie.Attributes) && (null != xmlDie.Attributes["Color"]))
            {
                sName = xmlDie.Attributes["Color"].Value;
            }

            if (null != xmlDie.ChildNodes)
            {
                foreach (XmlNode infoElement in xmlDie.ChildNodes)
                {
                    // Only support the "Face" element
                    if (infoElement.Name == "Face")
                    {
                        DieFace newDieFace = new DieFace();

                        string sFaceContent = infoElement.InnerText;
                        string[] faceConents = sFaceContent.ToLower().Split(",".ToCharArray());
                        foreach (string sContentItem in faceConents)
                        {
                            switch (sContentItem)
                            {
                                case "sword":
                                    newDieFace.Swords++;
                                    break;
                                case "shield":
                                    newDieFace.Shields++;
                                    break;
                                case "bam":
                                    newDieFace.Bams++;
                                    break;
                                case "diamond":
                                    newDieFace.Diamonds++;
                                    break;
                                default:
                                    break;
                            }
                        }

                        lstFaces.Add(newDieFace);
                    }
                }
            }
        }

        public string Name
        {
            get { return sName; }
        }
        public ArrayList Faces
        {
            get { return lstFaces; }
        }
    }

    public class DieFace
    {
        public int Swords = 0;
        public int Shields = 0;
        public int Bams = 0;
        public int Diamonds = 0;

        public DieFace()
        {
        }
    }
}
