using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;

namespace GenericSaveForm
{
    public class GenericSavForm : Form
    {
        private SetSav _SetSav;
        private string _SettFileName = "settings.xml";
        private bool _DisableAutosave;
        private IContainer components;
        public bool DisableAutosave
        {
            get
            {
                return this._DisableAutosave;
            }
            set
            {
                this._DisableAutosave = value;
            }
        }
        public string SettFileName
        {
            get
            {
                return this._SettFileName;
            }
            set
            {
                this._SettFileName = value;
            }
        }
        public GenericSavForm()
        {
            this._SetSav = new SetSav();
            this.InitializeComponent();
        }
        private string GetFullFileName(string FilePath)
        {
            int num = FilePath.IndexOf('/');
            if (num != -1)
            {
                return FilePath;
            }
            num = FilePath.IndexOf('\\');
            if (num != -1)
            {
                return FilePath;
            }
            return Application.StartupPath + "\\" + FilePath;
        }
        public void LoadFile(string FileName)
        {
            this._SettFileName = FileName;
            try
            {
                string fullFileName = this.GetFullFileName(this._SettFileName);
                this._SetSav = (SetSav)this._SetSav._Deserialize(fullFileName);
            }
            catch (Exception)
            {
            }
            base.Width = this._SetSav.WinWidth;
            base.Height = this._SetSav.WinHeight;
            base.Location = new Point(this._SetSav.WinPosX, this._SetSav.WinPosY);
        }
        protected override void OnLoad(EventArgs e)
        {
            this.LoadFile(this._SettFileName);
            base.OnLoad(e);
        }
        public object GetSettingsObject(Type t)
        {
            if (this._SetSav.ExternObj == null)
            {
                this._SetSav.ExternObj = Activator.CreateInstance(t);
            }
            return this._SetSav.ExternObj;
        }
        public object GetSettingsObject()
        {
            return this._SetSav.ExternObj;
        }
        public void SetSettingsObject(object SetSav)
        {
            this._SetSav.ExternObj = SetSav;
        }
        public void SaveAs(string FileName)
        {
            this._SetSav.WinWidth = base.Width;
            if (this._SetSav.WinWidth < 50)
            {
                this._SetSav.WinWidth = 50;
            }
            this._SetSav.WinHeight = base.Height;
            if (this._SetSav.WinHeight < 50)
            {
                this._SetSav.WinHeight = 50;
            }
            this._SetSav.WinPosX = base.Location.X;
            if (this._SetSav.WinPosX < 50)
            {
                this._SetSav.WinPosX = 50;
            }
            this._SetSav.WinPosY = base.Location.Y;
            if (this._SetSav.WinPosY < 50)
            {
                this._SetSav.WinPosY = 50;
            }
            string fullFileName = this.GetFullFileName(FileName);
            this._SetSav._Serialize(fullFileName);
        }
        private void GenericSavForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._DisableAutosave)
            {
                return;
            }
            this.SaveAs(this._SettFileName);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(459, 266);
            base.Name = "GenericSavForm";
            this.Text = "GenericSavForm";
            base.FormClosing += new FormClosingEventHandler(this.GenericSavForm_FormClosing);
            base.ResumeLayout(false);
        }
    }

    [Serializable]
    public class SetSav : BaseSerializable
    {
        public int WinPosX = 200;
        public int WinPosY = 200;
        public int WinWidth = 450;
        public int WinHeight = 400;
        public object ExternObj;
        public SetSav()
        {
            this.ChildObjectRef = this;
        }
    }

    [Serializable]
    public abstract class BaseSerializable
    {
        protected object ChildObjectRef;
        public BaseSerializable()
        {
        }
        public void _Serialize(string FullPath)
        {
            if (this.ChildObjectRef == null)
            {
                throw new Exception("ChildObjectRef is not set");
            }
            if (FullPath == null)
            {
                throw new Exception("_Serialize: Path not set");
            }
            string directoryName = Path.GetDirectoryName(FullPath);
            FileStream fileStream = null;
            if (directoryName != null && directoryName.Trim().Length != 0 && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            try
            {
                fileStream = new FileStream(FullPath, FileMode.Create);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, this.ChildObjectRef);
                fileStream.Close();
                fileStream = null;
            }
            catch (Exception ex)
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
        public object _Deserialize(string FullPath)
        {
            FileStream fileStream = null;
            object result = null;
            try
            {
                fileStream = new FileStream(FullPath, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                result = binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                fileStream = null;
            }
            catch (Exception ex)
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                throw ex;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
            return result;
        }
    }
}
