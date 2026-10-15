using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualBasic;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace KsiazkaKsiegowegoWPF
{
    public partial class MainWindow : Window
    {
        [DataContract]
        public class Firma
        {
            [DataMember]
            public string Nazwa { get; set; }

            [DataMember]
            public bool CzyZaznaczone { get; set; }

            [DataMember]
            public DateTime? DataZaznaczenia { get; set; }

            [DataMember]
            public string Notatka { get; set; }

            [DataMember]
            public bool CzyWiadomoscWyslana { get; set; }

            [DataMember]

            public DateTime? DataWiadomosci { get; set; }
        }


        public ObservableCollection<Firma> Firmy { get; set; }

        private string SaveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firmy.json");

        public MainWindow()
        {
            InitializeComponent();
            Firmy = new ObservableCollection<Firma>();
            FirmaListView.ItemsSource = Firmy;
            MiesiacComboBox.SelectionChanged += ComboBox_SelectionChanged;
            RokComboBox.SelectionChanged += ComboBox_SelectionChanged;
            MiesiacComboBox.DropDownOpened += ComboBox_DropDownOpened;
            RokComboBox.DropDownOpened += ComboBox_DropDownOpened;
            LoadPages();
            LoadDataForSelectedMonthAndYear();


        }
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            SaveData();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDataForSelectedMonthAndYear();
        }
        private void DodajFirme_Click(object sender, RoutedEventArgs e)
        {
            var nazwaFirmy = Interaction.InputBox("Podaj nazwę firmy:", "Dodaj firmę", "Nowa Firma");

            if (!string.IsNullOrWhiteSpace(nazwaFirmy))
            {
                Firmy.Add(new Firma { Nazwa = nazwaFirmy });
            }
        }

        private void EdytujNotatke(Firma firma, object sender)
        {
            var notatka = Interaction.InputBox("", "Notatka", firma.Notatka);
            if (notatka != null && notatka != "")
            {
                firma.Notatka = notatka;
            }
            var textBlock = (TextBlock)sender;
            textBlock.Text = firma.Notatka;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            var firma = (Firma)textBlock.DataContext;
            EdytujNotatke(firma, sender);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var firma = (Firma)checkBox.DataContext;
            firma.DataZaznaczenia = DateTime.Now;
            // Zaktualizuj tooltip
            var toolTip = new ToolTip();
            toolTip.Content = $"Data zaznaczenia: {firma.DataZaznaczenia?.ToString("dd/MM/yyyy HH:mm:ss")}";
            checkBox.ToolTip = toolTip;
            checkBox.Content = $"{firma.DataZaznaczenia?.ToString("dd/MM/yyyy")}";
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var firma = (Firma)checkBox.DataContext;
            firma.DataZaznaczenia = null;
            checkBox.ToolTip = null;
            checkBox.Content = null;
        }

        private void CheckBox_Wiadomosc_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var firma = (Firma)checkBox.DataContext;
            firma.DataWiadomosci = DateTime.Now;
            // Zaktualizuj tooltip
            var toolTip = new ToolTip();
            toolTip.Content = $"Data wysłania: {firma.DataWiadomosci?.ToString("dd/MM/yyyy HH:mm:ss")}";
            checkBox.ToolTip = toolTip;
            checkBox.Content = $"{firma.DataWiadomosci?.ToString("dd/MM/yyyy")}";
        }

        private void CheckBox_Wiadomosc_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var firma = (Firma)checkBox.DataContext;
            firma.DataWiadomosci = null;
            checkBox.ToolTip = null;
            checkBox.Content = null;
        }

        private void SaveData()
        {
            try
            {
                var selectedMonth = (this.MiesiacComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                var selectedYear = (this.RokComboBox.SelectedItem as ComboBoxItem).Content.ToString();

                string fileName = $"firmy{selectedMonth}-{selectedYear}.json";

                var jsonString = JsonSerializer.Serialize(Firmy);
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName), jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas zapisywania danych: " + ex.Message);
            }
        }



        private void LoadDataForSelectedMonthAndYear()
        {
            try
            {
                var selectedMonth = (MiesiacComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                var selectedYear = (RokComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (selectedMonth != null && selectedYear != null)
                {
                    string fileName = $"firmy{selectedMonth}-{selectedYear}.json";
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

                    if (File.Exists(filePath))
                    {
                        var jsonString = File.ReadAllText(filePath);
                        Firmy = JsonSerializer.Deserialize<ObservableCollection<Firma>>(jsonString);
                        FirmaListView.ItemsSource = Firmy; // Aktualizacja źródła danych
                    }
                    else
                    {
                        // Jeśli plik nie istnieje, utwórz nową kolekcję firm
                        Firmy = new ObservableCollection<Firma>();
                        FirmaListView.ItemsSource = Firmy; // Aktualizacja źródła danych
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas wczytywania danych: " + ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
        }
    }
}