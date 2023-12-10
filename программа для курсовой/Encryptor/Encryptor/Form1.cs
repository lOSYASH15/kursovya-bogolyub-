using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Encryptor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        // массив символов для обозначения зашифрованного файла
        char[] mychar = { '!', '.', 'L', 'O', 'C', 'K', 'E', 'D' };     


        // функция шифрования с параметрами: входной файл, выходной файл, пароль
        private void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch { }
        }  // EncryptFile()


        // функция дешифрования с параметрами: входной файл, выходной файл, пароль
        private void DecryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch { }
        }  // DecryptFile()


        // выбор пункта меню ВЫХОД
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }  // exitToolStripMenuItem_Click()


        // выбор пункта меню О РАЗРАБОТЧИКЕ
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Боголюб Иулиания Сергеевна", "3Б, ИСиТ", MessageBoxButtons.OK);
        }  // aboutToolStripMenuItem_Click()


        // при нажатии на кнопку ДОБАВИТЬ ФАЙЛ
        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            // добавить файлы
            OpenFileDialog filepath = new OpenFileDialog();
            filepath.Title = "Select File";
            filepath.InitialDirectory = @"C:\";
            filepath.Filter = "All files (*.*)|*.*";
            filepath.Multiselect = true;
            filepath.FilterIndex = 1;
            filepath.ShowDialog();
            foreach (String file in filepath.FileNames)
            {
                // добавить путь к файлу в listbox
                listBoxFiles.Items.Add(file);
            }
        }  // buttonAddFile_Click()


        // при нажатии на кнопку ОЧИСТИТЬ ФАЙЛ
        private void buttonClearFile_Click(object sender, EventArgs e)
        {
            // очистить все значения в listbox
            listBoxFiles.Items.Clear();
        }  // buttonClearFile_Click()


        // при нажатии на кнопку ДОБАВИТЬ ПАПКУ
        private void buttonAddFolder_Click(object sender, EventArgs e)
        {
            // добавить каталоги
            FolderBrowserDialog folderpath = new FolderBrowserDialog();
            folderpath.ShowDialog();
            listBoxFolders.Items.Add(folderpath.SelectedPath);
        }  // buttonAddFolder_Click()


        // при нажатии на кнопку ОЧИСТИТЬ ПАПКУ
        private void buttonClearFolder_Click(object sender, EventArgs e)
        {
            // очистить все значения в listbox
            listBoxFolders.Items.Clear();
        }  // buttonClearFolder_Click()


        // при нажатии на кнопку ШИФРОВАТЬ
        private void buttonEncrypt_Click(object sender, EventArgs e)
        {
            // функция шифрования выбранных файлов
            // длина пароля длолжна быть от 8 символов
            if (textBoxPassword.Text.Length < 8)
            {
                MessageBox.Show("Password must have 8 characters!", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // для выбранных файлов
            if (listBoxFiles.Items.Count > 0)
            {
                for (int num = 0; num < listBoxFiles.Items.Count; num++)
                {
                    string e_file = "" + listBoxFiles.Items[num];
                    if (!e_file.Trim().EndsWith(".!LOCKED") && File.Exists(e_file))
                    {
                        EncryptFile("" + listBoxFiles.Items[num], "" + listBoxFiles.Items[num] + ".!LOCKED", textBoxPassword.Text);
                        File.Delete("" + listBoxFiles.Items[num]);
                    }
                }
            }
            // для выбранных папок
            if (listBoxFolders.Items.Count > 0)
            {
                for (int num = 0; num < listBoxFolders.Items.Count; num++)
                {
                    string d_file = "" + listBoxFolders.Items[num];
                    string[] get_files = Directory.GetFiles(d_file);
                    foreach (string name_file in get_files)
                    {
                        if (!name_file.Trim().EndsWith(".!LOCKED") && File.Exists(name_file))
                        {
                            EncryptFile(name_file, name_file + ".!LOCKED", textBoxPassword.Text);
                            File.Delete(name_file);
                        }
                    }
                }
            }
        }  // buttonEncrypt_Click()


        private void buttonClear_Click(object sender, EventArgs e)
        {
            // очистить значения listbox'ов и поле ввода пароля
            listBoxFiles.Items.Clear();
            listBoxFolders.Items.Clear();
            textBoxPassword.Text = "";
        }  // buttonClear_Click()


        private void buttonDecrypt_Click(object sender, EventArgs e)
        {
            // функция дешифрования выбранных файлов
            // длина пароля длолжна быть от 8 символов
            // пароль должен быть корректным
            if (textBoxPassword.Text.Length < 8)
            {
                MessageBox.Show("Password must have 8 characters!", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // для выборанных файлов
            if (listBoxFiles.Items.Count > 0)
            {
                for (int num = 0; num < listBoxFiles.Items.Count; num++)
                {
                    string e_file = "" + listBoxFiles.Items[num];
                    if (e_file.Trim().EndsWith(".!LOCKED") && File.Exists(e_file))
                    {
                        DecryptFile(e_file, e_file.TrimEnd(mychar), textBoxPassword.Text);
                        File.Delete(e_file);
                    }
                }
            }
            // для выбранных папок
            if (listBoxFolders.Items.Count > 0)
            {
                for (int num = 0; num < listBoxFolders.Items.Count; num++)
                {
                    string d_file = "" + listBoxFolders.Items[num];
                    string[] get_files = Directory.GetFiles(d_file);
                    foreach (string name_file in get_files)
                    {
                        if (name_file.Trim().EndsWith(".!LOCKED") && File.Exists(name_file))
                        {
                            DecryptFile(name_file, name_file.TrimEnd(mychar), textBoxPassword.Text);
                            File.Delete(name_file);
                        }
                    }
                }
            }
        }  // buttonDecrypt_Click()


    }  // class
}  // namespace
