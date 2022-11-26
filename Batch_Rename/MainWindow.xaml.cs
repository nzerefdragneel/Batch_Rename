using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fluent;
using Fluent.Localization.Languages;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using File = System.IO.File;
using MessageBox = System.Windows.MessageBox;
using Contract;
using System.Data;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Security.Policy;
using System.Windows.Markup;
using Microsoft.VisualBasic.Devices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Net;
using Path = System.IO.Path;

namespace Batch_Rename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
       
        public MainWindow()
        {
            InitializeComponent();  
        }
        static Dictionary<string, IRule> _prototypes = new Dictionary<string, IRule>();

        ObservableCollection<FileIOS> _fileList=new ObservableCollection<FileIOS>();

        ObservableCollection<FolderIOS> _folderList=new ObservableCollection<FolderIOS>();
        
        ObservableCollection<FolderIOS> _resetfolderList=new ObservableCollection<FolderIOS>();

        string newPath;

        ObservableCollection<FileIOS> _resetfileList = new ObservableCollection<FileIOS>();

        bool stoprename = false;

        ObservableCollection<FrameworkElement> _allListRuleElement;
        List<string> _activeRule=new List<string>();
        int[] _active;
        List<string> _ruleData;
        List<IRule?> rules = new List<IRule?>();

        BrushConverter bc = new BrushConverter();
        Brush? brushColorSelect;
        Brush? brushColorDefault;
        //danh sách tất cả các rule
        ObservableCollection<string> _ruleList;

        ObservableCollection<string> _nameRule = new ObservableCollection<String>();
        private void LoadRuleFromFile()
        {
           Debug.WriteLine("rule+++++++++++++++++++++++++++++++++++++");
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            var folderInfo = new DirectoryInfo(exeFolder);
            var dllFiles = folderInfo.GetFiles("*.dll");
            foreach (var file in dllFiles)
            {
                var assembly = Assembly.LoadFrom(file.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                   
                    if (type.IsClass && typeof(IRule).IsAssignableFrom(type))
                    {
                        IRule rule = (IRule)Activator.CreateInstance(type)!;
                        RuleFactory.Register(rule);
                        _prototypes.Add(rule.Name, rule);
                    }
                }
            }
         
           
        }
        private void CollapsedRule()
        {
            _allListRuleElement = new ObservableCollection<FrameworkElement>()
            {
               ChangeExtension,AddCase,AddCounter,AddPrefix,AddSuffix,Trim
            };
            _active = new int[10] ;
            int i=0 ;
            foreach (FrameworkElement ele in _allListRuleElement)
            {
                ele.Visibility= Visibility.Collapsed;
                _active[i++] = 0;
            }         
        }
        private void deleteDefault()
        {
            _allListRuleElement = new ObservableCollection<FrameworkElement>()
            {
               ChangeExtension,AddCase,AddCounter,AddPrefix,AddSuffix,Trim
            };
            int i = 0;
            _active = new int[10];
            foreach (FrameworkElement ele in _allListRuleElement)
            {
                ListRuleGroupBox.Children.Remove(ele);
                _active[i++] = 0;
            }


        }
        private void Window_loaded(object sender, RoutedEventArgs e)
        {
            /*
            ListRuleGroupBox.Children.Remove(changeExtensionRule);
            ListRuleGroupBox.Children.Remove(addCaseRule);
            ListRuleGroupBox.Children.Remove(addCounter);*/

            
            FilesListView.ItemsSource = _fileList;
            FoldersListView.ItemsSource = _folderList;
            brushColorSelect = (Brush)bc.ConvertFrom("#8DC4F8");
            brushColorDefault = (Brush)bc.ConvertFrom("#235295");
            _activeRule = new List<string>();
            _ruleList = new ObservableCollection<string>()
            {
                "Change Extension","Add Case","Add Counter","Add Prefix","Add Suffix","Trim"
            };

            deleteDefault();
            LoadRuleFromFile();
            cmbChooseRule.ItemsSource= _ruleList;
         
        }



        private void btn_click(object sender, RoutedEventArgs e)
        {
            var screen = new FolderBrowserDialog();
            if (screen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("show");
            }
        }

        private void btn_clic1k(object sender, RoutedEventArgs e)
        {
            var screen1 = new OpenFileDialog();


            if (screen1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("show");
            }

        }

        private void btnAddFile(object sender, RoutedEventArgs e)

        {
            string type=cmbAddfile.Text;
            if (type == "File")
            {
                var screen1 = new OpenFileDialog();


                if (screen1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                   
                        string filenamepath = screen1.FileName.ToString();
                    if (_fileList.Where(X => X.getType() == "File" && ((FileIOS)X)!.Pathname == filenamepath).FirstOrDefault() == null)
                    {
                        int index = filenamepath.LastIndexOf("\\");

                        if (index > -1)
                        {
                            var filename = filenamepath.Substring(index + 1);
                            var path = filenamepath.Substring(0, index);
                            int n = _fileList.Count; 
                            var file = new FileIOS()
                            {
                                Stt =n,
                                Filename = filename,
                                NewFilename = filename,
                                Pathname = path,
                                Type = "File"
                            };
                            foreach (var rule in rules)
                            {
                                file.NewFilename = rule?.Rename(file.NewFilename, "File")!;
                            }
                            _fileList.Add(file);
                        }
                    }else
                    {
                        MessageBox.Show("This file arealy exists");
                    }
                }
            }
            else
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                var result = dialog.ShowDialog();

                int counter = 0;
                if (System.Windows.Forms.DialogResult.OK == result)
                {
                    // ItemListView.ItemsSource = _folders;

                    string path = dialog.SelectedPath + "\\";
                    int count = _fileList.Count;
                     ReadAllFileInFolder(path);
                    count = _fileList.Count - count+1;
                    MessageBox.Show(count.ToString());
                  
                }

            }
        }
        private void ReadAllFileInFolder(string path)
        {
           
            foreach (string d in Directory.GetFiles(path))
            { 
                if (_fileList.Where(X => X.getType() == "File" && ((FileIOS)X)!.Pathname == d).FirstOrDefault() == null)
                {
                   
                    int index =d.LastIndexOf("\\");

                    if (index > -1)
                    {
                        var filename = d.Substring(index + 1);
                        var pathfile = d.Substring(0, index);
                        int n = _fileList.Count;
                        var file = new FileIOS
                        {
                            Stt = n,
                            Filename = filename,
                            NewFilename = filename,
                            Pathname = pathfile,
                            Type = "File",
                            Result = "",
                            Status = 0
                        };

                        foreach (var rule in rules)
                        {
                            file.NewFilename = rule?.Rename(file.NewFilename, "File")!;
                        }

                        _fileList.Add(file);
                    }
                    
                }

            }
            foreach (string d in Directory.GetDirectories(path))
            {
                 ReadAllFileInFolder(d);
            }

           
        }
        private void ReadAllFolderInFolder(string folder,string type)
        {
            
            foreach (string d in Directory.GetDirectories(folder))
                {
                String[] filenameSplit = d.Split('\\');
                String filename = filenameSplit[filenameSplit.Length - 1];
                if ( _folderList.Where(X => X.getType() == "Folder" && ((FolderIOS)X)!.Pathname == d).FirstOrDefault() == null)
                {  
                    int n = _folderList.Count;
                    var file = new FolderIOS
                    {   Stt = n,
                        Filename = filename,
                        NewFilename = filename,
                        Pathname =d,
                        Type = "Folder",
                        Result = "",
                        Status = 0
                    };

                    foreach (var rule in rules)
                    {
                        file.NewFilename = rule?.Rename(file.NewFilename, "Folder")!;
                    }

                    _folderList.Add(file);
                   ReadAllFolderInFolder(d, type);
                }
               
                }
           
          
        }
        
        private void btnAddFolder(object sender, RoutedEventArgs e)
        {
            string type = cmbAddFolder.Text;
            if (type == "Folder")
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                var result = dialog.ShowDialog();
                int counter = 0;
                if (System.Windows.Forms.DialogResult.OK == result)
                {
                    // ItemListView.ItemsSource = _folders;
                    string filenamepath = dialog.SelectedPath;
                    int index = filenamepath.LastIndexOf("\\");
                    if (index > -1)
                    {
                        var filename = filenamepath.Substring(index + 1);
                        var path = filenamepath.Substring(0, index);
                        int n = _folderList.Count;
                        var file = new FolderIOS()
                        {
                            Stt = n,
                            Filename = filename,
                            NewFilename = filename,
                            Pathname = path,
                            Type = "Folder"
                        };
                        foreach (var rule in rules)
                        {
                            file.NewFilename = rule?.Rename(file.NewFilename, "Folder")!;
                        }
                        _folderList.Add(file);
                    }
                }
            }
            else
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                var result = dialog.ShowDialog();

                int counter =_folderList.Count;
                if (System.Windows.Forms.DialogResult.OK == result)
                {
                    string path = dialog.SelectedPath + "\\";
                    ReadAllFolderInFolder(path, "Folder");
                    counter = _folderList.Count - counter;
                    MessageBox.Show(counter.ToString());
                }

            }

        }

        private void editRule(object sender, RoutedEventArgs e)
        {
           
        }

        private void deleteRule(object sender, RoutedEventArgs e)
        {
            FrameworkElement parent = (FrameworkElement)((System.Windows.Controls.Button)sender).Parent;
            parent = (StackPanel)parent.Parent;
            int index = 0;
            for (index=0;index<_allListRuleElement.Count;index++)
            {
                if (parent.Name == _allListRuleElement[index].Name)
                {
                    _active[index] = 0;
                }
            }
            
            foreach (var rule in rules)
            {
                if (rule.Name == parent.Name)
                {

                    rules.Remove(rule);
                    break;
                }
            }
            foreach ( var file in _fileList)
            {
                file.NewFilename = file.Filename;
                foreach(var rule in rules)
                {
                    file.NewFilename = rule?.Rename(file.NewFilename, "File")!;
                }
            }
            foreach (var file in _folderList)
            {
                file.NewFilename = file.Filename;
                foreach (var rule in rules)
                {
                    file.NewFilename = rule?.Rename(file.NewFilename, "Folder")!;
                }
            }
            Debug.WriteLine($"{parent.Name}");
            Debug.WriteLine("delete___________");
           ((RibbonGroupBoxWrapPanel)parent.Parent).Children.Remove(parent);
            
        }
    

        private void ChangeExtension_mouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _textboxReplace_TextChanged(object sender, TextChangedEventArgs e)
        {
            var parent = ((System.Windows.Controls.TextBox)sender);
          
            string data=parent.Text.ToString();
            string nameRuleEdit;
            switch (parent.Name.ToString())
            {
                case "tb_extension":
                    nameRuleEdit = "ChangeExtension";
                    break;
                case "textboxAddprefix":
                    nameRuleEdit = "AddPrefix";
                    break;
                default:
                    nameRuleEdit = "AddSuffix";
                    break;
            }
            if (!string.IsNullOrEmpty(data))
            {
                foreach (var rule in rules)
                {
                    if (rule.Name == nameRuleEdit) 
                        rule?.EditRule(data);

                }
                foreach (var file in _fileList)
                {
                    file.NewFilename= file.Filename;
                    foreach (var rule in rules)
                    {
                        file.NewFilename = rule?.Rename(file.NewFilename, "File")!;
                    }
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        
        private void EditChangeExtension(object sender, RoutedEventArgs e)
        {
            if (editChangeExtension.Visibility.ToString()== "Collapsed")
            {
                editChangeExtension.Visibility = Visibility.Visible;

            }
            else
            {
                editChangeExtension.Visibility = Visibility.Collapsed;
            }
        }
       
        private void EditaddCase(object sender, RoutedEventArgs e)
        {
            if (editAddCase.Visibility.ToString() == "Collapsed")
            {
                editAddCase.Visibility = Visibility.Visible;

            }
            else
            {
                editAddCase.Visibility = Visibility.Collapsed;
            }

        }
        bool isaddCounter = false;
        private void EditaddCounter(object sender, RoutedEventArgs e)
        {
            isaddCounter= !isaddCounter;
            if (isaddCounter == true)
            {
                this.AddCounter.Children.Add(editaddCounter);
               
                this.AddCounter.Background = brushColorSelect;
            }
            else
            {
                this.AddCounter.Background = brushColorDefault;
                this.AddCounter.Children.Remove(editaddCounter);
            }
        }
        private void previewFileName()
        {
            string presetPath = "HrRules.txt";
            var rulesData = File.ReadAllLines(presetPath);
            var rules = new List<IRule?>();

            foreach (var line in rulesData)
            {
                var rule = RuleFactory.Instance().Parse(line);

                if (rule != null)
                {
                    rules.Add(rule);
                }
            }

            for (int i = 0; i < _fileList.Count; i++)
            {
                string newName = _fileList[i].Filename;

                foreach (var rule in rules)
                {
                    newName = rule?.Rename(newName, "File")!;
                }
                Debug.WriteLine(newName);
                _fileList[i].NewFilename = newName;
            }
        }
        private void previewAfterAddRule(string line)
        {
            var rule = RuleFactory.Instance().Parse(line);
            Debug.WriteLine(line);
            if (rule != null)
            {
                rules.Add(rule);
                Debug.WriteLine("____________________________");
                Debug.WriteLine(rule.Name);
                for (int i = 0; i < _fileList.Count; i++)
                {
                    string newName = _fileList[i].NewFilename;
                    newName = rule?.Rename(newName,"File")!;
                    _fileList[i].NewFilename = newName;
                }
                for (int i = 0; i < _folderList.Count; i++)
                {
                    string newName = _folderList[i].NewFilename;
                    newName = rule?.Rename(newName, "Folder")!;
                    _folderList[i].NewFilename = newName;
                }
            }
            
            
        }
        private string toPascalCase(string line)
        {
            Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
            Regex whiteSpace = new Regex(@"(?<=\s)");
            Regex startsWithLowerCaseChar = new Regex("^[a-z]");
            Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
            Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
            Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");


            var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(line, "_"), string.Empty)
                .Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
                .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
                .Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
                .Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));
            string result= string.Concat(pascalCase);
            return result;
        }
        private void btnChooseRule(object sender, RoutedEventArgs e)

        {
            var isChoose = cmbChooseRule.SelectedIndex;
            if (isChoose != -1)
            {
                if (_active[isChoose] == 0)
                {
                    this.ListRuleGroupBox.Children.Add(_allListRuleElement[isChoose]);
                    _active[isChoose] = 1;
                    _activeRule.Add(_allListRuleElement[isChoose].Name.ToString());
                    string rulename = _allListRuleElement[isChoose].Name.ToString();
                    string line = toPascalCase(rulename) + " 0 0";
                    
                    previewAfterAddRule(line);
                }
                else
                {
                    MessageBox.Show("This rule already exists");

                }
            }
            else
            {
                MessageBox.Show("Please choose rule");
            }
            


        }

        private void ChooseRuleIsDown(object sender, RoutedEventArgs e)
        {
            int currentSelectedIndex = ListRuleGroupBox.Children.IndexOf(AddCase);
            int downIndex = currentSelectedIndex + 1;
            int childCount = ListRuleGroupBox.Children.Count;
           
            if (downIndex < childCount)
            {
                ListRuleGroupBox.Children.RemoveAt(currentSelectedIndex);
                ListRuleGroupBox.Children.Insert(downIndex,AddCase);
            }
            else if (downIndex == childCount)
            {
                ListRuleGroupBox.Children.RemoveAt(currentSelectedIndex);
                ListRuleGroupBox.Children.Insert(currentSelectedIndex, AddCase);
            }
        }
        public T XamlClone<T>(T source)
        {
            string savedObject = System.Windows.Markup.XamlWriter.Save(source);

            // Load the XamlObject
            StringReader stringReader = new StringReader(savedObject);
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(stringReader);
            T target = (T)System.Windows.Markup.XamlReader.Load(xmlReader);

            return target;
        }
        private FrameworkElement createNewRule(string name)
        {
            FrameworkElement parent = XamlClone<FrameworkElement>(ChangeExtension);
            Debug.WriteLine(parent);
            Debug.WriteLine(name);
            Debug.WriteLine(parent.Name);
            parent.Name ="newRule1";
            Debug.WriteLine(parent.Name);
            var child=((StackPanel)parent).Children;
            Debug.WriteLine(parent.DataContext);
            parent.DataContext=name;
            
            Debug.WriteLine(parent.DataContext);
            return parent;

        }


        private void StartRename(object sender, RoutedEventArgs e)
        {
            StartRenameGrid.Visibility = Visibility.Collapsed;
            ControlRename.Visibility = Visibility.Visible;
            foreach (var file in _fileList)
            {
                if (stoprename == false)
                {
                    if (newPath == "") { 
                        File.Move(file.Pathname + "\\" + file.Filename, file.Pathname + "\\" + file.NewFilename);
                    var filereset = new FileIOS()
                    {
                        Filename = file.NewFilename.ToString(),
                        NewFilename = file.Filename.ToString(),
                        Pathname = file.Pathname
                    };
                    _resetfileList.Add(filereset);
                    file.Filename = file.NewFilename;
                     }
                else
                {
                    File.Copy(file.Pathname + "\\" + file.Filename, newPath + "\\" + file.NewFilename);

                }
                file.Status = 1; 
                } 
                    
                   
                   
                
                else
                {
                    break;
                }

            }

            foreach (var file in _folderList)
            {
                if (stoprename == false)
                {
                  
                        Directory.Move(file.Pathname + "\\" + file.Filename, file.Pathname + "\\" + file.NewFilename);
                        var filereset = new FolderIOS()
                        {
                            Filename = file.NewFilename.ToString(),
                            NewFilename = file.Filename.ToString(),
                            Pathname = file.Pathname
                        };
                        _resetfolderList.Add(filereset);
                        file.Filename = file.NewFilename;
                    
             
                    file.Status = 1;
                }

                else
                {
                    break;
                }

            }

            MessageBox.Show("All files have been renamed!");
            ControlRename.Visibility=Visibility.Collapsed;
            StartRenameGrid.Visibility = Visibility.Visible;
        }

        private void StopRename(object sender, RoutedEventArgs e)
        {
            stoprename = true;
        }

        private void creditsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void deleteAllFile(object sender, RoutedEventArgs e)
        {
            _resetfileList = _fileList;
            _fileList.Clear();
        }
        
        //chua xong
        private void resetAllFile(object sender, RoutedEventArgs e)
        {
            
            if (_resetfileList.Count == 0)
            {
                MessageBox.Show("Nothing to reset");
            }
            else
            {
                _fileList = _resetfileList;
                _resetfileList.Clear();
                foreach (var file in _fileList)
                {
                    if (stoprename == false)
                    {
                        Debug.WriteLine(file.Filename);
                        Debug.WriteLine(file.NewFilename);
                        File.Move(file.Pathname + "\\" + file.Filename, file.Pathname + "\\" + file.NewFilename);
                        var filereset = new FileIOS()
                        {
                            Filename = file.NewFilename,
                            NewFilename = file.Filename,
                            Pathname = file.Pathname
                        };
                        _resetfileList.Add(filereset);
                        file.Filename = file.NewFilename;

                    }
                    else
                    {
                        break;
                    }
                }
            }
            MessageBox.Show("Preset complete");
        }

        private void EditAddSuffix(object sender, RoutedEventArgs e)
        {
            if (editAddSuffix.Visibility.ToString() == "Collapsed")
            {
                editAddSuffix.Visibility = Visibility.Visible;

            }
            else
            {
                editAddSuffix.Visibility = Visibility.Collapsed;
            }
        }

       
        private void EditAddPredix(object sender, RoutedEventArgs e)
        {
            if (editAddPrefix.Visibility.ToString() == "Collapsed")
            {
                editAddPrefix.Visibility = Visibility.Visible;

            }
            else
            {
                editAddPrefix.Visibility = Visibility.Collapsed;
            }
        }

        private void cmbchangedSelection(object sender, SelectionChangedEventArgs e)
        {
            string isCase = cmbAddCase.SelectedItem.ToString()!;

            isCase=toPascalCase(isCase.Substring(isCase.LastIndexOf(":")+1));
            string keyword = "AddCase";
            string nameRuleEdit = "AddCase";
            if (!string.IsNullOrEmpty(isCase))
            {

                foreach (var rule in rules)
                {
                    if (rule.Name == nameRuleEdit)
                        rule?.EditRule(isCase);

                }
                foreach (var file in _fileList)
                {
                    file.NewFilename = file.Filename;
                    foreach (var rule in rules)
                    {
                        file.NewFilename = rule?.Rename(file.NewFilename, "File")!;
                    }
                }
                foreach (var file in _folderList)
                {
                    file.NewFilename = file.Filename;
                    foreach (var rule in rules)
                    {
                        file.NewFilename = rule?.Rename(file.NewFilename, "Folder")!;
                    }
                }
            }

        }

        private void ChangePathDefault(object sender, RoutedEventArgs e)
        {
            
               
            Direction_stackPannel.Visibility = Visibility.Collapsed;
            DefaultAddress_stackPannel.Visibility = Visibility.Visible;
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                var result = dialog.ShowDialog();

                if (System.Windows.Forms.DialogResult.OK == result)
                {
                    newPath = dialog.SelectedPath;
                path_address.Text = newPath;
                    MessageBox.Show(newPath);
                
                }


           
        }

        private void changedefaultaddressbtn(object sender, RoutedEventArgs e)
        {
           
            Direction_stackPannel.Visibility = Visibility.Visible;
            DefaultAddress_stackPannel.Visibility = Visibility.Collapsed;
            newPath = "";
        }
        public void CopyFile(
            string sourceFilePath,
            string destinationFilePath,
            string destinationFileName = null)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentException("sourceFilePath cannot be null or whitespace.", nameof(sourceFilePath));

            if (string.IsNullOrWhiteSpace(destinationFilePath))
                throw new ArgumentException("destinationFilePath cannot be null or whitespace.", nameof(destinationFilePath));

            var targetDirectoryInfo = new DirectoryInfo(destinationFilePath);

            //this creates all the sub directories too
            if (!targetDirectoryInfo.Exists)
                targetDirectoryInfo.Create();

            var fileName = string.IsNullOrWhiteSpace(destinationFileName)
                ? Path.GetFileName(sourceFilePath)
                : destinationFileName;

            File.Copy(sourceFilePath, System.IO.Path.Combine(destinationFilePath, fileName));
        }
    }

}
