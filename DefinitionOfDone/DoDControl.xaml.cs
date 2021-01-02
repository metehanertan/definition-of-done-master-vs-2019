namespace DefinitionOfDone
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.VisualStudio.PlatformUI;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// Interaction logic for DoDControl.
    /// </summary>
    public partial class DoDControl : UserControl
    {

        public static readonly string rootDirectory = $"{System.Environment.CurrentDirectory}";
        /// <summary>
        /// Initializes a new instance of the <see cref="DoDControl"/> class.
        /// </summary>
        public DoDControl()
        {
            try{
                this.InitializeComponent();
                Done.Visibility = Visibility.Hidden;
                fillCombo(SelectMenu);
            }
            catch( Exception ex)
            {
                TreeList.Items.Clear();
                TreeList.Items.Add(ex.ToString());
            }
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]

        //Reads data from txt file
        private void fillCombo(ComboBox SelectMenu)
        {
            string txtPath = rootDirectory + "\\Definitions.txt";
            string[] definitions = System.IO.File.ReadAllLines(txtPath);
            foreach (string def in definitions)
            {
                string name = def.Split(';')[0];
                ListBoxItem box = new ListBoxItem();
                box.Content = name;
                SelectMenu.Items.Add(box);
            }
        }

        //Fills TreeList when a header is selected
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Done.Visibility = Visibility.Hidden;
                TreeList.Items.Clear();
                fillTree(SelectMenu.SelectedIndex);
            }
            catch (Exception ex)
            {
                TreeList.Items.Clear();
                TreeList.Items.Add(ex.ToString());
            }
        }

        //Fills with the data from txt
        private void fillTree(int selected)
        {
            //Read data
            string txtPath = rootDirectory + "\\Definitions.txt";
            string[] definitions = System.IO.File.ReadAllLines(txtPath);
            string[] rules = definitions[selected].Split(';');
            for (int i = 1; i < rules.Length; i++)
            {
                //If there is a sub checkbox
                if (rules[i].Contains("+"))
                {
                    //Main panel for main checkbox
                    StackPanel mainStack = new StackPanel();

                    //Main checkbox
                    CheckBox mainBox = new CheckBox();
                    string[] splitted = rules[i].Split('+');
                    mainBox.Content = splitted[0];
                    mainBox.Margin = new Thickness(1);
                    mainBox.Click += new RoutedEventHandler(this.CheckDone);
                    mainStack.Children.Add(mainBox);

                    //Sub panel for sub checkboxes
                    StackPanel subStack = new StackPanel();
                    subStack.Margin = new Thickness(10, 1, 1, 1);
                    mainStack.Children.Add(subStack);

                    //Adding sub checkboxes to sub panel
                    for (int x = 1; x < splitted.Length; x++)
                    {
                        CheckBox subBox = new CheckBox();
                        subBox.Content = splitted[x];
                        subBox.Click += new RoutedEventHandler(this.CheckDone);
                        subStack.Children.Add(subBox);
                    }
                    TreeList.Items.Add(mainStack);
                    continue;
                }

                //If the requirement is too long, splits it in half. this is done after subbed req.
                //because it sees all subrequirements and main requirement as one.
                if (rules[i].Length > 100)
                {
                    string[] sentence = rules[i].Split(' ');
                    rules[i] = "";
                    for (int x = 0; x < sentence.Length; x++)
                    {
                        if (x == sentence.Length / 2)
                        {
                            rules[i] += "\n";
                        }
                        rules[i] += sentence[x] + " ";
                    }
                    rules[i] += ".";
                }

                //If there is a note
                if (rules[i].Contains("Note"))
                {
                    TextBlock note = new TextBlock();
                    note.Text = rules[i];
                    TreeList.Items.Add(note);
                    continue;
                }

                //Normal requirement
                CheckBox check = new CheckBox();
                check.Content = rules[i];
                check.Margin = new Thickness(1);
                check.Click += new RoutedEventHandler(this.CheckDone);
                TreeList.Items.Add(check);
            }
        }

        //Checing if all requirements are done
        private void CheckDone(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < TreeList.Items.Count; i++)
            {
                //Single requirement
                if (TreeList.Items.GetItemAt(i).GetType().FullName.Contains("CheckBox"))
                {
                    if (!(bool)((CheckBox)TreeList.Items.GetItemAt(i)).IsChecked)
                    {
                        Done.Visibility = Visibility.Hidden;
                        return;
                    }
                }
                //Subbed requirement
                else if (TreeList.Items.GetItemAt(i).GetType().FullName.Contains("StackPanel"))
                {
                    if (!(bool)((CheckBox)((StackPanel)TreeList.Items.GetItemAt(i)).Children[0]).IsChecked)
                    {
                        Done.Visibility = Visibility.Hidden;
                        return;
                    }
                }
            }
            //if all checked
            Done.Visibility = Visibility.Visible;
        }
    }
}