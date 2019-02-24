using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    public abstract class FileDataSource : DataSource
    {
        #region Properties

        protected string path;

        [GlobalisedCategory(typeof(FileDataSource), nameof(Path)), GlobalisedDisplayName(typeof(FileDataSource), nameof(Path)), GlobalisedDecription(typeof(FileDataSource), nameof(Path)), Editor(typeof(FileNameEditor), typeof(UITypeEditor)), Browsable(true)]
        public virtual string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged(nameof(Path));
                    RefreshName();
                }
            }
        }

        #endregion

        public FileDataSource(IConnection parent) : base(parent)
        { }

        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(Path == default(string), Properties.Resources.FileDataSourceValidatePathMissing);
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Generates a friendly name for the data source based on the specified path.
        /// </summary>
        /// <returns>The friendly name.</returns>
        protected override string GenerateFriendlyName()
        {
            return !string.IsNullOrEmpty(Path) ? System.IO.Path.GetFileNameWithoutExtension(Path) : Properties.Resources.FileSystemConnectionFriendlyName;
        }
    }
}
