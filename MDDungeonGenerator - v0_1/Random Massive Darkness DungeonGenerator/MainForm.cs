using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace MassiveDarknessRandomDungeonGenerator
{
    public partial class DungeonGeneratorForm : Form
    {
        private String sLoadErrors = String.Empty;
        private ArrayList lstChips = null;
        private ArrayList lstDice = null;
        private DungeonMap mapDungeon = null;
        private Die dieRed = null;
        private Die dieGreen = null;
        private Chip chpStart = null;
        private Chip chpExit = null;
        private Chip chpDoor = null;
        private Chip chpBridge = null;
        private Chip chpLevel1 = null;
        private Chip chpLevel2 = null;
        private Chip chpLevel3 = null;
        private Chip chpLevel4 = null;
        private Chip chpLevel5 = null;

        public DungeonGeneratorForm()
        {
            InitializeComponent();

            lstChips = new ArrayList();
            lstDice = new ArrayList();
        }

        private void DungeonGeneratorForm_Load(object sender, EventArgs e)
        {
            // Load the default dungeon sizes
            cboDungeonSize.DisplayMember = "SizeDescription";
            cboDungeonSize.ValueMember = "NumberTiles";
            cboDungeonSize.DataSource = GenerateDungeonSizes();

            // Load the XML configuration file that defines the supported tilesets
            ParseConfigFile("MassiveDarknessInfo.cfg");
        }

        private DataTable GenerateDungeonSizes()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SizeDescription");
            dt.Columns.Add("NumberTiles");
            dt.Rows.Add("Small (2 Tiles)", 2);
            dt.Rows.Add("Medium Lesser (3 Tiles)", 3);
            dt.Rows.Add("Medium (4 Tiles)", 4);
            dt.Rows.Add("Large Lesser (5 Tiles)", 5);
            dt.Rows.Add("Large Greater (6 Tiles)", 6);
            dt.Rows.Add("Huge Lesser (7 Tiles)", 7);
            dt.Rows.Add("Huge (8 Tiles)", 8);
            dt.Rows.Add("Huge Greater (9 Tiles)", 9);

            return dt;
        }

        private void ParseConfigFile(string filename)
        {
            try
            {
                string sConfigContents = TokenUtilities.GetEmbeddedText("MassiveDarknessRandomDungeonGenerator.MassiveDarknessInfo.cfg");
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(sConfigContents);

                // Extract the Tile sets
                XmlNodeList xmlTileSets = xmlDoc.GetElementsByTagName("TileSet");
                foreach (XmlNode xmlTileSet in xmlTileSets)
                {
                    TileSet newTileSet = new TileSet(xmlTileSet);
                    lstTilesets.Items.Add(newTileSet);
                }

                // Extract the Chips
                XmlNodeList xmlChips = xmlDoc.GetElementsByTagName("Chip");
                foreach (XmlNode xmlChip in xmlChips)
                {
                    Chip newChip = new Chip(xmlChip);
                    lstChips.Add(newChip);

                    switch (newChip.Name.ToLower())
                    {
                        case "start":
                            chpStart = newChip;
                            break;
                        case "exit":
                            chpExit = newChip;
                            break;
                        case "door":
                            chpDoor = newChip;
                            break;
                        case "bridge":
                            chpBridge = newChip;
                            break;
                        case "lvl1":
                            chpLevel1 = newChip;
                            break;
                        case "lvl2":
                            chpLevel2 = newChip;
                            break;
                        case "lvl3":
                            chpLevel3 = newChip;
                            break;
                        case "lvl4":
                            chpLevel4 = newChip;
                            break;
                        case "lvl5":
                            chpLevel5 = newChip;
                            break;
                        default:
                            break;
                    }
                }

                // Extract the Dice
                XmlNodeList xmlDice = xmlDoc.GetElementsByTagName("Die");
                foreach (XmlNode xmlDie in xmlDice)
                {
                    Die newDie = new Die(xmlDie);
                    lstDice.Add(newDie);
                    switch (newDie.Name.ToLower())
                    {
                        case "red":
                            dieRed = newDie;
                            break;
                        case "green":
                            dieGreen = newDie;
                            break;
                        default:
                            break;
                    }
                }

                // Validate the extraction of the Chips and Dice
                if ((null == chpStart) || (null == chpExit) || (null == chpDoor) || (null == chpBridge))
                {
                    sLoadErrors += Environment.NewLine + "Error parsing configuration file: Chip extraction failed.";
                }
                if ((null == chpLevel1) || (null == chpLevel2) || (null == chpLevel3) || (null == chpLevel4) || (null == chpLevel5))
                {
                    sLoadErrors += Environment.NewLine + "Error parsing configuration file: Chip Level extraction failed.";
                }
                if ((null == dieRed) || (null == dieGreen))
                {
                    sLoadErrors += Environment.NewLine + "Error parsing configuration file: Die extraction failed.";
                }
            }
            catch (Exception exXMLParse)
            {
                sLoadErrors = "Error parsing configuration file:" + Environment.NewLine + Environment.NewLine + exXMLParse.Message;
            }
        }

        private void DungeonGeneratorForm_Shown(object sender, EventArgs e)
        {
            if (String.Empty != sLoadErrors)
            {
                MessageBox.Show(this, sLoadErrors, "Load Errors");
            }
        }

        #region Menu Event Handlers
        private void generateDungeonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // call the button event handler
            btnGenerate_Click(sender, e);
        }

        private void saveDungeonMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // call the button event handler
            btnSaveMap_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show an about message
            HelpForm frmHelp = new HelpForm();
            frmHelp.ShowDialog(this);
        }
        #endregion

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //// Generate a random Dungeon
            // * Start with form validation
            // ** Are there any tile sets selected?
            if (0 == lstTilesets.SelectedItems.Count)
            {
                MessageBox.Show(this, "You must select at least one Tile Set.", this.Text);
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            // * Declare our new Dungeon variables
            DungeonMap newDungeonMap = null;
            ArrayList lstAvailableTiles = new ArrayList();
            int iSelectedTileCount = 0;

            // Try to generate a dungeon several times before informing the user because this can fail easily on larger dungeons
            int iLayoutTries = 10;
            bool isLayoutSuccessful = false;
            while (iLayoutTries > 0)
            {
                iLayoutTries--;

                // * Reset out Dungeon variables
                newDungeonMap = new DungeonMap();
                lstAvailableTiles.Clear();
                iSelectedTileCount = 0;

                try
                {
                    // * Based upon the tile sets selected, create an aggregate collection of all available tiles to choose from
                    foreach (TileSet selectedTileSet in lstTilesets.SelectedItems)
                    {
                        // Copy each tile so that the dungeon can manipulate it without impacting the master list of available tiles
                        foreach (Tile tCurrentTile in selectedTileSet.Tiles)
                        {
                            Tile tileToAdd = new Tile(tCurrentTile);
                            // See if the reverse side of the tile is already in the tile collection
                            foreach (Tile tIterator in lstAvailableTiles)
                            {
                                if (tIterator.Name == tileToAdd.ReverseSideName)
                                {
                                    tileToAdd.ReverseSideTile = tIterator;
                                    tIterator.ReverseSideTile = tileToAdd;
                                    break;
                                }
                            }

                            // Add the tile
                            lstAvailableTiles.Add(tileToAdd);
                        }
                        iSelectedTileCount += selectedTileSet.UniqueTileCount;
                    }

                    // ** Is the Dungeon Size too big for the Tile Sets selected?
                    int iDungeonSizeInTiles = int.Parse((string)(cboDungeonSize.SelectedValue));
                    if (iDungeonSizeInTiles > iSelectedTileCount)
                    {
                        MessageBox.Show(this, "Your Dungeon Size is too big for the selected Tile Sets", this.Text);
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    // * Based upon the selected Dungeon size, randomly select the correct number of tiles
                    Random rndNumber = new Random();
                    for (int iCounter = 0; iCounter < iDungeonSizeInTiles; iCounter++)
                    {
                        // Get a random number tile index between 0 and (lstAvailableTiles.Count - 1).
                        // However, in the call to Next, the max is excluded, so we supply the full Count
                        int randomIndex = rndNumber.Next(0, lstAvailableTiles.Count);
                        Tile randomTile = (Tile)(lstAvailableTiles[randomIndex]);
                        newDungeonMap.AddTile(randomTile);
                        RemoveTiles(randomTile, lstAvailableTiles);
                    }

                    // * Randomly rotate and flip the selected dungeon tiles
                    newDungeonMap.RandomlyRotateFlipTiles(dieRed, chpBridge);
                    newDungeonMap.AddChips(dieGreen, chpStart, chpExit, chpDoor);
                    newDungeonMap.AssignLevels(chkDifficultyLevel.Checked, chpLevel1, chpLevel2, chpLevel3, chpLevel4, chpLevel5);

                    // * All setup is done, perform layout
                    newDungeonMap.PerformLayout(chkFadedTiles.Checked, chkIncludeLayout.Checked, chpDoor, chpLevel1, chpLevel2, chpLevel3, chpLevel4, chpLevel5);
                    isLayoutSuccessful = true;
                    break;
                }
                catch (Exception exPerformLayout)
                {
                    // Swallow the exception if we still have more tries left
                    if (0 >= iLayoutTries)
                    {
                        this.Cursor = Cursors.Default;
                        MessageBox.Show(this, exPerformLayout.Message, this.Text);
                    }
                    else
                    {
                        // Ensure we pause a bit between tries to allow the random number generator a chance to change
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }

            if (true == isLayoutSuccessful)
            {
                // * Add the image to the form as a preview
                pbDungeonPreview.Image = newDungeonMap.DungeonImage;

                // * Save the new map as the current map
                mapDungeon = newDungeonMap;
            }

            this.Cursor = Cursors.Default;
        }

        private void btnSaveMap_Click(object sender, EventArgs e)
        {
            if (null == mapDungeon)
            {
                MessageBox.Show(this, "You must generate a Dungeon before saving it as a BGMapEditor map.", this.Text);
                return;
            }
            // Generate a MAP file that is compatible with the open source BG Map Editor
            SaveFileDialog sfdSaveMapFile = new SaveFileDialog();
            sfdSaveMapFile.AddExtension = true;
            sfdSaveMapFile.CheckPathExists = true;
            sfdSaveMapFile.DefaultExt = ".map";
            sfdSaveMapFile.Filter = "BGMapEditor Map (*.map)|*.map";
            sfdSaveMapFile.OverwritePrompt = true;
            sfdSaveMapFile.Title = "Save your BGMapEditor dungeon map...";
            if (sfdSaveMapFile.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    // Does this overwrite existing files?
                    File.WriteAllText(sfdSaveMapFile.FileName, mapDungeon.BGMapEditorFileAsString);
                }
                catch (Exception exSave)
                {
                    MessageBox.Show(this, "An error occurred while saving the file:" + Environment.NewLine + Environment.NewLine + exSave.Message, this.Text);
                    return;
                }

                // All good if we got this far
                MessageBox.Show(this, "MAP file saved successfully.", this.Text);
            }
        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            if (null == mapDungeon)
            {
                MessageBox.Show(this, "You must generate a Dungeon before copying to clipboard.", this.Text);
                return;
            }

            if (null != pbDungeonPreview.Image)
            {
                try
                {
                    Clipboard.SetImage(pbDungeonPreview.Image);
                    MessageBox.Show(this, "Dungeon copied to clipboard.", this.Text);
                }
                catch (Exception exClipboard)
                {
                    MessageBox.Show(this, "Unable to copy Dungeon image to clipboard:" + Environment.NewLine + Environment.NewLine + exClipboard.Message, this.Text);
                }
            }
            else
            {
                MessageBox.Show(this, "No Dungeon image found.", this.Text);
            }
        }

        private void RemoveTiles(Tile tileToRemove, ArrayList lstTileCollection)
        {
            // First, see if the tile to remove has a corresponding flip side that must also be removed
            if (null != tileToRemove.ReverseSideTile)
            {
                lstTileCollection.Remove(tileToRemove.ReverseSideTile);
            }

            // Remove the requested tile
            lstTileCollection.Remove(tileToRemove);
        }

        private void copyDungeonToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnCopyToClipboard_Click(null, null);
        }
    }
}