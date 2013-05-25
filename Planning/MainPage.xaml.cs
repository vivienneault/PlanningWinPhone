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
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using Planning.WebServicePPE4;
using Microsoft.Phone.Tasks;
using System.Net.NetworkInformation;
namespace Planning
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
        }
        //Déclaration d'un SoapCLient basé sur les web services mis en référence
        WebServicePPE4.WsGestionAffectationsSoapClient proxy = new WebServicePPE4.WsGestionAffectationsSoapClient();
       
        //Evenement du chargement de page 
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {   
            //Requête asynchrone vers la web method GetCollaborateursAffectesProjet, lève un évenement après éxecution de la requête
            proxy.GetCollaborateursAffectesProjetCompleted += new EventHandler<GetCollaborateursAffectesProjetCompletedEventArgs>(proxy_GetCollaborateursAffectesProjetCompleted);
            proxy.GetCollaborateursAffectesProjetAsync();
        }

        //Evenement levé après execution de la requete
        void proxy_GetCollaborateursAffectesProjetCompleted(object sender, GetCollaborateursAffectesProjetCompletedEventArgs e)
        {
            //Listbox bindé sur le flux de résulat
            lstCollab.ItemsSource = e.Result;
        }

        private void cmdMdp_Click(object sender, RoutedEventArgs e)
        {
            if (lstCollab.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez sélectionner un collaborateur", "Erreur", MessageBoxButton.OK);
            }
            else
            {
                MessageBoxResult m = MessageBox.Show("Vous allez recevoir un mail contenant vos informations de connexion", "Assistance", MessageBoxButton.OKCancel);
                //if (m == MessageBoxResult.OK)
                //{
                //    string prenom = ((lstCollab.SelectedItem) as WebServicePPE4.Collaborateur).Nom;
                //    string nom = ((lstCollab.SelectedItem) as WebServicePPE4.Collaborateur).Prenom;
                //    WebClient wc = new WebClient();
                //    wc.DownloadStringAsync(new Uri("http://192.168.241.129/mail.php?" + nom + "_" + prenom));
                //    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                //}
            }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            MessageBox.Show(e.Result, "Assistance", MessageBoxButton.OK);
            //string res = e.Result;
            //string[] tab = res.Split('-');
            //string mail = tab[0];
            //string mdp = tab[1];
            //EmailComposeTask Myemail_Composetask = new EmailComposeTask();
            //Myemail_Composetask.To = mail;
            //Myemail_Composetask.Cc = mail;
            //Myemail_Composetask.Subject = "SSII21 - Mot de passe oublié";
            //Myemail_Composetask.Body = "Voici votre mot de passe : " + mdp;
            //Myemail_Composetask.Show();
        }

        //Evenement lors du clique sur le bouton connexion
        private void cmdConnexion_Click(object sender, RoutedEventArgs e)
        {
            //Si l'utilisateur n'a pas choisi de collaborateur -> Message d'erreur
            if (lstCollab.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez sélectionner un collaborateur", "Erreur", MessageBoxButton.OK);
            }
            //Si l'utilisateur n'a pas indiqué de mot de passe -> Message d'erreur
            else if (pwb.Password == "")
            {
                MessageBox.Show("Veuillez indiquer un password", "Erreur", MessageBoxButton.OK);
            }            
            else
            {
                //Récupère l'id par rapport à l'objet collborateur choisit dans la listbox
                int id = ((lstCollab.SelectedItem) as WebServicePPE4.Collaborateur).Matricule;
                //Récupère le mot de passe du champs password
                string password = pwb.Password;

                //Déclaration d'un webClient
                WebClient wc = new WebClient();
                //Requete asynchrone vers le script php placé sur le serveur débian, èvenement levé après execution de la requête
                wc.DownloadStringAsync(new Uri("http://192.168.241.129/connect.php?" + id + "_" + password));
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted2);
                //On vide le champs password
                pwb.Password = "";
            }
        }

        //Evenement levé après execution de la requete
        void wc_DownloadStringCompleted2(object sender, DownloadStringCompletedEventArgs e)
        {
            //Si le flux renvoi "oui"
            if (e.Result == "oui")
            {
                //Redirection vers la page date.xaml en passant le parametre id correspondant au collaborateur sélectionné dans la listbox
                NavigationService.Navigate(new Uri("/date.xaml?id=" + (lstCollab.SelectedItem as WebServicePPE4.Collaborateur).Matricule, UriKind.Relative));
            }
            else
            {
                //Message d'erreur
                MessageBox.Show("Mauvais mot de passe", "Erreur", MessageBoxButton.OK);
            }
        }

    }
}