using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Adora_Scratchpad.Models
{
    class EditorProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        [JsonIgnore]
        private FontFamily fontFamily;
        public FontFamily FontFamily {
            get { return fontFamily; }
            set { SetField(ref fontFamily, value); }
        }

        [JsonIgnore]
        private double fontSize;
        public double FontSize {
            get
            {
                LengthConverter lc = new LengthConverter();
                return (double)lc.ConvertFrom(fontSize + "pt");
            }
            set {
                FontSizeActual = value;
                SetField(ref fontSize, value);
            }
        }

        [JsonIgnore]
        private double fontSizeActual;
        public double FontSizeActual
        {
            get { return fontSizeActual; }
            set { SetField(ref fontSizeActual, value); }
        }

        [JsonIgnore]
        public FontWeight fontWeight;
        public FontWeight FontWeight {
            get { return fontWeight; }
            set { SetField(ref fontWeight, value); }
        }

        [JsonIgnore]
        public FontStyle fontStyle;
        public FontStyle FontStyle {
            get { return fontStyle; }
            set { SetField(ref fontStyle, value); }
        }

        [JsonIgnore]
        public TextDecorationCollection textDecorations;
        public TextDecorationCollection TextDecorations {
            get { return textDecorations; }
            set { SetField(ref textDecorations, value); }
        }
    }
}
