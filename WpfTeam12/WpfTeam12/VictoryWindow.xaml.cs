using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClassLibTeam12.Data.Framework;
using ClassLibTeam12.Business;
using System.ComponentModel;
using ClassLibTeam12.Business.Validation;

namespace WpfTeam12
{
    public partial class VictoryWindow : Window
    {
        // ObservableCollection voor dynamische binding
        public ObservableCollection<FormData> FormItems { get; set; } = new ObservableCollection<FormData>();

        public VictoryWindow()
        {
            InitializeComponent();
            MyItemsControl.ItemsSource = FormItems; // Koppel ItemsControl aan FormItems
        }

        private void MnuItem_Click(object sender, RoutedEventArgs e)
        {
            FormItems.Clear();
            MenuItem clickedItem = (MenuItem)sender;

            LoadData(clickedItem.Name);

            switch (clickedItem.Name)
            {
                case "MnuNieuweGebruiker":
                    FormItems.Add(new FormData("Voornaam", "", "Achternaam", "", "Email", "", "Wachtwoord", "", "Telefoon", "", "Locatie", "", "", "", "Voeg Gebruiker Toe", new RelayCommand(AddGebruiker))
                    {
                        HasLabel1 = Visibility.Visible,
                        HasLabel2 = Visibility.Visible,
                        HasLabel3 = Visibility.Visible,
                        HasLabel4 = Visibility.Visible,
                        HasLabel5 = Visibility.Visible,
                        HasLabel6 = Visibility.Visible,
                        HasLabel7 = Visibility.Collapsed
                    });
                    break;
                case "MnuNieuwBedrijf":
                    FormItems.Add(new FormData("Naam", "", "Locatie", "", "", "", "", "", "", "", "", "", "", "", "Voeg Bedrijf Toe", new RelayCommand(AddBedrijf))
                    {
                        HasLabel1 = Visibility.Visible,
                        HasLabel2 = Visibility.Visible,
                        HasLabel3 = Visibility.Collapsed,
                        HasLabel4 = Visibility.Collapsed,
                        HasLabel5 = Visibility.Collapsed,
                        HasLabel6 = Visibility.Collapsed,
                        HasLabel7 = Visibility.Collapsed
                    });
                    break;
                case "MnuNieuweInschrijving":
                    FormItems.Add(new FormData("GebruikerID", "", "TrainingID", "", "", "", "", "", "", "", "", "", "", "", "Voeg Inschrijving Toe", new RelayCommand(AddInschrijving))
                    {
                        HasLabel1 = Visibility.Visible,
                        HasLabel2 = Visibility.Visible,
                        HasLabel3 = Visibility.Collapsed,
                        HasLabel4 = Visibility.Collapsed,
                        HasLabel5 = Visibility.Collapsed,
                        HasLabel6 = Visibility.Collapsed,
                        HasLabel7 = Visibility.Collapsed
                    });
                    break;
                case "MnuNieuweJob":
                    FormItems.Add(new FormData("Naam", "", "Beschrijving", "", "Positie", "", "BedrijfID", "", "", "", "", "", "", "", "Voeg Job Toe", new RelayCommand(AddJob))
                    {
                        HasLabel1 = Visibility.Visible,
                        HasLabel2 = Visibility.Visible,
                        HasLabel3 = Visibility.Visible,
                        HasLabel4 = Visibility.Visible,
                        HasLabel5 = Visibility.Collapsed,
                        HasLabel6 = Visibility.Collapsed,
                        HasLabel7 = Visibility.Collapsed
                    });
                    break;
                case "MnuNieuweRol":
                    FormItems.Add(new FormData("Naam", "", "", "", "", "", "", "", "", "", "", "", "", "", "Voeg Rol Toe", new RelayCommand(AddRol))
                    {
                        HasLabel1 = Visibility.Visible,
                        HasLabel2 = Visibility.Collapsed,
                        HasLabel3 = Visibility.Collapsed,
                        HasLabel4 = Visibility.Collapsed,
                        HasLabel5 = Visibility.Collapsed,
                        HasLabel6 = Visibility.Collapsed,
                        HasLabel7 = Visibility.Collapsed
                    });
                    break;
                case "MnuNieuweSollicitatie":
                    FormItems.Add(new FormData("JobID", "", "GebruikerID", "", "", "", "", "", "", "", "", "", "", "", "Voeg Sollicitatie Toe", new RelayCommand(AddSollicitatie))
                    {
                        HasLabel1 = Visibility.Visible,
                        HasLabel2 = Visibility.Visible,
                        HasLabel3 = Visibility.Collapsed,
                        HasLabel4 = Visibility.Collapsed,
                        HasLabel5 = Visibility.Collapsed,
                        HasLabel6 = Visibility.Collapsed,
                        HasLabel7 = Visibility.Collapsed
                    });
                    break;
                case "MnuNieuweTijdslog":
                    FormItems.Add(new FormData("GebruikerID", "", "", "", "", "", "", "", "", "", "", "", "", "", "Voeg Tijdslog Toe", new RelayCommand(AddTijdslog))
                    {
                        HasLabel1 = Visibility.Visible,
                        HasLabel2 = Visibility.Collapsed,
                        HasLabel3 = Visibility.Collapsed,
                        HasLabel4 = Visibility.Collapsed,
                        HasLabel5 = Visibility.Collapsed,
                        HasLabel6 = Visibility.Collapsed,
                        HasLabel7 = Visibility.Collapsed
                    });
                    break;
                case "MnuNieuweTraining":
                    FormItems.Add(new FormData("Status", "", "Naam", "", "LearningMethod", "", "Duration", "", "Beschrijving", "", "Categorie", "", "BedrijfId", "", "Voeg Training Toe", new RelayCommand(AddTraining))
                    {
                        HasLabel1 = Visibility.Visible,
                        HasLabel2 = Visibility.Visible,
                        HasLabel3 = Visibility.Visible,
                        HasLabel4 = Visibility.Visible,
                        HasLabel5 = Visibility.Visible,
                        HasLabel6 = Visibility.Visible,
                        HasLabel7 = Visibility.Visible
                    });
                    break;
            }
        }

        private void AddBedrijf(object parameter)
        {
            if (FormItems.Count > 0)
            {
                string naam = FormItems[0].Text1;
                string locatie = FormItems[0].Text2;

                // Database code
                Bedrijven.Add(naam, locatie);
                LoadData("MnuNieuwBedrijf");
                MessageBox.Show($"Bedrijf {naam} is toegevoegd.");// Add message box
            }
        }

        private void AddGebruiker(object parameter)
        {
            if (FormItems.Count > 0)
            {
                string voornaam = FormItems[0].Text1;
                string achternaam = FormItems[0].Text2;
                string email = FormItems[0].Text3;
                string wachtwoord = FormItems[0].Text4;
                string telefoon = FormItems[0].Text5;
                string locatie = FormItems[0].Text6;

                // Valideer voornaam
                if (!ValidationRules.IsValidName(voornaam))
                {
                    MessageBox.Show("Voornaam is ongeldig. Gebruik alleen letters.");
                    return;
                }

                // Valideer achternaam
                if (!ValidationRules.IsValidName(achternaam))
                {
                    MessageBox.Show("Achternaam is ongeldig. Gebruik alleen letters.");
                    return;
                }

                // Valideer email
                if (!ValidationRules.IsValidEmail(email))
                {
                    MessageBox.Show("Email is ongeldig. Zorg ervoor dat het een @ en een . bevat.");
                    return;
                }

                // Valideer telefoon
                if (!ValidationRules.IsValidPhoneNumber(telefoon))
                {
                    MessageBox.Show("Telefoon is ongeldig. Gebruik alleen cijfers of begin met een +.");
                    return;
                }

                Gebruikers.Add(voornaam, achternaam, email, wachtwoord, telefoon, locatie);
                LoadData("MnuNieuweGebruiker");
                MessageBox.Show($"Gebruiker {voornaam} {achternaam} is toegevoegd.");// Add message box
            }

        }
        private void AddInschrijving(object parameter)
        {
            if (FormItems.Count > 0)
            {
                try
                {
                    // Haal de huidige datum en tijd op
                    DateTime datum = DateTime.Now;

                    // Haal GebruikerID en TrainingID op uit de tekstvakken
                    string gebruikerIdString = FormItems[0].Text1;
                    string trainingIdString = FormItems[0].Text2;

                    // Valideer GebruikerID en TrainingID
                    if (int.TryParse(gebruikerIdString, out int gebruikerId) &&
                        int.TryParse(trainingIdString, out int trainingId))
                    {
                        // Voeg inschrijving toe aan de database
                        Inschrijvingen.Add(datum, gebruikerId, trainingId);
                        LoadData("MnuNieuweInschrijving");
                        MessageBox.Show($"Inschrijving is toegevoegd.");// Add message box
                    }
                    else
                    {
                        MessageBox.Show("Ongeldige invoer. Controleer het formaat van 'GebruikerID' en 'TrainingID'.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Er is een fout opgetreden: {ex.Message}");
                }
            }
        }

        private void AddJob(object parameter)
        {
            if (FormItems.Count > 0)
            {
                try
                {
                    string naam = FormItems[0].Text1;
                    string beschrijving = FormItems[0].Text2;
                    string locatie = FormItems[0].Text3;
                    string positieString = FormItems[0].Text4;
                    string bedrijfIdString = FormItems[0].Text5;

                    if (int.TryParse(positieString, out int positie) && int.TryParse(bedrijfIdString, out int bedrijfId))
                    {
                        // Voeg job toe aan database
                        Jobs.Add(naam, beschrijving, positie, bedrijfId);
                        LoadData("MnuNieuweJob");
                        MessageBox.Show($"Job {naam} is toegevoegd.");// Add message box
                    }
                    else
                    {
                        MessageBox.Show("Ongeldige invoer. Controleer het formaat van 'Positie'.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Er is een fout opgetreden: {ex.Message}");
                }
            }
        }
        private void AddRol(object parameter)
        {
            if (FormItems.Count > 0)
            {
                try
                {
                    string naam = FormItems[0].Text1;

                    if (!string.IsNullOrWhiteSpace(naam))
                    {
                        // Voeg rol toe aan database
                        Rollen.Add(naam);
                        LoadData("MnuNieuweRol");

                        // Optioneel: Vernieuw DataGrid of geef een succesbericht weer
                        MessageBox.Show($"Rol toegevoegd: Naam: {naam}");// Add message box
                    }
                    else
                    {
                        MessageBox.Show("De naam van de rol mag niet leeg zijn.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Er is een fout opgetreden: {ex.Message}");
                }
            }
        }

        private void AddSollicitatie(object parameter)
        {
            if (FormItems.Count > 0)
            {
                try
                {
                    // Huidige datum ophalen
                    DateTime datum = DateTime.Now;

                    // GebruikerID en JobID ophalen uit de tekstvakken
                    string jobIdString = FormItems[0].Text1;
                    string gebruikerIdString = FormItems[0].Text2;

                    if (int.TryParse(gebruikerIdString, out int gebruikerId) && int.TryParse(jobIdString, out int jobId))
                    {
                        // Voeg sollicitatie toe aan database
                        Sollicitaties.Add(datum, gebruikerId, jobId);
                        LoadData("MnuNieuweSollicitatie");

                        // Succesbericht weergeven
                        MessageBox.Show($"Sollicitatie toegevoegd: Datum: {datum.ToString("yyyy-MM-dd")}, GebruikerID: {gebruikerId}, JobID: {jobId}");// Add message box
                    }
                    else
                    {
                        MessageBox.Show("Ongeldige invoer. Controleer het formaat van 'GebruikerID' en 'JobID'.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Er is een fout opgetreden: {ex.Message}");
                }
            }
        }

        private void AddTijdslog(object parameter)
        {
            if (FormItems.Count > 0)
            {
                try
                {
                    // Huidige datum ophalen
                    DateTime datum = DateTime.Now;

                    string gebruikerIdString = FormItems[0].Text1;

                    if (int.TryParse(gebruikerIdString, out int gebruikerId))
                    {
                        // Voeg tijdslog toe aan database
                        TijdslogHelper.Add(datum, gebruikerId);
                        LoadData("MnuNieuweTijdslog");
                        MessageBox.Show($"Tijdslog is toegevoegd.");// Add message box
                    }
                    else
                    {
                        MessageBox.Show("Ongeldige invoer. Controleer het formaat van 'InlogTime'.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Er is een fout opgetreden: {ex.Message}");
                }
            }
        }
        private void AddTraining(object parameter)
        {
            if (FormItems.Count > 0)
            {
                try
                {
                    string status = FormItems[0].Text1;
                    string naam = FormItems[0].Text2;
                    string learningMethod = FormItems[0].Text3;
                    string duration = FormItems[0].Text4;
                    string beschrijving = FormItems[0].Text5;
                    string categorie = FormItems[0].Text6;
                    string bedrijfIdString = FormItems[0].Text7;

                    if (int.TryParse(bedrijfIdString, out int bedrijfId) && bool.TryParse(status, out bool statusBool))
                    {
                        // Voeg training toe aan database
                        Trainingen.Add(statusBool, naam, learningMethod, duration, beschrijving, categorie, bedrijfId);
                        LoadData("MnuNieuweTraining");

                        // Optioneel: Vernieuw DataGrid of geef een succesbericht weer
                        MessageBox.Show($"Training toegevoegd: Naam: {naam}, Beschrijving: {beschrijving}, Categorie: {categorie}, BedrijfID: {bedrijfId}");// Add message box
                    }
                    else
                    {
                        MessageBox.Show("Ongeldige invoer. Controleer het formaat van 'BedrijfID'.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Er is een fout opgetreden: {ex.Message}");
                }
            }
        }

        private void MnuClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void LoadData(string menuItemKeuze)
        {
            SelectResult result;
            switch (menuItemKeuze)
            {
                case "MnuNieuweGebruiker":
                    result = Gebruikers.GetGebruikers();
                    if (result.Succeeded)
                    {
                        DgdSql.ItemsSource = result.DataTable.DefaultView;
                    }
                    break;
                case "MnuNieuweInschrijving":
                    result = Inschrijvingen.GetInschrijvingen();
                    if (result.Succeeded)
                    {
                        DgdSql.ItemsSource = result.DataTable.DefaultView;
                    }
                    break;
                case "MnuNieuweJob":
                    result = Jobs.GetJobs();
                    if (result.Succeeded)
                    {
                        DgdSql.ItemsSource = result.DataTable.DefaultView;
                    }
                    break;
                case "MnuNieuweRol":
                    result = Rollen.GetRollen();
                    if (result.Succeeded)
                    {
                        DgdSql.ItemsSource = result.DataTable.DefaultView;
                    }
                    break;
                case "MnuNieuweSollicitatie":
                    result = Sollicitaties.GetSollicitaties();
                    if (result.Succeeded)
                    {
                        DgdSql.ItemsSource = result.DataTable.DefaultView;
                    }
                    break;
                case "MnuNieuweTijdslog":
                    result = TijdslogHelper.GetTijdslogs();
                    if (result.Succeeded)
                    {
                        DgdSql.ItemsSource = result.DataTable.DefaultView;
                    }
                    break;
                case "MnuNieuweTraining":
                    result = Trainingen.GetTrainingen();
                    if (result.Succeeded)
                    {
                        DgdSql.ItemsSource = result.DataTable.DefaultView;
                    }
                    break;
                case "MnuNieuwBedrijf":
                    result = Bedrijven.GetBedrijven();
                    if (result.Succeeded)
                    {
                        DgdSql.ItemsSource = result.DataTable.DefaultView;
                    }
                    break;
                default:
                    break;
            
            }
        }
    }
}


public class FormData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Visibility _hasLabel1 = Visibility.Collapsed;
        private Visibility _hasLabel2 = Visibility.Collapsed;
        private Visibility _hasLabel3 = Visibility.Collapsed;
        private Visibility _hasLabel4 = Visibility.Collapsed;
        private Visibility _hasLabel5 = Visibility.Collapsed;
        private Visibility _hasLabel6 = Visibility.Collapsed;
        private Visibility _hasLabel7 = Visibility.Collapsed;

        public string Label1 { get; set; }
        public string Text1 { get; set; }
        public string Label2 { get; set; }
        public string Text2 { get; set; }
        public string Label3 { get; set; }
        public string Text3 { get; set; }
        public string Label4 { get; set; }
        public string Text4 { get; set; }
        public string Label5 { get; set; }
        public string Text5 { get; set; }
        public string Label6 { get; set; }
        public string Text6 { get; set; }
        public string Label7 { get; set; }
        public string Text7 { get; set; }
    public string ButtonText { get; set; }
        public ICommand ButtonClick { get; set; }

        public FormData(string label1, string text1, string label2, string text2, string label3, string text3, string label4, string text4, string label5, string text5, string label6, string text6, string label7, string text7, string buttonText, ICommand buttonClick)
        {
            Label1 = label1;
            Text1 = text1;
            Label2 = label2;
            Text2 = text2;
            Label3 = label3;
            Text3 = text3;
            Label4 = label4;
            Text4 = text4;
            Label5 = label5;
            Text5 = text5;
            Label6 = label6;
            Text6 = text6;
            Label7 = label7;
            Text7 = text7;


        ButtonText = buttonText;
            ButtonClick = buttonClick;
        }
        public Visibility HasLabel1
        {
            get { return _hasLabel1; }
            set
            {
                if (_hasLabel1 != value)
                {
                    _hasLabel1 = value;
                    OnPropertyChanged("HasLabel1");
                }
            }
        }

        public Visibility HasLabel2
        {
            get { return _hasLabel2; }
            set
            {
                if (_hasLabel2 != value)
                {
                    _hasLabel2 = value;
                    OnPropertyChanged("HasLabel2");
                }
            }
        }
        public Visibility HasLabel3
        {
            get { return _hasLabel3; }
            set
            {
                if (_hasLabel3 != value)
                {
                    _hasLabel3 = value;
                    OnPropertyChanged("HasLabel3");
                }
            }
        }
        public Visibility HasLabel4
        {
            get { return _hasLabel4; }
            set
            {
                if (_hasLabel4 != value)
                {
                    _hasLabel4 = value;
                    OnPropertyChanged("HasLabel4");
                }
            }
        }
        public Visibility HasLabel5
        {
            get { return _hasLabel5; }
            set
            {
                if (_hasLabel5 != value)
                {
                    _hasLabel5 = value;
                    OnPropertyChanged("HasLabel5");
                }
            }
        }

        public Visibility HasLabel6
        {
            get { return _hasLabel6; }
            set
            {
                if (_hasLabel6 != value)
                {
                    _hasLabel6 = value;
                    OnPropertyChanged("HasLabel6");
                }
            }
        }

        public Visibility HasLabel7
        {
            get { return _hasLabel7; }
            set
            {
                if (_hasLabel7 != value)
                {
                    _hasLabel7 = value;
                    OnPropertyChanged("HasLabel7");
                }
            }
        }

    protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
