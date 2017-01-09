using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXApplication1.Utils
{
    class OpenFileUtil
    {
        /// <summary>
        /// 获取当前文件路径
        /// </summary>
        /// <returns>当前文件路径</returns>
        public string OpenTextFile()
        {
            string foldPath = "";
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "文本文件(*.*)|*.txt";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < fileDialog.FileNames.Count(); i++)
                {
                    foldPath += Path.GetDirectoryName(fileDialog.FileNames[i]) + @"\" + Path.GetFileName(fileDialog.FileNames[i]) + ";";
                }
            }
            return foldPath;
        }
       
        public string OpenSMFile()
        {
            string foldPath = "";
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "全部文件(*.*)|*.*";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {

                for (int i = 0; i < fileDialog.FileNames.Count(); i++)
                {
                    foldPath += Path.GetDirectoryName(fileDialog.FileNames[i]) + @"\" + Path.GetFileName(fileDialog.FileNames[i]) + ";";
                }
            }
            return foldPath;
        }
    }
}
