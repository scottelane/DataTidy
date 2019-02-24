using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    /// <summary>
    /// Base class for all file operations.
    /// </summary>
    public abstract class FileOperation : Operation, IDataSourceFieldsProvider, ICustomMenuItemProvider
    {
        #region Properties

        protected string outputPath;

        /// <summary>
        /// The output file path.
        /// </summary>
        [GlobalisedCategory(typeof(FileOperation), nameof(OutputPath)), GlobalisedDisplayName(typeof(FileOperation), nameof(OutputPath)), GlobalisedDecription(typeof(FileOperation), nameof(OutputPath)), Editor(typeof(SaveFileNameEditor), typeof(UITypeEditor))]
        public virtual string OutputPath
        {
            get { return outputPath; }
            set
            {
                if (outputPath != value)
                {
                    outputPath = value;
                    OnPropertyChanged(nameof(OutputPath));
                    RefreshName();
                }
            }
        }

        protected IDataSource dataSource;

        /// <summary>
        /// The data source.
        /// </summary>
        [GlobalisedCategory(typeof(FileOperation), nameof(DataSource)), GlobalisedDisplayName(typeof(FileOperation), nameof(DataSource)), GlobalisedDecription(typeof(FileOperation), nameof(DataSource)), TypeConverter(typeof(DataSourceConverter))]
        public virtual IDataSource DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    OnPropertyChanged(nameof(DataSource));
                    RefreshName();
                }
            }
        }

        #endregion

        /// <summary>
        /// Initialises a new instance of the FileOperation class for the specified batch.
        /// </summary>
        /// <param name="batch">The parent batch.</param>
        public FileOperation(Batch batch) : base(batch)
        { }

        /// <summary>
        /// Validates the FileOperation settings.
        /// </summary>
        /// <returns>The validation result.</returns>
        public override ValidationResult Validate()
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result.AddErrorIf(OutputPath == default(string), Properties.Resources.FileOperationValidateOutputPath, nameof(OutputPath));
                result.AddErrorIf(DataSource == default(IDataSource), Properties.Resources.FileOperationValidateDataSource, nameof(DataSource));

                if (DataSource != default(IDataSource))
                {
                    result.Errors.AddRange(DataSource.Validate().Errors);
                }
            }
            catch (Exception ex)
            {
                result.AddErrorIf(true, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<DataTableField> GetDataSourceFields()
        {
            return DataTableField.GetDataTableFields(DataSource?.GetDataColumns());
        }

        public List<CustomMenuItem> GetCustomMenuItems()
        {
            List<CustomMenuItem> menuItems = new List<CustomMenuItem>
            {
                new CustomMenuItem()
                {
                    Text = Properties.Resources.FileOperationOpenFileText,
                    ToolTip = Properties.Resources.FileOperationOpenFileTooltip,
                    Icon = Properties.Resources.OpenFile,
                    Item = this,
                    Enabled = System.IO.File.Exists(OutputPath),
                    SynchronousEventHandler = OpenFile
                },
                new CustomMenuItem()
                {
                    Text = Properties.Resources.FileOperationOpenFolderText,
                    ToolTip = Properties.Resources.FileOperationOpenFolderTooltip,
                    Icon = Properties.Resources.FileOperationOpenFolder,
                    Item = this,
                    Enabled = System.IO.File.Exists(OutputPath),
                    SynchronousEventHandler = OpenFolder
                }
            };

            return menuItems;
        }

        private void OpenFile()
        {
            if (System.IO.File.Exists(OutputPath))
            {
                Process.Start(OutputPath);
            }
            else
            {
                throw new ApplicationException(string.Format(Properties.Resources.FileOperationOpenFileNotFound, OutputPath));
            }
        }

        private void OpenFolder()
        {
            if (System.IO.File.Exists(OutputPath))
            {
                Process.Start(System.IO.Path.GetDirectoryName(OutputPath));
            }
            else
            {
                throw new ApplicationException(string.Format(Properties.Resources.FileOperationOpenFolderNotFound, OutputPath));
            }
        }
    }
}
