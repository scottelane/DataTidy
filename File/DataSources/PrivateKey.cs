using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    public class PrivateKey
    {
        private string path;

        [GlobalisedCategory("Private Key"), GlobalisedDisplayName("Path"), GlobalisedDecription("The path of the private key file."), Editor(typeof(FileNameEditor), typeof(UITypeEditor)), Browsable(true)]
        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        private string passPhrase;

        [GlobalisedCategory("Private Key"), GlobalisedDisplayName("Pass Phrase"), GlobalisedDecription("The private key pass phrase."), TypeConverter(typeof(EncryptedStringConverter))]
        public string PassPhrase
        {
            get { return passPhrase; }
            set
            {
                if (passPhrase != value)
                {
                    passPhrase = value;
                    OnPropertyChanged(nameof(PassPhrase));
                }
            }
        }

        public override string ToString()
        {
            if (Path != default(string))
            {
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }

            return "Private Key";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != default(PropertyChangedEventHandler))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
