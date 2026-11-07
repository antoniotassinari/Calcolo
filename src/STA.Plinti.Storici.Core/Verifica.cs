using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Xml;

namespace STA_Dimensionamento_Plinti;

public class Verifica
{

    private double sEx { get; set; }
    private double sEy { get; set; }
    private double sAreaEff { get; set; }
    private double sPressione { get; set; }

    public double Ex { get { return sEx; } }
    public double Ey { get { return sEy; } }
    public double AreaEff { get { return sAreaEff; } }
    public double Pressione { get { return sPressione; } }

    public Verifica(Reazione rea, Plinto pli)
    {
        sEx = rea.MX * 100 * 100 / (pli.PesoTotale() + rea.FZ * 100);
        sEy = rea.MY * 100 * 100 / (pli.PesoTotale() + rea.FZ * 100);

        sAreaEff = (pli.A - 2 * Ex) * (pli.B - 2 * Ey);

        sPressione = (pli.PesoTotale() + rea.FZ * 100) / AreaEff;

    }
}
