using System.Windows.Forms;
namespace STA.Carichi.Sisma;
public static class SismaGridExtensions
{
        public static void elencaPuntiVicini(this Sisma sisma, DataGridView gridIn)    //
        {

            gridIn.Rows.Clear();
            gridIn.ColumnCount = 4;
            gridIn.ReadOnly = true;
            gridIn.RowHeadersVisible = false;
            gridIn.Columns[0].HeaderText = "ID";
            gridIn.Columns[1].HeaderText = "Long.";
            gridIn.Columns[2].HeaderText = "Lat.";
            gridIn.Columns[3].HeaderText = "Distanza virtuale";
            gridIn.AutoSize = true;
            int i;
            DataGridViewRowCollection rows = gridIn.Rows;
            for (i = 0; i < 4; i++)
            {
                string[] riga = { sisma.elencoDistanze[i].puntoEst.ID.ToString(), sisma.elencoDistanze[i].puntoEst.X.ToString("F5"), sisma.elencoDistanze[i].puntoEst.Y.ToString("F5"), sisma.elencoDistanze[i].distanzaPunti.ToString("F5") };


                rows.Add(riga);
            }
        }
        public static void stampaCaratteristicheSpettro(this Sisma sisma, DataGridView dataGridIn, spettro spettroIn)
        {
            dataGridIn.Rows.Clear();
            dataGridIn.ColumnCount = 2;
            dataGridIn.Columns[0].HeaderText = "Coeff.";
            dataGridIn.Columns[1].HeaderText = "Valore";
            dataGridIn.AutoSize = true;
            dataGridIn.RowHeadersVisible = false;
            dataGridIn.AllowUserToAddRows = false;
            dataGridIn.AllowUserToOrderColumns = false;


            string[] rigaPvr = {"Pvr", spettroIn.numPvr.ToString("F2") };
            dataGridIn.Rows.Add(rigaPvr);

            string[] rigaTr = { "Tr", spettroIn.numTr.ToString("F2") };
            dataGridIn.Rows.Add(rigaTr);

            string[] rigaTcstar = { "Tc*", spettroIn.numTcStarMedia.ToString("F5") };
            dataGridIn.Rows.Add(rigaTcstar);

            string[] rigaAgmedia = { "agMedia*", spettroIn.numAgMedia.ToString("F5") };
            dataGridIn.Rows.Add(rigaAgmedia);

            string[] rigaf0 = { "f0Media*", spettroIn.numF0Media.ToString("F5") };
            dataGridIn.Rows.Add(rigaf0);

            string[] rigaTb = { "Tb", spettroIn.numTb.ToString("F5") };
            dataGridIn.Rows.Add(rigaTb);

            string[] rigaTc = { "Tc", spettroIn.numTc.ToString("F5") };
            dataGridIn.Rows.Add(rigaTc);

            string[] rigaTd = { "Tb", spettroIn.numTd.ToString("F5") };
            dataGridIn.Rows.Add(rigaTd);

            string[] rigaS = { "S", spettroIn.numS.ToString("F5") };
            dataGridIn.Rows.Add(rigaS);

            string[] rigaFv = { "Fv", spettroIn.numFv.ToString("F5") };
            dataGridIn.Rows.Add(rigaFv);


            dataGridIn.ReadOnly = true;

        }
        public static void stampaSpettro(this Sisma sisma, DataGridView dataGridIn, spettro spettroIn)
        {
            dataGridIn.Rows.Clear();
            dataGridIn.ColumnCount = 3;
            dataGridIn.ReadOnly = true;
            dataGridIn.RowHeadersVisible = false;
            dataGridIn.Columns[0].HeaderText = "Periodo";
            dataGridIn.Columns[1].HeaderText = "Risposta Orizzontale";
            dataGridIn.Columns[2].HeaderText = "Risposta Verticale";
            dataGridIn.AutoSize = true;
            int i;
            for (i = 0; i < spettroIn.valoriSpettrali.Count; i++)
            {
                string[] riga = {spettroIn.valoriSpettrali[i].periodo.ToString("F5"), spettroIn.valoriSpettrali[i].risposta.ToString("F5"), spettroIn.valoriSpettrali[i].rispostaVert.ToString("F5")};
                dataGridIn.Rows.Add(riga);
            }


        }
}
