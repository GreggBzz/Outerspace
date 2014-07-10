using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OuterSpace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (OuterSpace OuterSpaceForm = new OuterSpace())
            {
                OuterSpaceForm.Show();
                             
                Application.Run(OuterSpaceForm);
            }
        }
    }
}