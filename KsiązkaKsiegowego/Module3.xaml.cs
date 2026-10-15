using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KsiazkaKsiegowegoWPF
{
    public partial class MainWindow : Window
    {
        private int currentPage = 0;
        private List<string> pages;
        private string pagesFilePath = "porady.json";

        private void LoadPages()
        {
            if (File.Exists(pagesFilePath))
            {
                string json = File.ReadAllText(pagesFilePath);
                pages = JsonConvert.DeserializeObject<List<string>>(json);
            }
            else
            {
                pages = new List<string> { "Strona 1" };
            }

            UpdatePageText();
            StrzalkaPrawo.Visibility = currentPage < pages.Count - 1 ? Visibility.Visible : Visibility.Hidden;
        }

        private void SavePages()
        {
            string json = JsonConvert.SerializeObject(pages);
            File.WriteAllText(pagesFilePath, json);
        }

        private void UpdatePageText()
        {
            if (pages.Count > 0)
            {
                TextBlock.Text = pages[currentPage];
            }
        }

        private void StrzalkaLewo_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                UpdatePageText();
                SavePages();
            }

            StrzalkaPrawo.Visibility = Visibility.Visible;
            StrzalkaLewo.Visibility = currentPage > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void StrzalkaPrawo_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < pages.Count - 1)
            {
                currentPage++;
                UpdatePageText();
                SavePages();
            }

            StrzalkaLewo.Visibility = Visibility.Visible;
            StrzalkaPrawo.Visibility = currentPage < pages.Count - 1 ? Visibility.Visible : Visibility.Hidden;
        }


        private void DodajStroneButton_Click(object sender, RoutedEventArgs e)
        {
            var nowaStronaTextBox = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                FontSize = 16,
                Width = 600,
                Height = 120, // Zwiększenie wysokości TextBox, aby umożliwić zawijanie tekstu
                TextWrapping = TextWrapping.Wrap, // Ustawienie zawijania tekstu
            };
            Grid.SetColumnSpan(nowaStronaTextBox, 2); // Rozszerzenie TextBox na dwa kolumny
            ((Grid)Modul1Tab.Content).Children.Add(nowaStronaTextBox); // Dodanie TextBox do Grid

            var zapiszStroneButton = new Button
            {
                Content = "Zapisz stronę",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 16
            };
            zapiszStroneButton.Click += (s, ev) =>
            {
                string nowaStrona = nowaStronaTextBox.Text;
                if (!string.IsNullOrEmpty(nowaStrona))
                {
                    // Zapisz nową stronę w liście
                    pages.Add(nowaStrona);
                    SavePages();

                    // Uaktualnij stronę
                    UpdatePageText();

                    // Usuń TextBox i przycisk "Zapisz stronę"
                    ((Grid)Modul1Tab.Content).Children.Remove(nowaStronaTextBox);
                    ((Grid)Modul1Tab.Content).Children.Remove(zapiszStroneButton);
                }
            };
            ((Grid)Modul1Tab.Content).Children.Add(zapiszStroneButton);

            StrzalkaPrawo.Visibility = Visibility.Visible;

        }



        private void UsunStroneButton_Click(object sender, RoutedEventArgs e)
        {
            if (pages.Count > 1)
            {
                pages.RemoveAt(currentPage);
                if (currentPage >= pages.Count)
                {
                    currentPage = pages.Count - 1;
                }
                SavePages();
                UpdatePageText();
            }

            StrzalkaPrawo.Visibility = currentPage < pages.Count - 1 ? Visibility.Visible : Visibility.Hidden;
            StrzalkaLewo.Visibility = currentPage > 0 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}

