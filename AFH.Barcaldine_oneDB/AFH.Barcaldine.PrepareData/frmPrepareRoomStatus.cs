using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AFH.Barcaldine.Core;
using AFH.Barcaldine.Models;
using AFH.Barcaldine.Common;

namespace AFH.Barcaldine.PrepareData
{
    public partial class frmPrepareRoomStatus : Form
    {
        public frmPrepareRoomStatus()
        {
            InitializeComponent();
        }

        private void btnIncrease_Click(object sender, EventArgs e)
        {
            RoomStatusCore core = new RoomStatusCore();

            RoomStatusModel model = new RoomStatusModel();
            model.Year = Convert.ToInt32(this.txtYear.Text);
 
            
            model.RoomType = GlobalVariable.RoomType.King;
            core.InsertData(model);

            model.RoomType = GlobalVariable.RoomType.Queen;
            core.InsertData(model);

            model.RoomType = GlobalVariable.RoomType.Princess;
            core.InsertData(model);

        }
    }
}
