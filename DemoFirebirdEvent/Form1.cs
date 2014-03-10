using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FirebirdSql.Data.FirebirdClient;

namespace DemoFirebirdEvent
{
    public partial class Form1 : Form
    {
        private FbConnection conn = null;

        public Form1()
        {
            InitializeComponent();

            string dbPath = System.IO.Directory.GetCurrentDirectory() + "\\FBEVENT.FDB";
            string server = "127.0.0.1";

            string strCon = "User ID=SYSDBA;Password=masterkey;Database=" + server + ":" + dbPath;
            conn = new FbConnection(strCon);
            conn.Open();

            // mendaftarkan event yang akan ditangkap
            string[] daftarEvent = { "NEW_CUSTOMER", "NEW_SUPPLIER" };
            FbRemoteEvent remoteEvent = new FbRemoteEvent(conn, daftarEvent);

            // proses delegate, untuk mendaftarkan method RemoteEventCounts pada event FbRemoteEventEventHandler
            remoteEvent.RemoteEventCounts += new FbRemoteEventEventHandler(RemoteEventCounts);

            remoteEvent.QueueEvents();
        }

        private void RemoteEventCounts(object sender, FbRemoteEventEventArgs args)
        {
            if (args.Counts == 1)
            {
                switch (args.Name)
                {
                    case "NEW_CUSTOMER":
                        LoadData("customer", lstCustomer);
                        break;

                    case "NEW_SUPPLIER":
                        LoadData("supplier", lstSupplier);
                        break;
                }
            }
        }

        private void LoadData(string tableName, ListBox lst)
        {
            string strSql = "SELECT nama, alamat FROM " + tableName;

            lst.Items.Clear();
            using (FbCommand cmd = new FbCommand(strSql, conn))
            {
                using (FbDataReader dtr = cmd.ExecuteReader())
                {
                    while (dtr.Read())
                    {
                        lst.Items.Add(dtr.GetString(0) + ", " + dtr.GetString(1));
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
    }
}