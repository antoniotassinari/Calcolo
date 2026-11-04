using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace STA.Carichi.Sisma.Funzioni
{
    public class funzioni   //mi serve solo per definire delle funzioni
    {
        public static int calcoloVn(int tipoOperaIn)
        {

            switch (tipoOperaIn)       
            {
                case 0:
                    return 10;
                    
                case 1:
                    return 50;
                    
                case 2:
                    return 100;
                    
                default:
                    return 50;
                    
           
        }

        }

        public static double calcoloVr(double VnIn, int classeUsoIn)
        {
            double vRprov = VnIn * cu(classeUsoIn);
            if (vRprov < 35)
            {
                vRprov = 35;
            }

            return vRprov;
        }

        public static double calcoloPvr(Sisma.statiLimite statoLimiteIn)
        {
            switch (statoLimiteIn)
            {
                case Sisma.statiLimite.SLO:
                    return 0.81;

                case Sisma.statiLimite.SLD:
                    return 0.63;

                case Sisma.statiLimite.SLV:
                    return 0.10;

                case Sisma.statiLimite.SLC:
                    return 0.05;
                    
                default:
                    return 0.10;
                    
            }
        }

        public static double cu(int i)
        {
            switch (i)
            {
                case 0:
                    return 0.7;

                case 1:
                    return 1;

                case 2:
                    return 1.5;

                case 3:
                    return 2;

                default:
                    return 1;

            }
        }

        public static int calcoloNumTr(double numVrIn, Sisma.statiLimite statoIn)
        {
            int prov = Convert.ToInt32(Math.Round(-numVrIn / Math.Log((1 - calcoloPvr(statoIn))), 0));

            if (prov < 30)
            { return 30; }

            if (prov > 2475)
            { return 2475; }
            return prov;
        }

        /// <summary>
        /// Metodo per il calcolo della risposta
        /// </summary>
        /// <param name="periodoIn">periodo di calcolo</param>
        /// <param name="spettroIn">spettro in ingresso</param>
        /// <param name="q">fattore di struttura</param>
        /// <returns></returns>
        public static double calcolaRisposta(double periodoIn, spettro spettroIn, double q)
        {
            double risp = new double();
            if (periodoIn < spettroIn.numTb)
            {
                risp = spettroIn.numAgMedia * spettroIn.numS / q * spettroIn.numF0Media * (periodoIn / spettroIn.numTb + 1 / spettroIn.numF0Media * q * (1 - (periodoIn / spettroIn.numTb)));

            }

            if (periodoIn >= spettroIn.numTb & periodoIn < spettroIn.numTc)
            {
                risp = spettroIn.numAgMedia * spettroIn.numS * spettroIn.numF0Media / q;
            }
            if (periodoIn >= spettroIn.numTc & periodoIn < spettroIn.numTd)
            {
                risp = spettroIn.numAgMedia * spettroIn.numS * spettroIn.numF0Media * spettroIn.numTc / periodoIn / q;
            }
            if (periodoIn >= spettroIn.numTd)
            {
                risp = spettroIn.numAgMedia * spettroIn.numS * spettroIn.numF0Media * (spettroIn.numTc * spettroIn.numTd / Math.Pow(periodoIn, 2)) / q;
            }
            return risp;


        }

        public static double calcolaRispostaVert(double periodoIn, spettro spettroIn)
        {
            double risp = new double();
            if (periodoIn < spettroIn.numTb)
            {
                risp = spettroIn.numAgMedia * spettroIn.numS * spettroIn.numFv * (periodoIn / spettroIn.numTb + 1 / spettroIn.numF0Media * (1 - (periodoIn / spettroIn.numTb)));

            }

            if (periodoIn >= spettroIn.numTb & periodoIn < spettroIn.numTc)
            {
                risp = spettroIn.numAgMedia * spettroIn.numS * spettroIn.numFv;
            }
            if (periodoIn >= spettroIn.numTc & periodoIn < spettroIn.numTd)
            {
                risp = spettroIn.numAgMedia * spettroIn.numS * spettroIn.numFv * spettroIn.numTc / periodoIn;
            }
            if (periodoIn >= spettroIn.numTd)
            {
                risp = spettroIn.numAgMedia * spettroIn.numS * spettroIn.numFv * (spettroIn.numTc * spettroIn.numTd / Math.Pow(periodoIn, 2));
            }
            return risp;


        }

        public static double calcolaDistanza(Point3d punto1, Point3d punto2)
        {
            double dist = Math.Sqrt(Math.Pow(Math.Abs(punto2.X) - Math.Abs(punto1.X), 2) + Math.Pow(Math.Abs(punto2.Y) - Math.Abs(punto1.Y), 2));
            return dist;
        }

        public static double media(string param, Sisma sismaIn, int tempoRitorno)
        {
            int i;
            switch (param)
            {
                case "ag":
                    i = 0;
                    break;
                case "f0":
                    i = 1;
                    break;
                case "tc*":
                    i = 2;
                    break;
                default:
                    i = 0;
                    break;
            }


            double tR01 = 0;
            double tR02 = 1;
            int val01 = 3;
            int val02 = 6;

            if (tempoRitorno < 30)
            {
                tR01 = 30;
                tR02 = 31;
            }

            if (tempoRitorno >= 30 && tempoRitorno < 50)
            {
                tR01 = 30;
                tR02 = 50;
            }

            if (tempoRitorno >= 50 && tempoRitorno < 72)
            {
                tR01 = 50;
                tR02 = 72;
                val01 = 6;
                val02 = 9;
            }

            if (tempoRitorno >= 72 && tempoRitorno <= 101)
            {
                tR01 = 72;
                tR02 = 101;
                val01 = 9;
                val02 = 12;
            }

            if (tempoRitorno >= 101 && tempoRitorno <= 140)
            {
                tR01 = 101;
                tR02 = 140;
                val01 = 12;
                val02 = 15;
            }

            if (tempoRitorno >= 140 && tempoRitorno <= 201)
            {
                tR01 = 140;
                tR02 = 201;
                val01 = 15;
                val02 = 18;
            }

            if (tempoRitorno >= 201 && tempoRitorno <= 475)
            {
                tR01 = 201;
                tR02 = 475;
                val01 = 18;
                val02 = 21;
            }

            if (tempoRitorno >= 475 && tempoRitorno <= 975)
            {
                tR01 = 475;
                tR02 = 975;
                val01 = 21;
                val02 = 24;
            }

            if (tempoRitorno >= 975 && tempoRitorno <= 2475)
            {
                tR01 = 975;
                tR02 = 2475;
                val01 = 24;
                val02 = 27;
            }

            if (tempoRitorno > 2475)
            {
                tR01 = 2474;
                tR02 = 2475;
                val01 = 27;
                val02 = 27;
            }

            double rapporto = (tempoRitorno - tR01) / (tR02 - tR01);

            double ag01;
            double ag02;
            double ag03;
            double ag04;

            val01 = val01 + i;
            val02 = val02 + i;
            ag01 = Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[0].puntoEst, val01)) + rapporto * (Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[0].puntoEst, val02)) - Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[0].puntoEst, val01)));
            ag02 = Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[1].puntoEst, val01)) + rapporto * (Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[1].puntoEst, val02)) - Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[1].puntoEst, val01)));
            ag03 = Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[2].puntoEst, val01)) + rapporto * (Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[2].puntoEst, val02)) - Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[2].puntoEst, val01)));
            ag04 = Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[3].puntoEst, val01)) + rapporto * (Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[3].puntoEst, val02)) - Convert.ToDouble(prelevaValore(sismaIn, sismaIn.elencoDistanze[3].puntoEst, val01)));


            double agMedia = (ag01 / sismaIn.elencoDistanze[0].distanzaPunti + ag02 / sismaIn.elencoDistanze[1].distanzaPunti + ag03 / sismaIn.elencoDistanze[2].distanzaPunti + ag04 / sismaIn.elencoDistanze[3].distanzaPunti) / (1 / sismaIn.elencoDistanze[0].distanzaPunti + 1 / sismaIn.elencoDistanze[1].distanzaPunti + 1 / sismaIn.elencoDistanze[2].distanzaPunti + 1 / sismaIn.elencoDistanze[3].distanzaPunti);

            return agMedia;
        }

        public static string prelevaValore(Sisma sismaIn, Point3d puntoIn, int colonna)
        {
            string valore = "";

            foreach (string[] controllo in sismaIn.righeConvertite)
            {
                if (controllo[0] == puntoIn.ID)
                {
                    valore = controllo[colonna];
                }
            }

            return valore;
        }

    }

}
