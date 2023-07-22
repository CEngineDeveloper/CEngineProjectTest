/**
 * ExcelUtil.cs
 * Created by: CYM [as8506@qq.com]
 * Created on: 2023/6/22 (zh-CN)
 */

using CYM.Excel;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CYM
{
    [Serializable, Unobfus]
    public class ExcelUtil
    {
        #region prop
        public static char CHAR_NUL = (char)0; // Null char
        public static char CHAR_BS = (char)8; // Back Space
        public static char CHAR_CR = (char)13; // Carriage Return
        public static char CHAR_SUB = (char)26; // Substitute
        #endregion

        #region static excel
        public static WorkBook ReadWorkbook(byte[] buffer)
        {
            try
            {
                var book = new WorkBook(buffer);
                return book;
            }
            catch (Exception e)
            {
                CLog.Error(e.ToString());
                return null;
            }
            finally
            {
            }
        }
        public static WorkBook ReadWorkbook(string path)
        {
            try
            {
                var book = new WorkBook(path);
                return book;
            }
            catch (Exception e)
            {
                CLog.Error(e.ToString());
                return null;
            }
            finally
            {
            }
        }
        public static Document ReadCSV(byte[] buffer)
        {
            try
            {
                var csv = Document.Load(buffer);
                return csv;
            }
            catch (Exception e)
            {
                CLog.Error(e.ToString());
                return null;
            }
            finally
            { 
            
            }
        }
        #endregion

        #region is
        //判断是否为二进制文件
        public static bool IsBinary(byte[] buffer)
        {
            if(buffer == null)
            {
                CLog.Error("buffer is null");
                return false;
            }
            foreach(var ch in buffer)
            {
                if (isControlChar(ch))
                {
                    return true;
                }
            }
            return false;
            bool isControlChar(int ch)
            {
                return (ch > CHAR_NUL && ch < CHAR_BS)
                    || (ch > CHAR_CR && ch < CHAR_SUB);
            }
        }
        #endregion
    }
}