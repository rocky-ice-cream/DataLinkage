using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

namespace DataLinkage
{
    /// <summary>
    /// ���O�o�̓N���X
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public Logger()
        {
        }


        /// <summary>
        /// ���O�o�͊֐�
        /// </summary>
        /// <param name="_text">���O�o�̓e�L�X�g</param>
        /// <param name="_methodname">���\�b�h��</param>
        /// <param name="_classname">�N���X��</param>
        /// <param name="_assemblyname">�A�Z���u����</param>
        /// <param name="_err">�G���[�R�[�h(�f�t�H���g 0)</param>
        /// <returns></returns>
        public int LogWrite(
            string _text, 
            string _methodname, 
            string _classname, 
            string _assemblyname, 
            Int32 _err = 0)
        {
            // ���O�p�̃R�l�N�V�����ڑ�
            using (SqlConnection logconn =
                new SqlConnection(Common.CONNECTION_STRING))
            {
                logconn.Open();

                //�R�}���h�̍쐬
                using (SqlCommand logCommand
                    = new SqlCommand())
                {
                    logCommand.Connection = logconn;
                    logCommand.CommandText = GetInsertCommand();

                    logCommand.Parameters.AddWithValue("@log_text", _text.ToString());
                    logCommand.Parameters.AddWithValue("@error_code", _err);
                    logCommand.Parameters.AddWithValue("@method_name", _methodname.ToString());
                    logCommand.Parameters.AddWithValue("@class_name", _classname.ToString());
                    logCommand.Parameters.AddWithValue("@program_name", _assemblyname.ToString());
                    logCommand.Parameters.AddWithValue("@log_create_time",DateTime.Now);

                    return logCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// ���O�o�^�pSQL��Ԃ�
        /// </summary>
        /// <returns></returns>
        private string GetInsertCommand() {
            StringBuilder strInsertCommand = new StringBuilder();
            strInsertCommand.Append("INSERT INTO [dbo].[ctb_log]");
            strInsertCommand.Append("           ([log_text]");
            strInsertCommand.Append("           ,[error_code]");
            strInsertCommand.Append("           ,[method_name]");
            strInsertCommand.Append("           ,[class_name]");
            strInsertCommand.Append("           ,[program_name]");
            strInsertCommand.Append("           ,[log_create_time])");
            strInsertCommand.Append("     VALUES");
            strInsertCommand.Append("           (@log_text");
            strInsertCommand.Append("           ,@error_code");
            strInsertCommand.Append("           ,@method_name");
            strInsertCommand.Append("           ,@class_name");
            strInsertCommand.Append("           ,@program_name");
            strInsertCommand.Append("           ,@log_create_time)");

            return strInsertCommand.ToString();
       
        }
    }
}
