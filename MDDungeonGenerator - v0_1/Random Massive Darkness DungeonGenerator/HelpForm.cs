using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MassiveDarknessRandomDungeonGenerator
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            string sHelpContent = String.Empty;

            try
            {
                sHelpContent = TokenUtilities.GetEmbeddedText("MassiveDarknessRandomDungeonGenerator.MDRandomDungeonGeneratorHelpContents.rtf");
            }
            catch (Exception exHelp)
            {
                // use default help content
                sHelpContent = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Courier New;}{\f1\fnil\fcharset2 Symbol;}}" +
                                    @" {\*\generator Msftedit 5.41.21.2510;}\viewkind4\uc1\pard\fs22 Unable to load About contents: " + exHelp.Message + @" \par}";
            }

            rtxtHelpContent.Rtf = sHelpContent;
        }
    }
}