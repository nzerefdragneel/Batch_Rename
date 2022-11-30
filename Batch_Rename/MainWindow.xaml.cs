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
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using ControlzEx.Standard;
using System.ComponentModel;
using System.CodeDom;
using System.Diagnostics.Metrics;

namespace Batch_Rename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : RibbonWindow
    {

       
        public MainWindow()
        {
            InitializeComponent();
        }
        class position
        {
            public double height { get; set;}
            public double width { get; set;}
            public double left { get; set;}
            public double top { get; set;}
        }
        string tab_control;
        ObservableCollection<string> _nodigits = new ObservableCollection<string>();

        static Dictionary<string, IRule> _prototypes = new Dictionary<string, IRule>();

        ObservableCollection<FileIOS> _fileList = new ObservableCollection<FileIOS>();

        ObservableCollection<FolderIOS> _folderList = new ObservableCollection<FolderIOS>();

        ObservableCollection<FolderIOS> _resetfolderList = new ObservableCollection<FolderIOS>();


        List<string> _case = new List<string>() { "PascalCase", "UpCase", "LowerCase", "RemoveAllSpace" };
        string newPath="";

        List<IRule?> _resetrules = new List<IRule?>();
        string pathPosition = Environment.CurrentDirectory.ToString() + "\\position.json";

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

        ObservableCollection<string> _nameRuleReset = new ObservableCollection<String>() { "All" };
        private void LoadRuleFromFile()
        {
           Debug.WriteLine("rule+++++++++++++++++++++++++++++++++++++");
            var exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            Debug.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Debug.WriteLine(exeFolder);
            Debug.WriteLine("__________*********************************");
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
               ChangeExtension,AddCase,AddCounter,AddPrefix,AddSuffix,Trim,ChangeCharacters
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
               ChangeExtension,AddCase,AddCounter,AddPrefix,AddSuffix,Trim,ChangeCharacters
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
                "Change Extension","Add Case","Add Counter","Add Prefix","Add Suffix","Trim","ChangeCharacters"
            };
            tab_control = "File";

            deleteDefault();
            LoadRuleFromFile();
            cmbChooseRule.ItemsSource= _ruleList;
            cmbChoosePreset.ItemsSource = _nameRuleReset;
            cmbAddCase.ItemsSource = _case;
            cmbAddCase.SelectedIndex = 0;
            cmSelectNoDigits.ItemsSource = _nodigits;
            StreamReader input;
            if (File.Exists(pathPosition)) { 
            input = new StreamReader(pathPosition);

            string file = input.ReadToEnd();
            input.Close();
            dynamic pos = JsonObject.Parse(file);

           
            position? sourcePosition = JsonSerializer.Deserialize<position>(pos);


              
           
                
                    if (sourcePosition != null)
                    {
                        this.Height = sourcePosition.height;
                        this.Width = sourcePosition.width;
                        this.Left = sourcePosition.left;
                        this.Top = sourcePosition.top;
                    }
                    
                
            }

            /*
            var destination = sourcePosition.Select(d => new position
            {
                height = d.height,
                width = d.width,
                left = d.left,
                top = d.top
            });*/


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
                   
                        int index = filenamepath.LastIndexOf("\\");

                        if (index > -1)
                        {
                            var filename = filenamepath.Substring(index + 1);
                            var path = filenamepath.Substring(0, index);
                            int n = _fileList.Count;
                            if (_fileList.Where(X => X.getType() == "File" && ((FileIOS)X)!.Pathname == path && ((FileIOS)X)!.Filename == filename).FirstOrDefault() == null)
                            {
                            var file = new FileIOS()
                            {
                                Stt =n+1,
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
                        else
                        {
                            MessageBox.Show("This file arealy exists");
                        }
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
                    count = _fileList.Count - count;
                    MessageBox.Show(count.ToString());

                  
                }

            }
        }
        private void ReadAllFileInFolder(string path)
        {
           
            foreach (string d in Directory.GetFiles(path))
            { 
                    int index =d.LastIndexOf("\\");

                    if (index > -1)
                    {
                        var filename = d.Substring(index + 1);
                        var pathfile = d.Substring(0, index);
                        int n = _fileList.Count;
                    if (_fileList.Where(X => X.getType() == "File" && ((FileIOS)X)!.Pathname == pathfile && ((FileIOS)X)!.Filename == filename).FirstOrDefault() == null)
                    {
                        var file = new FileIOS
                        {
                            Stt = n+1,
                            Filename = filename,
                            NewFilename = filename,
                            Pathname = pathfile,
                            Type = "File",
                            Result = ""
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
                int index = d.LastIndexOf("\\");
                if (index == -1) index = 0;
                var Pathname = d.Substring(0, index);
                var filename = d.Substring(index + 1);
                if ( _folderList.Where(X => X.getType() == "Folder" && ((FolderIOS)X)!.Pathname == Pathname && ((FolderIOS)X)!.Filename==filename).FirstOrDefault() == null)
                {  
                    int n = _folderList.Count;
                    var file = new FolderIOS
                    {   Stt = n+1,
                        Filename = filename,
                        NewFilename = filename,
                        Pathname =Pathname,
                        Type = "Folder",
                        Result = ""                    };

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
            int ruledelete = 0;
            for (index=0;index<_allListRuleElement.Count;index++)
            {
                if (parent.Name == _allListRuleElement[index].Name)
                {
                    _active[index] = 0;
                    ruledelete = index;
                }
            }
            
            foreach (var rule in rules)
            {
                if (rule.Name == parent.Name)
                {
                    
                    if (rule.Name == "AddCounter") rule.reset();
                    for (int i=0;i<_resetrules.Count;i++)
                    {
                        if (_resetrules[i].Name == rule.Name)
                        {
                            Debug.WriteLine(ruledelete);
                            _resetrules.Remove(_resetrules[i]);
                            
                            _resetrules.Add(rule);
                            _nameRuleReset.RemoveAt(i+1);
                            _nameRuleReset.Add(_ruleList[ruledelete]);
                            ruledelete = -1;
                            break;
                        }
                    }
                    Debug.WriteLine(ruledelete);
                    if (ruledelete !=-1)
                    {
                        _resetrules.Add(rule);
                        _nameRuleReset.Add(_ruleList[ruledelete]);
                    }
                    rules.Remove(rule);
                    break;
                }
            }
            reviewAllruleChange();
            Debug.WriteLine($"{parent.Name}");
            Debug.WriteLine("delete___________");
            switch (parent.Name.ToString())
            {
                case "ChangeExtension":
                    tb_extension.Text = "";
                    break;
                case "AddPrefix":
                    textboxAddprefix.Text = "";
                    break;
                case "AddSuffix":
                    textboxAddsuffix.Text = "";
                    break;
                case "ChangeCharacters":
                    textboxNewCharacter.Text = "";
                    textboxOldCharacter.Text = "";
                    break;
                case "AddCase":
                    cmbAddCase.SelectedIndex = 0;
                    break;
                    
            }
           ((RibbonGroupBoxWrapPanel)parent.Parent).Children.Remove(parent);
            
        }
    
        private void reviewAllruleChange()
        {
            if (Tab1.IsSelected)
            {
                foreach (var rule in rules)
                { 
                if (rule.Name == "AddCounter")
                    {
                        rule.reset();
                    }
                        }
                    for (int i = 0; i < _fileList.Count; i++)
                {
                    _fileList[i].NewFilename = _fileList[i].Filename;
                    foreach (var rule in rules)
                    {

                        Debug.WriteLine(rule.Name);
                        _fileList[i].NewFilename = rule?.Rename(_fileList[i].NewFilename, "File")!;
                    }
                     
                }
            }
            else
            {
                foreach (var rule in rules)
                {
                    if (rule.Name == "AddCounter")
                    {
                        rule.reset();
                    }
                }
                for (int i = 0; i < _folderList.Count; i++)
                {
                    _folderList[i].NewFilename = _folderList[i].Filename;
                    foreach (var rule in rules)
                    {
                        _folderList[i].NewFilename = rule?.Rename(_folderList[i].NewFilename, "Folder")!;
                    }
                }
            }
        }
        private void ChangeExtension_mouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void _textboxReplace_TextChanged(object sender, TextChangedEventArgs e)
        {
            var parent = ((System.Windows.Controls.TextBox)sender);
            Regex reg = new Regex("^[ \\.\\w-$()+=[\\];#@~,&']+$");



            string data = parent.Text.ToString();
            if (!reg.Match(data).Success)
            {
                MessageBox.Show("New extension is invalid!");

                parent.Text = data.Substring(0, data.Length - 1);
            }
            else if (data.Length > 50)
            {
                MessageBox.Show("Input to long!");
                parent.Text = data.Substring(0, data.Length - 1);
            }
            else
            {
                string nameRuleEdit;
                switch (parent.Name.ToString())
                {
                    case "tb_extension":
                        nameRuleEdit = "ChangeExtension";
                        break;
                    case "textboxAddprefix":
                        nameRuleEdit = "AddPrefix";
                        break;
                    case "textboxOldCharacter":
                        nameRuleEdit = "ChangeCharacters";
                        if (textboxNewCharacter.Text.Length > 0 && textboxOldCharacter.Text.Length > 0)
                            data = textboxOldCharacter.Text + "?" + textboxNewCharacter.Text[0];
                        else data = "";
                        break;
                    case "textboxNewCharacter":
                        nameRuleEdit = "ChangeCharacters";
                        if (textboxNewCharacter.Text.Length > 0 && textboxOldCharacter.Text.Length > 0)
                            data = textboxOldCharacter.Text + "?" + textboxNewCharacter.Text[0];
                        else data = "";
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

                    reviewAllruleChange();
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tab_control == "File")
            {
                tab_control = "Folder";

                reviewAllruleChange();
            }
            else { 
                tab_control = "File";

                reviewAllruleChange();
            }
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
     
        private void EditaddCounter(object sender, RoutedEventArgs e)
        {
            

            if (editaddCounter.Visibility.ToString() == "Collapsed")
            {
                editaddCounter.Visibility = Visibility.Visible;
               
                this.AddCounter.Background = brushColorSelect;
            }
            else
            {
                this.AddCounter.Background = brushColorDefault;
                editaddCounter.Visibility = Visibility.Collapsed;
                int number;
                if (Tab1.IsSelected)
                {
                    number = (int)(Math.Round(Math.Log10(_fileList.Count + 1)) + 1);
                    _nodigits.Clear();
                    for (int i = number; i<9;i++)
                    {
                        _nodigits.Add(i.ToString());
                    }
                }

                else
                {
                    number = (int)(Math.Round(Math.Log10(_folderList.Count + 1)) + 1);
                    _nodigits.Clear();
                    for (int i = number; i < 9; i++)
                    {
                        _nodigits.Add(i.ToString());
                    }
                }
                
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
                    if (rule.Name == "AddCounter") rule.reset();
                    newName = rule?.Rename(newName, "File")!;
                }
                Debug.WriteLine(newName);
                _fileList[i].NewFilename = newName;
            }
        }
        private void reviewFile_folderNamebyOneRule(IRule? rule)
        {
            Debug.WriteLine(rule.Name.ToString());
            if (rule.Name == "AddCounter") rule.reset();
            if (Tab1.IsSelected)
            {
                
                for (int i = 0; i < _fileList.Count; i++)
                {
                    Debug.WriteLine(rule.Name);
                    string newName = _fileList[i].NewFilename;
                    newName = rule?.Rename(newName, "File")!;
                    _fileList[i].NewFilename = newName;
                }
            }
            else
            {
                for (int i = 0; i < _folderList.Count; i++)
                {
                    string newName = _folderList[i].NewFilename;
                    newName = rule?.Rename(newName, "Folder")!;
                    _folderList[i].NewFilename = newName;
                }
            }
        }
        private void previewAfterAddRule(string line)
        {
            var rule = RuleFactory.Instance().Parse(line);
            Debug.WriteLine(line);
            if (rule!=null )
            {
              
                rules.Add(rule);
                reviewFile_folderNamebyOneRule(rule);

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
                    string data;
                    previewAfterAddRule(line);
                   /*
                    switch (_allListRuleElement[isChoose].Name)
                    {
                        case "ChangeExtension":
                            data = tb_extension.Text;
                            break;
                        case "AddPrefix":
                            data = textboxAddprefix.Text;
                            break;
                        case "AddSuffix":
                            data=textboxAddsuffix.Text;
                            break;
                        case "AddCase":
                            data=cmbAddCase.Text; break;
                    }*/

                    
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

        private int Collapse()
        {
            
            return 0;
        }
        public async Task<int> LongRunningOperationAsync() // assume we return an int from this long running operation 
        {
            await Task.Delay(10); // 1 second delay
            return 1;
        }

        public event Action<object, string> Click;
        private Task<Visibility> ClickEvent()
        {
            return Task.Run(() =>
            {

                if (Click != null)
                    MessageBox.Show("Click");
                MessageBox.Show("Click");
                return Visibility.Hidden;

            });
        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            processbar.Value = e.ProgressPercentage;
        }
        private async void StartRename(object sender, RoutedEventArgs e)

        {
            /*  Task<int> longRunningTask = LongRunningOperationAsync();
              // independent work which doesn't need the result of LongRunningOperationAsync can be done here
              StartRenameGrid.Visibility = Visibility.Collapsed;
              ControlRename.Visibility = Visibility.Visible;
              //and now we call await on the task 
              int result = await longRunningTask;
              int numoff = 0;
              if (Click != null)
              {
                  ProgressButton.Visibility = Visibility.Visible;
              }*/
            processbar.Maximum = _fileList.Count+1;
            int index = 0;
            var backgroundWorker = sender as BackgroundWorker;
            if (Tab1.IsSelected)
            {
                foreach (var file in _fileList)
                {
                    backgroundWorker.ReportProgress(index++);
                    if (Click != null)
                        ProgressButton.Visibility = await ClickEvent();
                    else
                        ProgressButton.Visibility = Visibility.Hidden;

                    Debug.WriteLine("___________________________");
                    Debug.WriteLine(file.Pathname); Debug.WriteLine(file.NewFilename);

                    if (newPath == "")
                    {
                        var patht = file.Pathname + "\\" + file.Filename;
                        if (File.Exists(patht))
                        {
                            var newpath = file.Pathname + "\\" + file.NewFilename;
                            if (File.Exists(newpath))
                            {
                                //MessageBox.Show(newPath, "having!");
                            }
                            else
                            {
                                File.Move(file.Pathname + "\\" + file.Filename, file.Pathname + "\\" + file.NewFilename);
                                var filereset = new FileIOS()
                                {
                                    Filename = file.NewFilename.ToString(),
                                    NewFilename = file.Filename.ToString(),
                                    Pathname = file.Pathname
                                };
                                _resetfileList.Add(filereset);
                                file.Filename = file.NewFilename;
                                file.Status = "complete";
                            }
                        }
                        else
                        {
                            //
                            //MessageBox.Show(patht, "Not exists");
                            file.Error = "can't find file";
                        }


                    }
                    else
                    {
                        var oldpath = file.Pathname + "\\" + file.Filename;
                        var newpath = newPath + "\\" + file.NewFilename;
                        if (File.Exists(oldpath))
                        {
                            if (File.Exists(newpath))
                            {

                                //  MessageBox.Show(newpath + " is Exists");
                            }
                            else
                            {
                                File.Copy(oldpath, newpath);
                                file.Status = "complete";
                            }
                        }
                        else
                        {
                            // MessageBox.Show(oldpath + " not exist");
                            file.Status = "err"; file.Error = "can't find file";

                        }




                    }

                }
            }
            else
            {

                for (int i = _folderList.Count - 1; i >= 0; i--)
                {
                    var file = _folderList[i];
                    var oldpath = file.Pathname + "\\" + file.Filename;
                    var newpath = file.Pathname + "\\" + file.NewFilename;
                    if (Directory.Exists(oldpath))
                    {
                        if (Directory.Exists(newpath))
                        {
                            //  MessageBox.Show(newpath + " exist");
                            file.Status = "err";
                            file.Error = "new name is exist";
                        }
                        else
                        {
                            Directory.Move(oldpath, newpath);
                            var filereset = new FolderIOS()
                            {
                                Filename = file.NewFilename.ToString(),
                                NewFilename = file.Filename.ToString(),
                                Pathname = file.Pathname
                            };
                            _resetfolderList.Add(filereset);
                            _folderList[i].Filename = file.NewFilename;
                            _folderList[i].Status = "complete";

                        }
                    }
                    else
                    {
                        //  MessageBox.Show(oldpath, "cant find");
                        file.Status = "err";
                        file.Error = "cant find path";
                    }
                }
            }

           

            MessageBox.Show("All files have been renamed!");
            ControlRename.Visibility=Visibility.Collapsed;
            StartRenameGrid.Visibility = Visibility.Visible;
        }
        
        private async void StopRenameAsync(object sender, RoutedEventArgs e)
        {
            
            // independent work which doesn't need the result of LongRunningOperationAsync can be done here
            stoprename = !stoprename;
   
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
            int index = cmbAddCase.SelectedIndex;
            if (index != -1)
            {
                string isCase = _case[index];

                isCase = toPascalCase(isCase);

                Debug.WriteLine(isCase);

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

        private void EditChangeCharacters(object sender, RoutedEventArgs e)
        {
            if (editOldCharacter.Visibility.ToString() == "Collapsed")
            {
                editOldCharacter.Visibility = Visibility.Visible;

            }
            else
            {
                editOldCharacter.Visibility = Visibility.Collapsed;
            }
        }

        public void AddRuleReset(IRule? rule,int isChoose)
        {
            bool check = false;
            for (int i=0;i<rules.Count;i++)
            {
                if (rules[i].Name == rule.Name)
                {
                    
                    rules.RemoveAt(i);
                    rules.Insert(i, rule);
                    check = true;
                    break;
                }
               
            }
            if (check==false)
            {
                rules.Add(rule);
                Debug.WriteLine(_allListRuleElement[isChoose].Name);
                this.ListRuleGroupBox.Children.Add(_allListRuleElement[isChoose]);
                _active[isChoose] = 1;
                _activeRule.Add(_allListRuleElement[isChoose].Name.ToString());
               

            }
            switch (rule.Name)
            {
                case "ChangeExtension":

                    tb_extension.Text = rule.getData();
                    break;
                case "AddPrefix":
                    textboxAddprefix.Text = rule.getData();

                    break;
                case "AddSuffix":
                    textboxAddsuffix.Text = rule.getData();
                    break;
                case "ChangeCharacters":
                    textboxNewCharacter.Text = rule.getData();
                    textboxOldCharacter.Text = rule.getData();
                    break;
                case "AddCase":
                    var x = rule.getData();
                    int index = -1;
                    for (int i = 0; i < _case.Count; i++)
                    {
                        if (x == _case[i]) { index = i; }
                    }
                    cmbAddCase.SelectedIndex = index;
                    break;
            }
            
        }
        // cần tìm rule và index tương ứng
        private void btnResetRule(object sender, RoutedEventArgs e)
        {
            var reset = cmbChoosePreset.Text;
            var resetRule =toPascalCase(reset);
            int isChoose = 0;
            if (resetRule == "")
            {
                MessageBox.Show("Please Choose One Rule!");
            }
            else
            {
                if (resetRule == "All")
                {
                    for (int index= 0;index<_resetrules.Count;index++)
                    {
                        resetRule = _resetrules[index].Name;
                        reset = _nameRuleReset[index + 1];
                        isChoose = 0;
                        for (int i = 0; i < _ruleList.Count; i++)
                        {
                            if (reset == _ruleList[i]) isChoose = i;
                        }
                        for (int i = 0; i < _resetrules.Count; i++)
                        {
                            if (resetRule == _resetrules[i].Name)
                            {
                                Debug.WriteLine(_resetrules[i].Name);
                                Debug.WriteLine(isChoose);
                                AddRuleReset(_resetrules[i], isChoose);
                            }
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < _ruleList.Count; i++)
                    {
                        if (reset == _ruleList[i]) isChoose = i;
                    }
                    for ( int i = 0; i < _resetrules.Count; i++)
                    {
                        if (resetRule == _resetrules[i].Name)
                        {
                            Debug.WriteLine(_resetrules[i].Name);
                            Debug.WriteLine(isChoose);
                            AddRuleReset(_resetrules[i], isChoose);
                        }
                    }
                }
                
            }
        }

        private void Window_close(object sender, EventArgs e)
        {
            var position = new position()
            {
                height = this.Height,
                width = this.Width,
                left = this.Left,
                top = this.Top
            };
            string jsonString = JsonSerializer.Serialize(position, new JsonSerializerOptions() { WriteIndented = true });
            using (StreamWriter outputFile = new StreamWriter(pathPosition))
            {
                outputFile.WriteLine(jsonString);
            }
        }

        private void btnOkChange(object sender, RoutedEventArgs e)
        {
            var parent = ((System.Windows.Controls.Button)sender).Parent;
           
        }

        private void ChangeAddcounter(object sender, RoutedEventArgs e)
        {
            var inod = cmSelectNoDigits.SelectedIndex;
            var icase = cmbChooseType.SelectedIndex;
            var textStep = textboxStep.Text;
            var step = Int32.Parse(textStep);
            var textStart = textboxStartValue.Text;
            var start = Int32.Parse(textStart);
            var kind = cmbChooseType.SelectedIndex;
            string data;
            foreach (var rule in rules)
            {
                if (rule.Name == "AddCounter")
                {
                    if (inod != -1) rule.EditRule("NoDigits?" + _nodigits[inod]);
                    if (textStep != "")
                    {
                        Debug.WriteLine("Step:", step);
                        rule.EditRule("Step?" + textStep);
                    }
                    if (textStart != "")
                    {
                        data = "StartFile?" + textStart;
                        if (Tab1.IsSelected)

                            rule.EditRule(data);
                        else {
                            data = "StartFolder?" + textStart;
                            rule.EditRule(data);
                        }
                    }
                    if (kind != -1)
                    {
                        if (kind == 0) rule.EditRule("Kind?Pre");
                        else rule.EditRule("Kind?Suf");
                    }
                }
                break;
            }
            reviewAllruleChange();

        }

        private void checkStep(object sender, TextChangedEventArgs e)
        {
            var type = textboxStep.Text;
            if (type != "")
            {
                try
                {
                    var x = int.Parse(type);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Type integer pls!");
                    textboxStep.Text = type.Substring(0, type.Length - 2);
                }
            }
        }
       
        private void CheckStarValue(object sender, TextChangedEventArgs e)
        {
            var type = textboxStartValue.Text;
            if (type != "")
            {
                try
                {
                    var x = int.Parse(type);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Type integer pls!");
                    textboxStartValue.Text = type.Substring(0, type.Length - 1);
                }
            }
        }
    }
}
