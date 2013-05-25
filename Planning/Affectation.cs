using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Planning
{
    public class Affectation
    {
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Projet { get; set; }
        public string Metier { get; set; }
        public string Societe { get; set; }
    }
}
