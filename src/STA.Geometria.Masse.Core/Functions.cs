
using System.Collections.Generic;
using System;

using STA.Geometria;

namespace STA.Geometria.Masse.Functions
{
    public class Functions
    {
        /// <summary>
        /// Calcola l'area
        /// </summary>
        public static double Area2D(List<Punto2D> VerticiIn)
        {
            double sArea = 0;

            ///Verifica se i punti sono almeno 3
            if (VerticiIn.Count > 2)
            {
                for (int i = 1; i < VerticiIn.Count; i++)
                {
                    sArea += AreaSottesa(VerticiIn[i-1], VerticiIn[i]);
                }

                sArea += AreaSottesa(VerticiIn[VerticiIn.Count-1], VerticiIn[0]);
            }
            return sArea;
        }


        /// <summary>
        /// Metodo per il calcolo dell'area sottesa
        /// </summary>
        /// <param name="punto1">punto iniziale</param>
        /// <param name="punto2">punto finale</param>
        /// <returns>area sottesa</returns>
        public static double AreaSottesa(Punto2D punto1, Punto2D punto2)
        {
            double sAreaSottesa = 0;

            sAreaSottesa = (punto2.X - punto1.X) * (punto2.Y + punto1.Y) / 2;

            return sAreaSottesa;
        }

        /// <summary>
        /// Calcola il baricentro della sezione
        /// </summary>
        public static Punto2D calcolaBaricentro(List<Punto2D> VerticiIn)
        {
            ArgumentNullException.ThrowIfNull(VerticiIn);
            if (VerticiIn.Count < 3) throw new ArgumentException("Occorrono almeno tre vertici");
            double twiceArea = 0, sx = 0, sy = 0;
            for (int i = 0; i < VerticiIn.Count; i++)
            {
                var a = VerticiIn[i]; var b = VerticiIn[(i + 1) % VerticiIn.Count];
                if (!double.IsFinite(a.X) || !double.IsFinite(a.Y)) throw new ArgumentException("Coordinate non finite");
                double cross = a.X * b.Y - b.X * a.Y;
                twiceArea += cross; sx += (a.X + b.X) * cross; sy += (a.Y + b.Y) * cross;
            }
            if (twiceArea == 0 || !double.IsFinite(twiceArea)) throw new ArgumentException("Sezione degenere");
            return new Punto2D { ID = "Baricentro", X = sx / (3 * twiceArea), Y = sy / (3 * twiceArea) };

        }

        /// <summary>Alias mantenuto per compatibilita sorgente; usa il calcolo gestito.</summary>
        [Obsolete("Usare Area2D: il calcolo non richiede DLL Fortran.")]
        public static double FortranArea2D(List<Punto2D> vertices) => Area2D(vertices);
    }
}
