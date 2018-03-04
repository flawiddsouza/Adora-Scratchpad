using System.Windows.Media;
using System.Windows;
using System.IO;
using Newtonsoft.Json;
using System;

namespace Adora_Scratchpad
{
    class ViewModel
    {
        public DelegateCommand ChangeFontCommand { get; set; }
        public Models.EditorProperties EditorProperties { get; set; }
        public Models.Editor Editor { get; set; }

        public ViewModel()
        {
            ChangeFontCommand = new DelegateCommand(ExecuteChangeFontCommand, (x) => true);

            try
            {
                using (StreamReader sr = new StreamReader("settings.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    EditorProperties = (Models.EditorProperties)serializer.Deserialize(sr, typeof(Models.EditorProperties));
                    EditorProperties.FontSize = EditorProperties.FontSizeActual;
                }
            }
            catch (FileNotFoundException) {
                EditorProperties = new Models.EditorProperties()
                {
                    FontSize = 20 // this will be our default font size
                };
            }

            Editor = new Models.Editor();

            try
            {
                using (StreamReader sr = new StreamReader("scratchpad.txt"))
                {
                    Editor.Text = sr.ReadToEnd();
                }
            }
            catch(FileNotFoundException) { /*do nothing*/ }

            Editor.PropertyChanged += Editor_PropertyChanged;
        }

        private void Editor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("scratchpad.txt"))
            {
                sw.Write(Editor.Text);
            }
        }

        private void ExecuteChangeFontCommand(object obj)
        {
            var fontDialog = new System.Windows.Forms.FontDialog();

            // set existing properties
            if (EditorProperties.FontFamily != null && !Double.IsNaN(EditorProperties.FontSizeActual)) // do only if FontSize is defined
            {
                var existingFont = new System.Drawing.Font(EditorProperties.FontFamily.ToString(), (float)EditorProperties.FontSizeActual);
                fontDialog.Font = existingFont;
            }

            var result = fontDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Font font = fontDialog.Font;
                EditorProperties.FontFamily = new FontFamily(font.Name);
                EditorProperties.FontSize = font.Size;
                EditorProperties.FontWeight = font.Bold ? FontWeights.Bold : FontWeights.Regular;
                EditorProperties.FontStyle = font.Italic ? FontStyles.Italic : FontStyles.Normal;

                TextDecorationCollection tdc = new TextDecorationCollection();
                if (fontDialog.Font.Underline) tdc.Add(TextDecorations.Underline);
                if (fontDialog.Font.Strikeout) tdc.Add(TextDecorations.Strikethrough);
                EditorProperties.TextDecorations = tdc;

                // save font change
                using (StreamWriter sw = new StreamWriter("settings.json"))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, EditorProperties);
                }
            }
        }
    }
}