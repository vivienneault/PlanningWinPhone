using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Planning
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }
        //Déclaration d'un SoapCLient basé sur les web services mis en référence
        WebServicePPE4.WsGestionAffectationsSoapClient proxy = new WebServicePPE4.WsGestionAffectationsSoapClient();
        //Déclaration de deux listes d'affectations
        List<Affectation> lesAffects = new List<Affectation>();
        List<Affectation> AllAffectations = new List<Affectation>();

        //Evenement du chargement de page 
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //vide la liste d'affectation lesAffects
            lesAffects.Clear();
            //Récupère le paramètre id passé
            string matricule = NavigationContext.QueryString["id"];
            //Requête asynchrone vers la web method GetAffectationCollab, lève un évenement après éxecution de la requête
            proxy.GetAffectationsCollabCompleted += new EventHandler<WebServicePPE4.GetAffectationsCollabCompletedEventArgs>(proxy_GetAffectationsCollabCompleted);
            proxy.GetAffectationsCollabAsync(Convert.ToInt16(matricule));
        }

        //Evenement levé après execution de la requete
        void proxy_GetAffectationsCollabCompleted(object sender, WebServicePPE4.GetAffectationsCollabCompletedEventArgs e)
        {
            //remplit la liste d'affectation AllAffectation avec toutes les affectations renvoyé par la web method
            foreach (WebServicePPE4.Affectation a in e.Result)
            {
                Affectation affectation = new Affectation
                {
                    DateDebut = a.DateDebut,
                    DateFin = a.DateFin,
                    Projet = a.LeProjet.Nom,
                    Metier = a.LeMetier.Libelle,
                    Societe = a.LeProjet.CodeSociete
                };
                AllAffectations.Add(affectation);
            }
            //Appel de la fonction remplirAffectations
            remplirAffectations();
        }

        //Evenement lors du changement de sélection dans la listbox
        private void lstConsultation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Change les valeurs des labels en récupérant les informations de l'objet Affectation sélectionné
            tbDateDeb.Text = (lstConsultation.SelectedItem as Affectation).DateDebut.ToShortDateString();
            tbDateFin.Text = (lstConsultation.SelectedItem as Affectation).DateFin.ToShortDateString();
            tbMetier.Text = (lstConsultation.SelectedItem as Affectation).Metier;
            tbSociete.Text = (lstConsultation.SelectedItem as Affectation).Societe;
        }

        //Evenement lors du changement de valeur du champs datepicker
        private void dtp_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            //Appel de la fonction remplirAffectations
            remplirAffectations();
        }

        //Fonction permettant de remplir la liste d'affectation lesAffects selon la date choisie
        public void remplirAffectations()
        {
            //Vide la liste
            lesAffects.DefaultIfEmpty();
            //Remplit à partir de la liste général d'affectation, la 2e liste en appliquant le filtre de date
            foreach (Affectation a in AllAffectations)
            {
                if (a.DateFin >= Convert.ToDateTime(dtp.Value))
                {
                    Affectation affectation = new Affectation
                    {
                        DateDebut = a.DateDebut,
                        DateFin = a.DateFin,
                        Projet = a.Projet,
                        Metier = a.Metier,
                        Societe = a.Societe
                    };
                    lesAffects.Add(affectation);
                }
            }
            //binde la listbox sur la liste lesAffects
            lstConsultation.ItemsSource = lesAffects;
            //focus le 1er item de la listbox
            lstConsultation.SelectedIndex = 0;
        }
      
    }
}